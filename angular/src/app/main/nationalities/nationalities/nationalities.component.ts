import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NationalitiesServiceProxy, NationalityDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditNationalityModalComponent } from './create-or-edit-nationality-modal.component';

import { ViewNationalityModalComponent } from './view-nationality-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './nationalities.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class NationalitiesComponent extends AppComponentBase {
  @ViewChild('createOrEditNationalityModal', { static: true }) createOrEditNationalityModal: CreateOrEditNationalityModalComponent;
  @ViewChild('viewNationalityModalComponent', { static: true }) viewNationalityModal: ViewNationalityModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';

  nationalityTranslationRowSelection: boolean[] = [];

  childEntitySelection: {} = {};

  constructor(
    injector: Injector,
    private _nationalitiesServiceProxy: NationalitiesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getNationalities(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._nationalitiesServiceProxy
      .getAll(
        this.filterText,
        this.nameFilter,
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

  createNationality(): void {
    this.createOrEditNationalityModal.show();
  }

  deleteNationality(nationality: NationalityDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._nationalitiesServiceProxy.delete(nationality.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  selectEditTable(table) {
    this.childEntitySelection = {};
    this.childEntitySelection[table] = true;
  }

  openChildRowForNationalityTranslation(index, table) {
    let isAlreadyOpened = this.nationalityTranslationRowSelection[index];
    this.closeAllChildRows();
    this.nationalityTranslationRowSelection = [];
    if (!isAlreadyOpened) {
      this.nationalityTranslationRowSelection[index] = true;
    }
    this.selectEditTable(table);
  }

  closeAllChildRows(): void {
    this.nationalityTranslationRowSelection = [];
  }
}
