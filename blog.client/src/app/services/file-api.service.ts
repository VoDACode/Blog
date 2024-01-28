import { Injectable } from '@angular/core';
import { BaseApiService } from './base-api';
import { HttpClient, HttpEvent, HttpEventType, HttpProgressEvent, HttpRequest, HttpResponse } from '@angular/common/http';
import { BaseResponse } from '../models/response.model';
import { FileModelResponse } from '../models/file.model';
import { PostFileModel } from '../models/post.model';
import { Observable, filter, forkJoin, map, scan } from 'rxjs';

interface Upload {
  progress: number
  state: 'PENDING' | 'IN_PROGRESS' | 'DONE',
  data?: FileModelResponse
}

type UploadSpeedData = {
  lastLoaded: number,
  time: number
}

type UploadFilesOptions = {
  complete(): void,
  error(error: any): void
}

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

  uploadFiles(postId: number, files: PostFileModel[], options: UploadFilesOptions | undefined = undefined){
    const uploadObservables: Observable<any>[] = [];
    files.forEach(fileModel => {
      if (fileModel.file instanceof File) {
        fileModel.processing = true;
        let uploadObservable = this.upload(postId, fileModel);
        const processedUploadObservable = uploadObservable.pipe(
          map((event) => {
            if (event.type === HttpEventType.UploadProgress) {
              if (event.total) {
                const progress = Math.round((100 * event.loaded) / event.total);
                fileModel.progress = progress;
                return { progress };
              }
            } else if (event.type === HttpEventType.Response) {
              return { result: event.body };
            }
            return null;
          })
        );

        uploadObservables.push(processedUploadObservable);
      }
    });
    forkJoin(uploadObservables).subscribe({
      next: (results) => {
        results.forEach((result, index) => {
          const fileModel = files[index];
          if (result.progress !== undefined) {
            fileModel.progress = result.progress;
          } else if (result.result !== undefined) {
            fileModel.processing = false;
            if (result.result.success === true && result.result.data !== undefined) {
              fileModel.file = result.result.data;
            }
          }
        });
      },
      complete: options?.complete,
      error: options?.error
    });
  }

  upload(postId: number, file: PostFileModel | File) {
    const formData = new FormData();
    if (file instanceof File) {
      formData.append('file', file);
    } else if (file.file instanceof File) {
      formData.append('file', file.file);
    } else {
      throw new Error('Invalid file');
    }

    const request = new HttpRequest('POST', `${this.url}/upload?postId=${postId}`, formData, {
      reportProgress: true, // Enable progress tracking
    });

    return this.http.request<BaseResponse<FileModelResponse>>(request);
  }
}

function isHttpProgressEvent(
  event: HttpEvent<unknown>
): event is HttpProgressEvent {
  return (
    event.type === HttpEventType.DownloadProgress ||
    event.type === HttpEventType.UploadProgress
  )
}