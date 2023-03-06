import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  GetShippingRequestForPriceOfferListDto,
  GetShippingRequestForViewOutput,
  PriceOfferChannel,
  PriceOfferServiceProxy,
  RoundTripType,
  ShippingRequestDirectRequestServiceProxy,
  ShippingRequestDirectRequestStatus,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestStatus,
  ShippingRequestType,
  ShippingTypeEnum,
} from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { ShippingRequestForPriceOfferGetAllInput } from '@app/shared/common/search/ShippingRequestForPriceOfferGetAllInput';
import { ActivatedRoute, Router } from '@angular/router';
import { ShippingrequestsDetailsModelComponent } from '../details/shippingrequests-details-model.component';
import { LoadEntityTemplateModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/load-entity-template-modal/load-entity-template-modal.component';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { TripsForViewShippingRequestComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/tripsForViewShippingRequest.component';
import { AssignTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/assign-trucks-and-drivers-modal/assign-trucks-and-drivers-modal.component';
import { DedicatedShippingRequestAttendanceSheetModalComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-shipping-request-attendance-sheet-modal.component';
import { ReplaceTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/replace-trucks-and-drivers-modal/replace-trucks-and-drivers-modal.component';

@Component({
  templateUrl: './shipping-request-card-template.component.html',
  selector: 'shipping-request-card-template',
  styleUrls: ['./shipping-request-card-template.component.scss'],
  animations: [appModuleAnimation()],
})
export class ShippingRequestCardTemplateComponent extends ScrollPagnationComponentBase implements OnInit {
  @ViewChild('Model', { static: false }) modalMore: ShippingrequestsDetailsModelComponent;
  @ViewChild('loadEntityTemplateModal', { static: false }) loadEntityTemplateModal: LoadEntityTemplateModalComponent;
  @ViewChild('assignTrucksAndDriversModal', { static: false }) assignTrucksAndDriversModal: AssignTrucksAndDriversModalComponent;
  @ViewChild('replaceTrucksAndDriversModal', { static: false }) replaceTrucksAndDriversModal: ReplaceTrucksAndDriversModalComponent;
  @ViewChild('tripsForViewShippingRequest', { static: true }) tripsForViewShippingRequest: TripsForViewShippingRequestComponent;
  @ViewChild('attendanceModal', { static: true }) attendanceModal: DedicatedShippingRequestAttendanceSheetModalComponent;
  shippingRequestforView: GetShippingRequestForViewOutput;
  ShippingRequestFlagEnum = ShippingRequestFlag;
  PriceOfferChannelEnum = PriceOfferChannel;
  items: GetShippingRequestForPriceOfferListDto[] = [];
  searchInput: ShippingRequestForPriceOfferGetAllInput = new ShippingRequestForPriceOfferGetAllInput();
  @Input() Channel: PriceOfferChannel | number | null | undefined = undefined;
  @Input() isTMS = false;
  @Input() Title: string;
  @Input() ShippingRequestId: number | null | undefined = undefined;
  origin: any;
  destination: any;
  direction = 'ltr';
  openCardId: number;
  bidsLoading = false;
  zoom: Number = 13; //map zoom
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  directRequestId!: number;
  activeShippingRequestId!: number;
  selectedShippingRequest: GetShippingRequestForPriceOfferListDto;
  shippingRequestStatusEnum = ShippingRequestStatus;

  ShippingTypeEnum = ShippingTypeEnum;
  RoundTripType = RoundTripType;
  ShippingRequestRouteType = ShippingRequestRouteType;

  constructor(
    injector: Injector,
    private _currentServ: PriceOfferServiceProxy,
    private _directRequestSrv: ShippingRequestDirectRequestServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    super(injector);
    this.directRequestId = this._activatedRoute.snapshot.queryParams['directRequestId'];
    this.activeShippingRequestId = this._activatedRoute.snapshot.queryParams['srId'];
  }

  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.searchInput.channel = this.Channel;
    this.searchInput.shippingRequestId = this.ShippingRequestId;
    if (this.isTMS) {
      this.searchInput.requestType = ShippingRequestType.TachyonManageService;
      this.searchInput.isTMS = true;
    }
    this.searchInput.directRequestId = this.directRequestId;
    if (isNotNullOrUndefined(this.activeShippingRequestId)) {
      this.searchInput.shippingRequestId = this.activeShippingRequestId;
    }
    this.LoadData();
  }

  LoadData() {
    this._currentServ
      .getAllShippingRequest(
        this.searchInput.filter,
        this.searchInput.carrier,
        this.searchInput.shippingRequestId,
        this.searchInput.directRequestId,
        this.searchInput.channel,
        this.searchInput.requestType,
        this.searchInput.truckTypeId,
        this.searchInput.originId,
        this.searchInput.destinationId,
        this.searchInput.pickupFromDate,
        this.searchInput.pickupToDate,
        this.searchInput.fromDate,
        this.searchInput.toDate,
        this.searchInput.routeTypeId,
        this.searchInput.status,
        this.searchInput.isTachyonDeal,
        this.searchInput.isTMS,
        '',
        this.skipCount,
        this.maxResultCount
      )
      .subscribe((result) => {
        this.IsLoading = false;
        if (result.items.length < this.maxResultCount) {
          this.StopLoading = true;
        }
        result.items.forEach((r) => {
          this.origin = null;
          this.destination = null;
          if (this.openCardId === r.id) {
            this.getCoordinatesByCityName(r.originCity, 'source');
            this.getCoordinatesByCityName(r.destinationCity, 'destination');
            r.latitude = this.origin?.lat;
            r.longitude = this.origin?.lng;
            this.origin = this.origin;
            this.destination = this.destination;
          }
          if (this.feature.isEnabled('App.Shipper')) {
            if (r.requestType === ShippingRequestType.TachyonManageService && r.status === ShippingRequestStatus.NeedsAction) {
              r.statusTitle = this.l('New');
            }
          }
          // only in this case I need to use double equal not triple (type is difference)
          if (
            (this.directRequestId && r.directRequestId === this.directRequestId) ||
            (this.activeShippingRequestId && r.id === this.activeShippingRequestId)
          ) {
            this.moreRedirectTo(r);
            this.directRequestId = undefined;
          }
        });
        this.items.push(...result.items);
      });
  }

  canDeleteDirectRequest(input: GetShippingRequestForPriceOfferListDto) {
    if (
      this.Channel === PriceOfferChannel.DirectRequest &&
      (input.directRequestStatus === ShippingRequestDirectRequestStatus.New ||
        input.directRequestStatus === ShippingRequestDirectRequestStatus.Declined)
    ) {
      if ((this.feature.isEnabled('App.TachyonDealer') && input.isTachyonDeal) || (this.feature.isEnabled('App.Shipper') && !input.isTachyonDeal)) {
        return true;
      }
    }
    return false;
  }

  // canSeeShippingRequestTrips() {
  //   //if there is no carrierTenantId  and the current user in not a carrier Hide Trips Section
  //   if (this.feature.isEnabled('App.Carrier') && !this.shippingRequestForView.shippingRequest.carrierTenantId) {
  //     return false;
  //   } else if (this.feature.isEnabled('App.TachyonDealer')) {
  //     //if Tachyon Dealer
  //     return true;
  //   }
  //   //By Default
  //   return true;
  // }

  delete(input: GetShippingRequestForPriceOfferListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._directRequestSrv.delete(input.directRequestId).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          _.remove(this.items, input);
        });
      }
    });
  }

  decline(input: GetShippingRequestForPriceOfferListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._directRequestSrv.decline(input.directRequestId).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeclined'));
          _.remove(this.items, input);
        });
      }
    });
  }

  mapReady(event: any) {
    event.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById('Settings'));
  }

  setTitle(item: GetShippingRequestForPriceOfferListDto): string {
    if (this.Channel === PriceOfferChannel.DirectRequest) {
      if (this.feature.isEnabled('App.Carrier')) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
      if (this.feature.isEnabled('App.Shipper') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.name;
      }
    } else if (this.Channel === PriceOfferChannel.MarketPlace) {
      if (this.feature.isEnabled('App.Carrier') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
    } else if (this.Channel === PriceOfferChannel.Offers) {
      if (this.feature.isEnabled('App.Carrier') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
    } /*Shipping request page*/ else {
      if (this.feature.isEnabled('App.Carrier')) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
      if (this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        if (item.status === ShippingRequestStatus.PostPrice || item.status === ShippingRequestStatus.Completed) {
          return `${item.name} - ${item.carrier}`;
        } else {
          return item.name;
        }
      }
    }
    return '';
  }

  canSeeTotalOffers(item: GetShippingRequestForPriceOfferListDto) {
    if (item.price > 0) {
      return false;
    }
    if (item.totalOffers > 0 && !this.feature.isEnabled('App.Carrier') && this.Channel != 2 && this.Channel != 10 && item.status == 2) {
      return true;
    }
    return false;
  }

  search(): void {
    this.IsLoading = true;
    this.skipCount = 0;
    this.items = [];
    this.LoadData();
  }

  getWordTitle(n: any, word: string): string {
    if (parseInt(n) === 1) {
      return this.l(word);
    }
    return this.l(`${word}s`);
  }

  moreRedirectTo(item: GetShippingRequestForPriceOfferListDto): void {
    if (!this.Channel && this.appSession.tenantId) {
      if (
        !this.feature.isEnabled('App.TachyonDealer') ||
        (this.feature.isEnabled('App.TachyonDealer') && item.requestType === ShippingRequestType.TachyonManageService) ||
        (this.feature.isEnabled('App.TachyonDealer') && item.shippingRequestFlag === ShippingRequestFlag.Dedicated)
      ) {
        this.router.navigateByUrl(`/app/main/shippingRequests/shippingRequests/view?id=${item.id}`);
        return;
      }
    }
    this.modalMore.show(item);
  }

  createNewRequest() {
    this.router.navigateByUrl('/app/main/shippingRequests/shippingRequestWizard');
  }

  createNewDedicatedRequest() {
    this.router.navigateByUrl('/app/main/shippingRequests/dedicatedShippingRequestWizard');
  }

  isCarrierOwnRequest(request: GetShippingRequestForPriceOfferListDto): boolean {
    return this.feature.isEnabled('App.CarrierAsASaas') && request.isSaas && this.appSession.tenantId === request.tenantId;
  }

  /**
   * Get City Coordinates By Providing its name
   * this function is to draw the shipping Request Main Route in View SR Details in marketPlace
   * @param cityName
   * @param cityType   source/dest
   */
  getCoordinatesByCityName(cityName: string, cityType: string) {
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode(
      {
        address: cityName,
      },
      (results, status) => {
        console.log(results);
        if (status === google.maps.GeocoderStatus.OK) {
          const Lat = results[0].geometry.location.lat();
          const Lng = results[0].geometry.location.lng();
          if (cityType === 'source') {
            this.origin = { lat: Lat, lng: Lng };
          } else {
            this.destination = { lat: Lat, lng: Lng };
          }
        } else {
          console.log('Something got wrong ' + status);
        }
      }
    );
  }

  refresh() {
    this.items = [];
    this.LoadData();
  }

  assignTrucksAndDrivers(item: GetShippingRequestForPriceOfferListDto) {
    console.log('item', item);
    this.assignTrucksAndDriversModal.show(item);
  }

  viewTrucksOrDrivers(item: GetShippingRequestForPriceOfferListDto, isForTruck: boolean) {
    console.log('item', item);
    this.replaceTrucksAndDriversModal.show(item, isForTruck);
  }

  openAttendanceModal(shippingRequest: GetShippingRequestForPriceOfferListDto) {
    this.selectedShippingRequest = shippingRequest;
    this.attendanceModal.show(this.selectedShippingRequest.status === this.shippingRequestStatusEnum.Completed, null, shippingRequest.id, {
      rentalStartDate: shippingRequest?.rentalStartDate,
      rentalEndDate: shippingRequest?.rentalEndDate,
    });
  }

  /**
   * we need to hide default price if the request is internal and the user is not TMS/Host
   * any other case he can see the price
   * @param item
   */
  canViewBrokerPrice(item: GetShippingRequestForPriceOfferListDto): boolean {
    if ((item.shipperActor || item.carrierActor) && !this.isTachyonDealerOrHost) {
      return false;
    }

    return true;
  }
}
