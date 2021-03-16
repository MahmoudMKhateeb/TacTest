import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import {
  BalanceRechargeServiceProxy,
  BalanceRechargeListDto,
  ISelectItemDto,
  CommonLookupServiceProxy,
  GetAllBalanceRechargeInput,
} from '@shared/service-proxies/service-proxies';
import { AutoCompleteModule } from 'primeng/autocomplete';
import * as moment from 'moment';

import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: './balances-list.component.html',
  animations: [appModuleAnimation()],
})
export class BalancesListComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Balances: BalanceRechargeListDto[] = [];
  IsStartSearch: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  constructor(
    injector: Injector,
    private _CurrentServ: BalanceRechargeServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }
  getAll(event?: LazyLoadEvent): void {
    if (this.creationDateRangeActive) {
      this.fromDate = moment(this.creationDateRange[0]);
      this.toDate = moment(this.creationDateRange[1]);
    } else {
      this.fromDate = null;
      this.toDate = null;
    }
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._CurrentServ
      .getAll(
        this.Tenant ? parseInt(this.Tenant.id) : undefined,
        this.fromDate,
        this.toDate,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.items.length;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  delete(input: BalanceRechargeListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.delete(input.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }

  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'shipper').subscribe((result) => {
      this.Tenants = result;
    });
  }

  exportToExcel(): void {
    var data = {
      tenantId: this.Tenant ? parseInt(this.Tenant.id) : undefined,
      fromDate: this.fromDate,
      toDate: this.toDate,
      sorting: this.primengTableHelper.getSorting(this.dataTable),
    };
    this._CurrentServ.exports(data as GetAllBalanceRechargeInput).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
