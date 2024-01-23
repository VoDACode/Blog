import { Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { PostFileModel, PostModelRequest } from 'src/app/models/post.model';
import { PostApiService } from 'src/app/services/post-api.service';
import { environment } from 'src/environments/environment';

interface TagSuggestion{
  tag: string;
  selected: boolean;
}

@Component({
  selector: 'app-create-post',
  templateUrl: './create-post.component.html',
  styleUrls: ['./create-post.component.css']
})
export class CreatePostComponent {
  @ViewChild('fileInput') fileInput: ElementRef<HTMLInputElement> | undefined;
  public isPreview: boolean = false;
  public post: PostModelRequest = new PostModelRequest();
  public tag: string = '';
  public tagSuggestion: TagSuggestion[] = [];
  uploadedFiles: PostFileModel[] = [];

  get hasFiles() {
    return this.uploadedFiles.length > 0;
  }

  _images: PostFileModel[] = [];
  _files: PostFileModel[] = [];

  get images() {
    return this._images;
  }

  get files() {
    return this._files;
  }

  constructor(private postApiService: PostApiService, private router: Router) {
  }

  searchTag(event: any) {
    if(event.key == 'Enter') {
      this.addTag(this.tag);
      return;
    }

    if(this.tag == ''){
      this.tagSuggestion = [];
    }

    if(event.key == 'Backspace' && this.tag == '') {
      this.post.tags.pop();
      return;
    }

    if(event.key == 'ArrowDown' || event.key == 'ArrowUp') {
      let index = this.tagSuggestion.findIndex(p => p.selected);
      if(index != -1) {
        this.tagSuggestion[index].selected = false;
        if(event.key == 'ArrowDown') {
          index++;
          if(index >= this.tagSuggestion.length) {
            index = 0;
          }
        }else{
          index--;
          if(index < 0) {
            index = this.tagSuggestion.length - 1;
          }
        }
        this.tagSuggestion[index].selected = true;
        this.tag = this.tagSuggestion[index].tag;
      }
      return;
    }

    let correctTagFormat = this.tag.replace(/\s/g, '_');
    correctTagFormat = correctTagFormat.replace(/[^\w0-9]/g, '');
    if (correctTagFormat != this.tag) {
      this.tag = correctTagFormat;
    }
    if (this.tag) {
      this.postApiService.searchTags(this.tag).subscribe(tags => {
        this.tagSuggestion = [];
        let result = tags.data?.map(p => {
          return {tag: p, selected: false};
        }) ?? [];
        let index = result.findIndex(p => p.tag == this.tag);
        if (index == -1) {
          this.tagSuggestion.push({tag: this.tag, selected: false});
        }
        this.tagSuggestion = [...this.tagSuggestion, ...result];
        this.tagSuggestion[0].selected = true;
      });
    }
  }

  addTag(selectedTag: string) {
    this.post.tags.push(selectedTag);
    this.tag = '';
    this.tagSuggestion = [];
  }

  deleteTag(tag: string) {
    this.post.tags = this.post.tags.filter(p => p != tag);
  }

  setPreviewMode(isPreview: boolean) {
    this.isPreview = isPreview;
  }

  onUploadFileClick() {
    this.fileInput?.nativeElement.click();
  }

  onFileChange(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      for (let i = 0; i < event.target.files.length; i++) {
        const file = event.target.files[i];
        if (!this.uploadedFiles.find(f => f.name == file.name)) {
          let fileModel = new PostFileModel(file);
          this.uploadedFiles.push(fileModel);
          if (file.type.startsWith('image')) {
            this._images.push(fileModel);
          } else {
            this._files.push(fileModel);
          }
        }
      }
    }
  }

  onDeleteFile(file: PostFileModel) {
    this._files = this._files.filter(f => f.uuid !== file.uuid);
    this._images = this._images.filter(f => f.uuid !== file.uuid);
    this.uploadedFiles = this.uploadedFiles.filter(f => f.uuid != file.uuid);
  }

  save() {
    let files: File[] = [];
    this.uploadedFiles.forEach(fileModel => {
      if (fileModel.file instanceof File) {
        files.push(fileModel.file);
      }
    });
    this.postApiService.createPost(this.post, files).subscribe(post => {
      if (post.success) {
        this.router.navigate([`/post/${post.data?.id}`]);
      }
    });
  }

  getFileSize(file: PostFileModel) {
    var roundSizeTo = 10 ** environment.roundSizeTo;
    if (file.size < 1024)
      return `${file.size} B`;
    if (file.size < 1024 * 1024)
      return `${Math.round((file.size / 1024) * roundSizeTo) / roundSizeTo} KB`;
    if (file.size < 1024 * 1024 * 1024)
      return `${Math.round((file.size / 1024 / 1024) * roundSizeTo) / roundSizeTo} MB`;
    return `${Math.round((file.size / 1024 / 1024 / 1024) * roundSizeTo) / roundSizeTo} GB`;
  }

  getDateTime(date: Date) {
    date = new Date(date);
    date = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);
    return date.toLocaleString();
  }
}


