import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from '@node_modules/primeng/api';

@Component({
  selector: 'profile-shipper-facilities',
  templateUrl: './shipper-facilities.component.html',
  styleUrls: ['./shipper-facilities.component.css'],
})
export class ShipperFacilitiesComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTableFacilities', { static: true }) dataTableFacilities: Table;
  @ViewChild('paginatorFacilities', { static: true }) paginatorFacilities: Paginator;
  @Output() emitFacilityCordinates: EventEmitter<any> = new EventEmitter<any>();
  @Input() giverUserId: number;
  facilityCord = { longitude: 1, latitude: 1 };
  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getAllFacilities(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorFacilities.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._profileServiceProxy
      .getFacilitiesInformation(
        this.giverUserId,
        this.primengTableHelper.getSorting(this.dataTableFacilities),
        this.primengTableHelper.getSkipCount(this.paginatorFacilities, event),
        this.primengTableHelper.getMaxResultCount(this.paginatorFacilities, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  /**
   * On Map Click
   * @param event
   */
  onFacilityClick(event) {
    this.facilityCord.longitude = event.longitude;
    this.facilityCord.latitude = event.latitude;
    this.emitFacilityCordinates.emit(this.facilityCord);
  }
}
