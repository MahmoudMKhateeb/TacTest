import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/public_api';
import * as _ from 'lodash';
import { InvoicePeriodServiceProxy, InvoicePeriodDto, InvoicePeriodType } from '@shared/service-proxies/service-proxies';
@Component({
  templateUrl: './invoice-periods-list.component.html',
  animations: [appModuleAnimation()],
})
export class InvoicePeriodsListComponent extends AppComponentBase {
  advancedFiltersAreShown: boolean = false;
  displayNameFilter: string = '';
  filterText: string = '';
  Periods: InvoicePeriodDto[] = [];
  IsStartSearch = false;
  constructor(injector: Injector, private _InvoicePeriodServiceProxy: InvoicePeriodServiceProxy) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._InvoicePeriodServiceProxy.getAll(this.filterText, null).subscribe((result) => {
      this.IsStartSearch = true;
      this.primengTableHelper.totalRecordsCount = result.items.length;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
    });
  }

  delete(period: InvoicePeriodDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._InvoicePeriodServiceProxy.delete(period.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          _.remove(this.primengTableHelper.records, period);
        });
      }
    });
  }
  DisplayPeriod(type: InvoicePeriodType) {
    return this.l(InvoicePeriodType[type]);
  }
  exportToExcel(): void {}
}
