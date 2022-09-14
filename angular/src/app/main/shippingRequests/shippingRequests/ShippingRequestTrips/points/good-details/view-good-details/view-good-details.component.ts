import { Component, Injector, Input, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { ShippingRequestsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'view-good-details',
  templateUrl: './view-good-details.component.html',
  styleUrls: ['./view-good-details.component.css'],
})
export class ViewGoodDetailsComponent extends AppComponentBase {
  @ViewChild('viewGoodDetail', { static: false }) public modal: ModalDirective;
  @Input() allSubGoodCategorys: any;
  active = false;
  goodDetails: any;

  constructor(injector: Injector, private _shippingRequest: ShippingRequestsServiceProxy) {
    super(injector);
  }

  show(record: any) {
    this.goodDetails = record;
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.modal.hide();
  }
}
