import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  CreateOrEditGoodCategoryDto,
  GetShippingRequestForViewDto,
  GoodCategoriesServiceProxy,
  ShippingRequestsServiceProxy,
  UpdatePriceInput,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-update-price-shipping-request-modal',
  templateUrl: './update-price-shipping-request-modal.component.html',
})
export class UpdatePriceShippingRequestModalComponent extends AppComponentBase {
  @ViewChild('UpdatePriceModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  item: GetShippingRequestForViewDto;
  updatePriceInput: UpdatePriceInput;

  constructor(injector: Injector, private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy) {
    super(injector);
    this.item = new GetShippingRequestForViewDto();
    this.updatePriceInput = new UpdatePriceInput();
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy.getShippingRequestForView(shippingRequestId).subscribe((result) => {
      this.item = result;
      this.updatePriceInput.price = result.shippingRequest.price;
      this.updatePriceInput.id = result.shippingRequest.id;
      this.active = true;
      this.modal.show();
    });
  }

  save(): void {
    this.saving = true;

    this._shippingRequestsServiceProxy
      .updatePrice(this.updatePriceInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
