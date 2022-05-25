import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  NormalPricePackagesServiceProxy,
  PricePackageOfferDto,
  PricePackageOfferItemDto,
  PriceOfferChannel,
  ShippingRequestStatus,
  ShippingRequestBidStatus,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './normal-price-package-calculation-component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'normal-price-package-calculation',
  animations: [appModuleAnimation()],
})
export class NormalPricePackageCalculationComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<number> = new EventEmitter<number>();
  @Input() directRequestStatus: number | null | undefined;

  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  offer: PricePackageOfferDto = new PricePackageOfferDto();
  Items: PricePackageOfferItemDto[] = [];
  priceOfferCommissionType: any;
  commissionTypeTitle: string;
  shippingRequestId: number;
  pricePackageId: number;
  matchingPricePackageId: number;
  isBid: boolean;
  requestStatus: ShippingRequestStatus;
  bidStatus: ShippingRequestBidStatus;
  channel: PriceOfferChannel;
  isTachyonDeal: boolean;

  constructor(injector: Injector, private _CurrentServ: NormalPricePackagesServiceProxy) {
    super(injector);
  }

  show(id: number, pricePackageId: number | undefined = undefined): void {
    this.shippingRequestId = id;
    this.pricePackageId = pricePackageId;
    this._CurrentServ.getPricePackageOfferForHandle(pricePackageId, id).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
    });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  Preview(id: number, pricePackageId: number | undefined = undefined): void {
    this.shippingRequestId = id;
    this.pricePackageId = pricePackageId;
    this._CurrentServ.getPricePackageOffer(pricePackageId, id).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
    });
  }

  canSendPriceOfferByPricePackage(): boolean {
    if (
      this.isBid &&
      (this.requestStatus === ShippingRequestStatus.PrePrice || this.requestStatus === ShippingRequestStatus.NeedsAction) &&
      this.bidStatus === ShippingRequestBidStatus.OnGoing &&
      this.channel == PriceOfferChannel.MarketPlace
    ) {
      return true;
    }
    return false;
  }

  PreviewMatching(
    id: number,
    matchingPricePackageId: number,
    channel: PriceOfferChannel,
    bidStatus: ShippingRequestBidStatus,
    requestStatus: ShippingRequestStatus,
    isBid: boolean,
    isTachyonDeal: boolean
  ): void {
    this.shippingRequestId = id;
    this.matchingPricePackageId = matchingPricePackageId;
    this.channel = channel;
    this.bidStatus = bidStatus;
    this.requestStatus = requestStatus;
    this.isBid = isBid;
    this.isTachyonDeal = isTachyonDeal;
    this._CurrentServ.getPricePackageOfferForHandle(matchingPricePackageId, id).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
    });
  }

  submit(id: number, pricePackageId: number): void {
    this.saving = true;
    this._CurrentServ
      .handlePricePackageOfferToCarrier(pricePackageId, id)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.info(this.l('SendSuccessfully'));
        this.close();
      });
  }

  accept(pricePackageId: number) {
    this.saving = true;
    this._CurrentServ
      .acceptPricePackageOffer(pricePackageId)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.info(this.l('SendSuccessfully'));
        this.close();
        this.modalSave.emit(result);
      });
  }
  sendPriceOfferByPricePackage() {
    this.saving = true;
    this._CurrentServ
      .sendPriceOfferByPricePackage(this.matchingPricePackageId, this.shippingRequestId)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.info(this.l('SendSuccessfully'));
        this.close();
        this.modalSave.emit(result);
      });
  }
  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
}
