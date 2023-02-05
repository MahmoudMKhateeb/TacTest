import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  AdditionalStepType,
  CreateOrEditDocumentFileDto,
  FileDto,
  InvokeStatusInputDto,
  InvokeStepInputDto,
  TrackingRoutePointDto,
  TrackingServiceProxy,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-upload-additional-documents',
  templateUrl: './upload-additional-documents.component.html',
  styleUrls: ['./upload-additional-documents.component.scss'],
})
export class UploadAdditionalDocumentsComponent extends AppComponentBase implements OnInit {
  @ViewChild('uploadAdditionalDocumentsModal', { static: true }) modal: ModalDirective;
  saving: boolean;
  active: boolean;
  receiverCode: string;
  showReceiverCode: boolean;
  showUploadManifestFile: boolean;
  showUploadPodFile: boolean;
  manifestFileToken: string;
  createOrEditDocumentManifestFileDto: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  manifestFileDocProgress: any;
  podFileToken: string;
  createOrEditDocumentPodFileDto: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  podFileDocProgress: any;
  public DocsUploaderManifest: FileUploader;
  public DocsUploaderPod: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  hasNewUploadManifest: boolean;
  hasNewUploadPod: boolean;
  fileTypeManifest: any;
  fileTypePod: any;
  fileNameManifest: any;
  fileNamePod: any;
  docProgressFileName: string;
  docProgressFileNameManifest: string;
  alldocumentsValidManifest: boolean;
  alldocumentsValidPod: boolean;
  private point: TrackingRoutePointDto;

  get isFileInputManifestValid() {
    return this.createOrEditDocumentManifestFileDto.name ? true : false;
  }
  get isFileInputPodValid() {
    return this.createOrEditDocumentPodFileDto.name ? true : false;
  }

  constructor(
    injector: Injector,
    private _tokenService: TokenService,
    private _fileDownloadService: FileDownloadService,
    private _trackingServiceProxy: TrackingServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  /**
   * initialize required documents fileUploader
   */
  initDocsUploaderManifest(): void {
    this.DocsUploaderManifest = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
    this._DocsUploaderOptions.autoUpload = false;
    this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._DocsUploaderOptions.removeAfterUpload = true;

    this.DocsUploaderManifest.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.DocsUploaderManifest.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', fileItem.file.name);
      form.append('FileToken', this.guid());
    };

    this.DocsUploaderManifest.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      if (resp.success) {
        //attach each fileToken to his CreateOrEditDocumentFileDto
        this.createOrEditDocumentManifestFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
        this.hasNewUploadManifest = true;
        this.manifestFileToken = resp.result.fileToken;
        this.fileTypeManifest = resp.result.fileType;
        this.fileNameManifest = resp.result.fileName;
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploaderManifest.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploaderManifest.onCompleteAll = () => {
      // this.documentFile.updateDocumentFileInput = new UpdateDocumentFileInput();
      // this.documentFile.updateDocumentFileInput.fileToken = this.fileToken;
      // if (this.documentFile.id) {
      //   this._documentFilesServiceProxy
      //     .createOrEdit(this.documentFile)
      //     .pipe(
      //       finalize(() => {
      //         this.saving = false;
      //       })
      //     )
      //     .subscribe(() => {
      //       this.saving = false;
      //       this.notify.info(this.l('UpdatedSuccessfully'));
      //       this.close();
      //       this.modalSave.emit(null);
      //     });
      // } else if (!this.documentFile.id) {
      //   this.createDocumentFile();
      // }
    };

    //for progressBar
    this.DocsUploaderManifest.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.manifestFileDocProgress = progress;
      this.docProgressFileNameManifest = fileItem.file.name;
    };

    this.DocsUploaderManifest.setOptions(this._DocsUploaderOptions);
  }

  initDocsUploaderPod(): void {
    this.DocsUploaderPod = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
    this._DocsUploaderOptions.autoUpload = false;
    this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._DocsUploaderOptions.removeAfterUpload = true;

    this.DocsUploaderPod.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.DocsUploaderPod.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', fileItem.file.name);
      form.append('FileToken', this.guid());
    };

    this.DocsUploaderPod.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      if (resp.success) {
        //attach each fileToken to his CreateOrEditDocumentFileDto
        this.createOrEditDocumentPodFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
        this.hasNewUploadPod = true;
        this.podFileToken = resp.result.fileToken;
        this.fileTypePod = resp.result.fileType;
        this.fileNamePod = resp.result.fileName;
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploaderPod.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploaderPod.onCompleteAll = () => {
      // this.documentFile.updateDocumentFileInput = new UpdateDocumentFileInput();
      // this.documentFile.updateDocumentFileInput.fileToken = this.fileToken;
      // if (this.documentFile.id) {
      //   this._documentFilesServiceProxy
      //     .createOrEdit(this.documentFile)
      //     .pipe(
      //       finalize(() => {
      //         this.saving = false;
      //       })
      //     )
      //     .subscribe(() => {
      //       this.saving = false;
      //       this.notify.info(this.l('UpdatedSuccessfully'));
      //       this.close();
      //       this.modalSave.emit(null);
      //     });
      // } else if (!this.documentFile.id) {
      //   this.createDocumentFile();
      // }
    };

    //for progressBar
    this.DocsUploaderPod.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.podFileDocProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploaderPod.setOptions(this._DocsUploaderOptions);
  }
  downloadAttatchment(): void {
    if (!this.hasNewUploadManifest) {
      this._fileDownloadService.downloadFileByBinary(
        this.createOrEditDocumentManifestFileDto.binaryObjectId,
        this.createOrEditDocumentManifestFileDto.name,
        this.createOrEditDocumentManifestFileDto.extn
      );
    } else {
      let fileDto = new FileDto();
      fileDto.fileName = this.fileNameManifest;
      fileDto.fileToken = this.manifestFileToken;
      fileDto.fileType = this.fileTypeManifest;
      this._fileDownloadService.downloadTempFile(fileDto);
    }
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      item.name = '';
      this.alldocumentsValidManifest = false;
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      item.name = '';
      this.alldocumentsValidManifest = false;
      return;
    }
    this.alldocumentsValidManifest = true;
    item.name = event.target.files[0].name;

