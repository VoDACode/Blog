import { Component, HostListener, OnInit } from '@angular/core';
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

  private page: number = 1;
  private pageSize: number = 10;
  private maxPageCount: number = 50;
  private isLoading: boolean = false;

  public loading: boolean = true;
  public posts: PostModelResponse[] = [];
  public userModel: UserResponseModel = new UserResponseModel();
  public get hasPosts() {
    return this.posts.length > 0;
  }

  constructor(private postApi: PostApiService, private userApi: UserApiService) { }

  ngOnInit(): void {
    this.postApi.getPosts(this.page, this.pageSize).subscribe(res => {
      this.posts = res.data?.items || [];
      this.loading = false;
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

  @HostListener('window:scroll', ['$event'])
  onScroll(event: any) {
    const pos = window.scrollY;
    const max = document.documentElement.scrollHeight - document.documentElement.clientHeight;
    if (pos == max && this.page < this.maxPageCount) {
      this.loadMore();
    }
  }

  private loadMore() {
    if(this.loading) return;
    this.page++;
    this.loading = true;
    this.postApi.getPosts(this.page, this.pageSize).subscribe(res => {
      if (res.data != null && res.data.items.length > 0) {
        this.maxPageCount = res.data.totalPagesCount;
        this.posts.push(...res.data?.items || []);
        this.isLoading = false;
        this.loading = false;
      }
    });
  }
}
