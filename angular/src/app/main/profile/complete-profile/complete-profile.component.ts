import { Component, Injector, OnInit, Renderer2, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import {
  NormalPricePackagesServiceProxy,
  ProfileServiceProxy,
  TenantCityLookupTableDto,
  UpdateTenantProfileInformationInputDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-complete-profile',
  templateUrl: './complete-profile.component.html',
  styleUrls: ['./complete-profile.component.less'],
  encapsulation: ViewEncapsulation.None,
})
export class CompleteProfileComponent extends AppComponentBase implements OnInit {
  completeFiled: UpdateTenantProfileInformationInputDto = new UpdateTenantProfileInformationInputDto();

  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
  saving = false;
  loading = true;
  selectedNodes1: any;
  allCities: TenantCityLookupTableDto[];

  constructor(
    injector: Injector,
    private _profileServiceProxy: ProfileServiceProxy,
    private _tenantServiceProxy: NormalPricePackagesServiceProxy,
    private _ActiveRoute: ActivatedRoute,
    private renderer: Renderer2
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._profileServiceProxy.getTenantProfileInformationForEdit().subscribe((result) => {
      this.completeFiled = result;
      this.loading = false;
    });
    this.getProfilePicture();
    abp.event.on('profilePictureChanged', () => {
      this.getProfilePicture();
    });
    this.loadAllCities();
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
   * Save
   */
  save(): void {
    this.saving = true;
    this._profileServiceProxy
      .updateTenantProfileInformation(this.completeFiled)
      .pipe(
        finalize(() => {
          this.saving = false;
          abp.event.trigger('tenantUpdatedHisProfileInformation');
        })
      )
      .subscribe(() => {
        this.notify.success(this.l('UpdateDoneSuccessfully'));
      });
  }

  /**
   * change Profile Picture
   */
  changeProfilePicture() {
    abp.event.trigger('app.show.changeProfilePictureModal');
  }

  /**
   * Get All System Cities
   */

  loadAllCities() {
    this._tenantServiceProxy.getAllCitiesForTableDropdown().subscribe((res) => {
      this.allCities = res;
    });
  }
}
