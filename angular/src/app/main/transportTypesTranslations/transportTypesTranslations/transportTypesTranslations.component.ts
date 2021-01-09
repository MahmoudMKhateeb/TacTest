import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TokenAuthServiceProxy, TransportTypesTranslationDto, TransportTypesTranslationsServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditTransportTypesTranslationModalComponent } from './create-or-edit-transportTypesTranslation-modal.component';

import { ViewTransportTypesTranslationModalComponent } from './view-transportTypesTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  templateUrl: './transportTypesTranslations.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TransportTypesTranslationsComponent extends AppComponentBase {
  @ViewChild('createOrEditTransportTypesTranslationModal', { static: true })
  createOrEditTransportTypesTranslationModal: CreateOrEditTransportTypesTranslationModalComponent;
  @ViewChild('viewTransportTypesTranslationModalComponent', { static: true })
  viewTransportTypesTranslationModal: ViewTransportTypesTranslationModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  languageFilter = '';
  transportTypeDisplayNameFilter = '';

  constructor(
    injector: Injector,
    private _transportTypesTranslationsServiceProxy: TransportTypesTranslationsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getTransportTypesTranslations(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._transportTypesTranslationsServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
        this.languageFilter,
        this.transportTypeDisplayNameFilter,
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

  createTransportTypesTranslation(): void {
    this.createOrEditTransportTypesTranslationModal.show();
  }

  deleteTransportTypesTranslation(transportTypesTranslation: TransportTypesTranslationDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._transportTypesTranslationsServiceProxy.delete(transportTypesTranslation.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
