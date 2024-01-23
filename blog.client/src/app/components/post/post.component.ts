import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PostFileModel, PostModelResponse } from 'src/app/models/post.model';
import { UserResponseModel } from 'src/app/models/user.model';
import { PostApiService } from 'src/app/services/post-api.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent {
  @Input()
  post: PostModelResponse = new PostModelResponse();

  @Output()
  delete: EventEmitter<PostModelResponse> = new EventEmitter<PostModelResponse>();

  @Input()
  userModel: UserResponseModel = new UserResponseModel();

  uploadedFiles: PostFileModel[] = [];

  get hasFiles() {
    return this.post.files && this.post.files.length > 0;
  }

  _images: PostFileModel[] = [];
  _files: PostFileModel[] = [];

  get images() {
    if (!this.post.files) return [];
    if (this._images.length == 0) {
      this._images = this.post.files?.filter(f => f.contentType.startsWith('image')).map(f => new PostFileModel(f));
    }
    return this._images;
  }

  get files() {
    if (!this.post.files) return [];
    if (this._files.length == 0) {
      this._files = this.post.files?.filter(f => !f.contentType.startsWith('image')).map(f => new PostFileModel(f));
    }
    return this._files;
  }

  constructor(private postApiService: PostApiService, private actionRouter: ActivatedRoute, private router: Router) {
    this.actionRouter.params.subscribe(params => {
      let id = params['id'];
      if(!id) return;
      this.postApiService.getPost(Number(id)).subscribe(post => {
        if (post.success == false || post.data == undefined) {
          this.post = new PostModelResponse();
          this.router.navigate(['/']);
          return;
        }
        this.post = post.data;
      });
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

  share() {
    let host = environment.baseUrl;
    if (!host || host == '') {
      host = `${window.location.protocol}//${window.location.host}`;
    }
    navigator.clipboard.writeText(`${host}/post/${this.post.id}`);
  }

  deletePost() {
    this.postApiService.deletePost(this.post.id).subscribe(res => {
      if (res.success) {
        this.delete.emit(this.post);
      }
    });
  }
}
