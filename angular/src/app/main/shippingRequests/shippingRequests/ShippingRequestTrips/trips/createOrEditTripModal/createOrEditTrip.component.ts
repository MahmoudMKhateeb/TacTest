import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditDocumentFileDto,
  CreateOrEditRoutPointDto,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditShippingRequestTripVasDto,
  EntityTemplateServiceProxy,
  FacilityForDropdownDto,
  GetShippingRequestVasForViewDto,
  PickingType,
  ReceiverFacilityLookupTableDto,
  ReceiversServiceProxy,
  RoutStepsServiceProxy,
  SavedEntityType,
  SelectItemDto,
  ShippingRequestDto,
  ShippingRequestRouteType,
  ShippingRequestsTripServiceProxy,
  UpdateDocumentFileInput,
  WaybillsServiceProxy,
  FileDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { PointsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.component';
import Swal from 'sweetalert2';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { FormControl, NgForm, Validators } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import * as moment from 'moment';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
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
  @ViewChild('createOrEditFacilityModal') createOrEditFacilityModal: CreateOrEditFacilityModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() shippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];
  startTripdate: any;
  endTripdate: any;
  minTripDate: any;
  tripStartDate = new FormControl('', Validators.required);
  endTripDate = new FormControl('');
  selectedDateType: DateType = DateType.Hijri; // or DateType.Gregorian
  @Input() parentForm: NgForm;
  @ViewChild('userForm', { static: false }) userForm: NgForm;
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

  trip = new CreateOrEditShippingRequestTripDto();
  facilityLoading = false;
  saving = false;
  loading = true;
  active = false;
  activeTripId: number = undefined;
  cleanVasesList: CreateOrEditShippingRequestTripVasDto[] = [];
  isApproximateValueRequired = false;
  RouteTypes = ShippingRequestRouteType;

  //documentFile: CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto();
  alldocumentsValid = false;
  public DocsUploader: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  fileToken: string;
  fileType: string;
  fileName: string;
  hasNewUpload: boolean;

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
  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    public _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private cdref: ChangeDetectorRef,
    public _TripService: TripService,
    private _PointsService: PointsService,
    private _tokenService: TokenService,
    private _receiversServiceProxy: ReceiversServiceProxy,
    private _templates: EntityTemplateServiceProxy
  ) {
    super(injector);
  }

  TripsServiceSubscription: any;
  PointsServiceSubscription: any;
  wayBillIsDownloading: boolean;
  receiversLoading: boolean;
  allReceivers: ReceiverFacilityLookupTableDto[];
  pickupPointSenderId: number;
  selectedTemplate: number;

  get isFileInputValid() {
    return this.trip.hasAttachment ? (this.trip.createOrEditDocumentFileDto.name ? true : false) : true;
  }
  get tripAsJson(): string {
    return JSON.stringify(this.trip);
  }
  ngOnInit() {
    //link the trip from the shared service to the this component
    this.TripsServiceSubscription = this._TripService.currentActiveTrip.subscribe((res) => (this.trip = res));
    //Take The Points List From the Points Shared Service
    this.PointsServiceSubscription = this._PointsService.currentWayPointsList.subscribe((res) => (this.trip.routPoints = res));
    //load the Facilites
    //this._PointsService.updateWayPoints(new CreateOrEditRoutPointDto[]);
    this.refreshOrGetFacilities(undefined);
    this.vasesHandler();
  }

  /**
   * Validate Trip Facilitites
   */
  ValidateTripFacilities() {
    //prevent the user from selecting same facilty
    if (this.trip.originFacilityId == this.trip.destinationFacilityId) {
      this.shippingRequestTripsForm.controls['sourceFacility'].setErrors({ invalid: true });
      this.shippingRequestTripsForm.controls['destFacility'].setErrors({ invalid: true });
    } else {
      this.shippingRequestTripsForm.controls['sourceFacility'].setErrors(null);
      this.shippingRequestTripsForm.controls['destFacility'].setErrors(null);
    }
  }
  /**
   * update Shared Service Trip and SingleDrop Validation
   */
  SyncFacilitiesWithService() {
    this._TripService.updateSourceFacility(this.trip.originFacilityId);
    this._TripService.updateDestFacility(this.trip.destinationFacilityId);
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
        vas.shippingRequestTripId = this.activeTripId || undefined; //the trip id
        vas.shippingRequestVasId = x.shippingRequestVas.id; //vas id in shipping request
        vas.name = x.vasName; //vas Name
        this.cleanVasesList.push(vas);
      });
    }
  }

  show(record?: CreateOrEditShippingRequestTripDto): void {
    if (this.shippingRequest) {
      this.setStartTripDate(this.shippingRequest.startTripDate);
      const EndDateGregorian = moment(this.shippingRequest.endTripDate).locale('en').format('D/M/YYYY');
      this.maxTripDateAsGrorg = this.dateFormatterService.ToGregorianDateStruct(EndDateGregorian, 'D/M/YYYY');
      this.maxTripDateAsHijri = this.dateFormatterService.ToHijriDateStruct(EndDateGregorian, 'D/M/YYYY');
    }
    if (record) {
      this.activeTripId = record.id;
      this._TripService.updateActiveTripId(this.activeTripId);
      this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this.trip = res;
        //this.startTripdate = this.dateFormatterService.MomentToNgbDateStruct(res.startTripDate);
        if (res.endTripDate != null && res.endTripDate != undefined)
          this.endTripdate = this.dateFormatterService.MomentToNgbDateStruct(res.endTripDate);
        this._PointsService.updateWayPoints(this.trip.routPoints);
        this.pickupPointSenderId = res.routPoints[0]?.receiverId;
        this.loadReceivers(this.trip.originFacilityId);
        this.loading = false;
      });
    } else {
      //this is a create
      //init file document
      this._shippingRequestTripsService.getShippingRequestTripForCreate().subscribe((result) => {
        this.trip = result;
        this.trip.createOrEditDocumentFileDto.extn = '_';
        this.trip.createOrEditDocumentFileDto.name = '_';
      });

      this._TripService.updateActiveTripId(null);
      this._PointsService.updateSinglePoint(new CreateOrEditRoutPointDto());
      this._PointsService.updateWayPoints([]);
      this.loading = true;
      this._TripService.currentShippingRequest.subscribe((res) => {
        if (this.loading == true) {
          this.setStartTripDate(res.shippingRequest.startTripDate);
        }
        this.loading = false;
      });
      this.loading = false;
    }

    this.active = true;
    this.modal.show();
    this.initDocsUploader();
    this.cdref.detectChanges();
  }

  setStartTripDate(startTripDate) {
    const todayGregorian = moment(startTripDate).locale('en').format('D/M/YYYY');
    this.minTripDateAsGrorg = this.dateFormatterService.ToGregorianDateStruct(todayGregorian, 'D/M/YYYY');
    this.minTripDateAsHijri = this.dateFormatterService.ToHijriDateStruct(todayGregorian, 'D/M/YYYY');
    this.startTripdate = this.minTripDateAsGrorg;
    this.minHijriTripdate = this.minTripDateAsHijri;
    this.minGrogTripdate = this.minTripDateAsGrorg;
  }

  close(): void {
    this.loading = true;
    this.active = false;
    this.modal.hide();
    this.trip = new CreateOrEditShippingRequestTripDto();
    this.fileToken = undefined;
    this.hasNewUpload = undefined;
    this.pickupPointSenderId = undefined;
    this._TripService.updateSourceFacility(null);
    this._TripService.updateDestFacility(null);
  }

  createOrEditTrip() {
    //if there is a Validation issue in the Points do Not Proceed
    if (this.validatePointsBeforeAddTrip()) {
      this.trip.routPoints[0].receiverId = this.pickupPointSenderId;
      this.trip.shippingRequestId = this.shippingRequest.id;
      this.saving = true;
      if (!this.trip.hasAttachment) {
        this.trip.createOrEditDocumentFileDto = null;
      }
      this.GetSelectedstartDateChange(this.startTripdate, 'start');
      if (this.endTripdate !== undefined) {
        this.GetSelectedstartDateChange(this.endTripdate, 'end');
      }

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
          abp.event.trigger('ShippingRequestTripCreatedEvent');
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

    var startDate = this.dateFormatterService.NgbDateStructToMoment(this.startTripdate);
    var endDate = this.dateFormatterService.NgbDateStructToMoment(this.endTripdate);

    if (this.startTripdate != null && this.startTripdate != undefined)
      this.trip.startTripDate = this.GetGregorianAndhijriFromDatepickerChange(this.startTripdate).GregorianDate;

    this.trip.startTripDate == null ? (this.trip.startTripDate = moment(new Date())) : null;

    if (this.endTripdate != null && this.endTripdate != undefined)
      this.trip.endTripDate = this.GetGregorianAndhijriFromDatepickerChange(this.endTripdate).GregorianDate;

    //checks if the trips end date is less than trips start date
    if (startDate != undefined && endDate != undefined) {
      if (endDate < startDate) this.trip.endTripDate = this.endTripdate = undefined;
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
  refreshOrGetFacilities(facility: FacilityForDropdownDto | undefined) {
    if (this.shippingRequest != null && this.shippingRequest != undefined) {
      this._TripService.GetOrRefreshFacilities(this.shippingRequest.id);
    }
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
        this.trip.createOrEditDocumentFileDto.updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
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
    if (this.trip.id && !this.hasNewUpload) {
      this._fileDownloadService.downloadFileByBinary(
        this.trip.createOrEditDocumentFileDto.binaryObjectId,
        this.trip.createOrEditDocumentFileDto.name,
        this.trip.createOrEditDocumentFileDto.extn
      );
    } else {
      var fileDto = new FileDto();
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
    this.trip = undefined;
    this.TripsServiceSubscription.unsubscribe();
    this.PointsServiceSubscription.unsubscribe();
    this.docProgress = undefined;
    console.log('Detsroid From Create/Edit Trip');
  }

  /**
   * loads a list of Receivers by facility Id
   * @param facilityId
   */
  loadReceivers(facilityId) {
    if (facilityId) {
      this.receiversLoading = true;
      //to be Changed
      this._receiversServiceProxy.getAllReceiversByFacilityForTableDropdown(facilityId).subscribe((result) => {
        this.allReceivers = result;
        this.receiversLoading = false;
      });
    }
  }

  /**
   * Validates Shipping Request Trip Before Create/Edit
   * @private
   */
  private validatePointsBeforeAddTrip() {
    //if trip Drop Points is less than number of drops Prevent Adding Trip
    if (this.trip.routPoints.find((x) => x.pickingType == PickingType.Dropoff && !x.goodsDetailListDto)) {
      console.log('first Condition Fired');
      Swal.fire(this.l('IncompleteTripPoint'), this.l('PleaseAddAllTheDropPoints'), 'warning');
      return false;
      //if the routetype is single drop and the Drop point setup is not completed prevent adding trip
    } else if (this.shippingRequest.routeTypeId === this.RouteTypes.SingleDrop && !this.trip.routPoints[1].goodsDetailListDto) {
      console.log('Secound Condition Fired');
      Swal.fire(this.l('IncompleteTripPoint'), this.l('PleaseCompleteTheDropPointSetup'), 'warning');
      return false;
    } else {
      return true;
    }
  }

  /**
   * Validates Shipping Request Trip Before Create Template
   * @private
   */
  public PointsAreInValid(): boolean {
    //if there is no routePoints
    if (!isNotNullOrUndefined(this.trip.routPoints)) {
      return true;
    }
    //if there is route points check for good details
    //if there is no good details return false
    if (this.trip.routPoints.find((x) => x.pickingType == PickingType.Dropoff && !isNotNullOrUndefined(x.goodsDetailListDto))) {
      return true;
    }
    //else return false
    return false;
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
      this.trip = jsonObject;
      this.trip.id = undefined;
      this.loadReceivers(this.trip.originFacilityId);
      this._PointsService.updateWayPoints(this.trip.routPoints);
    });
  }
}
