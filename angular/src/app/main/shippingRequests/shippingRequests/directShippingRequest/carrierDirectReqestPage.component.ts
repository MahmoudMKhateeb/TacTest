import { Component, ViewChild, Injector, ViewEncapsulation } from '@angular/core';
import {
  CreateOrEditTachyonPriceOfferDto,
  ShippingRequestsTachyonDealerServiceProxy,
  TachyonPriceOffersServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router } from '@angular/router';

@Component({
  templateUrl: './carrierDirectReqestPage.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class CarrierDirectReqestPageComponent extends AppComponentBase {
  @ViewChild('dataTableForCarrier', { static: true }) dataTableForCarrier: Table;
  @ViewChild('paginatorForCarrier', { static: true }) paginatorForCarrier: Paginator;
  saving = false;
  loading = true;
  filterText: any;

  constructor(injector: Injector, private _router: Router, private _shippingRequestsTachyonDealer: ShippingRequestsTachyonDealerServiceProxy) {
    super(injector);
  }
}
