import { Component } from '@angular/core';
import { AuthApiService } from 'src/app/services/auth-api.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent {
  public username = '';
  public password = '';

  constructor(private authApi: AuthApiService) { }

  login() {
    this.authApi.login(this.username, this.password).subscribe(r => {
      if (r.success) {
        localStorage.setItem('isLoggedin', 'true');
        window.location.href = '/';
      }
    });
  }
}
