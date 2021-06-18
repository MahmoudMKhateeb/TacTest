import { Component, OnInit, Injector, Input } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  GetShippingRequestForPriceOfferListDto,
  PriceOfferChannel,
  PriceOfferServiceProxy,
  ShippingRequestDirectRequestServiceProxy,
  ShippingRequestDirectRequestStatus,
} from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
import { log } from 'util';
import { ShippingRequestForPriceOfferGetAllInput } from '../../../../shared/common/search/ShippingRequestForPriceOfferGetAllInput';
@Component({
  templateUrl: './shipping-request-card-template.component.html',
  styleUrls: ['/assets/custom/css/style.scss'],
  selector: 'shipping-request-card-template',
  animations: [appModuleAnimation()],
})
export class ShippingRequestCardTemplateComponent extends ScrollPagnationComponentBase implements OnInit {
  Items: GetShippingRequestForPriceOfferListDto[] = [];
  searchInput: ShippingRequestForPriceOfferGetAllInput = new ShippingRequestForPriceOfferGetAllInput();
  @Input() Channel: PriceOfferChannel | null | undefined = undefined;
  @Input() Title: string;
  @Input() ShippingRequestId: number | null | undefined = undefined;
  direction = 'ltr';
  constructor(injector: Injector, private _currentServ: PriceOfferServiceProxy, private _directRequestSrv: ShippingRequestDirectRequestServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.searchInput.channel = this.Channel;
    this.searchInput.shippingRequestId = this.ShippingRequestId;
    this.LoadData();
  }
  LoadData() {
    this._currentServ
      .getAllShippingRequest(
        this.searchInput.filter,
        this.searchInput.shippingRequestId,
        this.searchInput.channel,
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
    } /*Shipping request page*/ else {
      if (this.feature.isEnabled('App.Carrier') || this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return item.name;
      }
    }
    return '';
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
}
