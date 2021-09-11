import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'side-profile',
  templateUrl: './side-profile.component.html',
})
export class SideProfileComponent extends AppComponentBase implements OnInit {
  public shownLoginName: string;
  public tenancyName: string;
  public userName: string;
  public email: string;
  public phone: number;
  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';

  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }
  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
  get isShipper(): boolean {
    return this.feature.isEnabled('App.Shipper');
  }

  ngOnInit(): void {
    console.log('Profiling Component Is Working');
    this.getProfilePicture();
    this.getCurrentUserInfo();
  }

  /**
   * get the Current User information Shipper/Carrier
   */
  getCurrentUserInfo() {
    console.log(this.isCarrier, this.isShipper);
    if (this.isCarrier || this.isShipper) {
      console.log('this is a Shipper || Carrier');
      this.shownLoginName = this.appSession.getShownLoginName();
      this.tenancyName = this.appSession.tenancyName;
      this.userName = this.appSession.user.userName;
      this.email = this.appSession.impersonatorUser.emailAddress;
      this.phone = 2365523361;
    }
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
