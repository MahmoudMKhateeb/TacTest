import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

import {
  TransactionServiceProxy,
  TransactionListDto,
  ISelectItemDto,
  CommonLookupServiceProxy,
  ChannelType,
  TransactionFilterInput,
  EditionServiceProxy,
  ComboboxItemDto,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: 'transaction-list.component.html',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class TransactionListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Transactions: TransactionListDto[] = [];
  IsStartSearch: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  minLongitude: number | null | undefined;
  maxLongitude: number | null | undefined;
  ChannelType: any;
  Channel: ChannelType | undefined = undefined;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  editions: ComboboxItemDto[] = [];
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  editionId!: number | undefined;
  constructor(
    injector: Injector,
    private _CurrentServ: TransactionServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private enumToArray: EnumToArrayPipe,
    private _editionService: EditionServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.ChannelType = this.enumToArray.transform(ChannelType);
    if (!this.appSession.tenantId || this.feature.isEnabled('App.TachyonDealer')) {
      this._editionService.getEditionComboboxItems(0, true, false).subscribe((editions) => {
        this.editions = editions;
      });
    }
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
        this.Channel,
        this.Tenant ? parseInt(this.Tenant.id) : undefined,
        this.fromDate,
        this.toDate,
        this.minLongitude,
        this.maxLongitude,
        this.editionId,
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

  exportToExcel(): void {
    var data: TransactionFilterInput = new TransactionFilterInput();
    data.channelType = this.Channel;
    data.tenantId = this.Tenant ? parseInt(this.Tenant.id) : undefined;
    data.fromDate = this.fromDate;
    data.toDate = this.toDate;
    data.sorting = this.primengTableHelper.getSorting(this.dataTable);

    this._CurrentServ.exports(data).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
