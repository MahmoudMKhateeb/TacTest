/* tslint:disable:member-ordering */
import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Injector, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CreateOrEditDocumentFileDto,
  CreateOrEditTruckDto,
  DocumentFileDto,
  DocumentFilesServiceProxy,
  SelectItemDto,
  TrucksServiceProxy,
  TruckTruckStatusLookupTableDto,
  TruckTrucksTypeLookupTableDto,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TruckUserLookupTableModalComponent } from './truck-user-lookup-table-modal.component';
import { base64ToFile, ImageCroppedEvent } from '@node_modules/ngx-image-cropper';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { AppConsts } from '@shared/AppConsts';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { DateType } from '@app/admin/required-document-files/hijri-gregorian-datepicker/consts';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/admin/required-document-files/hijri-gregorian-datepicker/date-formatter.service';
import * as _ from 'lodash';
import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'createOrEditTruckModal',
  templateUrl: './create-or-edit-truck-modal.component.html',
  providers: [DateFormatterService],
})
export class CreateOrEditTruckModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('truckUserLookupTableModal', { static: true }) truckUserLookupTableModal: TruckUserLookupTableModalComponent;
  @ViewChild('truckUserLookupTableModal2', { static: true }) truckUserLookupTableModal2: TruckUserLookupTableModalComponent;

  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;
  // @ViewChild('paginator', { static: true }) paginator: Paginator;
  // @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  //
  // @ViewChildren('dataTable') TableComponent: Table;
  // @ViewChildren('paginator') PaginatorComponent: Paginator;
  active = false;
  saving = false;
  ModalIsEdit = null;
  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  extnFilter = '';
  binaryObjectIdFilter = '';
  maxExpirationDateFilter: moment.Moment;
  minExpirationDateFilter: moment.Moment;
  isAcceptedFilter = false;
  documentTypeDisplayNameFilter = '';
  truckPlateNumberFilter = '';
  trailerTrailerCodeFilter = '';
  userNameFilter = '';
  routStepDisplayNameFilter = '';
  _entityTypeFullName = 'TACHYON.Documents.DocumentFiles.DocumentFile';
  entityHistoryEnabled = false;
  testCond = null;
  truck: CreateOrEditTruckDto;
  trucksTypeDisplayName = '';
  truckStatusDisplayName = '';
  userName = '';
  userName2 = '';
  allTrucksTypes: TruckTrucksTypeLookupTableDto[];
  allTruckStatuss: TruckTruckStatusLookupTableDto[];
  allTransportTypes: SelectItemDto[];
  allTransportSubTypes: SelectItemDto[];
  allTruckTypesByTransportSubtype: SelectItemDto[];
  allTruckSubTypesByTruckTypeId: SelectItemDto[];
  allTrucksCapByTruckSubTypeId: SelectItemDto[];
  imageChangedEvent: any = '';
  public maxProfilPictureBytesUserFriendlyValue = 5;
  public uploader: FileUploader;
  public temporaryPictureUrl: string;
  profilePicture = '';
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
  selectedDateTypeHijri = DateType.Hijri; // or DateType.Gregorian
  selectedDateTypeGregorian = DateType.Gregorian; // or DateType.Gregorian
  private dataTable: Table;
  private paginator: Paginator;
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
    private _localStorageService: LocalStorageService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private dateFormatterService: DateFormatterService,
    private _fileDownloadService: FileDownloadService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(injector);
  }

  show(truckId?: string): void {
    if (!truckId) {
      this.truck = new CreateOrEditTruckDto();
      //initlaize truck type values
      this.truck.id = truckId;
      this.trucksTypeDisplayName = '';
      this.truckStatusDisplayName = '';
      this.userName = '';
      this.userName2 = '';

      //RequiredDocuments
      this._documentFilesServiceProxy.getTruckRequiredDocumentFiles('').subscribe((result) => {
        this.truck.createOrEditDocumentFileDtos = result;
      });
      this.ModalIsEdit = false;
      this.active = true;
      this.modal.show();
    } else {
      this.ModalIsEdit = true;
      this._trucksServiceProxy.getTruckForEdit(truckId).subscribe((result) => {
        this.truck = result.truck;

        this.trucksTypeDisplayName = result.trucksTypeDisplayName;
        this.truckStatusDisplayName = result.truckStatusDisplayName;
        this.userName = result.userName;
        this.getTruckPictureUrl(this.truck.id);

        // dropDowns
        this._trucksServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
          this.allTransportTypes = result;
        });
        this._trucksServiceProxy.getAllTransportSubtypesByTransportTypeIdForDropdown(this.truck.transportTypeId).subscribe((result) => {
          this.allTransportSubTypes = result;
        });
        this._trucksServiceProxy.getAllTruckTypesByTransportSubtypeIdForDropdown(this.truck.transportSubtypeId).subscribe((result) => {
          this.allTruckTypesByTransportSubtype = result;
        });
        this._trucksServiceProxy.getAllTruckSubTypesByTruckTypeIdForDropdown(this.truck.trucksTypeId).subscribe((result) => {
          this.allTruckSubTypesByTruckTypeId = result;
        });
        this._trucksServiceProxy.getAllTuckCapacitiesByTuckSubTypeIdForDropdown(this.truck.truckSubtypeId).subscribe((result) => {
          this.allTrucksCapByTruckSubTypeId = result;
        });

        //dropDowns

        this.active = true;
        this.modal.show();
      });
    }
    this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe((result) => {
      this.allTruckStatuss = result;
    });
    this.GetTransportDropDownList();
    this.temporaryPictureUrl = '';
    this.initDocsUploader();
    this.active = true;
  } //end of show

  createOrEditTruck() {
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
  }

  save(): void {
    this.saving = true;
    if (this.truck.id) {
      this.createOrEditTruck();
    }
    if (this.DocsUploader && this.DocsUploader.queue.length > 0) {
      this.DocsUploader.uploadAll();
    } else {
      this.createOrEditTruck();
    }
    // this.uploader.uploadAll();
  }

  openSelectUserModal() {
    this.truckUserLookupTableModal.id = this.truck.driver1UserId;
    this.truckUserLookupTableModal.displayName = this.userName;
    this.truckUserLookupTableModal.show();
  }

  setDriver1UserIdNull() {
    this.truck.driver1UserId = null;
    this.userName = '';
  }

  getNewDriver1UserId() {
    this.truck.driver1UserId = this.truckUserLookupTableModal.id;
    this.userName = this.truckUserLookupTableModal.displayName;
  }

  getNewDriver2UserId() {
    this.userName2 = this.truckUserLookupTableModal2.displayName;
  }

  close(): void {
    this.active = false;
    this.imageChangedEvent = '';
    if (this.DocsUploader) {
      this.DocsUploader.clearQueue();
    }
    this.docProgressFileName = null;
    this.docProgress = null;
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
    };

    this.DocsUploader.onCompleteAll = () => {
      // create truck req.
      this.createOrEditTruck();
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

  ClearAllTransPortDropDowns() {
    this.allTransportTypes = null;
    this.allTransportSubTypes = null;
    this.allTruckTypesByTransportSubtype = null;
    this.allTruckSubTypesByTruckTypeId = null;
    this.allTrucksCapByTruckSubTypeId = null;
  }

  GetTransportDropDownList(mode?: string, value?: number) {
    switch (mode) {
      case 'GetAllTransportSubTypes':
        if (value > 0) {
          this._trucksServiceProxy.getAllTransportSubtypesByTransportTypeIdForDropdown(value).subscribe((result) => {
            this.allTransportSubTypes = result;
          });
        } else {
          this.allTransportSubTypes = null;
          this.allTruckTypesByTransportSubtype = null;
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllTruckTypesByTransportSubTypes':
        if (value > 0) {
          this._trucksServiceProxy.getAllTruckTypesByTransportSubtypeIdForDropdown(value).subscribe((result) => {
            this.allTruckTypesByTransportSubtype = result;
          });
        } else {
          this.allTruckTypesByTransportSubtype = null;
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllTruckSubTypesByTruckTypeId':
        if (value > 0) {
          this._trucksServiceProxy.getAllTruckSubTypesByTruckTypeIdForDropdown(value).subscribe((result) => {
            this.allTruckSubTypesByTruckTypeId = result;
          });
        } else {
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllCapByTruckSybTypeId':
        if (value > 0) {
          this._trucksServiceProxy.getAllTuckCapacitiesByTuckSubTypeIdForDropdown(value).subscribe((result) => {
            this.allTrucksCapByTruckSubTypeId = result;
          });
        } else {
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      default:
        this.ClearAllTransPortDropDowns();
        if (!this.truck.transportTypeId) {
          this._trucksServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
            this.allTransportTypes = result;
          });
        }
        break;
    }
  }

  reloadPage(): void {
    this.changeDetectorRef.detectChanges();

    this.paginator.changePage(this.paginator.getPage());
  }

  showHistory(documentFile: DocumentFileDto): void {
    this.entityTypeHistoryModal.show({
      entityId: documentFile.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  // }
  downloadDocument(documentFile: DocumentFileDto) {
    this._documentFilesServiceProxy.getDocumentFileDto(documentFile.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  // exportToExcel(): void {
  //   this._documentFilesServiceProxy
  //     .getDocumentFilesToExcel(
  //       this.filterText,
  //       this.nameFilter,
  //       this.extnFilter,
  //       this.binaryObjectIdFilter,
  //       this.maxExpirationDateFilter,
  //       this.minExpirationDateFilter,
  //       this.isAcceptedFilter,
  //       this.documentTypeDisplayNameFilter,
  //       this.truckPlateNumberFilter,
  //       this.trailerTrailerCodeFilter,
  //       this.userNameFilter,
  //       this.routStepDisplayNameFilter
  //     )
  //     .subscribe((result) => {
  //       this._fileDownloadService.downloadTempFile(result);
  //     });

  selectedDateChange($event: NgbDateStruct, item: CreateOrEditDocumentFileDto) {
    if ($event != null && $event.year < 2000) {
      this.dateFormatterService.SetFormat('DD/MM/YYYY', 'iDD/iMM/iYYYY');
      const incomingDate = this.dateFormatterService.ToGregorian($event);
      item.expirationDate = moment(incomingDate.month + '/' + incomingDate.day + '/' + incomingDate.year, 'MM/DD/YYYY');
    } else if ($event != null && $event.year > 2000) {
      item.expirationDate = moment($event.month + '/' + $event.day + '/' + $event.year, 'MM/DD/YYYY');
    }
  }

  //document files methods
  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }
}
