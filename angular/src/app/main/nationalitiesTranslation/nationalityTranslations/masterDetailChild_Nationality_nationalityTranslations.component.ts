import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NationalityTranslationsServiceProxy, NationalityTranslationDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { MasterDetailChild_Nationality_CreateOrEditNationalityTranslationModalComponent } from './masterDetailChild_Nationality_create-or-edit-nationalityTranslation-modal.component';

import { MasterDetailChild_Nationality_ViewNationalityTranslationModalComponent } from './masterDetailChild_Nationality_view-nationalityTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './masterDetailChild_Nationality_nationalityTranslations.component.html',
  selector: 'masterDetailChild_Nationality_nationalityTranslations-component',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class MasterDetailChild_Nationality_NationalityTranslationsComponent extends AppComponentBase {
  @Input('nationalityId') nationalityId: any;

  @ViewChild('createOrEditNationalityTranslationModal', { static: true })
  createOrEditNationalityTranslationModal: MasterDetailChild_Nationality_CreateOrEditNationalityTranslationModalComponent;
  @ViewChild('viewNationalityTranslationModalComponent', { static: true })
  viewNationalityTranslationModal: MasterDetailChild_Nationality_ViewNationalityTranslationModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  translatedNameFilter = '';
  languageFilter = '';

  constructor(
    injector: Injector,
    private _nationalityTranslationsServiceProxy: NationalityTranslationsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getNationalityTranslations(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._nationalityTranslationsServiceProxy
      .getAll(
        this.filterText,
        this.translatedNameFilter,
        this.languageFilter,
        undefined,
        this.nationalityId,
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

  createNationalityTranslation(): void {
    this.createOrEditNationalityTranslationModal.show(this.nationalityId);
  }

  deleteNationalityTranslation(nationalityTranslation: NationalityTranslationDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._nationalityTranslationsServiceProxy.delete(nationalityTranslation.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
