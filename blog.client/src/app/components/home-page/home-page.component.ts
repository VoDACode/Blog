import { Component, OnInit } from '@angular/core';
import { PostModelResponse } from 'src/app/models/post.model';
import { UserResponseModel } from 'src/app/models/user.model';
import { PostApiService } from 'src/app/services/post-api.service';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {
  public posts: PostModelResponse[] = [];
  public get hasPosts() {
    return this.posts.length > 0;
  }
  public userModel: UserResponseModel = new UserResponseModel();
  constructor(private postApi: PostApiService, private userApi: UserApiService) { }
  ngOnInit(): void {
    this.postApi.getPosts(1, 10).subscribe(res => {
      this.posts = res.data?.items || [];
    });
    this.userApi.getMe().subscribe(res => {
      this.userModel = res.data || new UserResponseModel();
    });
  }

  deletePost(post: PostModelResponse) {
    const index = this.posts.indexOf(post);
    if (index >= 0) {
      this.posts.splice(index, 1);
    }
  }
}
