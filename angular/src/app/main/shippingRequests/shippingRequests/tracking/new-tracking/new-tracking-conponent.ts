import { Component, ElementRef, EventEmitter, Injector, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  AdditionalStepTransitionDto,
  GetAllUploadedFileDto,
  InvokeStatusInputDto,
  PickingType,
  PointTransactionDto,
  RoutePointDocumentType,
  RoutePointStatus,
  RoutPointTransactionDto,
  ShippingRequestRouteType,
  ShippingRequestTripDriverStatus,
  ShippingRequestTripFlag,
  ShippingRequestTripStatus,
  ShippingRequestType,
  ShippingTypeEnum,
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
import { AngularFireDatabase } from '@angular/fire/compat/database';
import { DriverLocation, FirebaseHelperClass, trackingIconsList } from '@app/main/shippingRequests/shippingRequests/tracking/firebaseHelper.class';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';
import { Timeline } from '@node_modules/primeng/timeline';
import { CustomStep } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/custom-timeline/custom-step';
import * as moment from '@node_modules/moment';
import { UploadAdditionalDocumentsComponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/upload-additional-documents/upload-additional-documents.component';

@Component({
  selector: 'new-tracking-conponent',
  templateUrl: './new-tracking-conponent.html',
  styleUrls: ['./new-tracking-conponent.scss'],
  providers: [NgbDropdownConfig],
  animations: [appModuleAnimation()],
})
export class NewTrackingConponent extends AppComponentBase implements OnChanges, OnInit {
  @ViewChild('modelconfirm', { static: false }) modelConfirmCode: TrackingConfirmModalComponent;
  @ViewChild('modelpod', { static: false }) modelpod: TrackingPODModalComponent;
  @ViewChild('appEntityLog', { static: false }) activityLogModal: EntityLogComponent;
  @ViewChild('fileViwerComponent', { static: false }) fileViwerComponent: FileViwerComponent;
  @ViewChild('timeline', { static: false }) timeline: Timeline;
  @ViewChild('additionalDocumentsComponent', { static: false }) additionalDocumentsComponent: UploadAdditionalDocumentsComponent;

  @Input() trip: TrackingListDto = new TrackingListDto();
  @Output() getForViewReady: EventEmitter<boolean> = new EventEmitter<boolean>();
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
  pointDeliveryNoteList: GetAllUploadedFileDto[];
  pointPodList: GetAllUploadedFileDto[];
  pointAdditionalFilesList: GetAllUploadedFileDto[] = [];
  deliveryGoodPictureId: number;
  mapToggle = true;
  newReceiverCode: string;
  tripRoute = {
    origin: { lat: null, lng: null },
    wayPoints: [],
    destination: { lat: null, lng: null },
  };
  driversToggle = true;
  tripsToggle = true;
  driverLiveLocation: DriverLocation = { lng: 0, lat: 0 };
  trackingIconsList = trackingIconsList;
  driverOnline: boolean;
  TripFlag = ShippingRequestTripFlag;
  shippingType: ShippingTypeEnum;
  ShippingTypeEnum = ShippingTypeEnum;
  RoutePointStatus = RoutePointStatus;
  additionalFilesTransitions: { pointId: number; transactions: AdditionalStepTransitionDto[] }[] = [];

  constructor(
    injector: Injector,
    private elRef: ElementRef,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _db: AngularFireDatabase,
    config: NgbDropdownConfig
  ) {
    super(injector);
    config.autoClose = true;
    config.container = 'body';
  }

  ngOnInit() {
    abp.event.on('FileUploadedSuccessFromAdditionalSteps', () => {
      this.getForView();
    });
    abp.event.on('trackingConfirmCodeSubmittedFromAdditionalSteps', () => {
      this.getForView();
    });
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
    let firebaseHelper = new FirebaseHelperClass(this._db);
    firebaseHelper.getDriverLocationLiveByTripId(this.trip.id).subscribe((res) => {
      this.driverLiveLocation.lat = res[0]?.lat;
      this.driverLiveLocation.lng = res[0]?.lng;
      //if the trip is in transit and there is not driver data coming from firebase
      if (this.trip.status == this.tripStatusesEnum.InTransit && !isNotNullOrUndefined(res[0])) {
        this.driverOnline = false;
      } else {
        this.driverOnline = res[0]?.onlineStatus;
      }
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
    this.additionalFilesTransitions = [];
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
        this.shippingType = result.shippingType;
        if (this.shippingType === ShippingTypeEnum.ExportPortMovements || this.shippingType === ShippingTypeEnum.ImportPortMovements) {
          result.routPoints.filter((item) => {
            if (item.pickingType === PickingType.Dropoff) {
              this.getAllPointAdditionalFilesTransitions(item);
              item.statues.push(
                RoutPointTransactionDto.fromJS({
                  status: 0,
                  isDone: false,
                  name: this.l('UploadRequiredFiles'),
                  creationTime: null,
                })
              );
            }
          });
        }
        if (result.status === this.tripStatusesEnum.Delivered) {
          this.emitToMobileApplication('TripIsDelivered', null, 'delete');
        }
        this.syncTripInGetForView(this.trip);
        this.handleCanGoNextLocation(result.routPoints);
        this.getTripRouteForMap();
        setTimeout(() => {
          this.getForViewReady.emit(true);
        }, 500);
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
          this.emitToMobileApplication('Started', this.routePoints[0], 'write');
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
            this.emitToMobileApplication('Accepted', this.routePoints[0], 'write');
            this.notify.info(this.l('SuccessfullyAccepted'));
          });
      }
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
    this.emitToMobileApplication(transaction.action, point, 'write');
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

  /**
   * downloads the wayBill For MultiDrops Points
   * @param id
   */
  downloadMultiDropPointWaybill(id: number) {
    this.dropWaybillLoadingId = id;
    this._waybillsServiceProxy.getMultipleDropWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.fileViwerComponent.show(this._fileDownloadService.downloadTempFile(result), 'pdf');
      this.dropWaybillLoadingId = null;
    });
  }

  canDoActionsOnPoints(point: TrackingRoutePointDto): boolean {
    //prevent any Points Actions if the trip hasAccident
    if (this.trip.hasAccident && this.trip.isTripImpactEnabled) {
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

  getDeliveryNoteListForPoint(point: TrackingRoutePointDto): void {
    this.loadPodForPointId = point.id;
    this.pointDeliveryNoteList = null;
    this._trackingServiceProxy
      .getPointFile(point.id, RoutePointDocumentType.DeliveryNote)
      .pipe(
        finalize(() => {
          this.loadPodForPointId = null;
        })
      )
      .subscribe((res) => {
        this.pointDeliveryNoteList = res;
      });
  }

  /**
   * download a pod file
   * @param pod
   */
  downloadPOD(pod: GetAllUploadedFileDto): void {
    let image = this._fileDownloadService.downloadFileByBinary(pod.documentId, pod.fileName, pod.fileType);
    this.fileViwerComponent.show(image, pod.fileType == 'application/pdf' ? 'pdf' : 'img');
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
  resetReceiverCode(pointId: number, pointIndex: number) {
    this.message.confirm(this.l('ResetReceiverCode'), this.l('ReceiverCodeConfirmationMsg'), (isConfirmed) => {
      if (isConfirmed) {
        this._trackingServiceProxy.resetPointReceiverCode(pointId).subscribe((result) => {
          this.notify.success(this.l('ResetReceiverCodeSuccessfully'));
          this.routePoints[pointIndex].receiverCode = result;
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
   * Send Driver Location to Firebase On Trip Start
   * @private
   */
  private emitToMobileApplication(transaction: string, point?: TrackingRoutePointDto, mode?: 'write' | 'delete') {
    let helper = new FirebaseHelperClass(this._db);
    if (mode === 'delete') {
      helper.unAssignDriver(this.trip);
    } else if (mode === 'write') {
      helper.assignDriverToTrip(this.trip, point, this.appSession.tenantId, transaction);
    }
  }

  invokeUploadStep(point: TrackingRoutePointDto, transaction: PointTransactionDto) {
    console.log('invokeUploadStep', true);
    console.log('invokeUploadStep point', point);
    console.log('invokeUploadStep transaction', transaction);
    this.additionalDocumentsComponent.show(point);
  }

  getAllPointAdditionalFilesTransitions(point: TrackingRoutePointDto) {
    this._trackingServiceProxy.getAllPointAdditionalFilesTransitions(point.id).subscribe((res) => {
      this.additionalFilesTransitions.push({ pointId: point.id, transactions: res });
      console.log('point', point);
    });
  }

  getPointFile(pointId: number, transition: AdditionalStepTransitionDto) {
    this._trackingServiceProxy.getPointFile(pointId, transition.routePointDocumentType).subscribe((res) => {
      this.pointAdditionalFilesList = res;
    });
  }

  getTransactionsForPoint(pointId): AdditionalStepTransitionDto[] {
    if (this.additionalFilesTransitions.length > 0) {
      const found = this.additionalFilesTransitions.find((point) => point.pointId == pointId);
      if (isNotNullOrUndefined(found)) {
        return found.transactions;
      }
    }
    return [];
  }
}
