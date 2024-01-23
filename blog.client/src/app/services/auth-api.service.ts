import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api';
import { HttpClient } from '@angular/common/http';
import { BaseResponse } from '../models/response.model';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService extends BaseApiService {

  public get isAuthenticated(): boolean {
    return localStorage.getItem('isLoggedin') == 'true';
  }

  protected override get path(): string {
    return 'api/auth';
  }

  constructor(http: HttpClient) {
    super(http);
  }

  public login(username: string, password: string) {
    return this.http.post<BaseResponse<any>>(`${this.url}/login`, { username, password })
      .pipe(this.handleError<BaseResponse<any>>('login'));
  }

  public register(username: string, email: string, password: string) {
    return this.http.post<BaseResponse<any>>(`${this.url}/register`, { username, email, password })
      .pipe(this.handleError<BaseResponse<any>>('register'));
  }

  public check() {
    return this.http.get<BaseResponse<any>>(`${this.url}/is-authenticated`)
      .pipe(this.handleError<BaseResponse<any>>('isAuthenticated'));
  }

  public logout() {
    return this.http.post<any>(`${this.url}/logout`, null)
      .pipe(this.handleError<any>('logout'));
  }

  public refresh() {
    return this.http.post<BaseResponse<any>>(`${this.url}/refresh`, null)
      .pipe(this.handleError<BaseResponse<any>>('refresh'));
  }
}
