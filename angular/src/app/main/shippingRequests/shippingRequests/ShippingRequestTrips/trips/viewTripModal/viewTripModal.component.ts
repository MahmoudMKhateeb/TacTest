import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  AssignDriverAndTruckToShippmentByCarrierInput,
  CreateOrEditShippingRequestTripVasDto,
  DedicatedShippingRequestsServiceProxy,
  GetShippingRequestForViewOutput,
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestDriverServiceProxy,
  ShippingRequestRouteType,
  ShippingRequestsTripForViewDto,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripStatus,
  TrucksServiceProxy,
  UpdateExpectedDeliveryTimeInput,
  WaybillsServiceProxy,
  GetAllDedicatedDriversOrTrucksForDropDownDto,
  ShippingRequestFlag,
  ShippingRequestTripFlag,
  DropPaymentMethod,
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
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

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

  canAssignTrucksAndDrivers: boolean;
  fromTime: string;
  toTime: string;
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
  shippingRequestForView: GetShippingRequestForViewOutput;
  allDedicatedDrivers: GetAllDedicatedDriversOrTrucksForDropDownDto[];
  allDedicatedTrucks: GetAllDedicatedDriversOrTrucksForDropDownDto[];
  routeTypes: any[] = [];
  RouteTypesEnum = ShippingRequestRouteType;
  ShippingRequestFlagEnum = ShippingRequestFlag;
  ShippingRequestTripFlagEnum = ShippingRequestTripFlag;
  ShippingRequestTripFlagArray = [];

  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy,
    public _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _PointsService: PointsService,
    private _TripService: TripService,
    private _Router: ActivatedRoute,
    private _changeDetectorRef: ChangeDetectorRef,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit() {
    this._TripService.currentShippingRequest.subscribe((res) => {
      this.TruckTypeId = res?.truckTypeId;
    });
    this.ShippingRequestTripFlagArray = this.enumToArray.transform(ShippingRequestTripFlag);
  }

  ngAfterViewInit() {
    if (isNotNullOrUndefined(this.activeTripId)) {
      this.show(this.activeTripId, this.shippingRequestForView);
    }
  }

  show(id, shippingRequestForView?: GetShippingRequestForViewOutput): void {
    this.shippingRequestForView = shippingRequestForView;
    if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === this.ShippingRequestFlagEnum.Dedicated) {
      this.getAllDedicatedDriversForDropDown();
      this.getAllDedicateTrucksForDropDown();
      this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType);
    }
    this.loading = true;
    this.currentTripId = id;
    //update the active trip id in TripsService
    this._TripService.updateActiveTripId(id);
    this.getAllTrucks(this.TruckTypeId);
    this.getAllDrivers();

    this._shippingRequestTripsService
      .getShippingRequestTripForView(id)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.trip = res;
        this.fromTime = res.supposedPickupDateFrom?.format('HH:mm');
        this.toTime = res.supposedPickupDateTo?.format('HH:mm');
        //Get The Points From The View Service and send them to the Points Service To Draw Them
        this._PointsService.updateWayPoints(this.trip.routPoints);
        this._PointsService.updateCurrentUsedIn('view');
        this.pickUpPointSender = res.routPoints.length > 0 ? res.routPoints[0].senderOrReceiverContactName : null;
        this.assignDriverAndTruck.assignedTruckId = this.trip.assignedTruckId;
        this.assignDriverAndTruck.assignedDriverUserId = this.trip.assignedDriverUserId;
        this.assignDriverAndTruck.containerNumber = this.trip.containerNumber;
        this.assignDriverAndTruck.sealNumber = this.trip.sealNumber;
        this.expectedDeliveryTime = this.trip.expectedDeliveryTime;
        this._changeDetectorRef.detectChanges();
        this.canAssignTrucksAndDrivers = res.canAssignDriversAndTrucks;
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
    this._changeDetectorRef.detectChanges();
  }

  checkData(category) {
    if (category === 'truck' && this.allTrucks.length === 0) {
      this.getAlert(this.l('NoMatchingTrucks'));
    }
    if (category === 'driver' && this.allDrivers.length === 0) {
      this.getAlert(this.l('NoMatchingDrivers'));
    }
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
    if (this.isTachyonDealer || this.isCarrier || this.hasCarrierClients) {
      this._trucksServiceProxy.getDriversByShippingRequestId(this.shippingRequestForView.shippingRequest.id).subscribe((result) => {
        this.allDrivers = result;
      });
    }
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks(truckTypeId) {
    if (this.isTachyonDealer || this.isCarrier || this.hasCarrierClients) {
      this._trucksServiceProxy.getTrucksByShippingRequestId(truckTypeId, this.shippingRequestForView.shippingRequest.id).subscribe((result) => {
        this.allTrucks = result;
      });
    }
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
    if (!isNotNullOrUndefined(this.currentTripId)) {
      return;
    }
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

  private getAllDedicatedDriversForDropDown() {
    this._dedicatedShippingRequestsServiceProxy.getAllDedicatedDriversForDropDown(this.shippingRequestForView.shippingRequest.id).subscribe((res) => {
      this.allDedicatedDrivers = res;
    });
  }

  private getAllDedicateTrucksForDropDown() {
    this._dedicatedShippingRequestsServiceProxy.getAllDedicateTrucksForDropDown(this.shippingRequestForView.shippingRequest.id).subscribe((res) => {
      this.allDedicatedTrucks = res;
    });
  }

  DriverOrTruckSelected(driverUserId?: number, truckId?: number) {
    if (isNotNullOrUndefined(driverUserId)) {
      this._trucksServiceProxy.getTruckByDriverId(driverUserId, this.shippingRequestForView.truckTypeId).subscribe((result) => {
        if (!isNotNullOrUndefined(this.assignDriverAndTruck.assignedTruckId)) this.assignDriverAndTruck.assignedTruckId = result;
      });
    } else if (isNotNullOrUndefined(truckId)) {
      this._trucksServiceProxy.getDriverByTruckId(truckId).subscribe((result) => {
        if (!isNotNullOrUndefined(this.assignDriverAndTruck.assignedDriverUserId)) this.assignDriverAndTruck.assignedDriverUserId = result;
      });
    }
  }
}
