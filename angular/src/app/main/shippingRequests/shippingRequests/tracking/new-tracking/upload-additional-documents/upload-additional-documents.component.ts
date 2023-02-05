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
  // manifestFileToken: string;
  // createOrEditDocumentManifestFileDto: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  // manifestFileDocProgress: any;
  // podFileToken: string;
  // createOrEditDocumentPodFileDto: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  // podFileDocProgress: any;
  // public DocsUploaderManifest: FileUploader;
  // public DocsUploaderPod: FileUploader;
  // private _DocsUploaderOptions: FileUploaderOptions = {};
  // hasNewUploadManifest: boolean;
  // hasNewUploadPod: boolean;
  // fileTypeManifest: any;
  // fileTypePod: any;
  // fileNameManifest: any;
  // fileNamePod: any;
  // docProgressFileName: string;
  // docProgressFileNameManifest: string;
  // alldocumentsValidManifest: boolean;
  // alldocumentsValidPod: boolean;

  point: TrackingRoutePointDto;

  // get isFileInputPodValid() {
  //   return this.createOrEditDocumentPodFileDto.name ? true : false;
  // }

  fileTokens: string[] = [];
  createOrEditDocumentFileDtos: CreateOrEditDocumentFileDto[] = [];
  FileDocProgresses: any[] = [];
  public DocsUploaders: FileUploader[] = [];
  private _DocsUploadersOptions: FileUploaderOptions[] = [];
  hasNewUploads: boolean[] = [];
  fileTypes: any[] = [];
  fileNames: any[] = [];
  docProgressFileNames: string[] = [];
  alldocumentsValid: boolean[] = [];
  AdditionalStepType = AdditionalStepType;

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
  initDocsUploaders(): void {
    this.DocsUploaders = [];
    this.createOrEditDocumentFileDtos = [];
    this.hasNewUploads = [];
    this.fileTokens = [];
    this.fileTypes = [];
    this.fileNames = [];
    this.FileDocProgresses = [];
    this.docProgressFileNames = [];
    for (let i = 0; i < this.point.availableSteps.length; i++) {
      this.createOrEditDocumentFileDtos.push(new CreateOrEditDocumentFileDto());
      const uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
      const options: any = {};
      options.autoUpload = false;
      options.authToken = 'Bearer ' + this._tokenService.getToken();
      options.removeAfterUpload = true;
      this._DocsUploadersOptions.push(options);

      uploader.onAfterAddingFile = (file) => {
        file.withCredentials = false;
      };

      uploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
        form.append('FileType', fileItem.file.type);
        form.append('FileName', fileItem.file.name);
        form.append('FileToken', this.guid());
      };

      uploader.onSuccessItem = (item, response, status) => {
        const resp = <IAjaxResponse>JSON.parse(response);
        if (resp.success) {
          //attach each fileToken to his CreateOrEditDocumentFileDto
          this.createOrEditDocumentFileDtos[i].updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
          this.hasNewUploads[i] = true;
          this.fileTokens[i] = resp.result.fileToken;
          this.fileTypes[i] = resp.result.fileType;
          this.fileNames[i] = resp.result.fileName;
        } else {
          this.message.error(resp.error.message);
        }
      };

      uploader.onErrorItem = (item, response, status) => {
        const resp = <IAjaxResponse>JSON.parse(response);
      };

      uploader.onCompleteAll = () => {
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
      uploader.onProgressItem = (fileItem: FileItem, progress: any) => {
        this.FileDocProgresses[i] = progress;
        console.log('this.manifestFileDocProgress', this.FileDocProgresses);
        this.docProgressFileNames[i] = fileItem.file.name;
      };

      this.DocsUploaders.push(uploader);
      this.DocsUploaders[i].setOptions(this._DocsUploadersOptions[i]);
    }
  }

  // initDocsUploaderPod(): void {
  //   this.DocsUploaderPod = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
  //   this._DocsUploaderOptions.autoUpload = false;
  //   this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
  //   this._DocsUploaderOptions.removeAfterUpload = true;
  //
  //   this.DocsUploaderPod.onAfterAddingFile = (file) => {
  //     file.withCredentials = false;
  //   };
  //
  //   this.DocsUploaderPod.onBuildItemForm = (fileItem: FileItem, form: any) => {
  //     form.append('FileType', fileItem.file.type);
  //     form.append('FileName', fileItem.file.name);
  //     form.append('FileToken', this.guid());
  //   };
  //
  //   this.DocsUploaderPod.onSuccessItem = (item, response, status) => {
  //     const resp = <IAjaxResponse>JSON.parse(response);
  //     if (resp.success) {
  //       //attach each fileToken to his CreateOrEditDocumentFileDto
  //       this.createOrEditDocumentPodFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
  //       this.hasNewUploadPod = true;
  //       this.podFileToken = resp.result.fileToken;
  //       this.fileTypePod = resp.result.fileType;
  //       this.fileNamePod = resp.result.fileName;
  //     } else {
  //       this.message.error(resp.error.message);
  //     }
  //   };
  //
  //   this.DocsUploaderPod.onErrorItem = (item, response, status) => {
  //     const resp = <IAjaxResponse>JSON.parse(response);
  //   };
  //
  //   this.DocsUploaderPod.onCompleteAll = () => {
  //     // this.documentFile.updateDocumentFileInput = new UpdateDocumentFileInput();
  //     // this.documentFile.updateDocumentFileInput.fileToken = this.fileToken;
  //     // if (this.documentFile.id) {
  //     //   this._documentFilesServiceProxy
  //     //     .createOrEdit(this.documentFile)
  //     //     .pipe(
  //     //       finalize(() => {
  //     //         this.saving = false;
  //     //       })
  //     //     )
  //     //     .subscribe(() => {
  //     //       this.saving = false;
  //     //       this.notify.info(this.l('UpdatedSuccessfully'));
  //     //       this.close();
  //     //       this.modalSave.emit(null);
  //     //     });
  //     // } else if (!this.documentFile.id) {
  //     //   this.createDocumentFile();
  //     // }
  //   };
  //
  //   //for progressBar
  //   this.DocsUploaderPod.onProgressItem = (fileItem: FileItem, progress: any) => {
  //     this.podFileDocProgress = progress;
  //     console.log('this.podFileDocProgress', this.podFileDocProgress);
  //     this.docProgressFileName = fileItem.file.name;
  //   };
  //
  //   this.DocsUploaderPod.setOptions(this._DocsUploaderOptions);
  // }
  downloadAttatchment(index: number): void {
    if (!this.hasNewUploads[index]) {
      this._fileDownloadService.downloadFileByBinary(
        this.createOrEditDocumentFileDtos[index].binaryObjectId,
        this.createOrEditDocumentFileDtos[index].name,
        this.createOrEditDocumentFileDtos[index].extn
      );
    } else {
      let fileDto = new FileDto();
      fileDto.fileName = this.fileNames[index];
      fileDto.fileToken = this.fileTokens[index];
      fileDto.fileType = this.fileTypes[index];
      this._fileDownloadService.downloadTempFile(fileDto);
    }
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto, index: number): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      item.name = '';
      this.alldocumentsValid[index] = false;
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      item.name = '';
      this.alldocumentsValid[index] = false;
      return;
    }
    this.alldocumentsValid[index] = true;
    item.name = event.target.files[0].name;

    this.DocsUploaders[index].addToQueue(event.target.files);
    this.DocsUploaders[index].uploadAll();
  }

  show(point: TrackingRoutePointDto) {
    // this.initDocsUploaderPod();
    this.active = true;
    this.point = point;
    this.initDocsUploaders();
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
    // this.createOrEditDocumentPodFileDto = new CreateOrEditDocumentFileDto();
    // this.createOrEditDocumentManifestFileDto = new CreateOrEditDocumentFileDto();
    this.receiverCode = null;
    this.showReceiverCode = null;
    this.showUploadManifestFile = null;
    this.showUploadPodFile = null;
    // this.manifestFileToken = null;
    // this.podFileToken = null;
    // this.fileTypeManifest = null;
    // this.fileTypePod = null;
    // this.fileNameManifest = null;
    // this.fileNamePod = null;
    // this.manifestFileDocProgress = null;
    // this.podFileDocProgress = null;
    // this.docProgressFileNameManifest = null;
    // this.docProgressFileName = null;
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

  // downloadAttatchmentPod(): void {
  //   if (!this.hasNewUploadPod) {
  //     this._fileDownloadService.downloadFileByBinary(
  //       this.createOrEditDocumentPodFileDto.binaryObjectId,
  //       this.createOrEditDocumentPodFileDto.name,
  //       this.createOrEditDocumentPodFileDto.extn
  //     );
  //   } else {
  //     let fileDto = new FileDto();
  //     fileDto.fileName = this.fileNamePod;
  //     fileDto.fileToken = this.podFileToken;
  //     fileDto.fileType = this.fileTypePod;
  //     this._fileDownloadService.downloadTempFile(fileDto);
  //   }
  // }

  // PodDocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto): void {
  //   if (event.target.files[0].size > 5242880) {
  //     //5MB
  //     this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
  //     item.name = '';
  //     this.alldocumentsValidPod = false;
  //     return;
  //   }
  //   item.extn = event.target.files[0].type;
  //   if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
  //     item.name = '';
  //     this.alldocumentsValidPod = false;
  //     return;
  //   }
  //   this.alldocumentsValidPod = true;
  //   item.name = event.target.files[0].name;
  //
  //   this.DocsUploaderPod.addToQueue(event.target.files);
  //   this.DocsUploaderPod.uploadAll();
  // }

  upload(index: number) {
    if (!this.isFileInputValid(index)) {
      return;
    }
    const invokeRequestBody = new InvokeStepInputDto();
    invokeRequestBody.code = this.receiverCode;
    invokeRequestBody.id = this.point.id;
    invokeRequestBody.action = this.point.availableSteps[index].action;

    invokeRequestBody.documentId = this.fileTokens[index];
    invokeRequestBody.documentName = this.fileNames[index];
    invokeRequestBody.documentContentType = this.fileTypes[index];

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

  uploadPod(index: number) {
    if (!this.isFileInputValid(index)) {
      return;
    }
    const invokeRequestBody = new InvokeStepInputDto();
    invokeRequestBody.code = this.receiverCode;
    invokeRequestBody.id = this.point.id;
    invokeRequestBody.action = this.point.availableSteps.find((item) => item.stepType === AdditionalStepType.Pod)?.action;

    invokeRequestBody.documentId = this.fileTokens[index];
    invokeRequestBody.documentName = this.fileNames[index];
    invokeRequestBody.documentContentType = this.fileTypes[index];

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

  isFileInputValid(index: number) {
    return this.createOrEditDocumentFileDtos[index].name ? true : false;
  }
}
