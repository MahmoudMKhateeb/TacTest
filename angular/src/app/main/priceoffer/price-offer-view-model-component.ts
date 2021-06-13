import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  PriceOfferServiceProxy,
  PriceOfferViewDto,
  PriceOfferItem,
  PriceOfferChannel,
  CreateOrEditPriceOfferInput,
  PriceOfferDetailDto,
  ShippingRequestStatus,
  PriceOfferStatus,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './price-offer-view-model-component.html',
  styleUrls: ['./price-offer-view-model-component.scss'],
  selector: 'price-offer-view-model',
  animations: [appModuleAnimation()],
})
export class PriceOfferViewModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Output() modalDelete: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  offer: PriceOfferViewDto = new PriceOfferViewDto();
  input: CreateOrEditPriceOfferInput = new CreateOrEditPriceOfferInput();
  direction: string;
  Items: PriceOfferItem[] = [];
  constructor(injector: Injector, private _CurrentServ: PriceOfferServiceProxy) {
    super(injector);
  }

  show(shippingRequestId: number, offerId: number): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this._CurrentServ.getPriceOfferForView(offerId).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
      this.input.shippingRequestId = shippingRequestId;
      this.input.channel = this.Channel;
      console.log(this.offer);
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  delete(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.delete(this.offer.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.modalDelete.emit(null);
          this.close();
        });
      }
    });
  }

  canAcceptOrRejectOffer() {
    if (this.offer.shippingRequestStatus == ShippingRequestStatus.NeedsAction && this.offer.status == PriceOfferStatus.New) {
      if (this.offer.isTachyonDeal && (this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) && this.offer.editionId != 4) {
        return true;
      } else if (!this.offer.isTachyonDeal && this.feature.isEnabled('App.Shipper')) {
        return true;
      }
    }
    return false;
  }
  canDeleteOffer() {
    if (this.offer.shippingRequestStatus == ShippingRequestStatus.NeedsAction && this.offer.status == PriceOfferStatus.New) {
      if ((this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) && this.offer.editionId == 4) {
        return true;
      } else if (this.feature.isEnabled('App.Carrier')) {
        return true;
      }
    }
    return false;
  }
}
