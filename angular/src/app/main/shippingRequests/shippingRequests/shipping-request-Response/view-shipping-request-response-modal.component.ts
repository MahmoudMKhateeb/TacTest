import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  AcceptShippingRequestPriceInput,
  CreateOrEditGoodCategoryDto,
  GetShippingRequestForViewDto,
  GoodCategoriesServiceProxy,
  ShippingRequestPricingOutputforView,
  ShippingRequestsServiceProxy,
  ShippingRequestVasPriceDto,
  UpdatePriceInput,
} from '@shared/service-proxies/service-proxies';
import { elementAt, finalize } from '@node_modules/rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-view-shipping-request-price-response-modal',
  templateUrl: './view-shipping-request-response-modal.component.html',
})
export class ViewShippingRequestPriceResponseModalComponent extends AppComponentBase {
  @ViewChild('ViewPriceResponseModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: ShippingRequestPricingOutputforView = new ShippingRequestPricingOutputforView();
  fullPrice: number = 0.0;
  private _router: Router;

  constructor(injector: Injector, private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy) {
    super(injector);
    this.item.pricedVasesList = [];
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy.getAllShippingRequestPricingForView(shippingRequestId).subscribe((result) => {
      this.item = result;
      if (this.item != null) {
        this.getfullPrice();
      }
      this.active = true;
      this.modal.show();
    });
  }

  getfullPrice() {
    if (this.item.pricedVasesList.length > 0) {
      this.item.pricedVasesList.forEach((element) => {
        this.fullPrice = this.fullPrice + element.actualPrice * (element.shippingRequestVas.maxCount == 0 ? 1 : element.shippingRequestVas.maxCount);
      });
    }

    this.fullPrice = this.fullPrice + this.item.shippingRequestPrice;
  }

  close(): void {
    this.fullPrice = 0.0;
    this.active = false;
    this.modal.hide();
  }
}
