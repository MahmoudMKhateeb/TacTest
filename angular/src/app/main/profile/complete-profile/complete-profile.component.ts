import { Component, Injector, OnInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { UpdateTenantProfileInformationInputDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-complete-profile',
  templateUrl: './complete-profile.component.html',
  styleUrls: ['./complete-profile.component.css'],
})
export class CompleteProfileComponent extends AppComponentBase implements OnInit {
  completeFiled: UpdateTenantProfileInformationInputDto = new UpdateTenantProfileInformationInputDto();

  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
  saving = false;
  loading = true;

  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this._profileServiceProxy.getTenantProfileInformationForEdit().subscribe((result) => {
      this.completeFiled = result;
    });

    this.getProfilePicture();
    abp.event.on('profilePictureChanged', () => {
      this.getProfilePicture();
    });
  }

  /**
   * get User Profile Picture
   */
  getProfilePicture(): void {
    this._profileServiceProxy.getProfilePicture(null).subscribe((result) => {
      if (result && result.profilePicture) {
        this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
      }
    });
  }
  /**
   * change Profile Picture
   */
  changeProfilePicture() {
    abp.event.trigger('app.show.changeProfilePictureModal');
  }

  save(): void {
    this.saving = true;
    this._profileServiceProxy
      .updateTenantProfileInformation(this.completeFiled)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )

      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe(() => {
        this.notify.success(this.l('UpdateDoneSuccessfully'));
      });
  }
}
