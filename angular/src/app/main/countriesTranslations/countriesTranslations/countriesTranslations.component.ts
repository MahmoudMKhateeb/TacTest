import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CountriesTranslationsServiceProxy, CountriesTranslationDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCountriesTranslationModalComponent } from './create-or-edit-countriesTranslation-modal.component';

import { ViewCountriesTranslationModalComponent } from './view-countriesTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './countriesTranslations.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class CountriesTranslationsComponent extends AppComponentBase {
  @ViewChild('createOrEditCountriesTranslationModal', { static: true })
  createOrEditCountriesTranslationModal: CreateOrEditCountriesTranslationModalComponent;
  @ViewChild('viewCountriesTranslationModalComponent', { static: true }) viewCountriesTranslationModal: ViewCountriesTranslationModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  translatedDisplayNameFilter = '';
  languageFilter = '';
  countyDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _countriesTranslationsServiceProxy: CountriesTranslationsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getCountriesTranslations(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._countriesTranslationsServiceProxy
      .getAll(
        this.filterText,
        this.translatedDisplayNameFilter,
        this.languageFilter,
        this.countyDisplayNameFilter,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createCountriesTranslation(): void {
    this.createOrEditCountriesTranslationModal.show();
  }

  deleteCountriesTranslation(countriesTranslation: CountriesTranslationDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._countriesTranslationsServiceProxy.delete(countriesTranslation.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
