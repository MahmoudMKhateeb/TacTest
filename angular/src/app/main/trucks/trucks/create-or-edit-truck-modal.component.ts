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
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import * as _ from 'lodash';
import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'createOrEditTruckModal',
  styleUrls: ['./trucksComponent.css'],
  templateUrl: './create-or-edit-truck-modal.component.html',
  providers: [DateFormatterService],
})
export class CreateOrEditTruckModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
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
  routStepDisplayNameFilter = '';
  _entityTypeFullName = 'TACHYON.Documents.DocumentFiles.DocumentFile';
  entityHistoryEnabled = false;
  testCond = null;
  truck: CreateOrEditTruckDto;
  trucksTypeDisplayName = '';
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
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
  fileFormateIsInvalideIndexList: boolean[] = [];
  fileisDuplicateList: boolean[] = [];
  alldocumentsNotDuplicated = false;
  allnumbersValid = false;
  numbersInValidList: boolean[] = [];
  alldocumentsValid = false;
  allDatesValid = true;
  datesInValidList: boolean[] = [];

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
      this.truck.truckStatusId = null;
      this.truck.transportTypeId = null;
      this.truck.transportSubtypeId = null;
      this.truck.trucksTypeId = null;
      this.truck.truckSubtypeId = null;
      this.truck.capacityId = null;

      this.GetTransportDropDownList();
      this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe((result) => {
        this.allTruckStatuss = result;
      });

      //RequiredDocuments
      this._documentFilesServiceProxy.getTruckRequiredDocumentFiles('').subscribe((result) => {
        this.truck.createOrEditDocumentFileDtos = result;
        this.intilizedates();
      });
      this.ModalIsEdit = false;
      this.active = true;
      this.modal.show();
    } else {
      this.ModalIsEdit = true;
      this._trucksServiceProxy.getTruckForEdit(truckId).subscribe((result) => {
        this.truck = result.truck;
        this.GetTransportDropDownList();
        this.trucksTypeDisplayName = result.trucksTypeDisplayName;
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
        this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe((result) => {
          this.allTruckStatuss = result;
        });
        //dropDown
        this.active = true;
        this.modal.show();
      });
    }

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
      if (!this.alldocumentsValid || !this.allnumbersValid || !this.allDatesValid) {
        this.notify.error(this.l('makeSureThatYouFillAllRequiredFields'));
        return;
      }

      this.truck.createOrEditDocumentFileDtos.forEach((element) => {
        let date = this.dateFormatterService.MomentToNgbDateStruct(element.expirationDate);
        let hijriDate = this.dateFormatterService.ToHijri(date);
        element.hijriExpirationDate = this.dateFormatterService.ToString(hijriDate);
      });
      this.DocsUploader.uploadAll();
    } else {
      this.createOrEditTruck();
    }
    // this.uploader.uploadAll();
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
        // console.log(resp.result.fileToken);
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

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto, index: number): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      this.isAllfileFormatesAccepted();
      item.name = '';
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      this.fileFormateIsInvalideIndexList[index] = true;
      // this.truckFiles[index] = null;
      item.name = '';
      this.isAllfileFormatesAccepted();
      return;
    }
    item.name = event.target.files[0].name;
    //not allow uploading the same document twice
    for (let i = 0; i < this.truck.createOrEditDocumentFileDtos.length; i++) {
      const element = this.truck.createOrEditDocumentFileDtos[i];
      if (element.name == event.target.files[0].name && element.extn == event.target.files[0].type && i != index) {
        item.name = '';
        item.extn = '';
        this.fileisDuplicateList[index] = true;
        this.isAllfileNotDuplicated();
        this.message.warn(this.l('DuplicateFileUploadMsg', element.name, element.extn));
        return;
      }
    }

    this.fileisDuplicateList[index] = false;
    this.isAllfileNotDuplicated();
    this.fileFormateIsInvalideIndexList[index] = false;

    // item.name = '';
    // this.truckFiles[index] = event.target.files;
    this.isAllfileFormatesAccepted();
    this.DocsUploader.addToQueue(event.target.files);
  }

  ClearAllTransPortDropDowns() {
    this.allTransportTypes = null;
    this.allTransportSubTypes = null;
    this.allTruckTypesByTransportSubtype = null;
    this.allTruckSubTypesByTruckTypeId = null;
    this.allTrucksCapByTruckSubTypeId = null;
    this.allTruckStatuss = null;
  }

  GetTransportDropDownList(mode?: string, value?: number) {
    switch (mode) {
      case 'GetAllTransportSubTypes':
        if (value > 0) {
          this._trucksServiceProxy.getAllTransportSubtypesByTransportTypeIdForDropdown(value).subscribe((result) => {
            this.allTransportSubTypes = result;
            this.truck.transportSubtypeId = null;
          });
        } else {
          this.truck.transportSubtypeId = null;
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
            this.truck.trucksTypeId = null;
          });
        } else {
          this.truck.trucksTypeId = null;
          this.allTruckTypesByTransportSubtype = null;
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllTruckSubTypesByTruckTypeId':
        if (value > 0) {
          this._trucksServiceProxy.getAllTruckSubTypesByTruckTypeIdForDropdown(value).subscribe((result) => {
            this.allTruckSubTypesByTruckTypeId = result;
            this.truck.truckSubtypeId = null;
          });
        } else {
          this.truck.truckSubtypeId = null;
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllCapByTruckSybTypeId':
        if (value > 0) {
          this._trucksServiceProxy.getAllTuckCapacitiesByTuckSubTypeIdForDropdown(value).subscribe((result) => {
            this.allTrucksCapByTruckSubTypeId = result;
            this.truck.capacityId = null;
          });
        } else {
          this.truck.capacityId = null;
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
  isAllfileFormatesAccepted() {
    if (
      this.fileFormateIsInvalideIndexList.every((x) => x === false) &&
      this.fileFormateIsInvalideIndexList.length == this.truck.createOrEditDocumentFileDtos.length
    ) {
      this.alldocumentsValid = true;
    } else {
      this.alldocumentsValid = false;
    }
  }

  numberChange(item: CreateOrEditDocumentFileDto, index: number) {
    if (item.documentTypeDto.numberMinDigits <= item.number.length && item.number.length <= item.documentTypeDto.numberMaxDigits) {
      this.numbersInValidList[index] = false;
      this.isNumbersValid();
    } else {
      this.numbersInValidList[index] = true;
      this.isNumbersValid();
    }
  }
  isNumbersValid() {
    if (this.numbersInValidList.every((x) => x === false) && this.numbersInValidList.length == this.truck.createOrEditDocumentFileDtos.length) {
      this.allnumbersValid = true;
    } else {
      this.allnumbersValid = false;
    }
  }

  // dateSelected() {
  //   for (let index = 0; index < this.truck.createOrEditDocumentFileDtos.length; index++) {
  //     const element = this.truck.createOrEditDocumentFileDtos[index];
  //     if (element.documentTypeDto.hasExpirationDate) {
  //       this.datesInValidList[index] = element.expirationDate == null;
  //     }
  //   }
  //   if (
  //     this.datesInValidList.every((x) => x === false) &&
  //     this.truck.createOrEditDocumentFileDtos.filter((x) => x.documentTypeDto.hasExpirationDate).length == this.datesInValidList.length
  //   ) {
  //     this.allDatesValid = true;
  //   } else {
  //     this.allDatesValid = false;
  //   }
  // }

  isAllfileNotDuplicated() {
    if (this.fileisDuplicateList.every((x) => x === false) && this.fileisDuplicateList.length == this.truck.createOrEditDocumentFileDtos.length) {
      this.alldocumentsNotDuplicated = true;
    } else {
      this.alldocumentsNotDuplicated = false;
    }
  }

  intilizedates() {
    this.truck.createOrEditDocumentFileDtos.forEach((element) => {
      if (element.documentTypeDto.hasExpirationDate) {
        element.expirationDate = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
      }
    });
  }
}
