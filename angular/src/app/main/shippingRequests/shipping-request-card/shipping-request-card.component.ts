import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  GetShippingRequestForPriceOfferListDto,
  PriceOfferChannel,
  ShippingRequestDirectRequestStatus,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestStatus,
  ShippingRequestType,
  ShippingTypeEnum,
} from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import { ShippingrequestsDetailsModelComponent } from '@app/main/shippingRequests/shippingRequests/details/shippingrequests-details-model.component';
import { AssignTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/assign-trucks-and-drivers-modal/assign-trucks-and-drivers-modal.component';
import { ReplaceTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/replace-trucks-and-drivers-modal/replace-trucks-and-drivers-modal.component';
import { DedicatedShippingRequestAttendanceSheetModalComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-shipping-request-attendance-sheet-modal.component';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';

@Component({
  selector: 'app-shipping-request-card',
  templateUrl: './shipping-request-card.component.html',
  styleUrls: ['./shipping-request-card.component.scss'],
  animations: [appModuleAnimation()],
})
export class ShippingRequestCardComponent extends AppComponentBase implements OnInit {
  @Output() onSearch: EventEmitter<any> = new EventEmitter<any>();
  @Output() onLoadData: EventEmitter<any> = new EventEmitter<any>();
  @Output() onDelete: EventEmitter<GetShippingRequestForPriceOfferListDto> = new EventEmitter<GetShippingRequestForPriceOfferListDto>();
  @Output() onDecline: EventEmitter<GetShippingRequestForPriceOfferListDto> = new EventEmitter<GetShippingRequestForPriceOfferListDto>();
  @Output() onExpanded: EventEmitter<boolean> = new EventEmitter<boolean>();

  @Input() item: GetShippingRequestForPriceOfferListDto;
  @Input() Channel: PriceOfferChannel | number | null | undefined = undefined;

  PriceOfferChannelEnum = PriceOfferChannel;
  ShippingRequestFlagEnum = ShippingRequestFlag;
  shippingRequestStatusEnum = ShippingRequestStatus;
  ShippingTypeEnum = ShippingTypeEnum;
  ShippingRequestRouteType = ShippingRequestRouteType;
  expanded = false;

  constructor(injector: Injector, private router: Router, private _tripService: TripService) {
    super(injector);
  }

  ngOnInit() {}

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

  getWordTitle(n: any, word: string): string {
    if (parseInt(n) === 1) {
      return this.l(word);
    }
    return this.l(`${word}s`);
  }

  canSeeTotalOffers(item: GetShippingRequestForPriceOfferListDto) {
    if (item.price > 0) {
      return false;
    }
    if (item.totalOffers > 0 && !this.isCarrier && this.Channel != 2 && this.Channel != 10 && item.status == 2) {
      return true;
    }
    return false;
  }

  search(): void {
    this.onSearch.emit(true);
  }

  LoadData(): void {
    this.onLoadData.emit(true);
  }

  canViewBrokerPrice(item: GetShippingRequestForPriceOfferListDto): boolean {
    if ((item.shipperActor || item.carrierActor) && !this.isTachyonDealerOrHost) {
      return false;
    }

    return true;
  }

  mapReady(event: any) {
    event.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById('Settings'));
  }

  expand() {
    this.expanded = true;
    this.onExpanded.emit(true);
  }

  collapse() {
    this.expanded = false;
    this.onExpanded.emit(true);
  }
}
