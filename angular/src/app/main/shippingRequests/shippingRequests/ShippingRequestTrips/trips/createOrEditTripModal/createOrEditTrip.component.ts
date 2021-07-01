import { Component, ViewChild, Injector, Output, EventEmitter, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  FacilityForDropdownDto,
  RoutStepsServiceProxy,
  ShippingRequestsTripServiceProxy,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditRoutPointDto,
  ShippingRequestDto,
  CreateOrEditShippingRequestVasListDto,
  CreateOrEditShippingRequestTripVasDto,
  ShippingRequestVasDto,
  GetShippingRequestVasForViewDto,
  ShippingRequestsTripForViewDto,
  WaybillsServiceProxy,
  CreateOrEditDocumentFileDto,
  UpdateDocumentFileInput,
  DocumentFilesServiceProxy,
  DocumentTypeDto,
  ShippingRequestTripVasDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';
import { PointsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.component';
import Swal from 'sweetalert2';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';

@Component({
  selector: 'AddNewTripModal',
  styleUrls: ['./createOrEditTrip.component.scss'],
  templateUrl: './createOrEditTrip.component.html',
})
export class CreateOrEditTripComponent extends AppComponentBase implements OnInit {
  @ViewChild('addNewTripsModal', { static: true }) modal: ModalDirective;
  @ViewChild('PointsComponent') PointsComponent: PointsComponent;
  @ViewChild('createOrEditFacilityModal') createOrEditFacilityModal: CreateOrEditFacilityModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() shippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];

  allFacilities: FacilityForDropdownDto[];
  trip = new CreateOrEditShippingRequestTripDto();
  facilityLoading = false;
  saving = false;
  loading = true;
  active = false;
  activeTripId: number = undefined;
  cleanVasesList: CreateOrEditShippingRequestTripVasDto[] = [];

  //documentFile: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  alldocumentsValid = false;
  public DocsUploader: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  fileToken: string;
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
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private cdref: ChangeDetectorRef,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _tokenService: TokenService
  ) {
    super(injector);
  }

  ngOnInit() {
    //load the Facilites
    this.refreshOrGetFacilities();
    this.vasesHandler();
  }

  /**
   * takes the Vas List From the Shipping Request And Cleans them to use them in Trips Modal
   */
  vasesHandler() {
    this.VasListFromFather.forEach((x) => {
      //Get the Vase List From Father And Attach Them to new Array
      const vas: CreateOrEditShippingRequestTripVasDto = new CreateOrEditShippingRequestTripVasDto();
      vas.id = undefined; // vas id in shipping Request trip (Required for edit trip)
      vas.shippingRequestTripId = this.activeTripId || undefined; //the trip id
      vas.shippingRequestVasId = x.shippingRequestVas.id; //vas id in shipping request
      vas.name = x.vasName; //vas Name
      this.cleanVasesList.push(vas);
    });
  }

  show(record?: CreateOrEditShippingRequestTripDto): void {
    if (record) {
      this.activeTripId = record.id;
      this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this.trip = res;
        this.PointsComponent.wayPointsList = this.trip.routPoints;
        this.loading = false;
      });
    } else {
      //this is a create

      //init file document
      this.trip = new CreateOrEditShippingRequestTripDto();
      this.trip.createOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
      this.trip.createOrEditDocumentFileDto.extn = '_';
      this.trip.createOrEditDocumentFileDto.name = '_';
      this.trip.createOrEditDocumentFileDto.documentTypeDto = new DocumentTypeDto();
      this.loading = false;
      this.trip.shippingRequestId = this.shippingRequest.id;
    }
    this.active = true;
    this.modal.show();
    this.initDocsUploader();
    this.cdref.detectChanges();
  }
  close(): void {
    this.loading = true;
    this.active = false;
    this.modal.hide();
    this.trip = new CreateOrEditShippingRequestTripDto();
  }

  createOrEditTrip() {
    this.saving = true;
    this._shippingRequestTripsService
      .createOrEdit(this.trip)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.close();
        this.modalSave.emit(null);
        this.notify.info(this.l('SuccessfullySaved'));
      });
  }

  deleteTrip(tripid: number) {
    Swal.fire({
      title: this.l('areYouSure'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this._shippingRequestTripsService.delete(tripid).subscribe(() => {
          this.close();
          this.modalSave.emit(null);
          this.notify.info(this.l('SuccessfullyDeleted'));
        });
      } //end of if
    });
  }
  refreshOrGetFacilities(facility: FacilityForDropdownDto | undefined) {
    if (facility) {
      this.allFacilities.push(facility);
      this.trip.originFacilityId = facility.id;
    } else {
      this.facilityLoading = true;
      this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
        this.allFacilities = result;
        this.facilityLoading = false;
      });
    }
  }

  DownloadSingleDropWaybillPdf(id: number): void {
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  /**
   * validates add or Edit Trip Dates
   */
  validateTripDates() {
    // if (this.trip.endTripDate && this.trip.startTripDate > this.trip.endTripDate) {
    //   this.trip.endTripDate = undefined;
    //   this.notify.error(this.l('tripStartDateCantBeGretterThanTripEndDate'));
    // }
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
        this.trip.createOrEditDocumentFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
        this.fileToken = resp.result.fileToken;
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploader.onCompleteAll = () => {
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
      item.name = '';
      this.alldocumentsValid = false;
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      item.name = '';
      this.alldocumentsValid = false;
      return;
    }
    this.alldocumentsValid = true;
    item.name = event.target.files[0].name;

    this.DocsUploader.addToQueue(event.target.files);
    this.DocsUploader.uploadAll();
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
