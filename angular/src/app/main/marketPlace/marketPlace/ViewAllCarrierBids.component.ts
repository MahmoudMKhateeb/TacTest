import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShippingRequestBidsServiceProxy } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'ViewAllCarrierBidsModal',
  styleUrls: ['./marketPlaceStyling.css'],
  templateUrl: './View-all-carrier-bids.modal.html',
})
export class ViewAllCarrierBidsComponent extends AppComponentBase {
  @ViewChild('ViewAllCarrierBidsModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  AllCarrierBidsArray = [];
  Loading = true;
  constructor(injector: Injector, private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy) {
    super(injector);
  }

  // ngOnInit() {
  //
  // }
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
  /**
   * @return Success
   */
  GetAllCarrierBid() {
    this._shippingRequestBidsServiceProxy.getAllCarrierBidsForView().subscribe((results) => {
      this.AllCarrierBidsArray = results;
      this.Loading = false;
    });
  }
}
