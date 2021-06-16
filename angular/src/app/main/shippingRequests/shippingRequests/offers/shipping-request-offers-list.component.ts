import { Component, ViewChild, Injector, Input } from '@angular/core';
import { PriceOfferServiceProxy, PriceOfferChannel } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';

@Component({
  selector: 'shipping-request-offers-list',
  templateUrl: './shipping-request-offers-list.component.html',
})
export class ShippingRequestOffersList extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Input() shippingRequestId: number;
  @Input() Channel: PriceOfferChannel;
  @Input() isTachyonDeal: boolean;
  IsStartSearch: boolean = false;

  constructor(injector: Injector, private _currentServ: PriceOfferServiceProxy) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._currentServ
      .getAll(
        this.shippingRequestId,
        undefined,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  Reject() {
    this.reloadPage();
  }
}
