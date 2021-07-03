import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TermAndConditionTranslationsServiceProxy, TermAndConditionTranslationDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTermAndConditionTranslationModalComponent } from './create-or-edit-termAndConditionTranslation-modal.component';

import { ViewTermAndConditionTranslationModalComponent } from './view-termAndConditionTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './termAndConditionTranslations.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TermAndConditionTranslationsComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTermAndConditionTranslationModal', { static: true })
  createOrEditTermAndConditionTranslationModal: CreateOrEditTermAndConditionTranslationModalComponent;
  @ViewChild('viewTermAndConditionTranslationModalComponent', { static: true })
  viewTermAndConditionTranslationModal: ViewTermAndConditionTranslationModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  languageFilter = '';
  termAndConditionTitleFilter = '';

  _entityTypeFullName = 'TACHYON.TermsAndConditions.TermAndConditionTranslation';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _termAndConditionTranslationsServiceProxy: TermAndConditionTranslationsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
  }

  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }

  getTermAndConditionTranslations(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._termAndConditionTranslationsServiceProxy
      .getAll(
        this.filterText,
        this.languageFilter,
        this.termAndConditionTitleFilter,
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

  createTermAndConditionTranslation(): void {
    this.createOrEditTermAndConditionTranslationModal.show();
  }

  showHistory(termAndConditionTranslation: TermAndConditionTranslationDto): void {
    this.entityTypeHistoryModal.show({
      entityId: termAndConditionTranslation.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteTermAndConditionTranslation(termAndConditionTranslation: TermAndConditionTranslationDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._termAndConditionTranslationsServiceProxy.delete(termAndConditionTranslation.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
