import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

import { InvoicesProformaServiceProxy, ISelectItemDto, CommonLookupServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: 'proforma-list.component.html',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class ProformaListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  IsStartSearch: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  minAmount: number | null | undefined;
  maxAmount: number | null | undefined;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  constructor(
    injector: Injector,
    private _CurrentServ: InvoicesProformaServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }
  ngOnInit(): void {}
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
        this.minAmount,
        this.maxAmount,
        this.fromDate,
        this.toDate,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
        console.log(result);
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'shipper').subscribe((result) => {
      this.Tenants = result;
    });
  }

  //exportToExcel(): void {
  //  var data: TransactionFilterInput = new TransactionFilterInput();
  //  data.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
  //  data.fromDate = this.fromDate;
  //  data.toDate = this.toDate;
  //  data.sorting = this.primengTableHelper.getSorting(this.dataTable);

  //  this._CurrentServ.exports(data).subscribe((result) => {
  //    this._fileDownloadService.downloadTempFile(result);
  //  });
  //}
}
