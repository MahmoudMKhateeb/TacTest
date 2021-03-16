import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import * as _ from 'lodash';
import { AutoCompleteModule } from 'primeng/autocomplete';
import * as moment from 'moment';
import { GroupPeriodServiceProxy, GroupPeriodListDto, CommonLookupServiceProxy, ISelectItemDto } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  templateUrl: './group-periods-list.component.html',
  animations: [appModuleAnimation()],
})
export class GroupPeriodsListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Groups: GroupPeriodListDto[] = [];
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  Periods: ISelectItemDto[];
  periodId: number | null | undefined;
  IsStartSearch: boolean = false;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  DemandStatus: boolean | null | undefined;

  constructor(
    injector: Injector,
    private _CurrentService: GroupPeriodServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
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

    this._CurrentService
      .getAll(
        this.Tenant ? parseInt(this.Tenant.id) : undefined,
        this.periodId,
        this.DemandStatus,
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

  UnDemand(Group: GroupPeriodListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.unDemand(Group.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }
  Claim(Group: GroupPeriodListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.claim(Group.id).subscribe(() => {
          this.notify.success(this.l('Successfully'));
          this.reloadPage();
        });
      }
    });
  }
  UnClaim(Group: GroupPeriodListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentService.unClaim(Group.id).subscribe(() => {
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

  exportToExcel(): void {}
}
