import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  DocumentFilesServiceProxy,
  CreateOrEditDocumentFileDto,
  DocumentFileDocumentTypeLookupTableDto,
  DocumentFileTruckLookupTableDto,
  DocumentFileTrailerLookupTableDto,
  DocumentFileUserLookupTableDto,
  DocumentFileRoutStepLookupTableDto,
  UpdateDocumentFileInput,
  DocumentFileForEditDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { FileItem, FileSelectDirective, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { base64ToFile } from '@node_modules/ngx-image-cropper';
import { finalize } from '@node_modules/rxjs/internal/operators';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/admin/required-document-files/hijri-gregorian-datepicker/date-formatter.service';
import { DateType } from '@app/admin/required-document-files/hijri-gregorian-datepicker/consts';

@Component({
  selector: 'createOrEditDocumentFileModal',
  templateUrl: './create-or-edit-documentFile-modal.component.html',
})
export class CreateOrEditDocumentFileModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  documentFile: DocumentFileForEditDto = new DocumentFileForEditDto();

  documentTypeDisplayName = '';
  // truckPlateNumber = '';
  // trailerTrailerCode = '';
  // userName = '';
  // routStepDisplayName = '';
  selectedDateTypeHijri = DateType.Hijri; // or DateType.Gregorian
  allDocumentTypes: DocumentFileDocumentTypeLookupTableDto[];
  allTrucks: DocumentFileTruckLookupTableDto[];
  allTrailers: DocumentFileTrailerLookupTableDto[];
  allUsers: DocumentFileUserLookupTableDto[];
  allRoutSteps: DocumentFileRoutStepLookupTableDto[];
  public DocsUploader: FileUploader;
  numberpattern = '^[^`|~|!|@|#|$|%|^|&|*|(|)|+|=|[|{|]|}|||\\|\'|<|,|>|?|/|"|;|:|]{1,}';
  private _uploaderOptions: FileUploaderOptions = {};

  /**
   * required documents fileUploader options
   * @private
   */
  private _DocsUploaderOptions: FileUploaderOptions = {};

  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  fileToken: string;

  constructor(
    injector: Injector,
    private dateFormatterService: DateFormatterService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _tokenService: TokenService
  ) {
    super(injector);
  }

  show(documentFileId?: string): void {
    // if (!documentFileId) {
    //   this.documentFile = new CreateOrEditDocumentFileDto();
    //   this.documentFile.id = documentFileId;
    //   this.documentFile.expirationDate = moment().startOf('day');
    //   this.documentTypeDisplayName = '';
    //   this.truckPlateNumber = '';
    //   this.trailerTrailerCode = '';
    //   this.userName = '';
    //   this.routStepDisplayName = '';

    //   this.active = true;
    //   this.modal.show();
    // } else {
    this._documentFilesServiceProxy.getDocumentFileForEdit(documentFileId).subscribe((result) => {
      this.documentFile = result;
      this.active = true;
      this.modal.show();
    });
    // }
    // this._documentFilesServiceProxy.getAllDocumentTypeForTableDropdown().subscribe((result) => {
    //   this.allDocumentTypes = result;
    // });
    // this._documentFilesServiceProxy.getAllTruckForTableDropdown().subscribe((result) => {
    //   this.allTrucks = result;
    // });
    // this._documentFilesServiceProxy.getAllTrailerForTableDropdown().subscribe((result) => {
    //   this.allTrailers = result;
    // });
    // this._documentFilesServiceProxy.getAllUserForTableDropdown().subscribe((result) => {
    //   this.allUsers = result;
    // });
    // this._documentFilesServiceProxy.getAllRoutStepForTableDropdown().subscribe((result) => {
    //   this.allRoutSteps = result;
    // });

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
      this._documentFilesServiceProxy
        .updateDocument(this.documentFile)
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
      this.DocsUploader.uploadAll();
    } else {
      this._documentFilesServiceProxy
        .updateDocument(this.documentFile)
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
    }
  }

  close(): void {
    this.active = false;
    this.DocsUploader.clearQueue();
    this.docProgressFileName = null;
    this.docProgress = null;
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

  selectedDateChange($event: NgbDateStruct, item: DocumentFileForEditDto) {
    if ($event != null && $event.year < 2000) {
      this.dateFormatterService.SetFormat('DD/MM/YYYY', 'iDD/iMM/iYYYY');
      const incomingDate = this.dateFormatterService.ToGregorian($event);
      item.expirationDate = moment(incomingDate.month + '/' + incomingDate.day + '/' + incomingDate.year, 'MM/DD/YYYY');
    } else if ($event != null && $event.year > 2000) {
      item.expirationDate = moment($event.month + '/' + $event.day + '/' + $event.year, 'MM/DD/YYYY');
    }
  }

  DocFileChangeEvent(event: any, item: DocumentFileForEditDto): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      return;
    }
    this.DocsUploader.addToQueue(event.target.files);

    item.extn = event.target.files[0].type;
    item.name = event.target.files[0].name;
  }

  // isAcceptedChange() {
  //   this.documentFile.isRejected = !this.documentFile.isAccepted;
  // }

  // isRejectedChange() {
  //   this.documentFile.isAccepted = !this.documentFile.isRejected;
  // }
}
