import { Component, Injector, ViewChild, ChangeDetectorRef, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import {
  CreateOrEditShippingRequestVasListDto,
  GetShippingRequestForViewOutput,
  GetShippingRequestVasForViewDto,
  ShippingRequestDto,
  ShippingRequestsTripServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AddNewTripComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/AddNewTripModal/AddNewTrip.component';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';

@Component({
  selector: 'TripsForViewShippingRequest',
  templateUrl: './TripsForViewShippingRequest.html',
  styleUrls: ['./TripsForViewShippingRequest.scss'],
})
export class TripsForViewShippingRequestComponent extends AppComponentBase {
  @ViewChild('dataTablechild', { static: false }) dataTable: Table;
  @ViewChild('paginatorchild', { static: false }) paginator: Paginator;
  @ViewChild('AddNewTripModal', { static: false }) AddNewTripModal: AddNewTripComponent;
  @Input() ShippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];
  constructor(
    injector: Injector,
    private changeDetectorRef: ChangeDetectorRef,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy
  ) {
    super(injector);
  }

  getShippingRequestsTrips(event?: LazyLoadEvent) {
    this.changeDetectorRef.detectChanges();
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._shippingRequestTripsService
      .getAll(
        this.ShippingRequest.id,
        0,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
}
