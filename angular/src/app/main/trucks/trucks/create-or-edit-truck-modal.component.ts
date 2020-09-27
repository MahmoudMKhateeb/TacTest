/* tslint:disable:member-ordering */
import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CreateOrEditDocumentFileDto,
  CreateOrEditTruckDto,
  TrucksServiceProxy,
  TruckTruckStatusLookupTableDto,
  TruckTrucksTypeLookupTableDto,
  UpdateDocumentFileInput,
  UpdateTruckPictureInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { TruckUserLookupTableModalComponent } from './truck-user-lookup-table-modal.component';
import { base64ToFile, ImageCroppedEvent } from '@node_modules/ngx-image-cropper';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { AppConsts } from '@shared/AppConsts';
import { LocalStorageService } from '@shared/utils/local-storage.service';

@Component({
  selector: 'createOrEditTruckModal',
  templateUrl: './create-or-edit-truck-modal.component.html',
})
export class CreateOrEditTruckModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('truckUserLookupTableModal', { static: true }) truckUserLookupTableModal: TruckUserLookupTableModalComponent;
  @ViewChild('truckUserLookupTableModal2', { static: true }) truckUserLookupTableModal2: TruckUserLookupTableModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  truck: CreateOrEditTruckDto = new CreateOrEditTruckDto();

  trucksTypeDisplayName = '';
  truckStatusDisplayName = '';
  userName = '';
  userName2 = '';

  allTrucksTypes: TruckTrucksTypeLookupTableDto[];
  allTruckStatuss: TruckTruckStatusLookupTableDto[];

  imageChangedEvent: any = '';
  public maxProfilPictureBytesUserFriendlyValue = 5;
  public uploader: FileUploader;
  public temporaryPictureUrl: string;
  profilePicture: string;
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

  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _tokenService: TokenService,
    private _localStorageService: LocalStorageService
  ) {
    super(injector);
  }

  show(truckId?: string): void {
    if (!truckId) {
      this.truck = new CreateOrEditTruckDto();
      this.truck.id = truckId;
      this.truck.licenseExpirationDate = moment().startOf('day');
      this.trucksTypeDisplayName = '';
      this.truckStatusDisplayName = '';
      this.userName = '';
      this.userName2 = '';

      //RequiredDocuments
      this._trucksServiceProxy.getRequiredDocumentFileListForCreateOrEdit().subscribe((result) => {
        this.truck.createOrEditDocumentFileDtos = result;
      });

      this.active = true;
      this.modal.show();
    } else {
      this._trucksServiceProxy.getTruckForEdit(truckId).subscribe((result) => {
        this.truck = result.truck;

        this.trucksTypeDisplayName = result.trucksTypeDisplayName;
        this.truckStatusDisplayName = result.truckStatusDisplayName;
        this.userName = result.userName;
        this.userName2 = result.userName2;
        this.getTruckPictureUrl(this.truck.id);

        this.active = true;
        this.modal.show();
      });
    }
    this._trucksServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
    this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe((result) => {
      this.allTruckStatuss = result;
    });

    this.initializeModal();
  }

  initializeModal(): void {
    this.active = true;
    this.temporaryPictureUrl = '';
    this.initFileUploader();
    this.initDocsUploader();
  }

  save(): void {
    this.saving = true;
    // this.uploader.uploadAll();
    this.DocsUploader.uploadAll();
  }

  openSelectUserModal() {
    this.truckUserLookupTableModal.id = this.truck.driver1UserId;
    this.truckUserLookupTableModal.displayName = this.userName;
    this.truckUserLookupTableModal.show();
  }

  openSelectUserModal2() {
    this.truckUserLookupTableModal2.id = this.truck.driver2UserId;
    this.truckUserLookupTableModal2.displayName = this.userName;
    this.truckUserLookupTableModal2.show();
  }

  setDriver1UserIdNull() {
    this.truck.driver1UserId = null;
    this.userName = '';
  }

  setDriver2UserIdNull() {
    this.truck.driver2UserId = null;
    this.userName2 = '';
  }

  getNewDriver1UserId() {
    this.truck.driver1UserId = this.truckUserLookupTableModal.id;
    this.userName = this.truckUserLookupTableModal.displayName;
  }

  getNewDriver2UserId() {
    this.truck.driver2UserId = this.truckUserLookupTableModal2.id;
    this.userName2 = this.truckUserLookupTableModal2.displayName;
  }

  close(): void {
    this.active = false;
    this.imageChangedEvent = '';
    this.uploader.clearQueue();
    this.DocsUploader.clearQueue();
    this.modal.hide();
  }

  fileChangeEvent(event: any): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('ProfilePicture_Warn_SizeLimit', this.maxProfilPictureBytesUserFriendlyValue));
      return;
    }

    this.imageChangedEvent = event;
  }

  imageCroppedFile(event: ImageCroppedEvent) {
    this.uploader.clearQueue();
    this.uploader.addToQueue([<File>base64ToFile(event.base64)]);
  }

  initFileUploader(): void {
    this.uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Profile/UploadProfilePicture' });
    this._uploaderOptions.autoUpload = false;
    this._uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._uploaderOptions.removeAfterUpload = true;
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', 'ProfilePicture');
      form.append('FileToken', this.guid());
    };

    this.uploader.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      if (resp.success) {
        //this.updateProfilePicture(resp.result.fileToken);
        this.truck.updateTruckPictureInput = new UpdateTruckPictureInput();
        this.truck.updateTruckPictureInput.fileToken = resp.result.fileToken;
        this.truck.updateTruckPictureInput.x = 0;
        this.truck.updateTruckPictureInput.y = 0;
        this.truck.updateTruckPictureInput.width = 0;
        this.truck.updateTruckPictureInput.height = 0;
        this._trucksServiceProxy
          .createOrEdit(this.truck)
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
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.uploader.setOptions(this._uploaderOptions);
  }

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }

  getTruckPictureUrl(truckId: string): void {
    let self = this;
    this._localStorageService.getItem(AppConsts.authorization.encrptedAuthTokenName, function (err, value) {
      self.profilePicture =
        AppConsts.remoteServiceBaseUrl +
        '/Helper/GetTruckPictureByTruckId?truckId=' +
        truckId +
        '&' +
        AppConsts.authorization.encrptedAuthTokenName +
        '=' +
        encodeURIComponent(value.token);
    });
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
        this.truck.createOrEditDocumentFileDtos.find(
          (x) => x.name === item.file.name && x.extn === item.file.type
        ).updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      console.log(resp);
    };

    this.DocsUploader.onCompleteAll = () => {
      // create truck req.
      this._trucksServiceProxy
        .createOrEdit(this.truck)
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
    };

    //for progressBar
    this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.docProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploader.setOptions(this._DocsUploaderOptions);
  }

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
}
