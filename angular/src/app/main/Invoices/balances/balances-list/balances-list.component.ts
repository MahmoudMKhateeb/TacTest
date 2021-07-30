import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
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
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
@Component({
  templateUrl: './balances-list.component.html',
  animations: [appModuleAnimation()],
})
export class BalancesListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Balances: BalanceRechargeListDto[] = [];
  IsStartSearch: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  ReferenceNo: string | null | undefined;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  TenantId: number | undefined = undefined;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  minLongitude: number | null | undefined;
  maxLongitude: number | null | undefined;
  dataSource: any = {};
  constructor(
    injector: Injector,
    private _CurrentServ: BalanceRechargeServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.fillDataSource();
  }
  getAll(event?: LazyLoadEvent): void {
    if (this.creationDateRangeActive) {
      this.fromDate = moment(this.creationDateRange[0]);
      this.toDate = moment(this.creationDateRange[1]);
    } else {
      this.fromDate = null;
      this.toDate = null;
    }

    if (this.Tenant?.id) {
      this.TenantId = parseInt(this.Tenant.id);
    } else {
      this.Tenant = undefined;
    }
    this.primengTableHelper.showLoadingIndicator();
    // this._CurrentServ
    //   .getAll(
    //     this.TenantId,
    //     this.fromDate,
    //     this.toDate,
    //     this.ReferenceNo,
    //     this.minLongitude,
    //     this.maxLongitude,
    //     this.primengTableHelper.getSorting(this.dataTable),
    //     this.primengTableHelper.getSkipCount(this.paginator, event),
    //     this.primengTableHelper.getMaxResultCount(this.paginator, event)
    //   )
    //   .subscribe((result) => {
    //     this.IsStartSearch = true;
    //     this.primengTableHelper.totalRecordsCount = result.totalCount;
    //     this.primengTableHelper.records = result.items;
    //     this.primengTableHelper.hideLoadingIndicator();
    //   });
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
    // var data = {
    //   tenantId: this.Tenant ? parseInt(this.Tenant.id) : undefined,
    //   fromDate: this.fromDate,
    //   toDate: this.toDate,
    //   sorting: this.primengTableHelper.getSorting(this.dataTable),
    // };
    // this._CurrentServ.exports(data as GetAllBalanceRechargeInput).subscribe((result) => {
    //   this._fileDownloadService.downloadTempFile(result);
    // });
  }

  fillDataSource() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._CurrentServ
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.items,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
