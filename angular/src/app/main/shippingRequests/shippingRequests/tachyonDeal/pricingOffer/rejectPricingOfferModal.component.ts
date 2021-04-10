import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AcceptOrRejectOfferByShipperInput, TachyonPriceOffersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'rejectPricingOfferModal',
  templateUrl: './rejectPricingOfferModal.component.html',
})
export class RejectPricingOfferModalComponent extends AppComponentBase {
  @ViewChild('rejectPricingOfferModal', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();

  acceptOrRejectInputs: AcceptOrRejectOfferByShipperInput = new AcceptOrRejectOfferByShipperInput();
  saving = false;
  loading = true;

  constructor(injector: Injector, private _tachyonPriceOffersServiceProxy: TachyonPriceOffersServiceProxy) {
    super(injector);
  }

  show(id): void {
    this.acceptOrRejectInputs.id = id;
    this.modal.show();
  }
  close(): void {
    this.acceptOrRejectInputs = new AcceptOrRejectOfferByShipperInput();
    this.modal.hide();
  }

  send() {
    this.saving = true;
    this.acceptOrRejectInputs.isAccepted = false;
    this._tachyonPriceOffersServiceProxy
      .acceptOrRejectOfferByShipper(this.acceptOrRejectInputs)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.modalsave.emit('');
        this.close();
        this.notify.success('offerRejectedSuccessfully');
      });
  }
}
