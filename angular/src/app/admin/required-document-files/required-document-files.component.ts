import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse } from '@node_modules/abp-ng2-module';
import { CreateOrEditDocumentFileDto, UpdateDocumentFileInput } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-required-document-files',
  templateUrl: './required-document-files.component.html',
})
export class RequiredDocumentFilesComponent extends AppComponentBase implements OnInit {
  /**
   * required documents fileUploader
   */
  public DocsUploader: FileUploader;
  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  private _uploaderOptions: FileUploaderOptions = {};
  /**
   * required documents fileUploader options
   * @private
   */
  private _DocsUploaderOptions: FileUploaderOptions = {};

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}

  /**
   * initialize required documents fileUploader
   */
  // initDocsUploader(): void {
  //   this.DocsUploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
  //   this._DocsUploaderOptions.autoUpload = false;
  //   this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
  //   this._DocsUploaderOptions.removeAfterUpload = true;
  //
  //   this.DocsUploader.onAfterAddingFile = (file) => {
  //     file.withCredentials = false;
  //   };
  //
  //   this.DocsUploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
  //     form.append('FileType', fileItem.file.type);
  //     form.append('FileName', fileItem.file.name);
  //     form.append('FileToken', this.guid());
  //   };
  //
  //   this.DocsUploader.onSuccessItem = (item, response, status) => {
  //     const resp = <IAjaxResponse>JSON.parse(response);
  //
  //     if (resp.success) {
  //       //attach each fileToken to his CreateOrEditDocumentFileDto
  //       this.truck.createOrEditDocumentFileDtos.find(
  //         (x) => x.name === item.file.name && x.extn === item.file.type
  //       ).updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
  //     } else {
  //       this.message.error(resp.error.message);
  //     }
  //   };
  //
  //   this.DocsUploader.onErrorItem = (item, response, status) => {
  //     const resp = <IAjaxResponse>JSON.parse(response);
  //     console.log(resp);
  //   };
  //
  //   this.DocsUploader.onCompleteAll = () => {
  //     // create truck req.
  //     this._trucksServiceProxy
  //       .createOrEdit(this.truck)
  //       .pipe(
  //         finalize(() => {
  //           this.saving = false;
  //         })
  //       )
  //       .subscribe(() => {
  //         this.saving = false;
  //         this.notify.info(this.l('SavedSuccessfully'));
  //         this.close();
  //         this.modalSave.emit(null);
  //       });
  //   };
  //
  //   //for progressBar
  //   this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
  //     this.docProgress = progress;
  //     this.docProgressFileName = fileItem.file.name;
  //   };
  //
  //   this.DocsUploader.setOptions(this._DocsUploaderOptions);
  // }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      return;
    }
    this.DocsUploader.addToQueue(event.target.files);

    item.extn = event.target.files[0].type;
    item.name = event.target.files[0].name;
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
