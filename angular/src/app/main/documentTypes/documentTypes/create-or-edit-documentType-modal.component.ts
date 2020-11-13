import { Component, ViewChild, ViewEncapsulation, Injector, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';

import { finalize } from 'rxjs/operators';
import {
  DocumentTypesServiceProxy,
  CreateOrEditDocumentTypeDto,
  DocumentFilesServiceProxy,
  SelectItemDto,
  DocumentTypeTranslationsServiceProxy,
  DocumentTypeTranslationDto,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { CreateOrEditDocumentTypeTranslationModalComponent } from '@app/main/documentTypeTranslations/documentTypeTranslations/create-or-edit-documentTypeTranslation-modal.component';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { NotifyService } from '@node_modules/abp-ng2-module';
import { ActivatedRoute } from '@angular/router';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'createOrEditDocumentTypeModal',
  templateUrl: './create-or-edit-documentType-modal.component.html',
  encapsulation: ViewEncapsulation.None,

  animations: [appModuleAnimation()],
})
export class CreateOrEditDocumentTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('paginator', { static: false }) paginator: Paginator;
  @ViewChild('dataTable', { static: false }) dataTable: Table;
  createOrEditDocumentTypeTranslationModal: CreateOrEditDocumentTypeTranslationModalComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  languageFilter = '';
  documentTypeDisplayNameFilter = '';
  active = false;
  saving = false;

  documentType: CreateOrEditDocumentTypeDto = new CreateOrEditDocumentTypeDto();
  allDocumentsEntities: SelectItemDto[];

  editions: SelectItemDto[];
  tenantOptionSelected = false;

  constructor(
    injector: Injector,
    private _documentTypesServiceProxy: DocumentTypesServiceProxy,
    private _documentTypeTranslationsServiceProxy: DocumentTypeTranslationsServiceProxy,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,

    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(injector);
  }

  show(documentTypeId?: number): void {
    if (!documentTypeId) {
      this.documentType = new CreateOrEditDocumentTypeDto();
      this.documentType.id = documentTypeId;
      //this.documentType.expirationDate = moment().startOf('day');
      this._documentTypesServiceProxy.getAllDocumentsEntitiesForTableDropdown().subscribe((result) => {
        this.allDocumentsEntities = result;
      });
      this.active = true;
      this.modal.show();
    } else if (documentTypeId) {
      this._documentTypesServiceProxy.getDocumentTypeForEdit(documentTypeId).subscribe((result) => {
        this.documentType = result.documentType;
        this._documentTypesServiceProxy.getAllDocumentsEntitiesForTableDropdown().subscribe((result) => {
          this.allDocumentsEntities = result;
          this.active = true;
          this.modal.show();
        });
      });
    }

    this._documentFilesServiceProxy.getAllEditionsForDropdown().subscribe((result) => {
      this.editions = result;
    });
  }

  save(): void {
    this.saving = true;

    this._documentTypesServiceProxy
      .createOrEdit(this.documentType)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  createDocumentTypeTranslation(): void {
    // this.createOrEditDocumentTypeTranslationModal.
    this.createOrEditDocumentTypeTranslationModal.show();
  }

  close(): void {
    this.active = false;
    this.documentType = new CreateOrEditDocumentTypeDto();
    this.primengTableHelper.records = null;
    this.modal.hide();
  }

  documentsEntityDropDownOnChange($event) {
    if ($event.target.options[$event.target.options.selectedIndex].text === 'Tenant') {
      this.tenantOptionSelected = true;
    } else {
      this.tenantOptionSelected = false;
      this.documentType.editionId = undefined;
    }
  }

  getDocumentTypeTranslations(event?: LazyLoadEvent) {
    if (this.documentType.id) {
      this.changeDetectorRef.detectChanges();
      if (this.primengTableHelper.shouldResetPaging(event)) {
        this.paginator.changePage(0);
        return;
      }

      this.primengTableHelper.showLoadingIndicator();

      this._documentTypeTranslationsServiceProxy
        .getAll(
          this.filterText,
          this.nameFilter,
          this.languageFilter,
          this.documentType.displayName,
          this.primengTableHelper.getSorting(this.dataTable),
          this.primengTableHelper.getSkipCount(this.paginator, event),
          this.primengTableHelper.getMaxResultCount(this.paginator, event)
        )
        .subscribe((result) => {
          this.primengTableHelper.totalRecordsCount = result.totalCount;
          this.primengTableHelper.records = result.items;
          this.primengTableHelper.hideLoadingIndicator();
        });
    } else {
    }
  }
  reloadPage(): void {
    this.changeDetectorRef.detectChanges();
    this.paginator.changePage(this.paginator.getPage());
  }

  deleteDocumentTypeTranslation(documentTypeTranslation: DocumentTypeTranslationDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._documentTypeTranslationsServiceProxy.delete(documentTypeTranslation.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  hasNumberCheckBoxChange() {
    if (!this.documentType.hasNumber) {
      this.documentType.numberMaxDigits = undefined;
      this.documentType.numberMinDigits = undefined;
    }
  }

  HasExpDateCheckBoxChange() {
    if (!this.documentType.hasExpirationDate) {
      this.documentType.expirationAlertDays = undefined;
      this.documentType.inActiveToleranceDays = undefined;
    }
  }
}
