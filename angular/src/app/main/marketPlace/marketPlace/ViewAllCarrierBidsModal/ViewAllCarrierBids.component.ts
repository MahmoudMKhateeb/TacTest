import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShippingRequestBidsServiceProxy } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'ViewAllCarrierBidsModal',
  styleUrls: ['../marketPlace/marketPlaceStyling.css'],
  templateUrl: './view-all-carrier-bids.modal.html',
})
export class ViewAllCarrierBidsComponent extends AppComponentBase {
  @ViewChild('ViewAllCarrierBidsModal', { static: true }) modal: ModalDirective;

  active = false;
  saving = false;
  AllCarrierBidsArray = [];
  Loading = true;
  constructor(injector: Injector, private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy) {
    super(injector);
  }

  show(): void {
    this.Loading = true;
    this.GetAllCarrierBid();
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.AllCarrierBidsArray = null;
    this.modal.hide();
  }
  GetAllCarrierBid() {
    this._shippingRequestBidsServiceProxy.getAllCarrierShippingRequestBids().subscribe((results) => {
      this.AllCarrierBidsArray = results;
      this.Loading = false;
    });
  }
}
