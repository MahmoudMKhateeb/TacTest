import { Component, OnInit, Injector, NgZone } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { TrackingListDto, TrackingServiceProxy, ShippingRequestTripStatus } from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { TrackingSearchInput } from '../../../../shared/common/search/TrackingSearchInput';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';
import { TrackingSignalrService } from './tacking-signalr.service';

@Component({
  templateUrl: './tracking.component.html',
  styleUrls: ['/assets/custom/css/style.scss'],
  animations: [appModuleAnimation()],
})
export class TrackingComponent extends ScrollPagnationComponentBase implements OnInit {
  Items: TrackingListDto[] = [];
  direction = 'ltr';
  _zone: NgZone;
  searchInput: TrackingSearchInput = new TrackingSearchInput();
  constructor(
    injector: Injector,
    private _currentServ: TrackingServiceProxy,
    private _localStorageService: LocalStorageService,
    private _trackingSignalrService: TrackingSignalrService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.LoadData();
    this.registerToEvents();
  }
  LoadData() {
    this._currentServ
      .getAll(
        this.searchInput.status,
        this.searchInput.shipper,
        this.searchInput.carrier,
        this.searchInput.WaybillNumber,
        this.searchInput.truckTypeId,
        this.searchInput.originId,
        this.searchInput.destinationId,
        this.searchInput.pickupFromDate,
        this.searchInput.pickupToDate,
        this.searchInput.fromDate,
        this.searchInput.toDate,
        this.searchInput.routeTypeId,
        '',
        this.skipCount,
        this.maxResultCount
      )
      .subscribe((result) => {
        this.IsLoading = false;
        if (result.items.length < this.maxResultCount) {
          this.StopLoading = true;
        }
        this.setUsersProfilePictureUrl(result.items);
        this.Items.push(...result.items);
      });
  }

  search(): void {
    this.IsLoading = true;
    this.skipCount = 0;
    this.Items = [];
    this.LoadData();
  }

  setUsersProfilePictureUrl(drivers: TrackingListDto[]): void {
    for (let i = 0; i < drivers.length; i++) {
      let user = drivers[i];
      this._localStorageService.getItem(AppConsts.authorization.encrptedAuthTokenName, function (err, value) {
        let profilePictureUrl =
          AppConsts.remoteServiceBaseUrl +
          '/Profile/GetProfilePictureByUser?userId=' +
          user.assignedDriverUserId +
          '&' +
          AppConsts.authorization.encrptedAuthTokenName +
          '=' +
          encodeURIComponent(value.token);
        (user as any).profilePictureUrl = profilePictureUrl;
      });
    }
  }

  registerToEvents() {
    abp.event.on('app.tracking.accepted', (data) => {
      let item: TrackingListDto = <TrackingListDto>data.data;
      let currentitem = this.Items.find((x) => x.id == item.id);
      if (currentitem) {
        currentitem.driverStatus = item.driverStatus;
        currentitem.driverStatusTitle = item.driverStatusTitle;
      }
    });

    abp.event.on('app.tracking.started', (data) => {
      let item: TrackingListDto = <TrackingListDto>data.data;
      let currentitem = this.Items.find((x) => x.id == item.id);
      if (currentitem) {
        currentitem.status = ShippingRequestTripStatus.Intransit;
        currentitem.statusTitle = ShippingRequestTripStatus[ShippingRequestTripStatus.Intransit];
      }
    });
  }
}
