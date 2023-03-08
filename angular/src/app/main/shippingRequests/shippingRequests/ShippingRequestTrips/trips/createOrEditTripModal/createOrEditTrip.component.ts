/* tslint:disable:triple-equals curly */
// noinspection ES6ConvertVarToLetConst

import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditActorCarrierPrice,
  CreateOrEditActorShipperPriceDto,
  CreateOrEditDocumentFileDto,
  CreateOrEditRoutPointDto,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditShippingRequestTripVasDto,
  DedicatedShippingRequestsServiceProxy,
  DropPaymentMethod,
  EntityTemplateServiceProxy,
  FileDto,
  GetAllDedicatedDriversOrTrucksForDropDownDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GetAllTrucksWithDriversListDto,
  GetShippingRequestForViewOutput,
  GetShippingRequestVasForViewDto,
  GoodsDetailsServiceProxy,
  PickingType,
  SavedEntityType,
  SelectItemDto,
  ShippingRequestDto,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripFlag,
  UpdateDocumentFileInput,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { PointsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.component';
import Swal from 'sweetalert2';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { FormControl, NgForm } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { Subscription } from 'rxjs';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'AddNewTripModal',
  styleUrls: ['./createOrEditTrip.component.css'],
  templateUrl: './createOrEditTrip.component.html',
  providers: [DateFormatterService],
})
export class CreateOrEditTripComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('shippingRequestTripsForm') shippingRequestTripsForm: NgForm;
  @ViewChild('addNewTripsModal', { static: true }) modal: ModalDirective;
  @ViewChild('PointsComponent') PointsComponent: PointsComponent;
  @ViewChild('userForm', { static: false }) userForm: NgForm;

  @Input() shippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];
  @Input() parentForm: NgForm;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  startTripdate: any;
  endTripdate: any;
  endTripDate = new FormControl('');
  minGreg: NgbDateStruct = { day: 1, month: 1, year: 1900 };
  minHijri: NgbDateStruct = { day: 1, month: 1, year: 1342 };
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  minHijriTripdate: NgbDateStruct;
  minGrogTripdate: NgbDateStruct;
  minTripDateAsGrorg: NgbDateStruct;
  minTripDateAsHijri: NgbDateStruct;
  maxTripDateAsGrorg: NgbDateStruct;
  maxTripDateAsHijri: NgbDateStruct;

  //trip = new CreateOrEditShippingRequestTripDto();
  saving = false;
  loading = false;
  active = false;
  //activeTripId: number = undefined;
  cleanVasesList: CreateOrEditShippingRequestTripVasDto[] = [];
  isApproximateValueRequired = false;
  alldocumentsValid = false;
  public DocsUploader: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  fileToken: string;
  fileType: string;
  fileName: string;
  hasNewUpload: boolean;
  isDisabledTruck = false;
  isDisabledDriver = false;
  IsHaveSealNumberValue: any = '';
  IsHaveContainerNumberValue: any = '';
  ShippingRequestFlagEnum = ShippingRequestFlag;
  ShippingRequestTripFlagEnum = ShippingRequestTripFlag;
  ShippingRequestTripFlagArray = [];
  paymentMethodsArray = [];
  selectedPaymentMethod: number;

  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  templatesLoading: boolean;
  tripTemples: SelectItemDto[];
  SavedEntityType = SavedEntityType;
  private PickingType = PickingType;
  //private tripServiceShippingRequestSub: Subscription;
  private getTripForEditSub: Subscription;
  RouteTypesEnum = ShippingRequestRouteType;

  TripsServiceSubscription: any;
  wayBillIsDownloading: boolean;
  selectedTemplate: number;
  isTripPointsInvalid = false;
  routeTypes: any[] = [];
  canEditNumberOfDrops = true;
  numberOfDrops: number;
  allDrivers: any;
  allTrucks: GetAllTrucksWithDriversListDto[];
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];

  get isFileInputValid() {
    return this._TripService.CreateOrEditShippingRequestTripDto.hasAttachment
      ? this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.name
        ? true
        : false
      : true;
  }

  get tripAsJson(): string {
    if (this._TripService.CreateOrEditShippingRequestTripDto) {
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestId = this.shippingRequest.id;
    }
    return JSON.stringify(this._TripService.CreateOrEditShippingRequestTripDto);
  }

  callbacks: any[] = [];

  adapterConfig = {
    getValue: () => {
      return this.validatePointsFromPointsComponent();
    },
    applyValidationResults: (e) => {
      this.isTripPointsInvalid = !e.isValid;
    },
    validationRequestsCallbacks: this.callbacks,
  };
  isFormSubmitted = false;
  allDedicatedDrivers: GetAllDedicatedDriversOrTrucksForDropDownDto[] = [];
  allDedicatedTrucks: GetAllDedicatedDriversOrTrucksForDropDownDto[] = [];
  //shippingRequestForView: GetShippingRequestForViewOutput = null;
  selectedTripType;
  AllActorsShippers: SelectItemDto[];
  AllActorsCarriers: SelectItemDto[];

  constructor(
    injector: Injector,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy,
    public _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private cdref: ChangeDetectorRef,
    public _TripService: TripService,
    private _PointsService: PointsService,
    private _tokenService: TokenService,
    private _templates: EntityTemplateServiceProxy,
    private enumToArray: EnumToArrayPipe,
    private _dedicatedShippingRequestService: DedicatedShippingRequestsServiceProxy,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    //link the trip from the shared service to the this component
    //this.TripsServiceSubscription = this._TripService.currentActiveTrip.subscribe((res) => (this.trip = res));
    //this.trip = this._TripService.CreateOrEditShippingRequestTripDto;
    this.paymentMethodsArray = this.enumToArray.transform(DropPaymentMethod);
    //Take The Points List From the Points Shared Service
    // this.PointsServiceSubscription = this._PointsService.currentWayPointsList.subscribe((res) => (this._TripService.CreateOrEditShippingRequestTripDto.routPoints = res));
    this.vasesHandler();
    // this.ShippingRequestTripFlagEnum = Object.values(ShippingRequestTripFlag);
  }

  /**
   * takes the Vas List From the Shipping Request And Cleans them to use them in Trips Modal
   */
  vasesHandler() {
    if (this.VasListFromFather) {
      this.VasListFromFather.forEach((x) => {
        //Get the Vase List From Father And Attach Them to new Array
        const vas: CreateOrEditShippingRequestTripVasDto = new CreateOrEditShippingRequestTripVasDto();
        vas.id = undefined; // vas id in shipping Request trip (Required for edit trip)
        vas.shippingRequestTripId = this._TripService.CreateOrEditShippingRequestTripDto.id || undefined; //the trip id
        vas.shippingRequestVasId = x.shippingRequestVas.id; //vas id in shipping request
        vas.name = x.vasName; //vas Name
        this.cleanVasesList.push(vas);
      });
    }
  }

  show(record?: CreateOrEditShippingRequestTripDto, shippingRequestForView?: GetShippingRequestForViewOutput): void {
    this._TripService.GetShippingRequestForViewOutput = shippingRequestForView;
    //console.log(this._TripService.GetShippingRequestForViewOutput.toJSON());
    //console.log(shippingRequestForView.toJSON());
    // this._TripService.CreateOrEditShippingRequestTripDto.actorShipperPrice = new CreateOrEditActorShipperPriceDto();
    // this._TripService.CreateOrEditShippingRequestTripDto.actorCarrierPrice = new CreateOrEditActorCarrierPrice();

    if (shippingRequestForView?.shippingRequestFlag === 1) {
      this.getAllDedicatedDriversForDropDown();
      this.getAllDedicateTrucksForDropDown();
      this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType);
    }
    if (!shippingRequestForView) {
      //console.log('!shippingRequestForView');
      this.getAllDrivers();
      this.getAllTrucks(undefined);
      this.getAllGoodCategories();
      this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType);
      this.getActors();
    }
    if (this.shippingRequest) {
      this.setStartTripDate(this.shippingRequest.startTripDate);
      let endDate: moment.Moment;
      if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === this.ShippingRequestFlagEnum.Normal) {
        endDate = this.shippingRequest.endTripDate;
      } else {
        endDate = this._TripService.GetShippingRequestForViewOutput?.rentalEndDate;
      }

      if (!this._TripService.GetShippingRequestForViewOutput) {
        const EndDateGregorian = moment().add(3, 'years').locale('en').format('D/M/YYYY');
        this.maxTripDateAsGrorg = this.dateFormatterService.ToGregorianDateStruct(EndDateGregorian, 'D/M/YYYY');
        this.maxTripDateAsHijri = this.dateFormatterService.ToHijriDateStruct(EndDateGregorian, 'D/M/YYYY');
      } else {
        const EndDateGregorian = moment(endDate).locale('en').format('D/M/YYYY');
        this.maxTripDateAsGrorg = this.dateFormatterService.ToGregorianDateStruct(EndDateGregorian, 'D/M/YYYY');
        this.maxTripDateAsHijri = this.dateFormatterService.ToHijriDateStruct(EndDateGregorian, 'D/M/YYYY');
      }
    }
    if (record) {
      // this.activeTripId = record.id;

      this._TripService.CreateOrEditShippingRequestTripDto.id = record.id;
      this._TripService.activeTripId = record.id;

      this.loading = true;
      this.getTripForEditSub = this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this._TripService.CreateOrEditShippingRequestTripDto = res;
        if (this._TripService.CreateOrEditShippingRequestTripDto.shipperActorId) {
          (this._TripService.CreateOrEditShippingRequestTripDto.shipperActorId as any) = res.shipperActorId.toString();
        }
        if (this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId) {
          (this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId as any) = res.carrierActorId.toString();
        }

        this.IsHaveSealNumberValue = res.sealNumber?.length > 0;
        this.IsHaveContainerNumberValue = res.containerNumber?.length > 0;
        //console.log('res', res.containerNumber);
        const gregorian = moment(res.startTripDate).locale('en').format('D/M/YYYY');
        this.startTripdate = this.dateFormatterService.ToGregorianDateStruct(gregorian, 'D/M/YYYY');

        this.PointsComponent.wayPointsList = this._TripService.CreateOrEditShippingRequestTripDto.routPoints;
        this.PointsComponent.loadReceivers(null, true);
        // this._PointsService.updateWayPoints(this._TripService.CreateOrEditShippingRequestTripDto.routPoints);
        //this.startTripdate = this.dateFormatterService.MomentToNgbDateStruct(res.startTripDate);
        if (res.endTripDate != null) {
          this.endTripdate = this.dateFormatterService.MomentToNgbDateStruct(res.endTripDate);
        }
        this._PointsService.updateWayPoints(this._TripService.CreateOrEditShippingRequestTripDto.routPoints);
        if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === 1) {
          this.canEditNumberOfDrops = false;
          (this._TripService.CreateOrEditShippingRequestTripDto.routeType as any) =
            '' + this._TripService.CreateOrEditShippingRequestTripDto.routeType;
          (this._TripService.CreateOrEditShippingRequestTripDto.driverUserId as any) =
            '' + this._TripService.CreateOrEditShippingRequestTripDto.driverUserId;
          (this._TripService.CreateOrEditShippingRequestTripDto.truckId as any) = '' + this._TripService.CreateOrEditShippingRequestTripDto.truckId;
        }
        this.loading = false;
      });
    } else {
      this.loading = true;
      this._shippingRequestTripsService.getShippingRequestTripForCreate().subscribe((result) => {
        this._TripService.CreateOrEditShippingRequestTripDto = result;
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.extn = '_';
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.name = '_';
        this._TripService.CreateOrEditShippingRequestTripDto.actorShipperPrice = new CreateOrEditActorShipperPriceDto();
        this._TripService.CreateOrEditShippingRequestTripDto.actorCarrierPrice = new CreateOrEditActorCarrierPrice();

        if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === 1) {
          this._TripService.CreateOrEditShippingRequestTripDto.routeType = this.RouteTypesEnum.SingleDrop;
          this.onRouteTypeChange();
        }
        this.loading = false;
      });

      if (this._TripService.CreateOrEditShippingRequestTripDto) {
        this._TripService.CreateOrEditShippingRequestTripDto.id = null;
      }

      this.setStartTripDate(this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.startTripDate);
    }
    this._PointsService.updateCurrentUsedIn('createOrEdit');
    // this.PointsComponent.createEmptyPoints();
    this.active = true;
    this.modal.show();
    this.initDocsUploader();
    this.cdref.detectChanges();
  }

  setStartTripDate(startTripDate) {
    if (this.isTachyonDealer && this._TripService.GetShippingRequestForViewOutput.shippingRequestFlag === ShippingRequestFlag.Dedicated) {
      startTripDate = this._TripService.GetShippingRequestForViewOutput.rentalStartDate;
    }
    const todayGregorian = moment(startTripDate).locale('en').format('D/M/YYYY');
    this.minTripDateAsGrorg = this.dateFormatterService.ToGregorianDateStruct(todayGregorian, 'D/M/YYYY');
    this.minTripDateAsHijri = this.dateFormatterService.ToHijriDateStruct(todayGregorian, 'D/M/YYYY');
    this.startTripdate = this.minTripDateAsGrorg;
    this.minHijriTripdate = this.minTripDateAsHijri;
    this.minGrogTripdate = this.minTripDateAsGrorg;
  }

  close(): void {
    //this.loading = true;
    this.canEditNumberOfDrops = true;
    this.active = false;
    this.isFormSubmitted = false;
    this.modal.hide();
    this._TripService.CreateOrEditShippingRequestTripDto = new CreateOrEditShippingRequestTripDto();
    this.fileToken = undefined;
    this.hasNewUpload = undefined;
    this._TripService.currentSourceFacility = null;
    this._TripService.destFacility = null;
    this._TripService.CreateOrEditShippingRequestTripDto.routPoints = [];
    if (isNotNullOrUndefined(this.PointsComponent)) {
      this.PointsComponent.wayPointsList = this._TripService.CreateOrEditShippingRequestTripDto.routPoints;
    }
    this._PointsService.updateWayPoints([]);

    this.allDedicatedDrivers = [];
    this.allDedicatedTrucks = [];
    this.isDisabledTruck = false;
    this.isDisabledDriver = false;
    this.IsHaveContainerNumberValue = false;
    this.IsHaveSealNumberValue = false;
  }

  createOrEditTrip() {
    this.isFormSubmitted = true;
    this.revalidatePointsFromPointsComponent();
    //if there is a Validation issue in the Points do Not Proceed
    if (
      isNotNullOrUndefined(this._TripService.GetShippingRequestForViewOutput) &&
      this._TripService.GetShippingRequestForViewOutput.shippingRequestFlag === 1
    ) {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops =
        Number(this._TripService.CreateOrEditShippingRequestTripDto.routeType) === this.RouteTypesEnum.SingleDrop
          ? 1
          : this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops;
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestId = this.shippingRequest.id;
    }
    this._TripService.CreateOrEditShippingRequestTripDto.routPoints = this.PointsComponent.wayPointsList;
    this._TripService.CreateOrEditShippingRequestTripDto.originFacilityId =
      this._TripService.CreateOrEditShippingRequestTripDto.routPoints[0].facilityId;
    this._TripService.CreateOrEditShippingRequestTripDto.destinationFacilityId =
      this._TripService.CreateOrEditShippingRequestTripDto.routPoints[
        this._TripService.CreateOrEditShippingRequestTripDto.routPoints.length - 1
      ].facilityId;
    if (this.validatePointsBeforeAddTrip()) {
      this.saving = true;
      if (!this._TripService.CreateOrEditShippingRequestTripDto.hasAttachment) {
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto = null;
      }
      this.GetSelectedstartDateChange(this.startTripdate, 'start');
      if (this.endTripdate !== undefined) {
        this.GetSelectedstartDateChange(this.endTripdate, 'end');
      }

      this._shippingRequestTripsService
        .createOrEdit(this._TripService.CreateOrEditShippingRequestTripDto)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.close();
          this.modalSave.emit(null);
          this.notify.info(this.l('SuccessfullySaved'));
          abp.event.trigger('ShippingRequestTripCreatedEvent');
          this.isDisabledTruck = false;
          this.isDisabledDriver = false;
        });
    }
  }

  GetSelectedstartDateChange($event: NgbDateStruct, type) {
    if (type == 'start') {
      this.startTripdate = $event;
      if ($event != null && $event.year < 1900) {
        this.minHijriTripdate = $event;
      } else {
        this.minGrogTripdate = $event;
      }
    }
    if (type == 'end') this.endTripdate = $event;

    let startDate = this.dateFormatterService.NgbDateStructToMoment(this.startTripdate);
    let endDate = this.dateFormatterService.NgbDateStructToMoment(this.endTripdate);

    if (this.startTripdate != null && this.startTripdate != undefined)
      this._TripService.CreateOrEditShippingRequestTripDto.startTripDate = this.GetGregorianAndhijriFromDatepickerChange(
        this.startTripdate
      ).GregorianDate;

    this._TripService.CreateOrEditShippingRequestTripDto.startTripDate == null
      ? (this._TripService.CreateOrEditShippingRequestTripDto.startTripDate = moment(new Date()))
      : null;

    if (this.endTripdate != null && this.endTripdate != undefined)
      this._TripService.CreateOrEditShippingRequestTripDto.endTripDate = this.GetGregorianAndhijriFromDatepickerChange(
        this.endTripdate
      ).GregorianDate;

    //checks if the trips end date is less than trips start date
    if (startDate != undefined && endDate != undefined) {
      if (endDate < startDate) this._TripService.CreateOrEditShippingRequestTripDto.endTripDate = this.endTripdate = undefined;
    }
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

  /**
   * Downloads Single DropWaybill
   * @param tripId
   */
  DownloadSingleDropWaybillPdf(tripId: number): void {
    this.wayBillIsDownloading = true;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(tripId).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.wayBillIsDownloading = false;
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
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({
          fileToken: resp.result.fileToken,
        });
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
    if (this._TripService.CreateOrEditShippingRequestTripDto.id && !this.hasNewUpload) {
      this._fileDownloadService.downloadFileByBinary(
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.binaryObjectId,
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.name,
        this._TripService.CreateOrEditShippingRequestTripDto.createOrEditDocumentFileDto.extn
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
  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }

  /**
   * checks if Selected Vases in trip section has Insurance selected
   * because the Goods Approximate Value input is only required if the Selected vases has insurance
   * @param $event
   */
  isSelectedVasesHasinsurance($event: CreateOrEditShippingRequestTripVasDto[]) {
    $event.find((x) => x.name == 'Insurance') ? (this.isApproximateValueRequired = true) : (this.isApproximateValueRequired = false);
  }

  ngOnDestroy() {
    this._TripService.CreateOrEditShippingRequestTripDto = undefined;
    this._TripService.GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();

    //this.TripsServiceSubscription.unsubscribe();
    // this.tripServiceShippingRequestSub?.unsubscribe();
    // this.tripServiceShippingRequestSub?.unsubscribe();
    this.getTripForEditSub?.unsubscribe();
    this.docProgress = undefined;

    //console.log('Detsroid From Create/Edit Trip');
  }

  /**
   * Validates Shipping Request Trip Before Create/Edit
   * @private
   */
  private validatePointsBeforeAddTrip() {
    let isSingleDropTrip = this.shippingRequest.routeTypeId === this.RouteTypesEnum.SingleDrop;
    let isFacilitiesTheSame =
      isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints) &&
      isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints[1]) &&
      this._TripService.CreateOrEditShippingRequestTripDto.routPoints[0].facilityId ===
        this._TripService.CreateOrEditShippingRequestTripDto.routPoints[1].facilityId;
    let isFacilitiesEmpty =
      isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints) &&
      !isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints[0].facilityId) &&
      isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints[1]) &&
      !isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints[1].facilityId);
    if (
      isSingleDropTrip &&
      isFacilitiesEmpty &&
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal
    ) {
      Swal.fire(this.l('ValidationError'), this.l('SourcePickUpFacilityOrDropOffFacilityCantBeEmpty'), 'error');
      return false;
    }
    if (isSingleDropTrip && isFacilitiesTheSame) {
      Swal.fire(this.l('ValidationError'), this.l('SourcePickUpFacilityAndDropOffFacilityCantBeTheSame'), 'error');
      return false;
    }
    if (!isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints)) {
      return false;
    }
    //trip Details Validation
    for (const point of this._TripService.CreateOrEditShippingRequestTripDto.routPoints) {
      const isFacilityEmpty = !isNotNullOrUndefined(point.facilityId) || ('' + point.facilityId).length === 0;
      const isReceiverEmpty =
        (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
          (!isNotNullOrUndefined(point.receiverId) || ('' + point.receiverId).length === 0)) ||
        (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery &&
          (!isNotNullOrUndefined(point.receiverFullName) || point.receiverFullName.length === 0) &&
          (!isNotNullOrUndefined(point.receiverPhoneNumber) || point.receiverPhoneNumber.length === 0) &&
          (!isNotNullOrUndefined(point.receiverId) || ('' + point.receiverId).length === 0));
      if (point.pickingType === this.PickingType.Pickup && (isFacilityEmpty || isReceiverEmpty)) {
        Swal.fire(this.l('IncompleteTripPoint'), this.l('PleaseCompletePicKupPointDetails'), 'warning');
        return false;
        break;
      }
      if (point.pickingType === this.PickingType.Dropoff) {
        if (
          isFacilityEmpty ||
          isReceiverEmpty ||
          (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
            !isNotNullOrUndefined(point.goodsDetailListDto as any))
        ) {
          Swal.fire(this.l('IncompleteTripPoint'), this.l('PleaseCompleteAllDropPointDetails'), 'warning');
          return false;
          break;
        }
      }
    }
    return true;
  }

  private validatePointsFromPointsComponent() {
    if (!isNotNullOrUndefined(this.PointsComponent) || !isNotNullOrUndefined(this.PointsComponent.wayPointsList)) {
      return false;
    }
    for (const point of this.PointsComponent.wayPointsList) {
      const isFacilityEmpty = !isNotNullOrUndefined(point.facilityId) || ('' + point.facilityId).length === 0;
      const isReceiverEmpty =
        (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
          (!isNotNullOrUndefined(point.receiverId) || ('' + point.receiverId).length === 0)) ||
        (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery &&
          (!isNotNullOrUndefined(point.receiverFullName) || point.receiverFullName.length === 0) &&
          (!isNotNullOrUndefined(point.receiverPhoneNumber) || point.receiverPhoneNumber.length === 0) &&
          (!isNotNullOrUndefined(point.receiverId) || ('' + point.receiverId).length === 0));
      if (point.pickingType === this.PickingType.Pickup && (isFacilityEmpty || isReceiverEmpty)) {
        return false;
      }
      if (point.pickingType === this.PickingType.Dropoff) {
        if (
          isFacilityEmpty ||
          isReceiverEmpty ||
          (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
            (!isNotNullOrUndefined(point.goodsDetailListDto as any) || point.goodsDetailListDto.length === 0))
        ) {
          return false;
        }
      }
    }
    return true;
  }

  /**
   * Validates Shipping Request Trip Before Create Template
   * @private
   */
  public CanCreateTemplate(): boolean {
    //if there is no routePoints
    if (!isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.routPoints)) {
      return false;
    } else if (
      this._TripService.CreateOrEditShippingRequestTripDto.routPoints.find(
        (x) => x.pickingType == PickingType.Dropoff && !isNotNullOrUndefined(x.goodsDetailListDto)
      )
    ) {
      return false;
    } else if (this._TripService.CreateOrEditShippingRequestTripDto.routPoints.length < this.shippingRequest.numberOfDrops + 1) {
      return false;
    } else {
      return true;
    }
  }
  /**
   * load Trip Templates For Drop Down
   */
  loadTemplates() {
    this.templatesLoading = true;
    this._templates.getAllForDropdown(this.SavedEntityType.TripTemplate, this.shippingRequest.id.toString()).subscribe((res) => {
      this.templatesLoading = false;
      this.tripTemples = res;
    });
  }

  /**
   * apply Selected Template to the Trip
   */
  applyTemplate() {
    let jsonObject = null;
    this._templates.getForView(this.selectedTemplate).subscribe((res) => {
      jsonObject = JSON.parse(res.savedEntity);
      this._TripService.CreateOrEditShippingRequestTripDto = jsonObject;
      this.removeIdsFromTripTemplate(this._TripService.CreateOrEditShippingRequestTripDto);
      // this.loadReceivers(this._TripService.CreateOrEditShippingRequestTripDto.originFacilityId);
      this._PointsService.updateWayPoints(this._TripService.CreateOrEditShippingRequestTripDto.routPoints);
      this.PointsComponent.loadReceivers(null, true);
    });
  }

  private removeIdsFromTripTemplate(trip: CreateOrEditShippingRequestTripDto) {
    this._TripService.CreateOrEditShippingRequestTripDto.id = undefined;
    this._TripService.CreateOrEditShippingRequestTripDto.routPoints.map((x) => {
      x.goodsDetailListDto?.map((y) => {
        return (y.id = undefined);
      });
      return (x.id = undefined);
    });
    return this._TripService.CreateOrEditShippingRequestTripDto;
  }

  revalidatePointsFromPointsComponent() {
    this.callbacks.forEach((func) => {
      func();
    });
  }

  private getAllDedicatedDriversForDropDown() {
    if (
      this._TripService.GetShippingRequestForViewOutput?.shippingRequestFlag == ShippingRequestFlag.Dedicated ||
      !this._TripService.GetShippingRequestForViewOutput
    ) {
      this._dedicatedShippingRequestsServiceProxy
        .getAllDedicatedDriversForDropDown(this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.id)
        .subscribe((res) => {
          this.allDedicatedDrivers = res;
        });
    }
  }

  private getAllDedicateTrucksForDropDown() {
    if (
      this._TripService.GetShippingRequestForViewOutput?.shippingRequestFlag == ShippingRequestFlag.Dedicated ||
      !this._TripService.GetShippingRequestForViewOutput
    ) {
      this._dedicatedShippingRequestsServiceProxy
        .getAllDedicateTrucksForDropDown(this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.id)
        .subscribe((res) => {
          this.allDedicatedTrucks = res;
        });
    }
  }

  onRouteTypeChange() {
    if (Number(this._TripService.CreateOrEditShippingRequestTripDto.routeType) === this.RouteTypesEnum.MultipleDrops) {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops =
        Number(this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops) === 1
          ? 2
          : this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops;
    } else {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = 1;
    }
    if (this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery) {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = 0;
    }
    this.onNumberOfDropsChanged(true);
    //console.log('this.trip.numberOfDrops', this._TripService.CreateOrEditShippingRequestTripDto);
  }

  addAdditionalDrop() {
    if (Number(this._TripService.CreateOrEditShippingRequestTripDto.routeType) === this.RouteTypesEnum.MultipleDrops) {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops + 1;
    } else {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = 1;
    }
    this.onNumberOfDropsChanged(false, true);
    //('this.trip.numberOfDrops', this._TripService.CreateOrEditShippingRequestTripDto);
  }

  onNumberOfDropsChanged(shouldReset = false, shouldAddNewPoint = false) {
    if (this._TripService.GetShippingRequestForViewOutput) {
      this._TripService.GetShippingRequestForViewOutput.shippingRequest.numberOfDrops =
        this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops;
    }

    if (
      this.canEditNumberOfDrops ||
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery
    ) {
      const anyWayPointHasId = this.PointsComponent.wayPointsList.some((item) => isNotNullOrUndefined(item.id));
      //console.log('anyWayPointHasId', anyWayPointHasId);
      const wayPointsList =
        (this.PointsComponent.wayPointsList.length > 0 && !shouldReset) || anyWayPointHasId
          ? this._TripService.CreateOrEditShippingRequestTripDto.routeType == this.RouteTypesEnum.MultipleDrops
            ? [...this.PointsComponent.wayPointsList]
            : this.PointsComponent.wayPointsList.length >= 2
            ? [this.PointsComponent.wayPointsList[0], this.PointsComponent.wayPointsList[1]]
            : [this.PointsComponent.wayPointsList[0]]
          : [];
      this.PointsComponent.wayPointsList = [];
      this._PointsService.updateWayPoints([]);
      //console.log('wayPointsList', wayPointsList);
      if ((wayPointsList.length > 0 && !shouldReset) || anyWayPointHasId) {
        this.PointsComponent.wayPointsList = wayPointsList;
        this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops =
          this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops === 0
            ? wayPointsList.length - 1
            : this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops;
        this._PointsService.updateWayPoints(wayPointsList);
        if (shouldAddNewPoint) {
          this.PointsComponent.wayPointsList = this.addNewPoint(wayPointsList);
        }
      } else {
        this.PointsComponent.createEmptyPoints(this.selectedPaymentMethod);
      }
      this._PointsService.updateWayPoints(this.PointsComponent.wayPointsList);
    }
  }

  GetTruckOrDriver(driverId?: number, truckId?: number) {
    this._dedicatedShippingRequestsServiceProxy
      .getDriverOrTruckForTripAssign(truckId, driverId, this._TripService.GetShippingRequestForViewOutput.shippingRequest.id)
      .subscribe((res) => {
        if (driverId != null && res != null) {
          this._TripService.CreateOrEditShippingRequestTripDto.truckId = res;
          this.isDisabledTruck = true;
        } else if (truckId != null && res != null) {
          this._TripService.CreateOrEditShippingRequestTripDto.driverUserId = res;
          this.isDisabledDriver = true;
        }
      });
  }

  addNewPoint(wayPointsList: CreateOrEditRoutPointDto[]): CreateOrEditRoutPointDto[] {
    if (!isNotNullOrUndefined(wayPointsList)) {
      wayPointsList = [];
    }
    let point = new CreateOrEditRoutPointDto();
    //pickup Point
    if (wayPointsList.length === 0) {
      point.pickingType = this.PickingType.Pickup;
    } else {
      point.pickingType = this.PickingType.Dropoff;
    }
    point.dropPaymentMethod = this.selectedPaymentMethod;
    point.needsPOD = false;
    point.needsReceiverCode = false;
    wayPointsList.push(point);
    return wayPointsList;
  }

  /**
   * Driver Assignation Section
   * this method is for Getting All Carriers Drivers For DD
   */
  getAllDrivers() {
    this._dedicatedShippingRequestService.getAllDriversForDropDown(undefined).subscribe((res) => {
      this.allDrivers = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks(truckTypeId) {
    this._dedicatedShippingRequestService.getAllTrucksWithDriversList(truckTypeId, undefined).subscribe((res) => {
      this.allTrucks = res;
    });
  }

  getAllGoodCategories() {
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });
  }

  getActors() {
    this._shippingRequestsServiceProxy.getAllCarriersActorsForDropDown().subscribe((result) => {
      this.AllActorsCarriers = result;
      // this.AllActorsCarriers.unshift( SelectItemDto.fromJS({id: null, displayName: this.l('Myself'), isOther: false}));
    });

    this._shippingRequestsServiceProxy.getAllShippersActorsForDropDown().subscribe((result) => {
      this.AllActorsShippers = result;
      // this.AllActorsShippers.unshift( SelectItemDto.fromJS({id: null, displayName: this.l('Myself'), isOther: false}));
    });
  }

  calculatePrices(dto: CreateOrEditActorShipperPriceDto) {
    if (dto) {
      dto.vatAmountWithCommission = dto.subTotalAmountWithCommission * 0.15;
      dto.totalAmountWithCommission = dto.vatAmountWithCommission + dto.subTotalAmountWithCommission;
    }
  }

  calculateCarrierPrices(dto: CreateOrEditActorCarrierPrice) {
    if (dto) {
      dto.vatAmount = dto.subTotalAmount * 0.15;
    }
  }
}
