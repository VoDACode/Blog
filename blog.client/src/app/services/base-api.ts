import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable, catchError, of } from "rxjs";
import { environment } from "src/environments/environment";

export abstract class BaseApiService {
    constructor(protected http: HttpClient) { }
    protected get baseUrl(): string {
        return environment.apiBaseUrl;
    }
    protected abstract get path(): string;
    protected get url(): string {
        return `${this.baseUrl}/${this.path}`;
    }
    protected get httpOptions(): object {
        return {
            headers: new HttpHeaders({
                'Content-Type': 'application/json'
            })
        };
    }
    protected handleError<T>(operation = 'operation') {
        return catchError<T, Observable<T>>((e) => {
            console.error(`${operation} failed: ${e.message}`);
            return of(e.error as T);
        });
    }
}