import { Component, ElementRef, Injector, Input, OnChanges, SimpleChanges, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  GetAllUploadedFileDto,
  InvokeStatusInputDto,
  PickingType,
  PointTransactionDto,
  RoutPointTransactionDto,
  ShippingRequestRouteType,
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
import { EntityLogComponent } from '@app/shared/common/entity-log/entity-log.component';
import { AngularFireDatabase, AngularFireList } from '@angular/fire/compat/database';

@Component({
  selector: 'new-tracking-conponent',
  templateUrl: './new-tracking-conponent.html',
  styleUrls: ['./new-tracking-conponent.scss'],
  providers: [NgbDropdownConfig],
  animations: [appModuleAnimation()],
})
export class NewTrackingConponent extends AppComponentBase implements OnChanges {
  @ViewChild('modelconfirm', { static: false }) modelConfirmCode: TrackingConfirmModalComponent;
  @ViewChild('modelpod', { static: false }) modelpod: TrackingPODModalComponent;
  @ViewChild('appEntityLog', { static: false }) activityLogModal: EntityLogComponent;
  @Input() trip: TrackingListDto = new TrackingListDto();
  active = false;
  item: number;

  routePoints: TrackingRoutePointDto[];
  pointsIsLoading = true;
  distance: string;
  duration: string;
  readonly zoom: number = 12;
  saving = false;
  markerLong: number;
  markerLat: number;
  markerFacilityName: string;
  activeIndex = 1;
  driverStatusesEnum = ShippingRequestTripDriverStatus;
  tripStatusesEnum = ShippingRequestTripStatus;
  pickingTypeEnum = PickingType;
  routeTypeEnum = ShippingRequestRouteType;
  ShippingRequestTypeEnum = ShippingRequestType;
  canStartAnotherPoint = false;
  dropWaybillLoadingId: number;
  busyPointId: number;
  loadPodForPointId: number;
  pointPodList: GetAllUploadedFileDto[];
  deliveryGoodPictureId: number;
  mapToggle = true;
  private fireDB: AngularFireList<unknown>;
  driverLng: number;
  driverlat: number;

  newReceiverCode: string;
  tripRoute = {
    origin: { lat: null, lng: null },
    wayPoints: [],
    destination: { lat: null, lng: null },
  };
  driversToggle = true;
  tripsToggle = true;
  constructor(
    injector: Injector,
    private elRef: ElementRef,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    config: NgbDropdownConfig,
    private _db: AngularFireDatabase
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
    this.getDriverLiveLocation();
  }

  /**
   * when map is ready do stuff
   * @param event
   */
  mapReady(event: any) {
    event.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById('Settings'));
  }

  /**
   * Get source/Destionation/WayPoints For Map Drawing from Drop Points
   */
  getTripRouteForMap() {
    this.tripRoute.origin.lng = this.routePoints[0].lng;
    this.tripRoute.origin.lat = this.routePoints[0].lat;
    this.tripRoute.destination.lng = this.routePoints[this.routePoints.length - 1].lng;
    this.tripRoute.destination.lat = this.routePoints[this.routePoints.length - 1].lat;
    for (let i = 1; i < this.routePoints.length - 1; i++) {
      this.tripRoute.wayPoints.push({
        location: {
          lat: this.routePoints[i].lat,
          lng: this.routePoints[i].lng,
        },
      });
    }
  }

  /**
   * get live Driver Location
   */
  getDriverLiveLocation() {
    this.fireDB = this._db.list('maps', (ref) => ref.orderByChild('tripId').equalTo(this.trip.id));
    this.fireDB.valueChanges().subscribe((res: any) => {
      console.log(res);
      this.driverLng = res[0].lng;
      this.driverlat = res[0].lat;
    });
  }
  /**
   * Show Item
   * @param item
   */
  show(item: TrackingListDto) {
    this.active = true;
    this.trip = item;
    this.getForView();
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
        this.trip.driverStatus = result.driverStatus;
        this.trip.statusTitle = result.statusTitle;
        this.routePoints = result.routPoints;
        this.syncTripInGetForView(this.trip);
        this.handleCanGoNextLocation(result.routPoints);
        this.getTripRouteForMap();
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

  accept(tripId?: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this.busyPointId = this.routePoints[0].id;
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
      return this.handleUploadPod(point, transaction);
    }
    if (transaction.action === 'UplodeDeliveryNote') {
      // this.saving = false;
      return this.handleUploadDeliveryNotes(point, transaction);
    }
    if (transaction.action === 'ReceiverConfirmed' || transaction.action === 'DeliveryConfirmationReceiverConfirmed') {
      // this.saving = false;
      return this.handleDeliveryConfirmationCode(point, transaction);
    }
    this.busyPointId = point.id;

    this._trackingServiceProxy
      .invokeStatus(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
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
  getStepperSteps(statues: RoutPointTransactionDto[]): any[] {
    let items = [];
    //TODO change active index back to null
    this.activeIndex = null;
    for (let i = 0; i < statues.length; i++) {
      items.push({
        index: i + 1,
        status: statues[i].name,
        date: statues[i].name,
        icon: 'flaticon2-checkmark',
        time: statues[i].creationTime,
        isDone: statues[i].isDone,
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

  /**
   * gets the Pod List For a specific Point
   * @param point
   */
  getPodListForPoint(point: TrackingRoutePointDto): void {
    this.loadPodForPointId = point.id;
    this.pointPodList = null;
    this._trackingServiceProxy
      .pOD(point.id)
      .pipe(
        finalize(() => {
          this.loadPodForPointId = null;
        })
      )
      .subscribe((res) => {
        this.pointPodList = res;
      });
  }

  /**
   * download a pod file
   * @param pod
   */
  downloadPOD(pod: GetAllUploadedFileDto): void {
    this._fileDownloadService.downloadFileByBinary(pod.documentId, pod.fileName, pod.fileType);
  }

  showPointLog(pointId: number) {
    this.activityLogModal.entityId = pointId;
    this.activityLogModal.show();
  }

  /**
   * hide or show tracking map
   */
  toggleMap() {
    this.mapToggle = !this.mapToggle;
  }

  // Reset Rout Point Receiver Code By Host
  resetReceiverCode(pointId: number) {
    this.message.confirm(this.l('ResetReceiverCode'), this.l('ReceiverCodeConfirmationMsg'), (isConfirmed) => {
      if (isConfirmed) {
        this._trackingServiceProxy.resetPointReceiverCode(pointId).subscribe((result) => {
          this.notify.success(this.l('ResetReceiverCodeSuccessfully'));
          this.newReceiverCode = result;
        });
      }
    });
  }

  /**
   * show or hide drivers  from the map
   */
  driverToggle() {
    this.driversToggle = !this.driversToggle;
  }

  /**
   * show or hide trips from the map
   */
  tripToggle() {
    this.tripsToggle = !this.tripsToggle;
  }
}
