import { Component, Injector, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy, TenantProfileInformationForViewDto } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

import { finalize } from '@node_modules/rxjs/operators';
import { BehaviorSubject, Subscription } from 'rxjs';

@Component({
  selector: 'side-profile',
  templateUrl: './side-profile.component.html',
  styleUrls: ['./side-profile.component.css'],
})
export class SideProfileComponent extends AppComponentBase implements OnInit, OnDestroy {
  public givenId: number;
  public currentUserid: number;
  public loading: boolean;
  //side Profile Dto
  sideProfileUserInfo: TenantProfileInformationForViewDto = new TenantProfileInformationForViewDto();

  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
  private routeParmSubscription$: Subscription;
  constructor(
    injector: Injector,
    private _profileServiceProxy: ProfileServiceProxy,
    private _Activatedroute: ActivatedRoute,
    private _router: Router
  ) {
    super(injector);
  }

  /**
   * This One is to Determine the User Type Who Viewing the profile
   */

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
    abp.event.on('tenantUpdatedHisProfileInformation', () => {
      this.getUserInfo();
    });
    this.routeParmSubscription$ = this._Activatedroute.params.subscribe((params) => {
      this.givenId = parseInt(params['id']);
      this.getProfilePicture();
      this.getUserInfo();
    });
    this.currentUserid = this.appSession.tenantId;
    abp.event.on('profilePictureChanged', () => {
      this.getProfilePicture();
    });
  }

  /**
   * Destroy the Component
   */

  ngOnDestroy() {
    this.routeParmSubscription$.unsubscribe();
  }

  /**
   * get the Current User information Shipper/Carrier
   */
  getUserInfo() {
    this.loading = true;
    this._profileServiceProxy
      .getTenantProfileInformationForView(this.givenId)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe(
        (result) => {
          this.sideProfileUserInfo = result;
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
