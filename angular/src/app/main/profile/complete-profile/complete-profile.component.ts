import { Component, OnInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-complete-profile',
  templateUrl: './complete-profile.component.html',
  styleUrls: ['./complete-profile.component.css'],
})
export class CompleteProfileComponent implements OnInit {
  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';

  constructor(private _profileServiceProxy: ProfileServiceProxy) {}

  ngOnInit(): void {
    this.getProfilePicture();
    abp.event.on('profilePictureChanged', () => {
      this.getProfilePicture();
    });
  }

  /**
   * get User Profile Picture
   */
  getProfilePicture(): void {
    this._profileServiceProxy.getProfilePicture().subscribe((result) => {
      if (result && result.profilePicture) {
        this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
      }
    });
  }
}
