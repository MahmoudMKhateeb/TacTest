import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  AcceptShippingRequestPriceInput,
  CreateOrEditGoodCategoryDto,
  GetShippingRequestForViewOutput,
  GoodCategoriesServiceProxy,
  ShippingRequestsServiceProxy,
  ShippingRequestVasPriceDto,
  UpdatePriceInput,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-update-price-shipping-request-modal',
  templateUrl: './update-price-shipping-request-modal.component.html',
})
export class UpdatePriceShippingRequestModalComponent extends AppComponentBase {
  @ViewChild('UpdatePriceModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  allVases: ShippingRequestVasPriceDto[] = [];
  active = false;
  saving = false;
  item: GetShippingRequestForViewOutput;
  updatePriceInput: UpdatePriceInput;
  acceptShippingRequestPriceInput: AcceptShippingRequestPriceInput;
  private _router: Router;

  constructor(injector: Injector, private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy) {
    super(injector);
    this.item = new GetShippingRequestForViewOutput();
    this.updatePriceInput = new UpdatePriceInput();
    this.acceptShippingRequestPriceInput = new AcceptShippingRequestPriceInput();
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy.getShippingRequestForView(shippingRequestId).subscribe((result) => {
      this.item = result;
      this.updatePriceInput.price = result.shippingRequest.price;
      this.updatePriceInput.id = result.shippingRequest.id;
      this.acceptShippingRequestPriceInput.id = result.shippingRequest.id;
      this.active = true;
      this.getallRequestedVasesForPricing(shippingRequestId);
      this.modal.show();
    });
  }

  save(): void {
    this.saving = true;
    this.updatePriceInput.pricedVasesList = this.allVases;
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

  acceptPrice() {
    this.acceptShippingRequestPriceInput.isPriceAccepted = true;
    this._shippingRequestsServiceProxy
      .acceptOrRejectShippingRequestPrice(this.acceptShippingRequestPriceInput)
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

  Reject() {
    this.acceptShippingRequestPriceInput.isPriceAccepted = true;
    this._shippingRequestsServiceProxy
      .rejectShippingRequest(this.item.shippingRequest.id)
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

  clone() {
    this._router.navigate(['/app/main/shippingRequests/shippingRequests?id']);
  }

  // isPriceZero(item: ShippingRequestVasPriceDto) {
  //   if(item.actualPrice.toString() == ""){
  //     item.actualPrice = null;
  //   }
  // }

  getallRequestedVasesForPricing(shippingRequestId: number) {
    this._shippingRequestsServiceProxy.getAllShippingRequestVasForPricing(shippingRequestId).subscribe((result) => {
      this.allVases = result;
    });
  }
}
