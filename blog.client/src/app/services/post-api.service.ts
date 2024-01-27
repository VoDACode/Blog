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

  getPosts(page: number, pageSize: number, query: string = '') {
    let url = `${this.url}?page=${page}&pageSize=${pageSize}`;
    if (query) {
      // query to URL encode
      query = encodeURIComponent(query);
      url += `&query=${query}`;
    }
    return this.http.get<BaseResponse<PageResponse<PostModelResponse>>>(url, this.httpOptions);
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

    return this.http.post<BaseResponse<PostModelResponse>>(`${this.url}`, form)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('createPost'));
  }

  updatePost(postId: number, post: PostModelRequest, newFiles: File[] | null, deleteFiles: number[] | null) {
    let form = new FormData();
    form.append('Title', post.title);
    form.append('Content', post.content);
    form.append('IsPublished', post.isPublished.toString());
    form.append('HasComments', post.hasComments.toString());
    post.tags.forEach(tag => {
      form.append('Tags', tag);
    });
    if (newFiles) {
      for (let file of newFiles) {
        form.append('newFiles', file);
      }
    }

    if (deleteFiles) {
      for (let id of deleteFiles) {
        form.append('DeletedFiles', id.toString());
      }
    }

    return this.http.put<BaseResponse<PostModelResponse>>(`${this.url}/${postId}`, form)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('updatePost'));
  }

  deletePost(id: number) {
    return this.http.delete<BaseResponse<PostModelResponse>>(`${this.url}/${id}`, this.httpOptions)
      .pipe(this.handleError<BaseResponse<PostModelResponse>>('deletePost'));
  }

  searchTags(query: string) {
    return this.http.get<BaseResponse<string[]>>(`${this.url}/search/tags?query=${query}`, this.httpOptions)
      .pipe(this.handleError<BaseResponse<string[]>>('searchTags'));
  }

}
