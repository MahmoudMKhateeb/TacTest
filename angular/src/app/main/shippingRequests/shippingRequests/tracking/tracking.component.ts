import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  ShipmentTrackingMode,
  ShippingRequestDriverServiceProxy,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestStatus,
  ShippingRequestsTripListDto,
  ShippingRequestTripCancelStatus,
  ShippingRequestTripDriverRoutePointDto,
  ShippingRequestTripDriverStatus,
  ShippingRequestTripFlag,
  ShippingRequestTripStatus,
  ShippingRequestType,
  ShippingTypeEnum,
  TrackingListDto,
  TrackingServiceProxy,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { TrackingSearchInput } from '../../../../shared/common/search/TrackingSearchInput';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';
import { ViewTripAccidentModelComponent } from '../ShippingRequestTrips/accident/View-trip-accident-modal.component';
import { animate, style, transition, trigger } from '@angular/animations';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { NewTrackingConponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/new-tracking-conponent';
import { finalize } from '@node_modules/rxjs/operators';
import Swal from 'sweetalert2';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './tracking.component.html',
  styleUrls: ['./tracking.component.scss'],
  animations: [
    appModuleAnimation(),
    trigger('grow', [
      transition(':enter', [style({ height: '0', overflow: 'hidden' }), animate(300, style({ height: '*' }))]),
      transition(':leave', [animate(200, style({ height: 0, overflow: 'hidden' }))]),
    ]),
  ],
})
export class TrackingComponent extends ScrollPagnationComponentBase implements OnInit {
  @ViewChild('ModelIncident', { static: false }) modelIncident: ViewTripAccidentModelComponent;
  @ViewChild('NewTrackingComponent', { static: false }) newTrackingComponent: NewTrackingConponent;
  @ViewChild('fileViwerComponent', { static: false }) fileViwerComponent: FileViwerComponent;

  public Items: TrackingListDto[] = [];
  direction = 'ltr';
  searchInput: TrackingSearchInput = new TrackingSearchInput();
  pointsIsLoading = false;
  routePoints: ShippingRequestTripDriverRoutePointDto[];
  activePanelId: number;
  routeTypeEnum = ShippingRequestRouteType;
  ShippingRequestTripStatusEnum = ShippingRequestTripStatus;
  ShippingRequestStatusEnum = ShippingRequestStatus;
  DriverStatusEnum = ShippingRequestTripDriverStatus;
  ShippingRequestTripCancelStatusEnum = ShippingRequestTripCancelStatus;
  ShippingRequestTypeEnum = ShippingRequestType;
  downloadingForItem: number;
  defaultProfilePic = AppConsts.appBaseUrl + '/assets/common/images/carrier-default-pic.jpg';
  loadingTripId: number;
  ShippingRequestFlagEnum = ShippingRequestFlag;
  TripFlag = ShippingRequestTripFlag;
  ShippingTypeEnum = ShippingTypeEnum;
  private waybillNumber: number;
  showNormalView = true;
  shipmentType: 'normalShipment' | 'directShipment';

