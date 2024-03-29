import { HttpEventType } from '@angular/common/http';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, combineLatest, filter, forkJoin, map } from 'rxjs';
import { FileModelResponse } from 'src/app/models/file.model';
import { PostFileModel, PostModelRequest } from 'src/app/models/post.model';
import { FileApiService } from 'src/app/services/file-api.service';
import { PostApiService } from 'src/app/services/post-api.service';
import { environment } from 'src/environments/environment';

interface TagSuggestion {
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

  public isEditMode: boolean = false;

  public isPreview: boolean = false;
  public post: PostModelRequest = new PostModelRequest();
  public tag: string = '';
  public tagSuggestion: TagSuggestion[] = [];
  uploadedFiles: PostFileModel[] = [];

  get hasFiles() {
    return this.uploadedFiles.length > 0;
  }

  private id: number = 0;

  public images: PostFileModel[] = [];
  public files: PostFileModel[] = [];

  private deleteFiles: PostFileModel[] = [];
  private newFiles: PostFileModel[] = [];

  constructor(private postApiService: PostApiService, private fileApiService: FileApiService, private router: Router, private activeRouter: ActivatedRoute) {
    this.activeRouter.params.subscribe(params => {
      let id = params['id'];
      this.activeRouter.data.subscribe(data => {
        this.isEditMode = data['editMode'] ?? false;
        if (this.isEditMode) {
          if (!id) {
            this.router.navigate(['/']);
            return;
          }
          this.loadPost(id);
        }
      });
    });
  }

  searchTag(event: any) {
    if (event.key == 'Enter') {
      this.addTag(this.tag);
      return;
    }

    if (this.tag == '') {
      this.tagSuggestion = [];
    }

    if (event.key == 'Backspace' && this.tag == '') {
      this.post.tags.pop();
      return;
    }

    if (event.key == 'ArrowDown' || event.key == 'ArrowUp') {
      let index = this.tagSuggestion.findIndex(p => p.selected);
      if (index != -1) {
        this.tagSuggestion[index].selected = false;
        if (event.key == 'ArrowDown') {
          index++;
          if (index >= this.tagSuggestion.length) {
            index = 0;
          }
        } else {
          index--;
          if (index < 0) {
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
          return { tag: p, selected: false };
        }) ?? [];
        let index = result.findIndex(p => p.tag == this.tag);
        if (index == -1) {
          this.tagSuggestion.push({ tag: this.tag, selected: false });
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
        const file: File = event.target.files[i];
        let fileModel = new PostFileModel(file);
        this.uploadedFiles.push(fileModel);
        if (fileModel.contentType.startsWith('image')) {
          this.images.push(fileModel);
        } else {
          this.files.push(fileModel);
        }
        if (this.isEditMode) {
          this.newFiles.push(fileModel);
        }
      }
    }
    this.images = [...this.images];
  }

  onDeleteFile(file: PostFileModel) {
    this.files = this.files.filter(f => f.uuid !== file.uuid);
    this.images = this.images.filter(f => f.uuid !== file.uuid);
    this.uploadedFiles = this.uploadedFiles.filter(f => f.uuid != file.uuid);
    if (this.isEditMode) {
      this.deleteFiles.push(file);
      this.newFiles = this.newFiles.filter(f => f.uuid != file.uuid);
    }
  }

  save() {
    if (this.isEditMode) {
      this.editPost();
    } else {
      this.createPost();
    }
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

  private editPost() {
    let deleteFilesIds: number[] = [];
    this.deleteFiles.forEach(fileModel => {
      if (fileModel.file instanceof FileModelResponse) {
        console.log(fileModel.file);
        deleteFilesIds.push(fileModel.file.id);
      }
    });
    
    this.postApiService.updatePost(this.id, this.post, deleteFilesIds).subscribe(async post => {
      if (post.success && post.data != undefined) {
        this.fileApiService.uploadFiles(post.data.id, this.newFiles, {
          complete: () => {
            this.router.navigate([`/post/${post.data?.id}`]);
          },
          error: (error) => {
            console.error('Error during file uploads:', error);
          }
        });
      }
    });
  }

  private createPost() {
    this.postApiService.createPost(this.post).subscribe(post => {
      if (post.success && post.data != undefined) {
        this.fileApiService.uploadFiles(post.data.id, this.uploadedFiles, {
          complete: () => {
            this.router.navigate([`/post/${post.data?.id}`]);
          },
          error: (error) => {
            console.error('Error during file uploads:', error);
          }
        });
      }
    });
  }

  private loadPost(id: number) {
    this.postApiService.getPost(id).subscribe(post => {
      if (post.success == false || post.data == undefined) {
        this.post = new PostModelRequest();
        this.router.navigate(['/']);
        return;
      }
      this.id = post.data.id;
      this.post = post.data;
      this.files = post.data.files.filter(f => !f.contentType.startsWith('image')).map(f => {
        let fileModel = new PostFileModel(f);
        fileModel.file = new FileModelResponse(f);
        return fileModel;
      });
      this.images = post.data.files.filter(f => f.contentType.startsWith('image')).map(f => {
        let fileModel = new PostFileModel(f);
        fileModel.file = new FileModelResponse(f);
        return fileModel;
      });
      this.uploadedFiles = [...this.files, ...this.images];
    });
  }
}