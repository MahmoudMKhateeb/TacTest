import { Component, ViewChild, Injector, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  FacilityForDropdownDto,
  RoutStepsServiceProxy,
  ShippingRequestsTripServiceProxy,
  CreateOrEditRoutPointDto,
  CreateOrEditShippingRequestTripVasDto,
  ShippingRequestsTripForViewDto,
  TrucksServiceProxy,
  AssignDriverAndTruckToShippmentByCarrierInput,
  ISelectItemDto,
  SelectItemDto,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { PointsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.component';
import Swal from 'sweetalert2';
import { AssignDriverTruckModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/assignDriverTruckModal/assignDriverTruckModal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'viewTripModal',
  templateUrl: './viewTripModal.component.html',
})
export class ViewTripModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('viewTripDetails', { static: false }) modal: ModalDirective;
  // @ViewChild('wayPointsComponent') wayPointsComponent: PointsComponent;
  // @ViewChild('assignDriverTruckModal', { static: true }) assignDriverTruckModal: AssignDriverTruckModalComponent;

  Vases: CreateOrEditShippingRequestTripVasDto[];
  selectedVases: CreateOrEditShippingRequestTripVasDto[];
  allFacilities: FacilityForDropdownDto[];
  trip: ShippingRequestsTripForViewDto = new ShippingRequestsTripForViewDto();

  facilityLoading = false;
  saving = false;
  loading = true;

  currentTripId: number;
  wayBillIsDownloading = false;
  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {}

  show(id): void {
    this.loading = true;
    this.currentTripId = id;
    this._shippingRequestTripsService
      .getShippingRequestTripForView(id)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.trip = res;
      });

    this.modal.show();
  }

  close(): void {
    this.trip = new ShippingRequestsTripForViewDto();
    //this.wayPointsComponent.wayPointsList = [];
    this.loading = true;
    this.modal.hide();
  }

  DownloadSingleDropWaybillPdf(id: number): void {
    this.wayBillIsDownloading = true;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.wayBillIsDownloading = false;
    });
  }
}
