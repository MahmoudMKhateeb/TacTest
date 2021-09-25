import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap } from '@node_modules/rxjs/internal/operators';
import { result } from 'lodash-es';
import { any } from '@node_modules/codelyzer/util/function';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'side-profile',
  templateUrl: './side-profile.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class SideProfileComponent extends AppComponentBase implements OnInit {
  public shownLoginName: string;
  public tenancyName: string;
  public userName: string;
  public email: string;
  public phone: string;
  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
  public givenId: number;
  public currentUserid: number;
  rating: number;
  loading = true;
  location: string;
  companyInfo: string;

  constructor(
    injector: Injector,
    private _profileServiceProxy: ProfileServiceProxy,
    private _Activatedroute: ActivatedRoute,
    private _router: Router
  ) {
    super(injector);
  }
  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
  get isShipper(): boolean {
    return this.feature.isEnabled('App.Shipper');
  }
  get canSeeSideMenu(): boolean {
    //if tenant id is the same as the requested on in view        /he is viewing his owin profile
    //true
    // console.log('currentUser', this.currentUserid === this.givenId, 'Given Id', this.givenId);
    return this.currentUserid === this.givenId;
    //return;
  }

  ngOnInit(): void {
    this.givenId = parseInt(this._Activatedroute.snapshot.paramMap.get('id'));
    this.getProfilePicture();
    this.getUserInfo();
    this.currentUserid = this.appSession.tenantId;
    abp.event.on('profilePictureChanged', () => {
      this.getProfilePicture();
    });
  }

  /**
   * get the Current User information Shipper/Carrier
   */
  getUserInfo() {
    this._profileServiceProxy
      .getTenantProfileInformationForView(this.givenId)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe(
        (result) => {
          this.shownLoginName = result.companyName;
          this.email = result.companyEmailAddress;
          this.phone = result.companyPhone;
          this.rating = result.rating;
          this.companyInfo = result.companyInfo;
        },
        (error) => {
          this._router.navigate(['/app/main/dashboard']);
        }
      );
  }

  /**
   * get User Profile Picture
   */
  getProfilePicture(): void {
    this._profileServiceProxy.getProfilePicture(this.givenId).subscribe((result) => {
      if (result && result.profilePicture) {
        this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
      }
    });
  }
}
