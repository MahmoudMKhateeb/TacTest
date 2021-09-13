import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'tenants-profile',
  templateUrl: './tenants-profile.component.html',
  styleUrls: ['./tenants-profile.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class TenantsProfileComponent extends AppComponentBase implements OnInit {
  public id: number;
  public shownLoginName: string;
  public tenancyName: string;
  public userName: string;
  public email: string;
  public phone: number;
  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
  shipperFacilities = [
    { id: 1, name: 'Amazon', City: 'Jeddah', long: 46.6791872, lat: 24.7429006 },
    { id: 2, name: 'Ebay', City: 'Riyadh', long: 50.2082971, lat: 26.3025915 },
    { id: 3, name: 'Google', City: 'Ajman', long: 50.2082971, lat: 26.3025915 },
    { id: 4, name: 'Youtube', City: 'Jeddah', long: 46.6791872, lat: 24.7429006 },
    { id: 5, name: 'Amazon', City: 'Jeddah', long: 50.2082971, lat: 26.3025915 },
    { id: 6, name: 'Ebay', City: 'Riyadh', long: 46.6791872, lat: 24.7429006 },
    { id: 7, name: 'Google', City: 'Ajman', long: 50.2082971, lat: 26.3025915 },
    { id: 8, name: 'Youtube', City: 'Jeddah', long: 46.6791872, lat: 24.7429006 },
  ];
  selectedFacility: any;
  latitude: number;
  longitude: number;

  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy, private _Activatedroute: ActivatedRoute) {
    super(injector);
  }
  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
  get isShipper(): boolean {
    return this.feature.isEnabled('App.Shipper');
  }

  ngOnInit(): void {
    this.getProfilePicture();
    this.getCurrentUserInfo();
    // this._Activatedroute.params.subscribe((params) => {
    //   console.log('The Tenants Profile Component: ', +params['id']);
    //   console.log(+params['id']);
    //   // (+) converts string 'id' to a number
    //   // In a real app: dispatch action to load the details here.
    // });
    this._Activatedroute.params.subscribe((params) => {
      console.log('The Tenants Profile Component: ', +params['id']);
      console.log(+params['id']);
      // (+) converts string 'id' to a number
      // In a real app: dispatch action to load the details here.
    });
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

  /**
   *
   * Show Facility Location On Map For Shipper
   */
  AllocateFacilityOnMap($event) {
    this.longitude = $event.long;
    this.latitude = $event.lat;
  }
}
