import { Component, ChangeDetectorRef, EventEmitter, Injector, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  GetShippingRequestForPriceOfferListDto,
  GetShippingRequestForPricingOutput,
  PriceOfferChannel,
  PriceOfferItemDto,
  PriceOfferServiceProxy,
  ShippingRequestBidStatus,
  ShippingRequestStatus,
  ShippingRequestUpdateStatus,
  ShippingRequestUpdateServiceProxy,
  CreateSrUpdateActionInputDto,
  CreateOrEditPriceOfferInput,
} from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { PrimengTableHelper } from '@shared/helpers/PrimengTableHelper';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './shippingrequests-details-model.component.html',
  styleUrls: ['./shippingrequests-details-model.component.css'],
  selector: 'shippingrequests-details-model',
  animations: [appModuleAnimation()],
})
export class ShippingrequestsDetailsModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('dataTable', { static: false }) dataTable: Table;
  @ViewChild('paginator', { static: false }) paginator: Paginator;
  rows = 5;
  ShippingRequestUpdateStatusEnum = ShippingRequestUpdateStatus;
  primengTableHelperEntityChanges = new PrimengTableHelper();
  CreateSrUpdateActionInput = new CreateSrUpdateActionInputDto();
  priceOfferInput: CreateOrEditPriceOfferInput;
  active = false;
  saving = false;
  request: GetShippingRequestForPricingOutput = new GetShippingRequestForPricingOutput();
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  distance: string;
  duration: string;
  direction: string;
  Items: PriceOfferItemDto[] = [];
  shippingrequest: GetShippingRequestForPriceOfferListDto = new GetShippingRequestForPriceOfferListDto();
  constructor(
    injector: Injector,
    private changeDetectorRef: ChangeDetectorRef,
    private _CurrentServ: PriceOfferServiceProxy,
    private _srUpdateService: ShippingRequestUpdateServiceProxy
  ) {
    super(injector);
  }

  show(request: GetShippingRequestForPriceOfferListDto): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.shippingrequest = request;
    //console.log(this.shippingrequest);
    this._CurrentServ.getShippingRequestForPricing(this.Channel, this.shippingrequest.id).subscribe((result) => {
      this.request = result;
      this.Items = this.request.items;
      this.active = true;
      this.getCordinatesByCityName(this.request.originCity, 'source');
      this.getCordinatesByCityName(this.request.destinationCity, 'destanation');
      this.modal.show();
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  update(offerId: number) {
    this.shippingrequest.offerId = offerId;
    this.shippingrequest.isPriced = true;
  }
  delete() {
    this.shippingrequest.offerId = undefined;
    this.shippingrequest.isPriced = false;
  }
  /**
   * Check the current user log in can set price or not
   */
  canSetPrice(): boolean {
    if (!this.Channel) return false;
    if (this.shippingrequest.isPriced) return false;
    if (this.request.status != ShippingRequestStatus.NeedsAction && this.request.status != ShippingRequestStatus.PrePrice) return false;
    if (this.Channel == PriceOfferChannel.MarketPlace && this.request.bidStatus != ShippingRequestBidStatus.OnGoing) return false;
    if (this.feature.isEnabled('App.Shipper')) return false;
    if (this.feature.isEnabled('App.Carrier')) return true;
    if (this.feature.isEnabled('App.TachyonDealer') && !this.request.isTachyonDeal) return true;
    return false;
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

  showSrUpdates(id: number, offerId: number) {
    this._srUpdateService.getAll(offerId, id, null, 0, 10).subscribe((result) => {
      console.log(result);
      abp.notify.info('GetAll ShippingRequestUpdates Successfully');
    });
  }

  getAll(offerId, event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.changeDetectorRef.detectChanges();
    this.primengTableHelper.defaultRecordsCountPerPage = 5;

    this.primengTableHelper.showLoadingIndicator();
    this._srUpdateService
      .getAll(
        offerId,
        this.shippingrequest.id,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.rows
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  KeepSamePrice(offerId, id) {
    this.CreateSrUpdateActionInput.status = ShippingRequestUpdateStatus.KeepSamePrice;
    this.CreateSrUpdateActionInput.id = id;
    this.primengTableHelper.showLoadingIndicator();

    this._srUpdateService
      .takeAction(this.CreateSrUpdateActionInput)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullySaved'));
        this.getAll(offerId);
      });
  }

  DismissOffer(offerId, id, request) {
    this.CreateSrUpdateActionInput.status = ShippingRequestUpdateStatus.Dismissed;
    this.CreateSrUpdateActionInput.id = id;
    this.primengTableHelper.showLoadingIndicator();

    this._srUpdateService
      .takeAction(this.CreateSrUpdateActionInput)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullySaved'));
        abp.event.trigger('dismissOffer', request);
      });
  }

  ngAfterViewInit(): void {
    this.changeDetectorRef.detectChanges();
    this.primengTableHelper.adjustScroll(this.dataTable);
    abp.event.on('RepriceOffer', () => {
      this.getAll(this.shippingrequest.offerId);
    });
    abp.event.on('dismissOffer', (request) => {
      this.show(request);
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
}
