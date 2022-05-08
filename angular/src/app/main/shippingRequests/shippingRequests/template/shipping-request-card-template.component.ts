import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  GetShippingRequestForPriceOfferListDto,
  GetShippingRequestForViewOutput,
  PriceOfferChannel,
  PriceOfferServiceProxy,
  ShippingRequestDirectRequestServiceProxy,
  ShippingRequestDirectRequestStatus,
  ShippingRequestStatus,
  ShippingRequestType,
} from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { ShippingRequestForPriceOfferGetAllInput } from '../../../../shared/common/search/ShippingRequestForPriceOfferGetAllInput';
import { Router } from '@angular/router';
import { ShippingrequestsDetailsModelComponent } from '../details/shippingrequests-details-model.component';

@Component({
  templateUrl: './shipping-request-card-template.component.html',
  // styleUrls: ['/assets/custom/css/style.scss'],
  selector: 'shipping-request-card-template',
  animations: [appModuleAnimation()],
})
export class ShippingRequestCardTemplateComponent extends ScrollPagnationComponentBase implements OnInit {
  @ViewChild('Model', { static: false }) modalMore: ShippingrequestsDetailsModelComponent;
  shippingRequestforView: GetShippingRequestForViewOutput;

  Items: GetShippingRequestForPriceOfferListDto[] = [];
  searchInput: ShippingRequestForPriceOfferGetAllInput = new ShippingRequestForPriceOfferGetAllInput();
  @Input() Channel: PriceOfferChannel | number | null | undefined = undefined;
  @Input() isTMS: boolean = false;
  @Input() Title: string;
  @Input() ShippingRequestId: number | null | undefined = undefined;
  direction = 'ltr';
  openCardId: number;
  bidsloading = false;
  zoom: Number = 13; //map zoom
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  constructor(
    injector: Injector,
    private _currentServ: PriceOfferServiceProxy,
    private _directRequestSrv: ShippingRequestDirectRequestServiceProxy,
    private router: Router
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.searchInput.channel = this.Channel;
    this.searchInput.shippingRequestId = this.ShippingRequestId;
    if (this.isTMS) {
      this.searchInput.requestType = ShippingRequestType.TachyonManageService;
      this.searchInput.isTMS = true;
    }
    this.LoadData();
  }
  LoadData() {
    this._currentServ
      .getAllShippingRequest(
        this.searchInput.filter,
        this.searchInput.carrier,
        this.searchInput.shippingRequestId,
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
          if (this.feature.isEnabled('App.Shipper')) {
            if (r.requestTypeTitle == 'TachyonManageService' && r.statusTitle == 'NeedsAction') {
              r.statusTitle = 'New';
            }
          }
        });
        this.Items.push(...result.items);
        console.log('LoadingMore Date .....');
      });
  }
  canDeleteDirectRequest(input: GetShippingRequestForPriceOfferListDto) {
    if (
      this.Channel == PriceOfferChannel.DirectRequest &&
      (input.directRequestStatus == ShippingRequestDirectRequestStatus.New ||
        input.directRequestStatus == ShippingRequestDirectRequestStatus.Declined)
    ) {
      if ((this.feature.isEnabled('App.TachyonDealer') && input.isTachyonDeal) || (this.feature.isEnabled('App.Shipper') && !input.isTachyonDeal))
        return true;
    }
    return false;
  }
  canSeeShippingRequestTrips() {
    //if there is no carrierTenantId  and the current user in not a carrier Hide Trips Section
    if (this.feature.isEnabled('App.Carrier') && !this.shippingRequestforView.shippingRequest.carrierTenantId) {
      return false;
    } else if (this.feature.isEnabled('App.TachyonDealer')) {
      //if Tachyon Dealer
      return true;
    }
    //By Default
    return true;
  }

  delete(input: GetShippingRequestForPriceOfferListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._directRequestSrv.delete(input.directRequestId).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          _.remove(this.Items, input);
        });
      }
    });
  }

  decline(input: GetShippingRequestForPriceOfferListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._directRequestSrv.decline(input.directRequestId).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeclined'));
          _.remove(this.Items, input);
        });
      }
    });
  }

  setTitle(item: GetShippingRequestForPriceOfferListDto): string {
    if (this.Channel == PriceOfferChannel.DirectRequest) {
      if (this.feature.isEnabled('App.Carrier')) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
      if (this.feature.isEnabled('App.Shipper') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.name;
      }
    } else if (this.Channel == PriceOfferChannel.MarketPlace) {
      if (this.feature.isEnabled('App.Carrier') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
    } else if (this.Channel == PriceOfferChannel.Offers) {
      if (this.feature.isEnabled('App.Carrier') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      }
    } /*Shipping request page*/ else {
      if (this.feature.isEnabled('App.Carrier')) return item.isTachyonDeal ? this.l('TachyonManageService') : item.name;
      if (this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        if (item.status == ShippingRequestStatus.PostPrice || item.status == ShippingRequestStatus.Completed) return `${item.name} - ${item.carrier}`;
        else {
          return item.name;
        }
      }
    }
    return '';
  }
  canSeeTotalOffers(item: GetShippingRequestForPriceOfferListDto) {
    if (item.totalOffers > 0 && !this.feature.isEnabled('App.Carrier') && this.Channel != 2 && this.Channel != 10 && item.status == 2) {
      return true;
    }
    return false;
  }
  search(): void {
    this.IsLoading = true;
    this.skipCount = 0;
    this.Items = [];
    this.LoadData();
  }

  getWordTitle(n: any, word: string): string {
    if (parseInt(n) == 1) {
      return this.l(word);
    }
    return this.l(`${word}s`);
  }

  moreRedirectTo(item: GetShippingRequestForPriceOfferListDto): void {
    if (!this.Channel && this.appSession.tenantId) {
      if (
        !this.feature.isEnabled('App.TachyonDealer') ||
        (this.feature.isEnabled('App.TachyonDealer') && item.requestType == ShippingRequestType.TachyonManageService)
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

  isCarrierOwnRequest(request: GetShippingRequestForPriceOfferListDto): boolean {
    return this.feature.isEnabled('App.CarrierAsASaas') && request.isSaas && this.appSession.tenantId === request.tenantId;
  }
}
