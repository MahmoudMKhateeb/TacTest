import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  ActorsServiceProxy,
  CreateOrEditActorDto,
  CreateOrEditDocumentFileDto,
  DocumentFilesServiceProxy,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { IAjaxResponse, TokenService } from 'abp-ng2-module';
import { AppConsts } from '@shared/AppConsts';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { RequiredDocumentFormChildComponent } from '@app/shared/common/required-document-form-child/required-document-form-child.component';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'createOrEditActorModal',
  templateUrl: './create-or-edit-actor-modal.component.html',
  providers: [DateFormatterService],
})
export class CreateOrEditActorModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('requiredDocumentFormChildComponent', { static: false }) requiredDocumentFormChildComponent: RequiredDocumentFormChildComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  actor: CreateOrEditActorDto = new CreateOrEditActorDto();
  alldocumentsValid = false;
  public DocsUploader: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  fileToken: string;
  fileType: string;
  fileName: string;
  hasNewUpload: boolean;
  HaveShipperClients: boolean;
  HaveCarrierClients: boolean;

  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  constructor(
    injector: Injector,
    private _actorsServiceProxy: ActorsServiceProxy,
    public _fileDownloadService: FileDownloadService,
    private _tokenService: TokenService,
    private cdref: ChangeDetectorRef,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy
  ) {
    super(injector);
  }

  show(actorId?: number): void {
    this.HaveShipperClients = this.feature.isEnabled('App.ShipperClients');
    this.HaveCarrierClients = this.feature.isEnabled('App.CarrierClients');

    if (!actorId) {
      this.actor = new CreateOrEditActorDto();
      this.actor.createOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
      this.actor.id = actorId;
      this.actor.createOrEditDocumentFileDto.extn = '_';
      this.actor.createOrEditDocumentFileDto.name = '_';

      this.active = true;
      this.modal.show();
    } else {
      this._actorsServiceProxy.getActorForEdit(actorId).subscribe((result) => {
        this.actor = result.actor;
        this.active = true;
        this.BindRequiredDocs();
        this.modal.show();
      });
    }
    this.initDocsUploader();
    this.cdref.detectChanges();
  }

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
        this.actor.createOrEditDocumentFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
        this.hasNewUpload = true;
        this.fileToken = resp.result.fileToken;
        this.fileType = resp.result.fileType;
        this.fileName = resp.result.fileName;
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    //for progressBar
    this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.docProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploader.setOptions(this._DocsUploaderOptions);
  }

  createOrEd(): void {
    this.saving = true;
    if (!this.actor.createOrEditDocumentFileDto.updateDocumentFileInput) this.actor.createOrEditDocumentFileDto = null;

    this._actorsServiceProxy
      .createOrEdit(this.actor)
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
    this.actor = new CreateOrEditActorDto();
    this.fileToken = undefined;
    this.docProgress = undefined;
    this.active = false;
    this.modal.hide();
  }

  ngOnDestroy() {
    this.actor = undefined;
    this.docProgress = undefined;
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
    if (item.extn != 'image/jpeg' && item.extn != 'image/png') {
      item.name = '';
      this.alldocumentsValid = false;
      return;
    }
    this.alldocumentsValid = true;
    item.name = event.target.files[0].name;

    this.DocsUploader.addToQueue(event.target.files);
    this.DocsUploader.uploadAll();
  }

  downloadAttatchment(): void {
    this._fileDownloadService.downloadFileByBinary(
      this.actor.createOrEditDocumentFileDto.binaryObjectId,
      this.actor.createOrEditDocumentFileDto.name,
      this.actor.createOrEditDocumentFileDto.extn
    );
  }

  save(): void {
    this.saving = true;

    if (this.actor.id) {
      this.createOrEd();
    }

    if (
      this.actor.createOrEditDocumentFileDtos != undefined &&
      this.actor.createOrEditDocumentFileDtos.length > 0 &&
      this.requiredDocumentFormChildComponent.DocsUploader.queue?.length > 0
    ) {
      this.requiredDocumentFormChildComponent.DocsUploader.uploadAll();
    } else {
      this.createOrEd();
    }
  }

  isInUpdateStage(): boolean {
    // in case of the Id have value then we are in update stage
    return isNotNullOrUndefined(this.actor?.id);
  }

  BindRequiredDocs() {
    //RequiredDocuments
    if (this.actor.actorType == 1) {
      this._documentFilesServiceProxy.getActorShipperRequiredDocumentFiles('').subscribe((result) => {
        this.actor.createOrEditDocumentFileDtos = result;
      });
    } else if (this.actor.actorType == 2) {
      this._documentFilesServiceProxy.getActorCarrierRequiredDocumentFiles('').subscribe((result) => {
        this.actor.createOrEditDocumentFileDtos = result;
      });
    }
  }
}
