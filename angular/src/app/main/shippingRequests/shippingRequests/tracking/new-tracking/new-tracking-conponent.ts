import { Component, ElementRef, Injector, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  InvokeStatusInputDto,
  PickingType,
  PointTransactionDto,
  RoutPointTransactionDto,
  ShippingRequestRouteType,
  ShippingRequestTripDriverRoutePointDto,
  ShippingRequestTripDriverStatus,
  ShippingRequestTripStatus,
  ShippingRequestType,
  TrackingListDto,
  TrackingRoutePointDto,
  TrackingServiceProxy,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';
import { TrackingPODModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-pod-model.component';
import { NgbDropdownConfig } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'new-tracking-conponent',
  templateUrl: './new-tracking-conponent.html',
  styleUrls: ['./new-tracking-conponent.css'],
  providers: [NgbDropdownConfig],
  animations: [appModuleAnimation()],
})
export class NewTrackingConponent extends AppComponentBase implements OnChanges {
  @ViewChild('modelconfirm', { static: false }) modelConfirmCode: TrackingConfirmModalComponent;
  @ViewChild('modelpod', { static: false }) modelpod: TrackingPODModalComponent;
  @Input() trip: TrackingListDto = new TrackingListDto();
  active = false;
  item: number;
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  routePoints: TrackingRoutePointDto[];
  pointsIsLoading = true;
  distance: string;
  duration: string;
  readonly zoom: number = 15;
  saving = false;
  markerLong: number;
  markerLat: number;
  markerFacilityName: string;
  activeIndex: number = 1;
  driverStatusesEnum = ShippingRequestTripDriverStatus;
  tripStatusesEnum = ShippingRequestTripStatus;
  pickingTypeEnum = PickingType;
  routeTypeEnum = ShippingRequestRouteType;
  ShippingRequestTypeEnum = ShippingRequestType;
  canStartAnotherPoint = false;
  dropWaybillLoadingId: number;
  busyPointId: number;

  constructor(
    injector: Injector,
    private elRef: ElementRef,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    config: NgbDropdownConfig
  ) {
    super(injector);
    config.autoClose = true;
    config.container = 'body';
  }

  /**
   * to Detect Changes On Variables
   * @param changes
   */
  ngOnChanges(changes: SimpleChanges) {
    this.getForView();
    this.getCordinatesByCityName(this.trip.origin, 'source');
    this.getCordinatesByCityName(this.trip.destination, 'destanation');
  }

  /**
   * Show Item
   * @param item
   */
  show(item: TrackingListDto) {
    this.active = true;
    this.trip = item;
    this.getForView();
    // this.modal.show();
  }

  /**
   * syncs the currents Trip Status with Father Component
   */
  syncTripInGetForView(trip: TrackingListDto) {
    abp.event.on('TripAccepted', () => this.getForView());
    abp.event.trigger('TripDataChanged', trip);
  }

  /**
   * get the trip for view
   */
  getForView() {
    this.pointsIsLoading = true;
    this._trackingServiceProxy
      .getForView(this.trip.id)
      .pipe(
        finalize(() => {
          this.pointsIsLoading = false;
          this.busyPointId = null;
        })
      )
      .subscribe((result) => {
        this.trip.status = result.status;
        this.trip.canStartTrip = result.canStartTrip;
        this.trip.driverStatus = result.driverStatus;
        this.trip.statusTitle = result.statusTitle;
        this.routePoints = result.routPoints;
        this.syncTripInGetForView(this.trip);
        this.handleCanGoNextLocation(result.routPoints);
      });
  }

