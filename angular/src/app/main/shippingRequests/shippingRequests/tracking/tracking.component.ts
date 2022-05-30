import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  ShippingRequestDriverServiceProxy,
  ShippingRequestRouteType,
  ShippingRequestsTripListDto,
  ShippingRequestTripDriverRoutePointDto,
  ShippingRequestTripDriverStatus,
  ShippingRequestTripStatus,
  ShippingRequestType,
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
  DriverStatusEnum = ShippingRequestTripDriverStatus;
  ShippingRequestTypeEnum = ShippingRequestType;
  downloadingForItem: number;
  defaultProfilePic = AppConsts.appBaseUrl + '/assets/common/images/carrier-default-pic.jpg';
  loadingTripId: number;

  constructor(
    injector: Injector,
    private _currentServ: TrackingServiceProxy,
    private _localStorageService: LocalStorageService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.syncTrip();
    this.LoadData();
    this.handleTripIncidentReport();
  }
  LoadData() {
    this._currentServ
      .getAll(
        this.searchInput.status,
        this.searchInput.shipper,
        this.searchInput.carrier,
        this.searchInput.WaybillNumber,
        this.searchInput.truckTypeId,
        this.searchInput.originId,
        this.searchInput.destinationId,
        this.searchInput.pickupFromDate,
        this.searchInput.pickupToDate,
        this.searchInput.fromDate,
        this.searchInput.toDate,
        this.searchInput.shippingRequestReferance,
        this.searchInput.routeTypeId,
        '',
        this.skipCount,
        this.maxResultCount
      )
      .subscribe((result) => {
        this.IsLoading = false;
        if (result.items.length < this.maxResultCount) {
          this.StopLoading = true;
        }
        this.Items.push(...result.items);
      });
  }

  search(): void {
    this.IsLoading = true;
    this.skipCount = 0;
    this.Items = [];
    this.LoadData();
  }

  showIncident(item: TrackingListDto): void {
    if (item.hasAccident) {
      let trip: ShippingRequestsTripListDto = new ShippingRequestsTripListDto();
      trip.id = item.id;
      trip.isApproveCancledByCarrier = item.isApproveCancledByCarrier;
      trip.isApproveCancledByShipper = item.isApproveCancledByShipper;
      trip.status = item.status;
      this.modelIncident.getAll(trip);
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
}
