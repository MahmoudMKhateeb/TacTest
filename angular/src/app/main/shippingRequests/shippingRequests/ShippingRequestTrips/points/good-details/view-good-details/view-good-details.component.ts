import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { ShippingRequestsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'view-good-details',
  templateUrl: './view-good-details.component.html',
  styleUrls: ['./view-good-details.component.css'],
})
export class ViewGoodDetailsComponent extends AppComponentBase implements OnInit {
  @ViewChild('viewGoodDetail', { static: false }) public modal: ModalDirective;
  active = false;
  goodDetails: any;
  @Input() allSubGoodCategorys: any;
  constructor(injector: Injector, private _shippingRequest: ShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    console.log('View Component is Working');
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

  getGoodSubDisplayname(id: number) {
    return this.allSubGoodCategorys ? this.allSubGoodCategorys.find((x) => x.id == id)?.displayName : 0;
  }
}
