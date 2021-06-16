import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/public_api';
import { Table } from 'primeng/table';
import * as _ from 'lodash';
import { InvoicePaymentMethodServiceProxy, InvoicePaymentMethodListDto, InvoicePaymentType } from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: './invoice-payment-method.component.html',
  animations: [appModuleAnimation()],
})
export class InvoicePaymentMethodComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  filterText: string = '';
  IsStartSearch = false;
  constructor(injector: Injector, private _currentSrv: InvoicePaymentMethodServiceProxy) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._currentSrv.getAll(this.filterText, this.primengTableHelper.getSorting(this.dataTable)).subscribe((result) => {
      this.IsStartSearch = true;
      this.primengTableHelper.totalRecordsCount = result.items.length;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
    });
  }

  delete(payment: InvoicePaymentMethodListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._currentSrv.delete(payment.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          _.remove(this.primengTableHelper.records, payment);
        });
      }
    });
  }

  DisplayPaymentTitle(type: InvoicePaymentType) {
    return this.l(InvoicePaymentType[type]);
  }

  update(payment: InvoicePaymentMethodListDto): void {
    this.primengTableHelper.records.push(payment);
  }
}
