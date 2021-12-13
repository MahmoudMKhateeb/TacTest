import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  ShippingRequestDriverServiceProxy,
  ShippingRequestRouteType,
  ShippingRequestsTripListDto,
  ShippingRequestTripDriverRoutePointDto,
  ShippingRequestTripDriverStatus,
  ShippingRequestTripStatus,
  TrackingListDto,
  TrackingServiceProxy,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { TrackingSearchInput } from '../../../../shared/common/search/TrackingSearchInput';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';
import { TrackingSignalrService } from './tacking-signalr.service';
import { ViewTripAccidentModelComponent } from '../ShippingRequestTrips/accident/View-trip-accident-modal.component';
import { animate, style, transition, trigger } from '@angular/animations';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { NewTrackingConponent } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/new-tracking-conponent';
import { ViewTripModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/viewTripModal/viewTripModal.component';
import { finalize } from '@node_modules/rxjs/operators';
import Swal from 'sweetalert2';

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
  @ViewChild('ViewTripModal', { static: false }) ViewTripModal: ViewTripModalComponent;

  Items: TrackingListDto[] = [];
  direction = 'ltr';
  searchInput: TrackingSearchInput = new TrackingSearchInput();
  pointsIsLoading = false;
  routePoints: ShippingRequestTripDriverRoutePointDto[];
  activePanelId: number;
  routeTypeEnum = ShippingRequestRouteType;
  ShippingRequestTripStatusEnum = ShippingRequestTripStatus;
  DriverStatusEnum = ShippingRequestTripDriverStatus;
  downloadingForItem: number;
  defaultProfilePic = AppConsts.appBaseUrl + '/assets/common/images/carrier-default-pic.jpg';

  constructor(
    injector: Injector,
    private _currentServ: TrackingServiceProxy,
    private _localStorageService: LocalStorageService,
    private _trackingSignalrService: TrackingSignalrService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.LoadData();
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
      case ShippingRequestTripStatus.Intransit: {
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
        this._shippingRequestDriverServiceProxy.reset(tripId).subscribe(() => {
          this.notify.info(this.l('SuccessfullyReseated'));
          this.search();
        });
      } //end of if
    });
  }

  /**
   * accepts the trip
   */
  accept(tripId?: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._trackingServiceProxy
          .accept(tripId)
          .pipe(finalize(() => {}))
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyAccepted'));
          });
      }
    });
  }
}
