import { AfterViewInit, ChangeDetectorRef, Component, Injector, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';

import {
  GetShippingRequestVasForViewDto,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  ShippingRequestsTripServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { CreateOrEditTripComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/createOrEditTripModal/createOrEditTrip.component';
import { ViewTripModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/viewTripModal/viewTripModal.component';
import { TripService } from '../trip.service';
import Swal from 'sweetalert2';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'TripsForViewShippingRequest',
  templateUrl: './tripsForViewShippingRequest.component.html',
  styleUrls: ['./tripsForViewShippingRequest.component.scss'],
})
export class TripsForViewShippingRequestComponent extends AppComponentBase implements AfterViewInit {
  @ViewChild('dataTablechild', { static: false }) dataTable: Table;
  @ViewChild('paginatorchild', { static: false }) paginator: Paginator;
  @ViewChild('AddNewTripModal', { static: false }) AddNewTripModal: CreateOrEditTripComponent;
  @ViewChild('ViewTripModal', { static: false }) ViewTripModal: ViewTripModalComponent;
  @ViewChild('saveAsTemplateModal', { static: false }) saveAsTemplateModal: ViewTripModalComponent;
  @Input() ShippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];
  tripsByTmsEnabled: boolean;
  saving = false;
  constructor(
    injector: Injector,
    private _TripService: TripService,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
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
        undefined,
        this.primengTableHelper.getSorting(this.dataTable) || 'startTripDate ASC',
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  updateAddTripsByTmsFeature() {
    Swal.fire({
      title: this.l('areYouSure'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.saving = true;
        this._shippingRequestTripsService.changeAddTripsByTmsFeature().subscribe(() => {
          this.tripsByTmsEnabled = !this.tripsByTmsEnabled;
          this.saving = false;
        });
      } //end of if
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  ngAfterViewInit(): void {
    // update Trip Service and send vases list to trip component
    this._shippingRequestsServiceProxy.getShippingRequestForView(this.ShippingRequest.id).subscribe((result) => {
      this.VasListFromFather = result.shippingRequestVasDtoList;
      this._TripService.updateShippingRequest(result);
    });

    this.primengTableHelper.adjustScroll(this.dataTable);
    this.tripsByTmsEnabled = this.ShippingRequest.addTripsByTmsEnabled;
  }
}
