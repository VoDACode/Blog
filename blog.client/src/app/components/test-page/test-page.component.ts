import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-test-page',
  templateUrl: './test-page.component.html',
  styleUrls: ['./test-page.component.css']
})
export class TestPageComponent {

  file: File | null = null;

  constructor(private http: HttpClient) { }

  onFileChange(event: any) {
    this.file = event.target.files[0];
  }

  uploadFile() {
    if(!this.file) return;
    const formData = new FormData();
    formData.append('file', this.file as File);
    this.http.post('/api/file/upload?postId=1', formData).pipe().subscribe((res: any) => {
      console.log(res);
    });
  }
}
