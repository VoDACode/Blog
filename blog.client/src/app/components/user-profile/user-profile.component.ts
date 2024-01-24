import { Component, OnInit } from '@angular/core';
import { UserUpdateRequestModel } from 'src/app/models/user.model';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css']
})
export class UserProfileComponent implements OnInit {
  private originalUserJson: string = '';
  user: UserUpdateRequestModel = new UserUpdateRequestModel();

  public get hasChanges(): boolean {
    return this.originalUserJson !== JSON.stringify(this.user);
  }

  constructor(private userApi: UserApiService) { }

  ngOnInit(): void {
    this.userApi.getMe().subscribe((response) => {
      if (response.success && response.data) {
        this.user = UserUpdateRequestModel.fromResponse(response.data);
        this.originalUserJson = JSON.stringify(this.user);
      }
    });
  }

  onSubmit(): void {
    this.userApi.updateMe(this.user).subscribe((response) => {
      if (response.success && response.data) {
        this.user = UserUpdateRequestModel.fromResponse(response.data);
      }else{
        alert('Error: ' + response.message);
      }
    });
  }
}