    this.DocsUploaderManifest.addToQueue(event.target.files);
    this.DocsUploaderManifest.uploadAll();
  }

  show(point: TrackingRoutePointDto) {
    this.initDocsUploaderManifest();
    this.initDocsUploaderPod();
    this.active = true;
    this.point = point;
    this.showReceiverCode = point.availableSteps.filter((item) => item.stepType === AdditionalStepType.ReceiverCode).length > 0;
    this.showUploadManifestFile =
      point.availableSteps.filter(
        (item) => item.stepType === AdditionalStepType.Manifest || item.stepType === AdditionalStepType.ConfirmationDocument
      ).length > 0;
    this.showUploadPodFile = point.availableSteps.filter((item) => item.stepType === AdditionalStepType.Pod).length > 0;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.createOrEditDocumentPodFileDto = new CreateOrEditDocumentFileDto();
    this.createOrEditDocumentManifestFileDto = new CreateOrEditDocumentFileDto();
    this.receiverCode = null;
    this.showReceiverCode = null;
    this.showUploadManifestFile = null;
    this.showUploadPodFile = null;
    this.manifestFileToken = null;
    this.podFileToken = null;
    this.fileTypeManifest = null;
    this.fileTypePod = null;
    this.fileNameManifest = null;
    this.fileNamePod = null;
    this.manifestFileDocProgress = null;
    this.podFileDocProgress = null;
    this.docProgressFileNameManifest = null;
    this.docProgressFileName = null;
    this.modal.hide();
  }

  confirmReceiverCode() {
    const invokeRequestBody = new InvokeStepInputDto();
    invokeRequestBody.code = this.receiverCode;
    invokeRequestBody.id = this.point.id;
    invokeRequestBody.action = this.point.availableSteps.find((item) => item.stepType === AdditionalStepType.ReceiverCode)?.action;
    this._trackingServiceProxy
      .invokeAdditionalStep(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(
        (res) => {
          this.close();
          abp.event.trigger('trackingConfirmCodeSubmittedFromAdditionalSteps');
        },
        (error) => {
          this.notify.error(this.l('InvalidCode'));
        }
      );
  }

  downloadAttatchmentPod(): void {
    if (!this.hasNewUploadPod) {
      this._fileDownloadService.downloadFileByBinary(
        this.createOrEditDocumentPodFileDto.binaryObjectId,
        this.createOrEditDocumentPodFileDto.name,
        this.createOrEditDocumentPodFileDto.extn
      );
    } else {
      let fileDto = new FileDto();
      fileDto.fileName = this.fileNamePod;
      fileDto.fileToken = this.podFileToken;
      fileDto.fileType = this.fileTypePod;
      this._fileDownloadService.downloadTempFile(fileDto);
    }
  }

  PodDocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      item.name = '';
      this.alldocumentsValidPod = false;
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      item.name = '';
      this.alldocumentsValidPod = false;
      return;
    }
    this.alldocumentsValidPod = true;
    item.name = event.target.files[0].name;

    this.DocsUploaderPod.addToQueue(event.target.files);
    this.DocsUploaderPod.uploadAll();
  }

  uploadManifest() {
    if (!this.isFileInputManifestValid) {
      return;
    }
    const invokeRequestBody = new InvokeStepInputDto();
    invokeRequestBody.code = this.receiverCode;
    invokeRequestBody.id = this.point.id;
    invokeRequestBody.action = this.point.availableSteps.find((item) => item.stepType === AdditionalStepType.Manifest)?.action;

    invokeRequestBody.documentId = this.manifestFileToken;
    invokeRequestBody.documentName = this.fileNameManifest;
    invokeRequestBody.documentContentType = this.fileTypeManifest;

    this._trackingServiceProxy
      .invokeAdditionalStep(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(
        (res) => {
          this.close();
          abp.event.trigger('FileUploadedSuccessFromAdditionalSteps');
        },
        (error) => {
          this.notify.error(this.l('ErrorWhenUploadingFile'));
        }
      );
  }

  uploadPod() {
    if (!this.isFileInputPodValid) {
      return;
    }
    const invokeRequestBody = new InvokeStepInputDto();
    invokeRequestBody.code = this.receiverCode;
    invokeRequestBody.id = this.point.id;
    invokeRequestBody.action = this.point.availableSteps.find((item) => item.stepType === AdditionalStepType.Pod)?.action;

    invokeRequestBody.documentId = this.podFileToken;
    invokeRequestBody.documentName = this.fileNamePod;
    invokeRequestBody.documentContentType = this.fileTypePod;

    this._trackingServiceProxy
      .invokeAdditionalStep(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(
        (res) => {
          this.close();
          abp.event.trigger('FileUploadedSuccessFromAdditionalSteps');
        },
        (error) => {
          this.notify.error(this.l('ErrorWhenUploadingFile'));
        }
      );
  }
}
