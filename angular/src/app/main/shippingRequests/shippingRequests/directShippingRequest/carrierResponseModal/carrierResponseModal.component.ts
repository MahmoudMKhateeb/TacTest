import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CarrirSetPriceForDirectRequestDto,
  CreateOrEditTachyonPriceOfferDto,
  ShippingRequestsTachyonDealerServiceProxy,
  TachyonDealerCreateDirectOfferToCarrirerInuptDto,
  TachyonPriceOffersServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'carrierResponceModal',
  templateUrl: './carrierResponceModal.component.html',
})
export class CarrierResponseModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('carrierResponceModalChild', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();
  carrierPriceResponse: CarrirSetPriceForDirectRequestDto = new CarrirSetPriceForDirectRequestDto();
  saving = false;
  loading = true;

  constructor(injector: Injector, private _shippingRequestsTachyonDealer: ShippingRequestsTachyonDealerServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(id): void {
    //direct price id
    this.carrierPriceResponse.id = id;
    this.modal.show();
  }
  close(): void {
    this.carrierPriceResponse = new CarrirSetPriceForDirectRequestDto();
    this.modal.hide();
  }

  send() {
    this.saving = true;
    //this.carrierResponce.
    this._shippingRequestsTachyonDealer
      .carrierSetPriceForDirectRequest(this.carrierPriceResponse)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.modalsave.emit('');
        this.close();
        this.notify.success('priceOfferSentSuccessfully');
      });
  }
}
