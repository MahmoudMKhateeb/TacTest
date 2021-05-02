import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/public_api';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';

import * as _ from 'lodash';
import {
  AppLocalizationServiceProxy,
  IAppLocalizationListDto,
  AppLocalizationFilterInput,
  ComboboxItemDto,
  EditionServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: './applocalization.component.html',
  animations: [appModuleAnimation()],
})
export class AppLocalizationComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  input: AppLocalizationFilterInput = new AppLocalizationFilterInput();
  AppLocalizations: IAppLocalizationListDto[] = [];
  editions: ComboboxItemDto[] = [];
  IsStartSearch = false;
  constructor(
    injector: Injector,
    private _ServiceProxy: AppLocalizationServiceProxy,
    private _editionService: EditionServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this._editionService.getEditionComboboxItems(0, true, false).subscribe((editions) => {
      this.editions = editions;
    });
  }
  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();

    this._ServiceProxy
      .getAll(
        this.input.filter,
        this.input.editionId,
        this.input.page,
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
    this.input.sorting = this.primengTableHelper.getSorting(this.dataTable);
    this._ServiceProxy.exports(this.input).subscribe((result) => {
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
