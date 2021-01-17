import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditDocumentFileDto,
  DocumentFileRoutStepLookupTableDto,
  DocumentFilesServiceProxy,
  DocumentFileTrailerLookupTableDto,
  DocumentFileTruckLookupTableDto,
  DocumentFileUserLookupTableDto,
  DocumentTypeDto,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { finalize } from '@node_modules/rxjs/internal/operators';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import * as _ from 'lodash';
@Component({
  selector: 'createOrEditDocumentFileModal',
  templateUrl: './create-or-edit-documentFile-modal.component.html',
  providers: [DateFormatterService],
})
export class CreateOrEditDocumentFileModalComponent extends AppComponentBase {
  //region fields
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;

  documentFile: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  // fileFormateIsInvalideIndex: boolean;
  alldocumentsValid = false;
  documentTypeDisplayName = '';
  documentEntity = '';
  selectedDateType = DateType.Gregorian; // or DateType.Gregorian
  CreateOrEditDocumentFileDtoList: CreateOrEditDocumentFileDto[] = [];
  isNumberValid = false;
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  // allTrucks: DocumentFileTruckLookupTableDto[];
  // allTrailers: DocumentFileTrailerLookupTableDto[];
  // allUsers: DocumentFileUserLookupTableDto[];
  // allRoutSteps: DocumentFileRoutStepLookupTableDto[];
  public DocsUploader: FileUploader;
  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  fileToken: string;
  // numberpattern = '^[^`|~|!|@|#|$|%|^|&|*|(|)|+|=|[|{|]|}|||\\|\'|<|,|>|?|/|"|;|:|]{1,}';
  private _uploaderOptions: FileUploaderOptions = {};
  /**
   * required documents fileUploader options
   * @private
   */
  private _DocsUploaderOptions: FileUploaderOptions = {};
  private entityId: string;

  public selectedDate: NgbDateStruct;
  //endregion
  isDateValid = false;
  constructor(injector: Injector, private _documentFilesServiceProxy: DocumentFilesServiceProxy, private _tokenService: TokenService) {
    super(injector);
  }

  /**
   *
   * @param entityId - is for the Id of entity truckId or driverId or TenantId
   * @param documentEntity - if for the required document from entity type Driver or Truck or Tenant
   * @param documentFileId - is for the documentFileId for edit
   */
  show(entityId: string, documentEntity: string, documentFileId: string): void {
    this.documentFile = new CreateOrEditDocumentFileDto();
    this.documentFile.documentTypeDto = new DocumentTypeDto();
    this.fileToken = '';
    this.entityId = entityId;
    this.documentEntity = documentEntity;
    this.selectedDate = this.todayGregorian;
    //create
    if (!documentFileId) {
      this.getRequiredDocumentFiles(documentEntity);
    }
    //edit
    // tslint:disable-next-line:one-line
    else {
      this._documentFilesServiceProxy.getDocumentFileForEdit(documentFileId).subscribe((result) => {
        this.documentFile = result.documentFile;
        this.selectedDate = this.dateFormatterService.MomentToNgbDateStruct(this.documentFile.expirationDate);
        // console.log(this.selectedDate);
        this.selectedDateType = this.documentFile.documentTypeDto.hasHijriExpirationDate ? DateType.Hijri : DateType.Gregorian;
        if (this.selectedDateType == DateType.Hijri) {
          this.selectedDate = this.dateFormatterService.ToHijri(this.selectedDate);
        }
        this.isNumberValid = true;
        this.isDateValid = true;
        this.alldocumentsValid = true;
      });
    }
    this.active = true;
    this.modal.show();
    this.initDocsUploader();
  }

  /**
   * initialize required documents fileUploader
   */
  initDocsUploader(): void {
    this.DocsUploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
    this._DocsUploaderOptions.autoUpload = false;
    this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._DocsUploaderOptions.removeAfterUpload = true;

    this.DocsUploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.DocsUploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', fileItem.file.name);
      form.append('FileToken', this.guid());
    };

