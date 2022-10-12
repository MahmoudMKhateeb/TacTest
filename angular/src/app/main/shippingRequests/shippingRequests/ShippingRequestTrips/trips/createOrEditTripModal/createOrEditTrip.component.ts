import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditDocumentFileDto,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditShippingRequestTripVasDto,
  EntityTemplateServiceProxy,
  GetShippingRequestVasForViewDto,
  PickingType,
  SavedEntityType,
  SelectItemDto,
  ShippingRequestDto,
  ShippingRequestsTripServiceProxy,
  UpdateDocumentFileInput,
  WaybillsServiceProxy,
  FileDto,
  ShippingRequestRouteType,
  GetShippingRequestForViewOutput,
  DedicatedShippingRequestsServiceProxy,
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

  trip = new CreateOrEditShippingRequestTripDto();
  saving = false;
  loading = true;
  active = false;
  activeTripId: number = undefined;
  cleanVasesList: CreateOrEditShippingRequestTripVasDto[] = [];
  isApproximateValueRequired = false;
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
  private PickingType = PickingType;
  private tripServiceShippingRequestSub: Subscription;
  private getTripForEditSub: Subscription;
  RouteTypesEnum = ShippingRequestRouteType;

  TripsServiceSubscription: any;
  wayBillIsDownloading: boolean;
  selectedTemplate: number;
  isTripPointsInvalid = false;
  routeTypes: any[] = [];
  canEditNumberOfDrops = true;

  get isFileInputValid() {
    return this.trip.hasAttachment ? (this.trip.createOrEditDocumentFileDto.name ? true : false) : true;
  }

  get tripAsJson(): string {
    if (this.trip) this.trip.shippingRequestId = this.shippingRequest.id;
    return JSON.stringify(this.trip);
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
  allDedicatedDrivers: SelectItemDto[] = [];
  allDedicatedTrucks: SelectItemDto[] = [];
  shippingRequestForView: GetShippingRequestForViewOutput = null;

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
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit() {
    //link the trip from the shared service to the this component
    this.TripsServiceSubscription = this._TripService.currentActiveTrip.subscribe((res) => (this.trip = res));
    //Take The Points List From the Points Shared Service
    // this.PointsServiceSubscription = this._PointsService.currentWayPointsList.subscribe((res) => (this.trip.routPoints = res));
    this.vasesHandler();
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

  show(record?: CreateOrEditShippingRequestTripDto, shippingRequestForView?: GetShippingRequestForViewOutput): void {
    if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === 1) {
      this.shippingRequestForView = shippingRequestForView;
      this.getAllDedicatedDriversForDropDown();
      this.getAllDedicateTrucksForDropDown();
      this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType);
    }
    if (this.shippingRequest) {
      this.setStartTripDate(this.shippingRequest.startTripDate);
      const endDate =
        isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === 0
          ? this.shippingRequest.endTripDate
          : this.shippingRequestForView.rentalEndDate;
      const EndDateGregorian = moment(endDate).locale('en').format('D/M/YYYY');
      this.maxTripDateAsGrorg = this.dateFormatterService.ToGregorianDateStruct(EndDateGregorian, 'D/M/YYYY');
      this.maxTripDateAsHijri = this.dateFormatterService.ToHijriDateStruct(EndDateGregorian, 'D/M/YYYY');
    }
    if (record) {
      this.activeTripId = record.id;
      this._TripService.updateActiveTripId(this.activeTripId);
      this.getTripForEditSub = this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this.trip = res;
        this.PointsComponent.wayPointsList = this.trip.routPoints;
        this.PointsComponent.loadReceivers(null, true);
        // this._PointsService.updateWayPoints(this.trip.routPoints);
        //this.startTripdate = this.dateFormatterService.MomentToNgbDateStruct(res.startTripDate);
        if (res.endTripDate != null && res.endTripDate != undefined)
          this.endTripdate = this.dateFormatterService.MomentToNgbDateStruct(res.endTripDate);
        this._PointsService.updateWayPoints(this.trip.routPoints);
        if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === 1) {
          this.canEditNumberOfDrops = false;
          (this.trip.routeType as any) = '' + this.trip.routeType;
        }
        this.loading = false;
      });
    } else {
      //this is a create
      //init file document
      this._shippingRequestTripsService.getShippingRequestTripForCreate().subscribe((result) => {
        this.trip = result;
        this.trip.createOrEditDocumentFileDto.extn = '_';
        this.trip.createOrEditDocumentFileDto.name = '_';
        if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === 1) {
          this.trip.routeType = this.RouteTypesEnum.SingleDrop;
          this.onRouteTypeChange();
        }
      });

      this._TripService.updateActiveTripId(null);
      // this._PointsService.updateSinglePoint(new CreateOrEditRoutPointDto());
      // this._PointsService.updateWayPoints([]);
      this.loading = true;
      this.tripServiceShippingRequestSub = this._TripService.currentShippingRequest.subscribe((res) => {
        if (this.loading == true) {
          this.setStartTripDate(res.shippingRequest.startTripDate);
        }
        this.loading = false;
      });
      this.loading = false;
    }
    this._PointsService.updateCurrentUsedIn('createOrEdit');
    // this.PointsComponent.createEmptyPoints();
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
    this.canEditNumberOfDrops = true;
    this.active = false;
    this.isFormSubmitted = false;
    this.modal.hide();
    this.trip = new CreateOrEditShippingRequestTripDto();
    this.fileToken = undefined;
    this.hasNewUpload = undefined;
    this._TripService.updateSourceFacility(null);
    this._TripService.updateDestFacility(null);
    this.trip.routPoints = [];
    if (isNotNullOrUndefined(this.PointsComponent)) {
      this.PointsComponent.wayPointsList = this.trip.routPoints;
    }
    this._PointsService.updateWayPoints([]);

    this.allDedicatedDrivers = [];
    this.allDedicatedTrucks = [];
  }

  createOrEditTrip() {
    this.isFormSubmitted = true;
    this.revalidatePointsFromPointsComponent();
    //if there is a Validation issue in the Points do Not Proceed
    if (isNotNullOrUndefined(this.shippingRequestForView) && this.shippingRequestForView.shippingRequestFlag === 1) {
      this.trip.numberOfDrops = Number(this.trip.routeType) === this.RouteTypesEnum.SingleDrop ? 1 : this.trip.numberOfDrops;
    }
    this.trip.shippingRequestId = this.shippingRequest.id;
    this.trip.routPoints = this.PointsComponent.wayPointsList;
    this.trip.originFacilityId = this.trip.routPoints[0].facilityId;
    this.trip.destinationFacilityId = this.trip.routPoints[this.trip.routPoints.length - 1].facilityId;
    if (this.validatePointsBeforeAddTrip()) {
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
    this._TripService.updateShippingRequest(null);

    this.TripsServiceSubscription.unsubscribe();
    // this.tripServiceShippingRequestSub?.unsubscribe();
    this.tripServiceShippingRequestSub?.unsubscribe();
    this.getTripForEditSub?.unsubscribe();
    this.docProgress = undefined;

    console.log('Detsroid From Create/Edit Trip');
  }

  /**
   * Validates Shipping Request Trip Before Create/Edit
   * @private
   */
  private validatePointsBeforeAddTrip() {
    let isSingleDropTrip = this.shippingRequest.routeTypeId === this.RouteTypesEnum.SingleDrop;
    let isFacilitiesTheSame = isNotNullOrUndefined(this.trip.routPoints) && this.trip.routPoints[0].facilityId === this.trip.routPoints[1].facilityId;
    let isFacilitiesEmpty =
      isNotNullOrUndefined(this.trip.routPoints) &&
      !isNotNullOrUndefined(this.trip.routPoints[0].facilityId) &&
      !isNotNullOrUndefined(this.trip.routPoints[1].facilityId);
    if (isSingleDropTrip && isFacilitiesEmpty) {
      Swal.fire(this.l('ValidationError'), this.l('SourcePickUpFacilityOrDropOffFacilityCantBeEmpty'), 'error');
      return false;
    }
    if (isSingleDropTrip && isFacilitiesTheSame) {
      Swal.fire(this.l('ValidationError'), this.l('SourcePickUpFacilityAndDropOffFacilityCantBeTheSame'), 'error');
      return false;
    }
    if (!isNotNullOrUndefined(this.trip.routPoints)) {
      return false;
    }
    //trip Details Validation
    for (const point of this.trip.routPoints) {
      const isFacilityOrReceiverEmpty =
        !isNotNullOrUndefined(point.facilityId) ||
        !isNotNullOrUndefined(point.receiverId) ||
        ('' + point.facilityId).length === 0 ||
        ('' + point.receiverId).length === 0;
      if (point.pickingType === this.PickingType.Pickup && isFacilityOrReceiverEmpty) {
        Swal.fire(this.l('IncompleteTripPoint'), this.l('PleaseCompletePicKupPointDetails'), 'warning');
        return false;
        break;
      }
      if (point.pickingType === this.PickingType.Dropoff) {
        if (isFacilityOrReceiverEmpty || !isNotNullOrUndefined(point.goodsDetailListDto as any)) {
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
      const isFacilityOrReceiverEmpty =
        !isNotNullOrUndefined(point.facilityId) ||
        !isNotNullOrUndefined(point.receiverId) ||
        ('' + point.facilityId).length === 0 ||
        ('' + point.receiverId).length === 0;
      if (point.pickingType === this.PickingType.Pickup && isFacilityOrReceiverEmpty) {
        return false;
      }
      if (point.pickingType === this.PickingType.Dropoff) {
        if (isFacilityOrReceiverEmpty || !isNotNullOrUndefined(point.goodsDetailListDto as any) || point.goodsDetailListDto.length === 0) {
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
    if (!isNotNullOrUndefined(this.trip.routPoints)) {
      return false;
    } else if (this.trip.routPoints.find((x) => x.pickingType == PickingType.Dropoff && !isNotNullOrUndefined(x.goodsDetailListDto))) {
      return false;
    } else if (this.trip.routPoints.length < this.shippingRequest.numberOfDrops + 1) {
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
      this.trip = jsonObject;
      this.removeIdsFromTripTemplate(this.trip);
      // this.loadReceivers(this.trip.originFacilityId);
      this._PointsService.updateWayPoints(this.trip.routPoints);
      this.PointsComponent.loadReceivers(null, true);
    });
  }

  private removeIdsFromTripTemplate(trip: CreateOrEditShippingRequestTripDto) {
    this.trip.id = undefined;
    this.trip.routPoints.map((x) => {
      x.goodsDetailListDto?.map((y) => {
        return (y.id = undefined);
      });
      return (x.id = undefined);
    });
    return this.trip;
  }

  revalidatePointsFromPointsComponent() {
    this.callbacks.forEach((func) => {
      func();
    });
  }

  private getAllDedicatedDriversForDropDown() {
    this._dedicatedShippingRequestsServiceProxy.getAllDedicatedDriversForDropDown(this.shippingRequestForView.shippingRequest.id).subscribe((res) => {
      this.allDedicatedDrivers = res;
    });
  }

  private getAllDedicateTrucksForDropDown() {
    this._dedicatedShippingRequestsServiceProxy.getAllDedicateTrucksForDropDown(this.shippingRequestForView.shippingRequest.id).subscribe((res) => {
      this.allDedicatedTrucks = res;
    });
  }

  onRouteTypeChange() {
    console.log('this.trip.routeType', this.trip.routeType);
    console.log('onRouteTypeChange', Number(this.trip.routeType) === this.RouteTypesEnum.SingleDrop);
    if (Number(this.trip.routeType) === this.RouteTypesEnum.MultipleDrops) {
      this.trip.numberOfDrops = Number(this.trip.numberOfDrops) === 1 ? 2 : this.trip.numberOfDrops;
    } else {
      this.trip.numberOfDrops = 1;
    }
    this.onNumberOfDropsChanged();
    console.log('this.trip.numberOfDrops', this.trip);
  }

  onNumberOfDropsChanged() {
    console.log('this.trip', { ...this.trip });
    this.shippingRequestForView.shippingRequest.numberOfDrops = this.trip.numberOfDrops;
    this._TripService.updateShippingRequest(this.shippingRequestForView);
    console.log('this.PointsComponent.wayPointsList', this.PointsComponent.wayPointsList);
    console.log('this.canEditNumberOfDrops', this.canEditNumberOfDrops);
    if (this.canEditNumberOfDrops) {
      this.PointsComponent.wayPointsList = [];
      this._PointsService.updateWayPoints([]);
      this.PointsComponent.createEmptyPoints();
      this._PointsService.updateWayPoints(this.PointsComponent.wayPointsList);
    }
  }
}
