import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/response.model';
import { UserResponseModel, UserUpdateRequestModel } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserApiService extends BaseApiService {
  protected override get path(): string {
    return 'api/user';
  }

  constructor(http: HttpClient) {
    super(http);
  }

  public getMe() {
    return this.http.get<BaseResponse<UserResponseModel>>(`${this.url}/me`)
      .pipe(this.handleError<BaseResponse<UserResponseModel>>('getMe'));
  }

  public getUser(id: number) {
    return this.http.get<BaseResponse<UserResponseModel>>(`${this.url}/${id}`)
      .pipe(this.handleError<BaseResponse<UserResponseModel>>('getUser'));
  }

  public getUsers(page: number, pageSize: number) {
    return this.http.get<BaseResponse<UserResponseModel[]>>(`${this.url}?page=${page}&pageSize=${pageSize}`)
      .pipe(this.handleError<BaseResponse<UserResponseModel[]>>('getUsers'));
  }

  public updateMe(user: UserUpdateRequestModel) {
    return this.http.put<BaseResponse<UserResponseModel>>(`${this.url}/me`, user)
      .pipe(this.handleError<BaseResponse<UserResponseModel>>('updateMe'));
  }

  public bunUser(id: number) {
    return this.http.put<BaseResponse<UserResponseModel>>(`${this.url}/${id}/ban`, null)
      .pipe(this.handleError<BaseResponse<UserResponseModel>>('bunUser'));
  }

  public canPublishUser(id: number) {
    return this.http.put<BaseResponse<UserResponseModel>>(`${this.url}/${id}/can-publish`, null)
      .pipe(this.handleError<BaseResponse<UserResponseModel>>('canPublishUser'));
  }
}