    this.DocsUploader.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      if (resp.success) {
        //attach each fileToken to his CreateOrEditDocumentFileDto
        this.documentFile.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
        this.fileToken = resp.result.fileToken;
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploader.onCompleteAll = () => {
      this.documentFile.updateDocumentFileInput = new UpdateDocumentFileInput();
      this.documentFile.updateDocumentFileInput.fileToken = this.fileToken;
      if (this.documentFile.id) {
        this._documentFilesServiceProxy
          .createOrEdit(this.documentFile)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          )
          .subscribe(() => {
            this.saving = false;
            this.notify.info(this.l('UpdatedSuccessfully'));
            this.close();
            this.modalSave.emit(null);
          });
      } else if (!this.documentFile.id) {
        this.createDocumentFile();
      }
    };

    //for progressBar
    this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.docProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploader.setOptions(this._DocsUploaderOptions);
  }

  save(): void {
    this.saving = true;
    if (this.DocsUploader.queue.length > 0) {
      // console.log('documentFile', this.documentFile);
      this.DocsUploader.uploadAll();
    } else if (this.documentFile.id) {
      this._documentFilesServiceProxy
        .createOrEdit(this.documentFile)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.saving = false;
          this.notify.info(this.l('UpdatedSuccessfully'));
          this.close();
          this.modalSave.emit(null);
        });
    } else if (!this.documentFile.id) {
      this.createDocumentFile();
    }
  }

  createDocumentFile() {
    this._documentFilesServiceProxy
      .createOrEdit(this.documentFile)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.saving = false;
        this.notify.info(this.l('CreatedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.DocsUploader.clearQueue();
    this.fileToken = '';
    this.docProgressFileName = null;
    this.docProgress = null;
    this.modalSave.emit(null);
    this.modal.hide();
  }

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      item.name = '';
      this.alldocumentsValid = false;
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      item.name = '';
      this.alldocumentsValid = false;
      return;
    }
    this.alldocumentsValid = true;
    item.name = event.target.files[0].name;
    this.DocsUploader.addToQueue(event.target.files);
  }

  chooseDocumentToBeCreated(id: string) {
    this.documentFile = this.CreateOrEditDocumentFileDtoList.find((x) => x.documentTypeDto.id == this.Number(id));
  }

  getRequiredDocumentFiles(entityType) {
    if (entityType === 'Truck') {
      this._documentFilesServiceProxy.getTruckRequiredDocumentFiles(this.entityId).subscribe((result) => {
        this.CreateOrEditDocumentFileDtoList = result;
        this.documentFile = this.CreateOrEditDocumentFileDtoList[0];
        this.initializeDocumentFile();
      });
    } else if (entityType === 'Driver') {
      this._documentFilesServiceProxy.getDriverRequiredDocumentFiles(this.entityId).subscribe((result) => {
        this.CreateOrEditDocumentFileDtoList = result;
        this.documentFile = this.CreateOrEditDocumentFileDtoList[0];
        this.initializeDocumentFile();
      });
    }
  }

  initializeDocumentFile() {
    if (this.documentEntity === 'Truck') {
      this.documentFile.truckId = this.Number(this.entityId);
    } else if (this.documentEntity === 'Driver') {
      this.documentFile.userId = this.Number(this.entityId);
    }
    this.documentFile.expirationDate = moment().startOf('day');
    this.documentTypeDisplayName = '';
  }

  checkIfDateIsValid() {
    if (this.selectedDate != null) {
      this.isDateValid = true;
    }
  }

  numberChange(item: CreateOrEditDocumentFileDto) {
    if (item.documentTypeDto.numberMinDigits <= item.number.length && item.number.length <= item.documentTypeDto.numberMaxDigits) {
      this.isNumberValid = true;
    } else {
      this.isNumberValid = false;
    }
  }
}
