import { Component, OnInit } from '@angular/core';
import { AuthApiService } from 'src/app/services/auth-api.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css']
})
export class NavigationComponent {
  get isAuthenticated() {
    return this.authApiService.isAuthenticated
  }

  constructor(private authApiService: AuthApiService) { }

  logout() {
    this.authApiService.logout().subscribe(r => {
      if (r.success) {
        localStorage.removeItem('isLoggedin');
        window.location.href = '/';
      }
    });
  }
}
