import { Component, OnInit, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { TrackingListDto, TrackingServiceProxy } from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { TrackingSearchInput } from '../../../../shared/common/search/TrackingSearchInput';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';

@Component({
  templateUrl: './tracking.component.html',
  styleUrls: ['/assets/custom/css/style.scss'],
  animations: [appModuleAnimation()],
})
export class TrackingComponent extends ScrollPagnationComponentBase implements OnInit {
  Items: TrackingListDto[] = [];
  direction = 'ltr';
  searchInput: TrackingSearchInput = new TrackingSearchInput();
  constructor(injector: Injector, private _currentServ: TrackingServiceProxy, private _localStorageService: LocalStorageService) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.LoadData();
  }
  LoadData() {
    this._currentServ
      .getAll(
        this.searchInput.status,
        this.searchInput.shipper,
        this.searchInput.carrier,
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
}
