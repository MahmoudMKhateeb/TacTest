import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  ShippingRequestsServiceProxy,
  GetShippingRequestForPricingOutput,
  PriceOfferItemDto,
  PriceOfferChannel,
  ShippingRequestBidStatus,
  ShippingRequestStatus,
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: './price-offer-model-component.html',
  styleUrls: ['./price-offer-model-component.scss'],
  selector: 'price-offer-model',
  animations: [appModuleAnimation()],
})
export class PriceOfferModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  request: GetShippingRequestForPricingOutput = new GetShippingRequestForPricingOutput();
  direction: string;
  Items: PriceOfferItemDto[] = [];
  constructor(injector: Injector, private _CurrentServ: ShippingRequestsServiceProxy) {
    super(injector);
  }

  show(id: number): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this._CurrentServ.getShippingRequestForPricing(id).subscribe((result) => {
      this.request = result;
      this.Items = this.request.items;
      this.active = true;
      this.modal.show();
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  save(): void {
    this.saving = true;
  }
}
