import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import {
  PagedResultDtoOfAvailableVasDto,
  PagedResultDtoOfFacilityLocationListDto,
  ProfileServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';

@Component({
  selector: 'tenants-profile',
  templateUrl: './tenants-profile.component.html',
  styleUrls: ['./tenants-profile.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class TenantsProfileComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTableVases', { static: true }) dataTableVases: Table;
  @ViewChild('paginatorVases', { static: true }) paginatorVases: Paginator;
  public id: number;
  public loading = true;
  public givenId: number;
  editionType: number;
  sorting: string;
  skipCount: number;
  maxResultCount: number;
  shipperFacilities: any;
  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';

  //Dummy data For Carrier Vases Table
  carrierVases = [];
  //Dummy Carrier Service Areas
  carrierServiceAreas = [];
  active = false;
  selectedFacility: any;
  latitude: number;
  longitude: number;
  facilite: PagedResultDtoOfFacilityLocationListDto;
  giveUserEditionType: number;

  constructor(
    injector: Injector,
    private _profileServiceProxy: ProfileServiceProxy,
    private _Activatedroute: ActivatedRoute,
    private _router: Router
  ) {
    super(injector);
    this.givenId = parseInt(this._Activatedroute.parent.snapshot.paramMap.get('id'));
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
  get isTMS(): boolean {
    return this.feature.isEnabled('App.TachyonDealer');
  }

  ngOnInit(): void {
    this.getGiverUserEditionType();
  }

  /**
   * get Givven User Edition Type
   */
  getGiverUserEditionType() {
    this._profileServiceProxy.getTenantProfileInformationForView(this.givenId).subscribe((result) => {
      this.giveUserEditionType = result.editionId;
    });
  }

  /**
   * draw Cordiates on Map
   */
  drawCoordinates(event) {
    this.longitude = event.longitude;
    this.latitude = event.latitude;
  }
}
