import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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

  public loading: boolean = true;
  public posts: PostModelResponse[] = [];
  public userModel: UserResponseModel = new UserResponseModel();
  public searchText: string = '';
  public tagSuggestions: string[] = [];

  public get inTop(){
    return window.scrollY > 200;
  }

  public get hasPosts() {
    return this.posts.length > 0;
  }

  constructor(private postApi: PostApiService, private userApi: UserApiService, private actionRouter: ActivatedRoute, private route: Router) { 
    this.actionRouter.queryParams.subscribe(params => {
      let query = params['q'] || '';
      this.search(query);
    });
  }

  ngOnInit(): void {
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
  onScroll() {
    const pos = window.scrollY;
    const max = document.documentElement.scrollHeight - document.documentElement.clientHeight;
    if ((pos == max || pos < 0) && this.page < this.maxPageCount) {
      this.loadMore();
    }
  }

  goToTop() {
    window.scrollTo(0, 0);
  }

  public searchKeyUp(event: any) {
    if (event.key === 'Enter') {
      this.routeToSearch(this.searchText);
      if(this.searchText == '') {
        this.route.navigate(['/']);
      }
    }

    if(this.searchText.length == 0) {
      this.tagSuggestions = [];
    }

    if(this.searchText.matchAll(/#[a-zA-Z0-9]+/g)) {
      this.loadTagSuggestion();
    }
  }

  routeToSearch(text: string) {
    this.route.navigate(['/'], { queryParams: { q: text } });
  }

  search(text: string = this.searchText) {
    this.page = 1;
    this.loading = true;
    this.searchText = text;
    this.tagSuggestions = [];
    this.postApi.getPosts(this.page, this.pageSize, text).subscribe(res => {
      this.posts = res.data?.items || [];
      this.maxPageCount = res.data?.totalPagesCount || 0;
      this.loading = false;
      setTimeout(this.onScroll.bind(this), 100);
    });
  }

  private loadTagSuggestion() {
    let correctTagFormat = this.searchText.replace(/\s/g, '_');
    correctTagFormat = correctTagFormat.replace(/[^\w0-9]/g, '');
    if(correctTagFormat.length == 0) return;
    this.postApi.searchTags(correctTagFormat).subscribe(res => {
      this.tagSuggestions = res.data || [];
    });
  }

  private loadMore() {
    if(this.loading) return;
    this.page++;
    this.loading = true;
    this.postApi.getPosts(this.page, this.pageSize, this.searchText).subscribe(res => {
      if (res.data != null && res.data.items.length > 0) {
        this.maxPageCount = res.data.totalPagesCount;
        this.posts.push(...res.data?.items || []);
        this.loading = false;
      }
    });
  }
}
