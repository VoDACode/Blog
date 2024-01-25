import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class FileApiService extends BaseApiService {
  protected override get path(): string {
    return 'api/file';
  }

  constructor(http: HttpClient) {
    super(http);
  }

  delete(id: number) {
    return this.http.delete(`${this.url}/${id}`, this.httpOptions)
      .pipe(this.handleError('delete'));
  }
}
