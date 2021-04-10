import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { CreateOrEditTachyonPriceOfferDto, TachyonPriceOffersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'sendPricingOfferModal',
  templateUrl: './sendPricingOfferModal.component.html',
})
export class SendPricingOfferModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('sendPricingOfferModal', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();
  priceOffer: CreateOrEditTachyonPriceOfferDto = new CreateOrEditTachyonPriceOfferDto();
  saving = false;
  loading = true;

  constructor(injector: Injector, private _tachyonPriceOffersServiceProxy: TachyonPriceOffersServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(id): void {
    this.priceOffer.shippingRequestId = id;
    this.modal.show();
  }
  close(): void {
    this.priceOffer = new CreateOrEditTachyonPriceOfferDto();
    this.modal.hide();
  }

  send() {
    this.saving = true;
    this._tachyonPriceOffersServiceProxy
      .createOrEditTachyonPriceOffer(this.priceOffer)
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
