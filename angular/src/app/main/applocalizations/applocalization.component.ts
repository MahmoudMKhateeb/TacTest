import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/public_api';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';

import * as _ from 'lodash';
import { AppLocalizationServiceProxy, IAppLocalizationListDto, AppLocalizationFilterInput } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: './applocalization.component.html',
  animations: [appModuleAnimation()],
})
export class AppLocalizationComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  filterText: string = '';
  AppLocalizations: IAppLocalizationListDto[] = [];
  IsStartSearch = false;
  constructor(injector: Injector, private _ServiceProxy: AppLocalizationServiceProxy, private _fileDownloadService: FileDownloadService) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._ServiceProxy
      .getAll(
        this.filterText,
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
  delete(localize: IAppLocalizationListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.delete(localize.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }

  exportToExcel(): void {
    var data = {
      filter: this.filterText,
      sorting: this.primengTableHelper.getSorting(this.dataTable),
    };
    this._ServiceProxy.exports(data as AppLocalizationFilterInput).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  restore(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.restore().subscribe(() => {
          this.notify.success(this.l('SuccessfullyRestore'));
          this.reloadPage();
        });
      }
    });
  }
  generate(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.generate().subscribe(() => {
          this.notify.success(this.l('SuccessfullyGenerate'));
        });
      }
    });
  }
}
