import { Component, ViewChild, Injector, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  FacilityForDropdownDto,
  RoutStepsServiceProxy,
  ShippingRequestsTripServiceProxy,
  CreateOrEditShippingRequestTripVasDto,
  ShippingRequestsTripForViewDto,
  TrucksServiceProxy,
  WaybillsServiceProxy,
  SelectItemDto,
  AssignDriverAndTruckToShippmentByCarrierInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';

@Component({
  selector: 'viewTripModal',
  templateUrl: './viewTripModal.component.html',
})
export class ViewTripModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('viewTripDetails', { static: false }) modal: ModalDirective;
  // @ViewChild('wayPointsComponent') wayPointsComponent: PointsComponent;
  Vases: CreateOrEditShippingRequestTripVasDto[];
  selectedVases: CreateOrEditShippingRequestTripVasDto[];
  allFacilities: FacilityForDropdownDto[];
  trip: ShippingRequestsTripForViewDto = new ShippingRequestsTripForViewDto();
  assignDriverAndTruck: AssignDriverAndTruckToShippmentByCarrierInput = new AssignDriverAndTruckToShippmentByCarrierInput();
  facilityLoading = false;
  allDrivers: SelectItemDto[] = [];
  allTrucks: SelectItemDto[] = [];
  saving = false;
  loading = true;
  currentTripId: number;
  wayBillIsDownloading = false;
  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _PointsService: PointsService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.feature.isEnabled('App.Carrier')) {
      this.getAllTrucks();
      this.getAllDrivers();
    }
  }

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
        //Get The Points From The View Service and send them to the Points Service To Draw Them
        this._PointsService.updateWayPoints(this.trip.routPoints);
        this.assignDriverAndTruck.assignedTruckId = this.trip.assignedTruckId;
        this.assignDriverAndTruck.assignedDriverUserId = this.trip.assignedDriverUserId;
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

  /**
   * Driver Assignation Section
   * this method is for Getting All Carriers Drivers For DD
   */
  getAllDrivers() {
    if (this.feature.isEnabled('App.Carrier')) {
      this._trucksServiceProxy.getAllDriversForDropDown().subscribe((res) => {
        this.allDrivers = res;
      });
    }
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks() {
    if (this.feature.isEnabled('App.Carrier')) {
      this._trucksServiceProxy.getAllCarrierTrucksForDropDown().subscribe((res) => {
        this.allTrucks = res;
      });
    }
  }

  /**
   * this function is to assign Driver And Truck To shipping Request Trip
   */
  assignDriverandTruck() {
    this.saving = true;
    this.assignDriverAndTruck.id = this.currentTripId;
    this._shippingRequestTripsService
      .assignDriverAndTruckToShippmentByCarrier(this.assignDriverAndTruck)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.success('driverandTrucksAssignedSuccessfully');
      });
  }
}
