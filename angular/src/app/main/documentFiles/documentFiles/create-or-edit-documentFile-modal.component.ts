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
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { FileItem, FileSelectDirective, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { base64ToFile } from '@node_modules/ngx-image-cropper';
import { finalize } from '@node_modules/rxjs/internal/operators';

@Component({
  selector: 'createOrEditDocumentFileModal',
  templateUrl: './create-or-edit-documentFile-modal.component.html',
})
export class CreateOrEditDocumentFileModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  documentFile: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();

  documentTypeDisplayName = '';
  truckPlateNumber = '';
  trailerTrailerCode = '';
  userName = '';
  routStepDisplayName = '';

  allDocumentTypes: DocumentFileDocumentTypeLookupTableDto[];
  allTrucks: DocumentFileTruckLookupTableDto[];
  allTrailers: DocumentFileTrailerLookupTableDto[];
  allUsers: DocumentFileUserLookupTableDto[];
  allRoutSteps: DocumentFileRoutStepLookupTableDto[];

  public uploader: FileUploader;
  private _uploaderOptions: FileUploaderOptions = {};

  fileToken: string;

  constructor(injector: Injector, private _documentFilesServiceProxy: DocumentFilesServiceProxy, private _tokenService: TokenService) {
    super(injector);
  }

  show(documentFileId?: string): void {
    if (!documentFileId) {
      this.documentFile = new CreateOrEditDocumentFileDto();
      this.documentFile.id = documentFileId;
      this.documentFile.expirationDate = moment().startOf('day');
      this.documentTypeDisplayName = '';
      this.truckPlateNumber = '';
      this.trailerTrailerCode = '';
      this.userName = '';
      this.routStepDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._documentFilesServiceProxy.getDocumentFileForEdit(documentFileId).subscribe((result) => {
        this.documentFile = result.documentFile;
        this.documentTypeDisplayName = result.documentTypeDisplayName;
        this.truckPlateNumber = result.truckPlateNumber;
        this.trailerTrailerCode = result.trailerTrailerCode;
        this.userName = result.userName;
        this.routStepDisplayName = result.routStepDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._documentFilesServiceProxy.getAllDocumentTypeForTableDropdown().subscribe((result) => {
      this.allDocumentTypes = result;
    });
    this._documentFilesServiceProxy.getAllTruckForTableDropdown().subscribe((result) => {
      this.allTrucks = result;
    });
    this._documentFilesServiceProxy.getAllTrailerForTableDropdown().subscribe((result) => {
      this.allTrailers = result;
    });
    this._documentFilesServiceProxy.getAllUserForTableDropdown().subscribe((result) => {
      this.allUsers = result;
    });
    this._documentFilesServiceProxy.getAllRoutStepForTableDropdown().subscribe((result) => {
      this.allRoutSteps = result;
    });

    this.initFileUploader();
  }

  save(): void {
    this.saving = true;
    if (this.uploader.queue.length > 0) {
      this.uploader.uploadAll();

      this.documentFile.updateDocumentFileInput = new UpdateDocumentFileInput();
      this.documentFile.updateDocumentFileInput.fileToken = this.fileToken;
    }

    this._documentFilesServiceProxy
      .createOrEdit(this.documentFile)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.saving = false;
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.uploader.clearQueue();
    this.modal.hide();
  }

  initFileUploader(): void {
    this.uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
    this._uploaderOptions.autoUpload = false;
    this._uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._uploaderOptions.removeAfterUpload = true;
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', fileItem.file.name);
      form.append('FileToken', this.guid());
    };
    console.log('initFileUploader');

    this.uploader.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      if (resp.success) {
        this.fileToken = resp.result.fileToken;
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.uploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      console.log(resp);
    };

    this.uploader.setOptions(this._uploaderOptions);
  }

  fileChangeEvent(event: any): void {
    console.log(event.target.files[0]);
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      return;
    }
    this.uploader.clearQueue();
    // this.uploader.addToQueue([<File>base64ToFile(event.base64)]);
    this.uploader.addToQueue(event.target.files);
    this.documentFile.extn = event.target.files[0].type;
    this.documentFile.name = event.target.files[0].name;
  }

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }
}
