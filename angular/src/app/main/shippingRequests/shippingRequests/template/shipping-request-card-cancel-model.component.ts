import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  PriceOfferServiceProxy,
  CancelShippingRequestInput,
  GetShippingRequestForPriceOfferListDto,
  ShippingRequestStatus,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'shipping-request-card-cancel-model',
  templateUrl: './shipping-request-card-cancel-model.component.html',
})
export class ShippingRequestCardCancelModelComponent extends AppComponentBase implements OnInit {
  @ViewChild('model', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();
  rejectInput: CancelShippingRequestInput = new CancelShippingRequestInput();
  saving = false;
  loading = true;
  request: GetShippingRequestForPriceOfferListDto;
  constructor(injector: Injector, private _currentServ: PriceOfferServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(request: GetShippingRequestForPriceOfferListDto): void {
    this.request = request;
    this.rejectInput.id = request.id;
    this.modal.show();
  }
  close(): void {
    this.rejectInput = new CancelShippingRequestInput();

    this.modal.hide();
  }

  send() {
    this.saving = true;
    this._currentServ
      .cancelShipment(this.rejectInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.request.status = ShippingRequestStatus.Cancled;
        this.request.statusTitle = 'Cancled';
        this.modalsave.emit(this.rejectInput.cancelReason);
        this.close();
        this.notify.success('SuccessfullyCancled');
      });
  }
}
