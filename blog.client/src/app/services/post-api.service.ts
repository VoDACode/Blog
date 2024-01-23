import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api';
import { HttpClient } from '@angular/common/http';
import { PostModelRequest, PostModelResponse } from '../models/post.model';
import { BaseResponse, PagedResponse as PageResponse } from '../models/response.model';

@Injectable({
  providedIn: 'root'
})
export class PostApiService extends BaseApiService {
  protected override get path(): string {
    return 'api/post';
  }
  constructor(http: HttpClient) {
    super(http);
  }

  getPosts(page: number, pageSize: number) {
    return this.http.get<BaseResponse<PageResponse<PostModelResponse>>>(`${this.url}?page=${page}&pageSize=${pageSize}`, this.httpOptions);
  }

  getPost(id: number) {
    return this.http.get<BaseResponse<PostModelResponse>>(`${this.url}/${id}`, this.httpOptions)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('getPost'))
  }

  createPost(post: PostModelRequest, files: File[] | null) {
    let form = new FormData();
    form.append('Title', post.title);
    form.append('Content', post.content);
    form.append('IsPublished', post.isPublished.toString());
    form.append('HasComments', post.hasComments.toString());
    
    post.tags.forEach(tag => {
      form.append('Tags', tag);
    });
    
    if (files) {
      for (let file of files) {
        form.append('files', file);
      }
    }

    console.log(form);

    return this.http.post<BaseResponse<PostModelResponse>>(`${this.url}`, form)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('getPost'));
  }

  updatePost(post: PostModelRequest) {
    return this.http.put<BaseResponse<PostModelResponse>>(`${this.url}`, post, this.httpOptions)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('getPost'));
  }

  deletePost(id: number) {
    return this.http.delete<BaseResponse<PostModelResponse>>(`${this.url}/${id}`, this.httpOptions)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('getPost'));
  }

  searchPosts(query: string, page: number, pageSize: number) {
    return this.http.get<BaseResponse<PageResponse<PostModelResponse>>>(`${this.url}/search?query=${query}&page=${page}&pageSize=${pageSize}`, this.httpOptions)
      .pipe(this.handleError<BaseResponse<PageResponse<PostModelResponse>>>('searchPosts'));
  }

  searchTags(query: string) {
    return this.http.get<BaseResponse<string[]>>(`${this.url}/search/tags?query=${query}`, this.httpOptions)
      .pipe(this.handleError<BaseResponse<string[]>>('searchTags'));
  }

}