  /**
   * starts a trip
   */
  start(): void {
    this.saving = true;
    this.busyPointId = this.routePoints[0].id;

    this._trackingServiceProxy
      .start(this.trip.id)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.getForView();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyStarted'));
      });
  }

  /**
   * Get City Cordinates By Providing its name
   * this finction is to draw the shipping Request Main Route in View SR Details in marketPlace
   * @param cityName
   * @param cityType   source/dest
   */
  getCordinatesByCityName(cityName: string, cityType: string) {
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode(
      {
        address: cityName,
      },
      (results, status) => {
        if (status == google.maps.GeocoderStatus.OK) {
          const Lat = results[0].geometry.location.lat();
          const Lng = results[0].geometry.location.lng();
          if (cityType == 'source') {
            this.origin = { lat: Lat, lng: Lng };
          } else {
            this.destination = { lat: Lat, lng: Lng };
            this.messuareDistance(this.origin, this.destination);
          }
        } else {
          console.log('Something got wrong ' + status);
        }
      }
    );
  }

  /**
   * Measure the Distance Between 2 Points using Cordinates
   * @param Oring { lat: null, lng: null }
   * @param destination { lat: null, lng: null }
   */

  messuareDistance(origin, destination) {
    origin = this.origin;
    destination = this.destination;
    const service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix(
      {
        origins: [{ lat: origin.lat, lng: origin.lng }],
        destinations: [{ lat: destination.lat, lng: destination.lng }],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false,
      },
      (response, status) => {
        this.distance = response.rows[0].elements[0].distance.text;
        this.duration = response.rows[0].elements[0].duration.text;
      }
    );
  }

  /**
   * accepts the trip
   */
  accept(tripId?: number): void {
    this.busyPointId = this.routePoints[0].id;
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this._trackingServiceProxy
          .accept(this.trip.id || tripId)
          .pipe(
            finalize(() => {
              this.saving = false;
              this.getForView();
            })
          )
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyAccepted'));
          });
      }
    });
  }

  /**
   * time line Panel Click
   * @param $event
   */
  timeLinePanelClick(point: TrackingRoutePointDto) {
    this.item = point.id;
    if (this.markerLong == point.lng && this.markerLat == point.lat) {
      this.markerLong = null;
      this.markerLat = null;
      this.markerFacilityName = '';
      return;
    }
    this.markerLong = point.lng;
    this.markerLat = point.lat;
    this.markerFacilityName = '';
  }

  /**
   * handels Pod Uploading Process
   * @param point
   * @param transaction
   * @private
   */
  private handleUploadPod(point: TrackingRoutePointDto, transaction: PointTransactionDto) {
    this.modelpod.show(point.id, transaction.action);
    abp.event.on('PodUploadedSuccess', () => {
      this.getForView();
    });
  }

  /**
   * handles the Trip Confirmation code process
   * opens the modal the send a request with the code to the invoke service and if code is correct refresh the data (getforView)
   * @param point
   * @param transaction
   */
  private handleDeliveryConfirmationCode(point: TrackingRoutePointDto, transaction: PointTransactionDto) {
    //show the modal
    this.modelConfirmCode.show(point.id, transaction.action);
    abp.event.on('trackingConfirmCodeSubmitted', () => {
      this.getForView();
    });
  }

  /**
   * handels the upload proccess of a delivery note for the points
   * @param point
   * @param transaction
   * @private
   */
  private handleUploadDeliveryNotes(point: TrackingRoutePointDto, transaction: PointTransactionDto) {
    this.modelpod.show(point.id, transaction.action);
    abp.event.on('tripDeliveryNotesUploadSuccess', () => {
      this.getForView();
    });
  }

  /**
   * the core function of the tracking
   * its used to send the transaction selected by user in point action dropdown
   * @param point
   * @param transaction
   */
  invokeStatus(point: TrackingRoutePointDto, transaction: PointTransactionDto) {
    this.saving = true;
    const invokeRequestBody = new InvokeStatusInputDto();
    invokeRequestBody.id = point.id;
    invokeRequestBody.action = transaction.action;
    //TODO  - Dont Forget to take the secound acound that requires pod for karam
    if (
      transaction.action === 'FinishOffLoadShipmentDeliveryConfirmation' ||
      transaction.action === 'DeliveryConfirmation' ||
      transaction.action === 'UplodeDeliveryNoteDeliveryConfirmation'
    ) {
      //handle upload Pod
      //this.busyPointId = null;

      return this.handleUploadPod(point, transaction);
    }
    if (transaction.action === 'UplodeDeliveryNote') {
      // this.saving = false;
      // this.busyPointId = null;

      return this.handleUploadDeliveryNotes(point, transaction);
    }
    if (transaction.action === 'ReceiverConfirmed' || transaction.action === 'DeliveryConfirmationReceiverConfirmed') {
      // this.saving = false;
      // this.busyPointId = null;
      return this.handleDeliveryConfirmationCode(point, transaction);
    }
    this.busyPointId = point.id;

    this._trackingServiceProxy
      .invokeStatus(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
          //this.busyPointId = null;
        })
      )
      .subscribe(() => {
        this.getForView();
      });
  }

  /**
   * creates the Steps for the primeng stepper
   * @param statues
   */
  getStepperSteps(statues: RoutPointTransactionDto[]): {} {
    let items = [];
    //TODO change active index back to null
    this.activeIndex = null;
    for (let i = 0; i < statues.length; i++) {
      items.push({
        label: statues[i].name,
        styleClass: 'completed',
      });
      if (statues[i].isDone) {
        this.activeIndex = i;
      }
    }

    return items;
  }

  /**
   * go to next Location
   * @param point
   */
  nextLocation(point: TrackingRoutePointDto): void {
    this.saving = true;
    this.busyPointId = point.id;
    this._trackingServiceProxy
      .nextLocation(point.id)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.getForView();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyChanged'));
      });
  }

  private handleCanGoNextLocation(routPoints: TrackingRoutePointDto[]): boolean {
    const singleDrop = this.routeTypeEnum.SingleDrop;
    const MultipleDrops = this.routeTypeEnum.MultipleDrops;
    const canStartAnotherPoint = routPoints.find((item) => item.canGoToNextLocation === true) ? true : false;
    console.log(
      'routPoints.find((item) => item.canGoToNextLocation)',
      routPoints.find((item) => item.canGoToNextLocation)
    );
    console.log('routPoints', routPoints);
    console.log('canStartAnotherPoint', canStartAnotherPoint);
    //single Drop
    if (this.trip.routeTypeId === singleDrop && canStartAnotherPoint) {
      console.log('should go next');

      this.saving = true;
      this.nextLocation(this.routePoints[1]);
      this.notify.info('CanGoNextForSingleDrop');
    }
    //for Multible Drops
    if (this.trip.routeTypeId === MultipleDrops && canStartAnotherPoint && routPoints[0].isComplete) {
      this.notify.info('CanGoNext');
      console.log('can Go Next For MultiDrops');
      this.canStartAnotherPoint = canStartAnotherPoint;
    }
    return (this.canStartAnotherPoint = canStartAnotherPoint);
  }

  /**
   * downloads the wayBill For MultiDrops Points
   * @param id
   */
  downloadMultiDropPointWaybill(id: number) {
    this.dropWaybillLoadingId = id;
    this._waybillsServiceProxy.getMultipleDropWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.dropWaybillLoadingId = null;
    });
  }

  canDoActionsOnPoints(point: TrackingRoutePointDto): boolean {
    //prevent any Points Actions if the trip hasAccident
    if (this.trip.hasAccident) {
      return false;
    }
    //singleDrop and there is available transactions
    if (this.trip.routeTypeId === this.routeTypeEnum.SingleDrop && point.availableTransactions.length !== 0) {
      return true;
    }

    if (this.trip.routeTypeId === this.routeTypeEnum.MultipleDrops && this.trip.status !== this.tripStatusesEnum.New) {
      if (point.availableTransactions.length !== 0 || this.canStartAnotherPoint) {
        return true;
      }
    }

    //multiDrops
    //return false;
    // if (!this.canStartAnotherPoint &&  this.trip.routeTypeId === this.routeTypeEnum.MultipleDrops) && !point.isResolve || this.trip.routeTypeId === this.routeTypeEnum.SingleDrop && this.routePoints[1].availableTransactions.length === 0)
  }
}
