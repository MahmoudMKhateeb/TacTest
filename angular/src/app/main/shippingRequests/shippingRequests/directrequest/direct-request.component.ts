import { Component, ViewChild, Injector, Input } from '@angular/core';
import { ShippingRequestDirectRequestServiceProxy, ShippingRequestDirectRequestListDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';

import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { Router } from '@angular/router';

@Component({
  selector: 'direct-request',
  templateUrl: './direct-request.component.html',
})
export class DirectRequestComponent extends AppComponentBase {
  @ViewChild('dataTableForCarrier', { static: true }) dataTableForCarrier: Table;
  @ViewChild('paginatorForCarrier', { static: true }) paginatorForCarrier: Paginator;
  @Input() shippingRequestId: number;
  saving = false;
  loading = true;
  filterText: any;

  constructor(injector: Injector, private _router: Router, private _currentServ: ShippingRequestDirectRequestServiceProxy) {
    super(injector);
  }

  reloadPage(): void {
    this.paginatorForCarrier.changePage(this.paginatorForCarrier.getPage());
  }
  getDirectRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorForCarrier.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._currentServ
      .getAll(
        this.shippingRequestId,
        this.primengTableHelper.getSorting(this.dataTableForCarrier),
        this.primengTableHelper.getSkipCount(this.paginatorForCarrier, event),
        this.primengTableHelper.getMaxResultCount(this.paginatorForCarrier, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  delete(input: ShippingRequestDirectRequestListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._currentServ.delete(input.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }
}
