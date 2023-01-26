import { FileDownloadService } from '@shared/utils/file-download.service';
import { EnumToArrayPipe } from './../../../../../../../shared/common/pipes/enum-to-array.pipe';
import { Component, Injector, OnDestroy, OnInit, Output, ViewChild, EventEmitter, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  PriceOfferCommissionType,
  TripAppointmentDataDto,
  CreateOrEditDocumentFileDto,
  TripClearancePricesDto,
  PenaltiesServiceProxy,
  FileDto,
  ShippingRequestsTripServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { isNotNullOrUndefined } from 'codelyzer/util/isNotNullOrUndefined';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'appointment-and-clearance-modal',
  styleUrls: ['./appointment-and-clearance.component.scss'],
  templateUrl: './appointment-and-clearance.component.html',
})
export class AppointmentAndClearanceModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('appointmentAndClearanceModal', { static: true }) modal: ModalDirective;
  @Output('saved') saved = new EventEmitter<{ tripAppointment: TripAppointmentDataDto; tripClearance: TripClearancePricesDto }>();
  @Input('isEdit') isEdit = false;
  tripAppointment: TripAppointmentDataDto = new TripAppointmentDataDto();
  tripClearance: TripClearancePricesDto = new TripClearancePricesDto();
  isFormSubmitted: boolean;
  saving: boolean;
  private pointId: number;

  get isFileInputValid() {
    return this.createOrEditDocumentFileDto.name ? true : false;
  }

  get canEdit(): boolean {
    if (this.isTachyonDealer) {
      return true;
    }
    if (this.isCarrier || this.isCarrierSaas) {
      return true;
    }
    return false;
  }
  alldocumentsValid: boolean;
  public DocsUploader: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  fileToken: any;
  fileType: any;
  fileName: any;
  hasNewUpload: boolean;
  docProgress: any;
  createOrEditDocumentFileDto: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  docProgressFileName: string;
  allPriceOfferCommissionTypes: any[] = [];
  taxVat: number;
  needsAppointment = false;
  needsClearance = false;

  constructor(
    injector: Injector,
    private _tokenService: TokenService,
    private _enumService: EnumToArrayPipe,
    private _PenaltiesServiceProxy: PenaltiesServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _PointsService: PointsService,
    private _shippingRequestsTripServiceProxy: ShippingRequestsTripServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.allPriceOfferCommissionTypes = this._enumService.transform(PriceOfferCommissionType).map((item) => {
      item.key = Number(item.key);
      return item;
    });
  }

  show(
    pointId: number | null,
    dropNeedsClearance: boolean,
    dropNeedsAppointment: boolean,
    tripAppointment: TripAppointmentDataDto,
    tripClearance: TripClearancePricesDto
  ) {
    this.needsAppointment = dropNeedsAppointment;
    this.needsClearance = dropNeedsClearance;
    this.pointId = pointId;
    if (isNotNullOrUndefined(tripClearance)) {
      this.tripClearance = tripClearance;
    }
    if (isNotNullOrUndefined(tripAppointment)) {
      this.tripAppointment = tripAppointment;
      if (tripAppointment.documentName) {
        this.createOrEditDocumentFileDto.name = tripAppointment.documentName;
      }
    }
    this.modal.show();
    this.initDocsUploader();
    this.getTaxVat();
  }

  close(): void {
    this.modal.hide();
    this.tripAppointment = new TripAppointmentDataDto();
    this.tripClearance = new TripClearancePricesDto();
    this.hasNewUpload = undefined;
    this.needsAppointment = false;
    this.needsClearance = false;
  }

  save() {
    if (this.isFileInputValid) {
      this.tripAppointment.documentId = this.fileToken;
      this.tripAppointment.documentName = this.fileName;
      this.tripAppointment.documentContentType = this.fileType;
    }
    console.log('tripClearance', this.tripClearance);
    console.log('tripAppointment', this.tripAppointment);
    this.tripAppointment.appointmentDateTime = moment(this.tripAppointment.appointmentDateTime);
    this.tripAppointment.shippingRequestId = this._PointsService.currentShippingRequest.shippingRequest.id;
    this.tripClearance.shippingRequestId = this._PointsService.currentShippingRequest.shippingRequest.id;
    this.saved.emit({ tripAppointment: this.tripAppointment, tripClearance: this.tripClearance });
    this.close();
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
        // this.trip.createOrEditDocumentFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
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

  downloadAttatchment(): void {
    if (this.isEdit) {
      this._shippingRequestsTripServiceProxy.getAppointmentFile(this.pointId).subscribe((res) => {
        console.log('res');
        if (res.length > 0) {
          this._fileDownloadService.downloadFileByBinary(res[0].documentId, res[0].fileName, res[0].fileType);
        }
      });
      return;
    }
    if (!this.hasNewUpload) {
      this._fileDownloadService.downloadFileByBinary(
        this.createOrEditDocumentFileDto.binaryObjectId,
        this.createOrEditDocumentFileDto.name,
        this.createOrEditDocumentFileDto.extn
      );
    } else {
      let fileDto = new FileDto();
      fileDto.fileName = this.fileName;
      fileDto.fileToken = this.fileToken;
      fileDto.fileType = this.fileType;
      this._fileDownloadService.downloadTempFile(fileDto);
    }
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

  private getTaxVat() {
    this._PenaltiesServiceProxy.getTaxVat().subscribe((res) => {
      this.taxVat = res;
    });
  }

  calculateValues(isForAppointment: boolean) {
    console.log('calculateValues');
    if (isForAppointment) {
      this.tripAppointment.subTotalAmount = this.tripAppointment.itemPrice;
      this.tripAppointment.vatAmount = (this.tripAppointment.itemPrice * this.taxVat) / 100;
      this.tripAppointment.totalAmount = this.tripAppointment.vatAmount + this.tripAppointment.itemPrice;
      this.tripAppointment.commissionAmount = this.calculateCommission(
        this.tripAppointment.commissionType,
        this.tripAppointment.itemPrice,
        this.tripAppointment.commissionPercentageOrAddValue
      );
      this.tripAppointment.subTotalAmountWithCommission = this.tripAppointment.itemPrice + this.tripAppointment.commissionAmount;
      this.tripAppointment.vatAmountWithCommission = (this.tripAppointment.subTotalAmountWithCommission * this.taxVat) / 100;
      this.tripAppointment.totalAmountWithCommission =
        this.tripAppointment.subTotalAmountWithCommission + this.tripAppointment.vatAmountWithCommission;
      return;
    }
    this.tripClearance.subTotalAmount = this.tripClearance.itemPrice;
    this.tripClearance.vatAmount = (this.tripClearance.itemPrice * this.taxVat) / 100;
    this.tripClearance.totalAmount = this.tripClearance.vatAmount + this.tripClearance.itemPrice;
    this.tripClearance.commissionAmount = this.calculateCommission(
      this.tripClearance.commissionType,
      this.tripClearance.itemPrice,
      this.tripClearance.commissionPercentageOrAddValue
    );
    this.tripClearance.subTotalAmountWithCommission = this.tripClearance.itemPrice + this.tripClearance.commissionAmount;
    this.tripClearance.vatAmountWithCommission = (this.tripClearance.subTotalAmountWithCommission * this.taxVat) / 100;
    this.tripClearance.totalAmountWithCommission = this.tripClearance.subTotalAmountWithCommission + this.tripClearance.vatAmountWithCommission;
  }

  calculateCommission(commissionType: PriceOfferCommissionType, itemPrice: number, commissionPercentageOrAddValue: number) {
    switch (Number(commissionType)) {
      case PriceOfferCommissionType.CommissionPercentage:
        return (itemPrice * commissionPercentageOrAddValue) / 100;

      case PriceOfferCommissionType.CommissionMinimumValue:
      case PriceOfferCommissionType.CommissionValue:
        return commissionPercentageOrAddValue;

      default:
        return 0;
    }
  }

  shouldDisable() {
    const isClearanceNotValid =
      !this.tripClearance?.itemPrice ||
      this.tripClearance?.itemPrice?.toString()?.length === 0 ||
      !this.tripClearance?.commissionType ||
      !this.tripClearance?.commissionPercentageOrAddValue ||
      this.tripClearance?.commissionPercentageOrAddValue?.toString()?.length === 0;
    const isAppointmentNotValid =
      !this.tripAppointment.itemPrice ||
      this.tripAppointment?.itemPrice?.toString()?.length === 0 ||
      !this.tripAppointment?.commissionType ||
      !this.tripAppointment?.commissionPercentageOrAddValue ||
      this.tripAppointment?.commissionPercentageOrAddValue?.toString()?.length === 0;
    if (this.needsClearance && this.needsAppointment) {
      return isClearanceNotValid || isAppointmentNotValid;
    }
    if (this.needsClearance && !this.needsAppointment) {
      return isClearanceNotValid;
    }
    return isAppointmentNotValid;
  }

  ngOnDestroy(): void {}
}
