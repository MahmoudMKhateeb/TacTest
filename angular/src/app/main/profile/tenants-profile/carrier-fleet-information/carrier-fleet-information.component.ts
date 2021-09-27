import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';

@Component({
  selector: 'profile-carrier-fleet-information',
  templateUrl: './carrier-fleet-information.component.html',
  styleUrls: ['./carrier-fleet-information.component.css'],
})
export class CarrierFleetInformationComponent extends AppComponentBase implements OnInit {
  @Input() giverUserId: number;
  @ViewChild(`FleetTable`, { static: true }) FleetTable: Table;
  @ViewChild('FleetPaginator', { static: true }) FleetPaginator: Paginator;
  totalDrivers: number;
  loadingForFleet = true;
  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getFleet(event: LazyLoadEvent) {
    console.log('event: ', event);
    console.log('this.FleetTable ', this.FleetTable);
    console.log('this.FleetPaginator ', this.FleetPaginator);

    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.FleetPaginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._profileServiceProxy
      .getFleetInformation(
        this.giverUserId,
        this.primengTableHelper.getSorting(this.FleetTable),
        this.primengTableHelper.getSkipCount(this.FleetPaginator, event),
        this.primengTableHelper.getMaxResultCount(this.FleetPaginator, event)
      )
      .subscribe((result) => {
        this.totalDrivers = result.totalDrivers;
        this.primengTableHelper.totalRecordsCount = result.availableTrucksDto.totalCount;
        this.primengTableHelper.records = result.availableTrucksDto.items;
        this.primengTableHelper.hideLoadingIndicator();
        this.loadingForFleet = false;
      });
  }
  reloadPageForFleet(): void {
    this.FleetPaginator.changePage(this.FleetPaginator.getPage());
  }
}
