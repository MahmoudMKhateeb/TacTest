import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { PriceOfferServiceProxy, RejectPriceOfferInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'price-offer-reject-model',
  templateUrl: './price-offer-reject-model-component.html',
})
export class PriceOfferRejectModelComponent extends AppComponentBase implements OnInit {
  @ViewChild('carrierResponceModalChild', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();
  rejectInput: RejectPriceOfferInput = new RejectPriceOfferInput();
  saving = false;
  loading = true;

  constructor(injector: Injector, private _currentServ: PriceOfferServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(id): void {
    this.rejectInput.id = id;
    this.modal.show();
  }
  close(): void {
    this.rejectInput = new RejectPriceOfferInput();

    this.modal.hide();
  }

  send() {
    this.saving = true;
    this._currentServ
      .reject(this.rejectInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.modalsave.emit(this.rejectInput.reason);
        this.close();
        this.notify.success('priceOfferSentSuccessfully');
      });
  }
}
