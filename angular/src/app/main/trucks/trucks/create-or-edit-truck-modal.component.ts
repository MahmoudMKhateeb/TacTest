/* tslint:disable:member-ordering */
import { ChangeDetectorRef, Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CarriersForDropDownDto,
  CreateOrEditTruckDto,
  DocumentFileDto,
  DocumentFilesServiceProxy,
  ISelectItemDto,
  PlateTypeSelectItemDto,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  TrucksServiceProxy,
  TruckTruckStatusLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TruckUserLookupTableModalComponent } from './truck-user-lookup-table-modal.component';
import { base64ToFile, ImageCroppedEvent } from '@node_modules/ngx-image-cropper';
import { FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { TokenService } from '@node_modules/abp-ng2-module';
import { AppConsts } from '@shared/AppConsts';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import * as _ from 'lodash';
import { Paginator } from '@node_modules/primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as moment from '@node_modules/moment';
import { RequiredDocumentFormChildComponent } from '@app/shared/common/required-document-form-child/required-document-form-child.component';
import { NgForm } from '@angular/forms';
import { formatDate } from '@angular/common';

@Component({
  selector: 'createOrEditTruckModal',
  styleUrls: ['./trucks.Component.css'],
  templateUrl: './create-or-edit-truck-modal.component.html',
  providers: [DateFormatterService],
})
export class CreateOrEditTruckModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('truckForm', { static: false }) truckForm: NgForm;

  @ViewChild('truckUserLookupTableModal2', { static: true }) truckUserLookupTableModal2: TruckUserLookupTableModalComponent;

  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

  @ViewChild('requiredDocumentFormChildComponent', { static: false }) requiredDocumentFormChildComponent: RequiredDocumentFormChildComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
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
  allTrucksTypes: ISelectItemDto[];
  allTruckStatuss: TruckTruckStatusLookupTableDto[];
  allTransportTypes: SelectItemDto[];
  allTruckTypesByTransportType: SelectItemDto[];
  allTrucksCapByTruckTypeId: SelectItemDto[];
  imageChangedEvent: any = '';
  public maxProfilPictureBytesUserFriendlyValue = 5;
  public uploader: FileUploader;
  public temporaryPictureUrl: string;
  profilePicture = '';
  fileFormateIsInvalideIndexList: boolean[] = [];
  carriers: CarriersForDropDownDto[] = [];

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

  allPlateTypes: PlateTypeSelectItemDto[];

  truckModelMaxYear = new Date();
  //truckModelMinYear = new Date();

  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _tokenService: TokenService,
    private _localStorageService: LocalStorageService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private changeDetectorRef: ChangeDetectorRef,
    private _shippingRequestsService: ShippingRequestsServiceProxy
  ) {
    super(injector);
  }

  show(truckId?: number): void {
    this.truck = new CreateOrEditTruckDto();
    if (!truckId) {
      //initlaize truck type values
      this.truck.id = truckId;
      this.trucksTypeDisplayName = '';
      this.truck.truckStatusId = null;
      this.truck.transportTypeId = null;
      this.truck.transportTypeId = null;
      this.truck.trucksTypeId = null;
      this.truck.trucksTypeId = null;
      this.truck.capacityId = null;
      this.truck.plateTypeId = null;
      this.truck.capacity = null;

      this.initTransportDropDownList();
      this._trucksServiceProxy.getAllTruckStatusForTableDropdown().subscribe((result) => {
        this.allTruckStatuss = result;
      });

      this._shippingRequestsService.getAllCarriersForDropDown().subscribe((result) => (this.carriers = result));

      this._trucksServiceProxy.getAllPlateTypeIdForDropdown().subscribe((result) => {
        this.allPlateTypes = result;
      });

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
        this.initTransportDropDownList();
        this.trucksTypeDisplayName = result.trucksTypeDisplayName;
        this.getTruckPictureUrl(this.truck.id);

        // dropDowns
        this._trucksServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
          this.allTransportTypes = result;
        });
        this._trucksServiceProxy.getAllPlateTypeIdForDropdown().subscribe((result) => {
          this.allPlateTypes = result;
        });
        this._trucksServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(this.truck.transportTypeId).subscribe((result) => {
          this.allTruckTypesByTransportType = result;
        });
        this._trucksServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(this.truck.trucksTypeId).subscribe((result) => {
          this.allTrucksCapByTruckTypeId = result;
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
    this.active = true;
  } //end of show

  createOrEditTruck() {
    this.truck.modelYear = moment(this.truck.modelYear).locale('en').format('YYYY');
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
    // if (!this.alldocumentsValid || !this.allnumbersValid || !this.allDatesValid) {
    //   this.notify.error(this.l('makeSureThatYouFillAllRequiredFields'));
    //   return;
    // }

    if (this.requiredDocumentFormChildComponent.DocsUploader.queue?.length > 0) {
      this.normalizeDates();
      this.requiredDocumentFormChildComponent.DocsUploader.uploadAll();
    } else {
      this.createOrEditTruck();
    }
    // this.uploader.uploadAll();
  }

  close(): void {
    this.active = false;
    this.imageChangedEvent = '';

    this.truck = new CreateOrEditTruckDto();
    this.requiredDocumentFormChildComponent?.DocsUploader?.clearQueue();

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
  get isTruckTenantRequired(): boolean {
    return this.feature.isEnabled('App.TachyonDealer') && !this.truck?.id;
  }

  getTruckPictureUrl(truckId: number): void {
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

  transportTypeSelectChange(transportTypeId?: number) {
    if (transportTypeId > 0) {
      this._trucksServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(transportTypeId).subscribe((result) => {
        this.allTruckTypesByTransportType = result;
        this.truck.trucksTypeId = null;
      });
    } else {
      this.truck.trucksTypeId = null;
      this.allTruckTypesByTransportType = null;
      this.allTrucksCapByTruckTypeId = null;
    }
  }

  trucksTypeSelectChange(trucksTypeId?: number) {
    if (trucksTypeId > 0) {
      this._trucksServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(trucksTypeId).subscribe((result) => {
        this.allTrucksCapByTruckTypeId = result;
        this.truck.capacityId = null;
      });
    } else {
      this.truck.capacityId = null;
      this.allTrucksCapByTruckTypeId = null;
    }
  }

  initTransportDropDownList() {
    this.allTransportTypes = null;
    this.allTruckTypesByTransportType = null;
    this.allTrucksCapByTruckTypeId = null;
    this.allTruckStatuss = null;
    if (!this.truck.transportTypeId) {
      this._trucksServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
        this.allTransportTypes = result;
      });
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

  private normalizeDates() {
    this.truck.createOrEditDocumentFileDtos.forEach((element) => {
      let date = this.dateFormatterService.MomentToNgbDateStruct(element.expirationDate);
      let hijriDate = this.dateFormatterService.ToHijri(date);
      element.hijriExpirationDate = this.dateFormatterService.ToString(hijriDate);
    });
  }

  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }

  // plateNumberNoramlize() {
  //   this.truck.plateNumber = this.truck.plateNumber
  //     .replace(/\s/g, '')
  //     .replace(/([A-Z])/g, ' $1')
  //     .replace(/([a-z])/g, ' $1')
  //     .replace(/([أ-ي])/g, ' $1')
  //     .trim();
  // }

  // validateModalYear() {
  //   console.log('Validator Modal Year Ran');
  //   const selectedModalYear = moment(this.truck.modelYear).locale('en').format('YYYY');
  //   if (this.Number(selectedModalYear) > 1999 && this.Number(selectedModalYear) < moment().year()) {
  //     return true;
  //   }
  //   return false;
  // }
}