  constructor(
    injector: Injector,
    private _currentServ: TrackingServiceProxy,
    private _localStorageService: LocalStorageService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    super(injector);
    this.waybillNumber = this._activatedRoute.snapshot.queryParams['waybillNumber'];
    this.shipmentType = this._activatedRoute.snapshot.data.shipmentType;
  }

  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.watchForRouterChangeToChangeView();
    this.syncTrip();
    if (this.showNormalView) {
      this.LoadData();
    }
    this.handleTripIncidentReport();
  }

  private watchForRouterChangeToChangeView() {
    this.router.events.subscribe((event) => {
      console.log('event', event);
      if (event instanceof NavigationEnd) {
        this.showNormalView = this._activatedRoute.snapshot.queryParamMap.get('showType')
          ? this._activatedRoute.snapshot.queryParamMap.get('showType').toLowerCase() != '1'
          : true;
        this.StopLoading = !this.showNormalView;
        if (this.showNormalView) {
          this.LoadData();
        }
      }
    });
    this.showNormalView = this._activatedRoute.snapshot.queryParamMap.get('showType')
      ? this._activatedRoute.snapshot.queryParamMap.get('showType').toLowerCase() != '1'
      : true;
    this.StopLoading = !this.showNormalView;
  }

  LoadData() {
    if (isNotNullOrUndefined(this.waybillNumber)) {
      this.searchInput.WaybillNumber = this.waybillNumber;
    }

    if (this.shipmentType === AppConsts.Tracking_NormalShipment) {
      let trackingMode = this.isTachyonDealerOrHost ? ShipmentTrackingMode.NormalShipment : ShipmentTrackingMode.Mixed;
      this._currentServ
        .getAll(
          this.searchInput.status,
          this.searchInput.shipper,
          this.searchInput.carrier,
          this.searchInput.WaybillNumber,
          this.searchInput.transportTypeId,
          this.searchInput.truckTypeId,
          this.searchInput.truckCapacityId,
          this.searchInput.originId,
          this.searchInput.destinationId,
          this.searchInput.pickupFromDate,
          this.searchInput.pickupToDate,
          this.searchInput.fromDate,
          this.searchInput.toDate,
          this.searchInput.shippingRequestReferance,
          this.searchInput.routeTypeId,
          this.searchInput.packingTypeId,
          this.searchInput.goodsOrSubGoodsCategoryId,
          this.searchInput.plateNumberId,
          this.searchInput.driverNameOrMobile,
          this.searchInput.deliveryFromDate,
          this.searchInput.deliveryToDate,
          this.searchInput.containerNumber,
          this.searchInput.isInvoiceIssued,
          this.searchInput.isSubmittedPOD,
          this.searchInput.requestTypeId,
          trackingMode,
          '',
          this.skipCount,
          this.maxResultCount
        )
        .subscribe((result) => {
          this.IsLoading = false;
          this.StopLoading = result.items.length < this.maxResultCount;
          this.Items.push(...result.items);
        });
      return;
    }

    if (this.shipmentType !== AppConsts.Tracking_DirectShipment) {
      return;
    }
    this._currentServ
      .getAll(
        this.searchInput.status,
        this.searchInput.shipper,
        this.searchInput.carrier,
        this.searchInput.WaybillNumber,
        this.searchInput.transportTypeId,
        this.searchInput.truckTypeId,
        this.searchInput.truckCapacityId,
        this.searchInput.originId,
        this.searchInput.destinationId,
        this.searchInput.pickupFromDate,
        this.searchInput.pickupToDate,
        this.searchInput.fromDate,
        this.searchInput.toDate,
        this.searchInput.shippingRequestReferance,
        this.searchInput.routeTypeId,
        this.searchInput.packingTypeId,
        this.searchInput.goodsOrSubGoodsCategoryId,
        this.searchInput.plateNumberId,
        this.searchInput.driverNameOrMobile,
        this.searchInput.deliveryFromDate,
        this.searchInput.deliveryToDate,
        this.searchInput.containerNumber,
        this.searchInput.isInvoiceIssued,
        this.searchInput.isSubmittedPOD,
        this.searchInput.requestTypeId,
        ShipmentTrackingMode.DirectShipment,
        '',
        this.skipCount,
        this.maxResultCount
      )
      .subscribe((result) => {
        this.IsLoading = false;
        this.StopLoading = result.items.length < this.maxResultCount;
        this.Items.push(...result.items);
      });
  }

  search(): void {
    if (!this.showNormalView) {
      const searchInput = { ...this.searchInput };
      this.searchInput = null;
      this.searchInput = { ...searchInput };
      return;
    }
    this.IsLoading = true;
    this.skipCount = 0;
    this.Items = [];
    this.resetScrolling();
    this.LoadData();
  }

  showIncident(item: TrackingListDto): void {
    if (item.hasAccident) {
      let trip: ShippingRequestsTripListDto = new ShippingRequestsTripListDto();
      trip.id = item.id;
      trip.isApproveCancledByCarrier = item.isApproveCancledByCarrier;
      trip.isApproveCancledByShipper = item.isApproveCancledByShipper;
      trip.status = item.status;
    }
  }

  /**
   * Downloads A single Drop Wayboll
   * @param id
   * @constructor
   */
  DownloadSingleDropWaybillPdf(id: number): void {
    this.downloadingForItem = id;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.fileViwerComponent.show(this._fileDownloadService.downloadTempFile(result), 'pdf');
      this.downloadingForItem = null;
    });
  }

  /**
   * returns the status % of the trip based of the current status
   * @param status
   */
  getProgress(status: ShippingRequestTripStatus): number {
    const ShippingRequestTripStatus = this.ShippingRequestTripStatusEnum;
    switch (status) {
      case ShippingRequestTripStatus.New: {
        return 0;
        break;
      }
      case ShippingRequestTripStatus.InTransit: {
        return 75;
        break;
      }
      case ShippingRequestTripStatus.Canceled: {
        return 0;
        break;
      }
      case ShippingRequestTripStatus.Delivered: {
        return 100;
        break;
      }
      case ShippingRequestTripStatus.DeliveredAndNeedsConfirmation: {
        return 90;
        break;
      }
    }
  }

  /**
   * resets a Trip
   * what to do when Trip is reseated
   */
  handleTripReset(tripId: number) {
    Swal.fire({
      title: this.l('PleaseConfirmThisAction'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.loadingTripId = tripId;
        this._shippingRequestDriverServiceProxy.reset(tripId).subscribe(() => {
          this.loadingTripId = null;
          this.notify.success(this.l('SuccessfullyReseated'));
          this.search();
        });
      } //end of if
    });
  }

  /**
   * accepts the trip
   */
  accept(trip?: TrackingListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.loadingTripId = trip.id;
        this._trackingServiceProxy
          .accept(trip.id)
          .pipe(
            finalize(() => {
              this.loadingTripId = null;
              trip.driverStatus = ShippingRequestTripDriverStatus.Accepted;
              abp.event.trigger('TripAccepted');
            })
          )
          .subscribe((result) => {
            this.notify.success(this.l('SuccessfullyAccepted'));
          });
      }
    });
  }

  start(trip?: TrackingListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.loadingTripId = trip.id;
        this._trackingServiceProxy
          .start(trip.id)
          .pipe(
            finalize(() => {
              this.loadingTripId = undefined;
            })
          )
          .subscribe(() => {
            this.activePanelId = trip.id;
            this.syncTrip();
            abp.event.trigger('TripAccepted'); // used to refresh child component
            this.notify.info(this.l('SuccessfullyStarted'));
          });
      }
    });
  }

  /**
   * syncs trips From Child Component
   * @private
   */
  private syncTrip() {
    if (this.activePanelId) {
      abp.event.on('TripDataChanged', function (incomingTrip: TrackingListDto) {
        if (this.Items) {
          const index = this.Items.findIndex((trip) => trip.id === incomingTrip.id);
          this.Items[index] = incomingTrip;
        }
      });
    }
  }

  /**
   * checks if the tenant can report an Accident to the current trip
   */
  canCreateAccident(trip: TrackingListDto) {
    if (trip.status === ShippingRequestTripStatus.InTransit) {
      if (!this.appSession.tenantId || this.feature.isEnabled('App.TachyonDealer')) {
        //if tachyon dealer and is assign return true
        return trip.isAssign;
      }
      //inTransit return true too
      return true;
    }
    //not in transit return false
    return false;
  }

  /**
   * handels incident Report From Modal
   */
  handleTripIncidentReport() {
    abp.event.on('TripReportedAccident', (tripId: number) => {
      const tripIndex = this.Items.findIndex((trip) => trip.id === tripId);
      this.Items[tripIndex].hasAccident = true;
    });
  }

  /**
   * check if user can reset a trip
   * if user is Carrier/TMS and the Trip Status is not new/Delivered
   * @param item
   */
  canResetTrip(item: TrackingListDto) {
    return (
      (this.isCarrier || this.isTachyonDealer) &&
      item.status !== this.ShippingRequestTripStatusEnum.New &&
      item.status !== this.ShippingRequestTripStatusEnum.Delivered
    );
  }

  showAsTable() {
    let pageName = this.shipmentType === AppConsts.Tracking_NormalShipment ? 'shipmentTracking' : 'directShipmentTracking';
    console.log(pageName);
    this.router.navigateByUrl(`/app/main/tracking/${pageName}?showType=1`);
  }

  showAsList() {
    let pageName = this.shipmentType === AppConsts.Tracking_NormalShipment ? 'shipmentTracking' : 'directShipmentTracking';
    console.log(pageName);
    this.router.navigateByUrl(`/app/main/tracking/${pageName}`);
  }

  protected readonly AppConsts = AppConsts;
}
