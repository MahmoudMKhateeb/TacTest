import { ChangeDetectorRef, Component, EventEmitter, Injector, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';

import { finalize } from 'rxjs/operators';
import {
  CreateOrEditDocumentTypeDto,
  DocumentFilesServiceProxy,
  DocumentsEntitiesEnum,
  DocumentTypeFlagEnum,
  DocumentTypesServiceProxy,
  DocumentTypeTranslationsServiceProxy,
  ISelectItemDto,
  SelectItemDto,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDocumentTypeTranslationModalComponent } from '@app/main/documentTypeTranslations/documentTypeTranslations/create-or-edit-documentTypeTranslation-modal.component';

import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { IAjaxResponse, NotifyService, TokenService } from '@node_modules/abp-ng2-module';
import { ActivatedRoute } from '@angular/router';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { AppConsts } from '@shared/AppConsts';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

const toBase64 = (file) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });

@Component({
  selector: 'createOrEditDocumentTypeModal',
  templateUrl: './create-or-edit-documentType-modal.component.html',
  encapsulation: ViewEncapsulation.None,

  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
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
  documentNameisAvaliable = true;
  documentType: CreateOrEditDocumentTypeDto = new CreateOrEditDocumentTypeDto();
  allDocumentsEntities: [];
  buttonOptions: any = {
    text: 'Add',
    type: 'success',
    useSubmitBehavior: true,
  };

  editions: SelectItemDto[];
  tenantOptionSelected = false;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  value: File[] = [];
  authorizationToken: string;
  uploadUrl: string;
  DocumentTypeFlags: any;

  constructor(
    private injector: Injector,
    public _documentTypesServiceProxy: DocumentTypesServiceProxy,
    private _documentTypeTranslationsServiceProxy: DocumentTypeTranslationsServiceProxy,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService,
    private changeDetectorRef: ChangeDetectorRef,
    private _tokenService: TokenService,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.authorizationToken = 'Bearer ' + this._tokenService.getToken();
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile';
  }

  show(documentTypeId?: number): void {
    this.allDocumentsEntities = this.enumToArray.transform(DocumentsEntitiesEnum);
    this.DocumentTypeFlags = this.enumToArray.transform(DocumentTypeFlagEnum);

    if (!documentTypeId) {
      this.documentType = new CreateOrEditDocumentTypeDto();
      this.active = true;
    } else if (documentTypeId) {
      this._documentTypesServiceProxy.getDocumentTypeForEdit(documentTypeId).subscribe((result) => {
        this.documentType = result.documentType;
      });
    }

    this._documentFilesServiceProxy.getAllEditionsForDropdown().subscribe((result) => {
      this.editions = result;
    });
    this.active = true;
    this.modal.show();
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

  close(): void {
    this.active = false;
    this.documentType = new CreateOrEditDocumentTypeDto();
    this.primengTableHelper.records = null;
    this.value = [];
    this.modal.hide();
  }

  documentsEntityDropDownOnChange($event) {
    if ($event.target.options[$event.target.options.selectedIndex].text === 'Tenant') {
      this.tenantOptionSelected = true;
    } else {
      this.tenantOptionSelected = false;
      this.documentType.editionId = null;
    }
  }

  reloadPage(): void {
    this.changeDetectorRef.detectChanges();
    this.paginator.changePage(this.paginator.getPage());
  }

  onFormSubmit($event: Event) {}

  editionOnValueChanged($event: any) {
    this._documentTypesServiceProxy.getAutoCompleteTenants(this.documentType.editionId, ' ').subscribe((result) => {
      this.Tenants = result;
    });
  }

  onUploaded($event: any) {
    const resp = <IAjaxResponse>JSON.parse($event.request.response);
    this.documentType.templateContentType = $event.file.type;
    this.documentType.templateName = $event.file.name;
    this.documentType.fileToken = resp.result.fileToken;
    this.notify.success(this.l('file is successfully uploaded.'));
  }

  DeleteTemplate() {
    console.log(this.documentType.templateId);
    if (this.documentType.templateId) {
      this._documentTypesServiceProxy.deleteTemplate(this.documentType.id).subscribe((result) => {
        this.clearFileTemplateData();
        this.notify.success('success');
      });
    } else {
      this.clearFileTemplateData();
    }
  }

  private clearFileTemplateData() {
    this.documentType.templateContentType =
      this.documentType.templateName =
      this.documentType.templateExt =
      this.documentType.templateBase64 =
      this.documentType.templateId =
      this.documentType.fileToken =
        undefined;
    this.value = [];
  }

  downloadTemplate(id: number): void {
    this._documentTypesServiceProxy.getFileDto(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
