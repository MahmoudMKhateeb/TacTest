import { AfterViewInit, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestDriverServiceProxy,
  ShippingRequestTripStatus,
  TrucksServiceProxy,
  WaybillsServiceProxy,
  UpdateExpectedDeliveryTimeInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import Swal from 'sweetalert2';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { ActivatedRoute } from '@angular/router';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';
import { Moment } from '@node_modules/moment';

@Component({
  selector: 'viewTripModal',
  templateUrl: './viewTripModal.component.html',
  styleUrls: ['./viewTripModal.component.scss'],
})
export class ViewTripModalComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('viewTripDetails', { static: false }) modal: ModalDirective;
  @ViewChild('TripNotesModal', { static: false }) TripNotesModal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter();
  @ViewChild('fileViwerComponent', { static: false }) fileViwerComponent: FileViwerComponent;

  Vases: CreateOrEditShippingRequestTripVasDto[];
  trip: ShippingRequestsTripForViewDto = new ShippingRequestsTripForViewDto();
  assignDriverAndTruck: AssignDriverAndTruckToShippmentByCarrierInput = new AssignDriverAndTruckToShippmentByCarrierInput();
  allDrivers: SelectItemDto[] = [];
  allTrucks: SelectItemDto[] = [];
  saving = false;
  loading = true;
  currentTripId: number;
  wayBillIsDownloading = false;
  isResetTripLoading = false;
  private TruckTypeId: number;
  pickUpPointSender: string;
  activeTripId: any;
  type = 'Trip';
  shippingRequestTripStatusEnum = ShippingRequestTripStatus;
  expectedDeliveryTime: moment.Moment;
  originalExpectedDeliveryTime: Moment;
  expectedDeliveryTimeLoading: boolean;
  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    public _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _PointsService: PointsService,
    private _TripService: TripService,
    private _Router: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit() {
    this._TripService.currentShippingRequest.subscribe((res) => {
      this.TruckTypeId = res.truckTypeId;
    });
  }

  ngAfterViewInit() {
    if (isNotNullOrUndefined(this.activeTripId)) {
      this.show(this.activeTripId);
    }
  }

  show(id): void {
    this.loading = true;
    this.currentTripId = id;
    //update the active trip id in TripsService
    this._TripService.updateActiveTripId(id);
    if (this.feature.isEnabled('App.Carrier') || this.isTachyonDealer) {
      this.getAllTrucks(this.TruckTypeId);
      this.getAllDrivers();
    }
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
        this._PointsService.updateCurrentUsedIn('view');
        this.pickUpPointSender = res.routPoints[0].senderOrReceiverContactName;
        this.assignDriverAndTruck.assignedTruckId = this.trip.assignedTruckId;
        this.assignDriverAndTruck.assignedDriverUserId = this.trip.assignedDriverUserId;
        this.expectedDeliveryTime = this.trip.expectedDeliveryTime;
        this.originalExpectedDeliveryTime = this.expectedDeliveryTime;
      });

    this.modal.show();
  }

  close(): void {
    this.trip = new ShippingRequestsTripForViewDto();
    //this.wayPointsComponent.wayPointsList = [];
    this.allDrivers = [];
    this.allTrucks = [];
    this.loading = true;
    this._PointsService.updateWayPoints([]);
    this.modal.hide();
  }

  checkData(category) {
    if (category == 'truck' && this.allTrucks.length == 0) this.getAlert(this.l('NoMatchingTrucks'));
    if (category == 'driver' && this.allDrivers.length == 0) this.getAlert(this.l('NoMatchingDrivers'));
  }

  getAlert(msg: string) {
    Swal.fire({
      title: msg,
      icon: 'warning',
      confirmButtonText: this.l('Ok'),
    });
  }

  DownloadSingleDropWaybillPdf(id: number): void {
    this.wayBillIsDownloading = true;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.fileViwerComponent.show(this._fileDownloadService.downloadTempFile(result), 'pdf');
      this.wayBillIsDownloading = false;
    });
  }

  /**
   * Driver Assignation Section
   * this method is for Getting All Carriers Drivers For DD
   */
  getAllDrivers() {
    this._trucksServiceProxy.getAllDriversForDropDown(this.currentTripId).subscribe((res) => {
      this.allDrivers = res;
    });
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks(truckTypeId) {
    this._trucksServiceProxy.getAllCarrierTrucksByTruckTypeForDropDown(truckTypeId, this.currentTripId).subscribe((res) => {
      this.allTrucks = res;
    });
  }

  getNotes() {
    this.TripNotesModal.show();
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

  /**
   * Resets Shipping Request Trip
   * @param tripId
   */
  ResetTrip(tripId: number) {
    Swal.fire({
      title: this.l('areYouSure'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.isResetTripLoading = true;
        this._shippingRequestDriverServiceProxy.reset(tripId).subscribe(() => {
          this.isResetTripLoading = false;
          this.notify.info(this.l('SuccessfullyReseated'));
          this.modalSave.emit('');
        });
      } //end of if
    });
  }

  /**
   * update the expected Delivery Time Of the Trip
   */
  updateTripExpectedDeliveryTime() {
    if (!isNotNullOrUndefined(this.currentTripId)) return;
    if (this.expectedDeliveryTime === this.originalExpectedDeliveryTime) return;
    // console.log('Trip Expected Delivery time Was Updated');
    this.expectedDeliveryTimeLoading = true;
    let body = new UpdateExpectedDeliveryTimeInput();
    body.id = this.currentTripId;
    body.expectedDeliveryTime = this.expectedDeliveryTime;
    this._shippingRequestTripsService.updateExpectedDeliveryTimeForTrip(body).subscribe((res) => {
      this.expectedDeliveryTimeLoading = false;
      this.notify.success(this.l('TripExpectedDateWasUpdated'));
    });
  }
}
