import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TruckStatusesTranslationsServiceProxy, TruckStatusesTranslationDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckStatusesTranslationModalComponent } from './create-or-edit-truckStatusesTranslation-modal.component';

import { ViewTruckStatusesTranslationModalComponent } from './view-truckStatusesTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './truckStatusesTranslations.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TruckStatusesTranslationsComponent extends AppComponentBase {
  @ViewChild('createOrEditTruckStatusesTranslationModal', { static: true })
  createOrEditTruckStatusesTranslationModal: CreateOrEditTruckStatusesTranslationModalComponent;
  @ViewChild('viewTruckStatusesTranslationModalComponent', { static: true })
  viewTruckStatusesTranslationModal: ViewTruckStatusesTranslationModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  translatedDisplayNameFilter = '';
  languageFilter = '';
  truckStatusDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _truckStatusesTranslationsServiceProxy: TruckStatusesTranslationsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getTruckStatusesTranslations(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._truckStatusesTranslationsServiceProxy
      .getAll(
        this.filterText,
        this.translatedDisplayNameFilter,
        this.languageFilter,
        this.truckStatusDisplayNameFilter,
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

  createTruckStatusesTranslation(): void {
    this.createOrEditTruckStatusesTranslationModal.show();
  }

  deleteTruckStatusesTranslation(truckStatusesTranslation: TruckStatusesTranslationDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._truckStatusesTranslationsServiceProxy.delete(truckStatusesTranslation.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
