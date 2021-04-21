import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import * as _ from 'lodash';
import { AutoCompleteModule } from 'primeng/autocomplete';
import * as moment from 'moment';
import {
  GroupPeriodServiceProxy,
  GroupPeriodListDto,
  CommonLookupServiceProxy,
  ISelectItemDto,
  SubmitInvoiceStatus,
  GroupPeriodFilterInput,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  templateUrl: './group-periods-list.component.html',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class GroupPeriodsListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Groups: GroupPeriodListDto[] = [];
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  Periods: ISelectItemDto[];
  TenantId: number | null | undefined;
  periodId: number | null | undefined;
  IsStartSearch: boolean = false;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  Status: SubmitInvoiceStatus | null | undefined;
  SubmitStatus: any;
  constructor(
    injector: Injector,
    private _CurrentService: GroupPeriodServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.SubmitStatus = this.enumToArray.transform(SubmitInvoiceStatus);
  }
  ngOnInit() {
    this._CommonServ.getPeriods().subscribe((result) => {
      this.Periods = result;
    });
  }
  getAll(event?: LazyLoadEvent): void {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    if (this.creationDateRangeActive) {
      this.fromDate = moment(this.creationDateRange[0]);
      this.toDate = moment(this.creationDateRange[1]);
    } else {
      this.fromDate = null;
      this.toDate = null;
    }

    if (this.Tenant != null && this.Tenant.id != null) {
      this.TenantId = parseInt(this.Tenant.id);
    } else {
      this.TenantId = undefined;
      this.Tenant = undefined;
    }
    this._CurrentService
      .getAll(
        this.TenantId,
        this.periodId,
        this.fromDate,
        this.toDate,
        this.Status,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.items.length;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
        console.log(result.items);
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  delete(Group: GroupPeriodListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.delete(Group.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }

  Accepted(Group: GroupPeriodListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.accepted(Group.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }

  downloadDocument(id: number): void {
    this._CurrentService.getFileDto(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, 'carrier').subscribe((result) => {
      this.Tenants = result;
    });
  }
  StyleStatus(Status: SubmitInvoiceStatus): string {
    switch (Status) {
      case SubmitInvoiceStatus.Accepted:
        return 'label label-success label-inline m-1';
      case SubmitInvoiceStatus.Rejected:
        return 'label label-danger label-inline m-1';
      case SubmitInvoiceStatus.Claim:
        return 'label label-info label-inline m-1';
      default:
        return 'label label-default label-inline m-1';
    }
    return '';
  }
  exportToExcel(): void {
    var data = {
      tenantId: this.Tenant ? parseInt(this.Tenant.id) : undefined,
      periodId: this.periodId,
      fromDate: this.fromDate,
      toDate: this.toDate,
      sorting: this.primengTableHelper.getSorting(this.dataTable),
    };
    this._CurrentService.exports(data as GroupPeriodFilterInput).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
