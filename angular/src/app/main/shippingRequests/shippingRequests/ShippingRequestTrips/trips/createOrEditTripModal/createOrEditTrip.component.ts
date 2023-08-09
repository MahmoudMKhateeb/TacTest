/* tslint:disable:triple-equals curly */
// noinspection ES6ConvertVarToLetConst

import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  ActorSelectItemDto,
  CountyDto,
  CreateOrEditActorCarrierPrice,
  CreateOrEditActorShipperPriceDto,
  CreateOrEditDocumentFileDto,
  CreateOrEditRoutPointDto,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditShippingRequestTripVasDto,
  DedicatedShippingRequestsServiceProxy,
  DropPaymentMethod,
  EntityTemplateServiceProxy,
  FacilitiesServiceProxy,
  FacilityType,
  FileDto,
  GetAllDedicatedDriversOrTrucksForDropDownDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GetAllTrucksWithDriversListDto,
  GetShippingRequestForViewOutput,
  GetShippingRequestVasForViewDto,
  GoodsDetailsServiceProxy,
  PickingType,
  RoundTripType,
  SavedEntityType,
  SelectFacilityItemDto,
  SelectItemDto,
  ShippingRequestDestinationCitiesDto,
  ShippingRequestDto,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripFlag,
  ShippingTypeEnum,
  TemplateSelectItemDto,
  TenantCityLookupTableDto,
  TenantRegistrationServiceProxy,
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

  @Input('isPortMovement') isPortMovement = false;
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
  allShippingTypes: SelectItemDto[];
  allRoundTripTypes: SelectItemDto[];
  originCountry: number = null;
  destinationCities: ShippingRequestDestinationCitiesDto[] = [];
  citiesLoading = false;
  sourceCities: TenantCityLookupTableDto[];
  destinationCountry: number;
  allCountries: CountyDto[];
  generalGoodsCategoryId: number;
  roundTripTypeEnum = RoundTripType;

  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  templatesLoading: boolean;
  tripTemples: TemplateSelectItemDto[];
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
  isEdit: boolean;
  allOriginPorts: SelectFacilityItemDto[] = [];
  selectedShippingRequestDestinationCities: ShippingRequestDestinationCitiesDto[];
  allpackingTypes: SelectItemDto[];

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
    let tripDto = this._TripService.CreateOrEditShippingRequestTripDto;
    tripDto.shippingTypeId ??= this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.shippingTypeId;
    tripDto.originCityId ??= this._TripService?.GetShippingRequestForViewOutput?.originalCityId;
    return JSON.stringify(tripDto);
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
  AllActorsShippers: ActorSelectItemDto[];
  AllActorsCarriers: ActorSelectItemDto[];
  ShippingTypeEnum = ShippingTypeEnum;

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
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _countriesServiceProxy: TenantRegistrationServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.paymentMethodsArray = this.enumToArray.transform(DropPaymentMethod);
    this.ShippingRequestTripFlagArray = this.enumToArray.transform(ShippingRequestTripFlag);

    //Take The Points List From the Points Shared Service
    // this.PointsServiceSubscription = this._PointsService.currentWayPointsList.subscribe((res) => (this._TripService.CreateOrEditShippingRequestTripDto.routPoints = res));
    this.vasesHandler();
    this.ShippingRequestTripFlagArray = this.enumToArray.transform(ShippingRequestTripFlag);

    if (!this.isEnabled('App.HomeDelivery')) {
      this.ShippingRequestTripFlagArray = this.ShippingRequestTripFlagArray.filter((x) => x.key != '1');
    }
    // this.ShippingRequestTripFlagEnum = Object.values(ShippingRequestTripFlag);
    this.allShippingTypes = this.enumToArray.transform(ShippingTypeEnum).map((item) => {
      const selectItem = new SelectItemDto();
      (selectItem.id as any) = Number(item.key);
      selectItem.displayName = item.value;
      return selectItem;
    });

    this.fillAllRoundTrips(true);

    this._countriesServiceProxy.getAllCountriesWithCode().subscribe((res) => {
      this.allCountries = res;
    });

    this._facilitiesServiceProxy.getAllPortsForTableDropdown().subscribe((result) => {
      this.allOriginPorts = result;
    });

    this._shippingRequestsServiceProxy.getAllPackingTypesForDropdown().subscribe((result) => {
      this.allpackingTypes = result;
    });
  }

  /**
   * takes the Vas List From the Shipping Request And Cleans them to use them in Trips Modal
   */
  vasesHandler() {
    if (this.VasListFromFather) {
      this.VasListFromFather.filter((item) => !item.shouldHide).forEach((x) => {
        //Get the Vase List From Father And Attach Them to new Array
        const vas: CreateOrEditShippingRequestTripVasDto = new CreateOrEditShippingRequestTripVasDto();
        vas.id = undefined; // vas id in shipping Request trip (Required for edit trip)
        vas.shippingRequestTripId = this._TripService?.CreateOrEditShippingRequestTripDto?.id || undefined; //the trip id
        vas.shippingRequestVasId = x.shippingRequestVas.id; //vas id in shipping request
        vas.name = x.vasName; //vas Name
        this.cleanVasesList.push(vas);
      });
    }
  }

  show(record?: CreateOrEditShippingRequestTripDto, shippingRequestForView?: GetShippingRequestForViewOutput): void {
    this._TripService.GetShippingRequestForViewOutput = shippingRequestForView;
    this._PointsService.currentShippingRequest = this._TripService.GetShippingRequestForViewOutput;
    if (
      isNotNullOrUndefined(this._TripService.GetShippingRequestForViewOutput) &&
      this._TripService?.GetShippingRequestForViewOutput?.shippingRequestFlag === this.ShippingRequestFlagEnum.Dedicated
    ) {
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
      if (!isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto)) {
        this._TripService.CreateOrEditShippingRequestTripDto = new CreateOrEditShippingRequestTripDto();
      }
      this._TripService.CreateOrEditShippingRequestTripDto.id = record.id;
      this._TripService.activeTripId = record.id;

      this.loading = true;
      this.isEdit = true;
      this.getTripForEditSub = this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this.selectedShippingRequestDestinationCities = res.shippingRequestDestinationCities;
        this._TripService.CreateOrEditShippingRequestTripDto = res;
        (this.originCountry as any) = res.countryId;
        if (!shippingRequestForView) {
          this.loadCitiesByCountryId(this.originCountry, 'source', true);
          if (res.shippingTypeId != ShippingTypeEnum.CrossBorderMovements) {
            this.loadCitiesByCountryId(this.originCountry, 'destination', true);
          }
          this.loadFacilitiesInPointComponent();
        }
        if (this._TripService.CreateOrEditShippingRequestTripDto.shipperActorId) {
          (this._TripService.CreateOrEditShippingRequestTripDto.shipperActorId as any) = res.shipperActorId.toString();
        }
        if (this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId) {
          (this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId as any) = res.carrierActorId.toString();
        }
        if (
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId === ShippingTypeEnum.ExportPortMovements ||
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId === ShippingTypeEnum.ImportPortMovements
        ) {
          this.fillAllRoundTrips(true);
          this.isPortMovement = true;
        }

        this.IsHaveSealNumberValue = res.sealNumber?.length > 0;
        this.IsHaveContainerNumberValue = res.containerNumber?.length > 0;
        //console.log('res', res.containerNumber);
        const gregorian = moment(res.startTripDate).locale('en').format('D/M/YYYY');
        this.startTripdate = this.dateFormatterService.ToGregorianDateStruct(gregorian, 'D/M/YYYY');

        this.PointsComponent.wayPointsList = this._TripService.CreateOrEditShippingRequestTripDto.routPoints;
        if (
          (!this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.id &&
            (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingTypeId === ShippingTypeEnum.ExportPortMovements ||
              this._TripService?.CreateOrEditShippingRequestTripDto?.shippingTypeId === ShippingTypeEnum.ImportPortMovements)) ||
          this._TripService?.GetShippingRequestForViewOutput?.shippingRequest.shippingTypeId === ShippingTypeEnum.ExportPortMovements ||
          this._TripService?.GetShippingRequestForViewOutput?.shippingRequest.shippingTypeId === ShippingTypeEnum.ImportPortMovements
        ) {
          this.PointsComponent.wayPointsList = this._TripService.CreateOrEditShippingRequestTripDto.routPoints.sort(
            (a, b) => a.pointOrder - b.pointOrder
          );
        }
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

        if (
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ImportPortMovements ||
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ExportPortMovements
        ) {
          this.prepareRoundTripInputs();
        }
      });
    } else {
      this.loading = true;
      this._shippingRequestTripsService.getShippingRequestTripForCreate().subscribe((result) => {
        this._TripService.CreateOrEditShippingRequestTripDto = result;
        this._TripService.CreateOrEditShippingRequestTripDto.routPoints = this.PointsComponent?.wayPointsList;
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

  prepareRoundTripInputs() {
    console.log('prepareStep2Inputs');
    if (isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.roundTripType)) {
      switch (Number(this._TripService.CreateOrEditShippingRequestTripDto.roundTripType)) {
        case RoundTripType.TwoWayRoutsWithPortShuttling: {
          this._TripService.CreateOrEditShippingRequestTripDto.routeType = ShippingRequestRouteType.MultipleDrops;
          this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = !this._TripService?.GetShippingRequestForViewOutput?.shippingRequest
            ?.id
            ? 3
            : 2;
          break;
        }
        case RoundTripType.TwoWayRoutsWithoutPortShuttling:
        case RoundTripType.WithReturnTrip: {
          this._TripService.CreateOrEditShippingRequestTripDto.routeType = ShippingRequestRouteType.MultipleDrops;
          this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = 2;
          break;
        }
        case RoundTripType.WithoutReturnTrip:
        case RoundTripType.OneWayRoutWithoutPortShuttling:
        default: {
          this._TripService.CreateOrEditShippingRequestTripDto.routeType = ShippingRequestRouteType.SingleDrop;
          this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops = 1;
          break;
        }
      }
    }
    this.onRouteTypeChange();
    //
  }

  setStartTripDate(startTripDate) {
    if (this._TripService?.GetShippingRequestForViewOutput?.shippingRequestFlag === ShippingRequestFlag.Dedicated) {
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
    this.originCountry = null;
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
    this.isEdit = false;
  }

  createOrEditTrip() {
    this.isFormSubmitted = true;
    this.revalidatePointsFromPointsComponent();
    //if there is a Validation issue in the Points do Not Proceed
    if (
      isNotNullOrUndefined(this._TripService.GetShippingRequestForViewOutput) &&
      this._TripService?.GetShippingRequestForViewOutput?.shippingRequestFlag === ShippingRequestFlag.Dedicated
    ) {
      this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops =
        Number(this._TripService.CreateOrEditShippingRequestTripDto.routeType) === this.RouteTypesEnum.SingleDrop
          ? 1
          : this._TripService.CreateOrEditShippingRequestTripDto.numberOfDrops;
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestId = this.shippingRequest.id;
    }
    this._TripService.CreateOrEditShippingRequestTripDto.routPoints = this.PointsComponent.wayPointsList;
    this._TripService.CreateOrEditShippingRequestTripDto.routPoints.map((item, index) => (item.pointOrder = index + 1));
    if (this._TripService.CreateOrEditShippingRequestTripDto.roundTripType != RoundTripType.WithReturnTrip) {
      this._TripService.CreateOrEditShippingRequestTripDto.originFacilityId =
        this._TripService.CreateOrEditShippingRequestTripDto.routPoints[0].facilityId;
    }
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
    if (
      this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.shippingTypeId === ShippingTypeEnum.ImportPortMovements ||
      this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.shippingTypeId === ShippingTypeEnum.ExportPortMovements
    ) {
      this.checkIfShouldSendAppointmentAndClearance();
      return this.validatePointsFromPointsComponentForPortsMovement();
    }
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
      this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal
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
        (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
          (!isNotNullOrUndefined(point.receiverId) || ('' + point.receiverId).length === 0)) ||
        (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery &&
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
          (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
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
    console.log('shippingRequestTripsForm', this.shippingRequestTripsForm);
    if (
      (this._TripService.GetShippingRequestForViewOutput &&
        (this._TripService.GetShippingRequestForViewOutput.shippingRequest.shippingTypeId === ShippingTypeEnum.ImportPortMovements ||
          this._TripService.GetShippingRequestForViewOutput.shippingRequest.shippingTypeId === ShippingTypeEnum.ExportPortMovements)) ||
      (!this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.id &&
        (this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId === ShippingTypeEnum.ImportPortMovements ||
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId === ShippingTypeEnum.ExportPortMovements))
    ) {
      return this.validatePointsFromPointsComponentForPortsMovement();
    }
    if (!isNotNullOrUndefined(this.PointsComponent) || !isNotNullOrUndefined(this.PointsComponent.wayPointsList)) {
      return false;
    }
    for (const point of this.PointsComponent.wayPointsList) {
      const isFacilityEmpty = !isNotNullOrUndefined(point.facilityId) || ('' + point.facilityId).length === 0;
      const isReceiverEmpty =
        (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
          (!isNotNullOrUndefined(point.receiverId) || ('' + point.receiverId).length === 0)) ||
        (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery &&
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
          (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.Normal &&
            (!isNotNullOrUndefined(point.goodsDetailListDto as any) || point.goodsDetailListDto.length === 0))
        ) {
          return false;
        }
      }
    }
    return true;
  }

  private validatePointsFromPointsComponentForPortsMovement(): boolean {
    if (!isNotNullOrUndefined(this.PointsComponent) || !isNotNullOrUndefined(this.PointsComponent.wayPointsList)) {
      return false;
    }
    if (
      (this._TripService?.CreateOrEditShippingRequestTripDto?.roundTripType === RoundTripType.WithReturnTrip ||
        this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.roundTripType === RoundTripType.WithReturnTrip) &&
      this.PointsComponent.wayPointsList.length > 0
    ) {
      return this.validateImportWithReturnTrip();
    }
    if (
      (this._TripService?.CreateOrEditShippingRequestTripDto?.roundTripType === RoundTripType.WithoutReturnTrip ||
        this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.roundTripType === RoundTripType.WithoutReturnTrip) &&
      this.PointsComponent.wayPointsList.length > 0
    ) {
      return this.validateImportWithoutReturnTrip();
    }
    if (
      (this._TripService?.CreateOrEditShippingRequestTripDto?.roundTripType === RoundTripType.OneWayRoutWithoutPortShuttling ||
        this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.roundTripType === RoundTripType.OneWayRoutWithoutPortShuttling) &&
      this.PointsComponent.wayPointsList.length > 0
    ) {
      return this.validateOneWayRoutWithPortShuttling();
    }
    if (
      (this._TripService?.CreateOrEditShippingRequestTripDto?.roundTripType === RoundTripType.TwoWayRoutsWithoutPortShuttling ||
        this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.roundTripType === RoundTripType.TwoWayRoutsWithoutPortShuttling) &&
      this.PointsComponent.wayPointsList.length > 0
    ) {
      return this.validateTwoWayRoutsWithoutPortShuttling();
    }
    if (
      (this._TripService?.CreateOrEditShippingRequestTripDto?.roundTripType === RoundTripType.TwoWayRoutsWithPortShuttling ||
        this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.roundTripType === RoundTripType.TwoWayRoutsWithPortShuttling) &&
      this.PointsComponent.wayPointsList.length > 0
    ) {
      return this.validateTwoWayRoutsWithPortShuttling();
    }
    return true;
  }

  private checkIfShouldSendAppointmentAndClearance() {
    if (this._TripService.CreateOrEditShippingRequestTripDto?.routPoints) {
      this._TripService.CreateOrEditShippingRequestTripDto?.routPoints?.map((item) => {
        if (!item.dropNeedsAppointment) {
          item.appointmentDataDto = null;
        }
        if (!item.dropNeedsClearance) {
          item.tripClearancePricesDto = null;
        }
      });
    }
  }

  validateImportWithReturnTrip(): boolean {
    const point1 = this.PointsComponent.wayPointsList[1];
    const point2 = this.PointsComponent.wayPointsList[2];
    const point3 = this.PointsComponent.wayPointsList[3];
    const goodDetailsValidForPoint1 = this.isGoodDetailsValidForPoint(point1, false, false, true);
    //     isNotNullOrUndefined(point1.goodsDetailListDto) && point1.goodsDetailListDto.length > 0
    //     ? point1.goodsDetailListDto.filter((goodDetail) => {
    //     return isNotNullOrUndefined(goodDetail.description) && goodDetail.description?.length > 0;
    // }).length === point1.goodsDetailListDto.length
    //     : false;

    const pointOneValid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.facilityId) &&
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.receiverId) &&
      goodDetailsValidForPoint1;
    if (!pointOneValid) {
      return false;
    }

    const pointTwoValid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point2.facilityId) && this.isReceiverIdOrSenderIdOrFacilityIdValid(point2.receiverId);
    if (!pointTwoValid) {
      return false;
    }

    const foundFacility = this.PointsComponent.allFacilities.find((fac) => fac.id === point3.facilityId);
    const recieverValid =
      foundFacility?.facilityType === FacilityType.Facility
        ? point3.facilityId && this.isReceiverIdOrSenderIdOrFacilityIdValid(point3.receiverId)
        : true;
    const goodsDetailsValidForPoint3 = this.isGoodDetailsValidForPoint(point3, true, false, true);
    // isNotNullOrUndefined(point3.goodsDetailListDto) && point3.goodsDetailListDto.length > 0
    // ? point3.goodsDetailListDto.filter((goodDetail) => {
    // return (
    //     isNotNullOrUndefined(goodDetail.weight) &&
    //     goodDetail.weight?.toString()?.length > 0 &&
    //     isNotNullOrUndefined(goodDetail.description) &&
    //     goodDetail.description?.length > 0
    // );
    // }).length === point3.goodsDetailListDto.length
    //     : false;
    const pointThreeValid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point3.facilityId) && recieverValid && goodsDetailsValidForPoint3;
    if (!pointThreeValid) {
      return false;
    }
    return true;
  }
  validateImportWithoutReturnTrip(): boolean {
    const point0 = this.PointsComponent.wayPointsList[0];
    const point1 = this.PointsComponent.wayPointsList[1];
    const goodDetailsValidForPoint1 = this.isGoodDetailsValidForPoint(point1, false, false, true);

    const pointOneValid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point0.facilityId);
    const pointTwoValid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.facilityId) &&
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.receiverId) &&
      goodDetailsValidForPoint1;
    if (!pointOneValid || !pointTwoValid) {
      return false;
    }

    return true;
  }

  validateOneWayRoutWithPortShuttling(): boolean {
    const point1 = this.PointsComponent.wayPointsList[0];
    const point2 = this.PointsComponent.wayPointsList[1];
    const goodDetailsValidForPoint2 = this.isGoodDetailsValidForPoint(point2, true, true, true);

    const pointOneValid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.facilityId) && this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.receiverId);
    const pointTwoValid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point2.facilityId) && goodDetailsValidForPoint2;
    if (!pointOneValid || !pointTwoValid) {
      return false;
    }

    return true;
  }

  validateTwoWayRoutsWithoutPortShuttling(): boolean {
    const point1 = this.PointsComponent.wayPointsList[0];
    const point2 = this.PointsComponent.wayPointsList[1];
    const point3 = this.PointsComponent.wayPointsList[2];
    const point4 = this.PointsComponent.wayPointsList[3];
    const goodDetailsValidForPoint2 = this.isGoodDetailsValidForPoint(point2, true, true, true);
    const goodDetailsValidForPoint4 = this.isGoodDetailsValidForPoint(point4, true, true, true);

    const point1Valid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.facilityId) && this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.receiverId);
    const point2Valid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point2.facilityId) && goodDetailsValidForPoint2;
    const point3Valid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point3.facilityId);
    const point4Valid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point4.facilityId) &&
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point4.receiverId) &&
      goodDetailsValidForPoint4;
    if (!point1Valid || !point2Valid || !point3Valid || !point4Valid) {
      return false;
    }

    return true;
  }

  validateTwoWayRoutsWithPortShuttling(): boolean {
    const point1 = this.PointsComponent.wayPointsList[0];
    const point2 = this.PointsComponent.wayPointsList[1];
    const point3 = this.PointsComponent.wayPointsList[2];
    const point4 = this.PointsComponent.wayPointsList[3];
    const point5 = this.PointsComponent.wayPointsList[4];
    const point6 = this.PointsComponent.wayPointsList[5];
    const goodDetailsValidForPoint2 = this.isGoodDetailsValidForPoint(point2, true, true, true);
    const goodDetailsValidForPoint4 = this.isGoodDetailsValidForPoint(point4, true, true, true);
    const goodDetailsValidForPoint6 = this.isGoodDetailsValidForPoint(point6, true, true, true);

    const point1Valid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.facilityId) && this.isReceiverIdOrSenderIdOrFacilityIdValid(point1.receiverId);
    const point2Valid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point2.facilityId) && goodDetailsValidForPoint2;
    const point3Valid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point3.facilityId);
    const point4Valid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point4.facilityId) &&
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point4.receiverId) &&
      goodDetailsValidForPoint4;
    const point5Valid =
      this.isReceiverIdOrSenderIdOrFacilityIdValid(point5.facilityId) && this.isReceiverIdOrSenderIdOrFacilityIdValid(point5.receiverId);
    const point6Valid = this.isReceiverIdOrSenderIdOrFacilityIdValid(point6.facilityId) && goodDetailsValidForPoint6;
    if (!point1Valid || !point2Valid || !point3Valid || !point4Valid || !point5Valid || !point6Valid) {
      return false;
    }
    return true;
  }

  isReceiverIdOrSenderIdOrFacilityIdValid(facilitOrReceiverId: number) {
    return isNotNullOrUndefined(facilitOrReceiverId) && facilitOrReceiverId?.toString() != '';
  }

  isGoodDetailsValidForPoint(point: CreateOrEditRoutPointDto, isWeightRequired: boolean, isQtyRequired: boolean, isDescRequired: boolean) {
    if (!isNotNullOrUndefined(point)) {
      return false;
    }
    return isNotNullOrUndefined(point?.goodsDetailListDto) && point?.goodsDetailListDto?.length > 0
      ? point?.goodsDetailListDto?.filter((goodDetail) => {
          if (isWeightRequired && isQtyRequired && isDescRequired) {
            return (
              isNotNullOrUndefined(goodDetail.amount) &&
              goodDetail.amount?.toString()?.length > 0 &&
              isNotNullOrUndefined(goodDetail.weight) &&
              goodDetail.weight?.toString()?.length > 0 &&
              isNotNullOrUndefined(goodDetail.description) &&
              goodDetail.description?.length > 0
            );
          }
          if (isWeightRequired && isDescRequired) {
            return (
              isNotNullOrUndefined(goodDetail.weight) &&
              goodDetail.weight?.toString()?.length > 0 &&
              isNotNullOrUndefined(goodDetail.description) &&
              goodDetail.description?.length > 0
            );
          }
          if (isWeightRequired) {
            return isNotNullOrUndefined(goodDetail.weight) && goodDetail.weight?.toString().length > 0;
          }
          if (isQtyRequired) {
            return isNotNullOrUndefined(goodDetail.amount) && goodDetail.amount?.toString().length > 0;
          }
          if (isDescRequired) {
            return isNotNullOrUndefined(goodDetail.description) && goodDetail.description?.length > 0;
          }
        })?.length === point?.goodsDetailListDto?.length
      : false;
  }

  /**
   * Validates Shipping Request Trip Before Create Template
   * @private
   */
  public CanCreateTemplate(): boolean {
    //if there is no routePoints
    if (
      !isNotNullOrUndefined(this._TripService?.CreateOrEditShippingRequestTripDto?.routPoints) &&
      !isNotNullOrUndefined(this.PointsComponent?.wayPointsList)
    ) {
      return false;
    } else if (
      this._TripService?.CreateOrEditShippingRequestTripDto?.routPoints?.find(
        (x) => x?.pickingType == PickingType.Dropoff && !isNotNullOrUndefined(x?.goodsDetailListDto)
      ) ||
      this.PointsComponent?.wayPointsList?.find((x) => x?.pickingType == PickingType.Dropoff && !isNotNullOrUndefined(x?.goodsDetailListDto))
    ) {
      return false;
    } else if (
      this._TripService?.CreateOrEditShippingRequestTripDto?.routPoints?.length < this.shippingRequest?.numberOfDrops + 1 &&
      this.PointsComponent?.wayPointsList?.length < this.shippingRequest?.numberOfDrops + 1
    ) {
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
    if (this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery) {
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
      this._TripService?.CreateOrEditShippingRequestTripDto?.shippingRequestTripFlag == this.ShippingRequestTripFlagEnum.HomeDelivery
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
    this._dedicatedShippingRequestService
      .getAllDriversForDropDown(undefined, this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId)
      .subscribe((res) => {
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
    this._dedicatedShippingRequestService
      .getAllTrucksWithDriversList(truckTypeId, undefined, this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId)
      .subscribe((res) => {
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

  canSetPriceForShipperActor(): boolean {
    if (isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.shipperActorId)) {
      return !this.AllActorsShippers?.find((x) => x.id == this._TripService.CreateOrEditShippingRequestTripDto?.shipperActorId?.toString())?.isMySelf;
    }
    return false;
  }

  canSetPriceForCarrierActor(): boolean {
    if (isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.carrierActorId)) {
      return !this.AllActorsCarriers?.find((x) => x.id == this._TripService.CreateOrEditShippingRequestTripDto?.carrierActorId?.toString())?.isMySelf;
    }
    return false;
  }

  ShippingTypeChanged() {
    this.resetShippingInputs();
    this.fillAllRoundTrips();
    this.isPortMovement =
      this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ImportPortMovements ||
      this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ExportPortMovements
        ? true
        : false;
    if (this.isPortMovement) {
      this.BindGeneralGoods();
    } else {
      this._TripService.CreateOrEditShippingRequestTripDto.goodCategoryId = undefined;
    }
  }

  private BindGeneralGoods() {
    this._goodsDetailsServiceProxy.getGeneralGoodsCategoryId().subscribe((result) => {
      this.generalGoodsCategoryId = result;
      this._TripService.CreateOrEditShippingRequestTripDto.goodCategoryId = this.generalGoodsCategoryId;
    });
  }

  fillAllRoundTrips(isInit = false) {
    console.log(this._TripService.CreateOrEditShippingRequestTripDto);
    if (!isNotNullOrUndefined(this._TripService) || !isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto)) {
      return;
    }
    if (!isInit) {
      this._TripService.CreateOrEditShippingRequestTripDto.roundTripType = null;
    }
    if (
      this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId != ShippingTypeEnum.ImportPortMovements &&
      this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId != ShippingTypeEnum.ExportPortMovements
    ) {
      return;
    }
    // this.step1Form.get('roundTripType').setValidators([Validators.required]);
    //this.step1Form.get('roundTripType').updateValueAndValidity();
    this.allRoundTripTypes = this.enumToArray
      .transform(RoundTripType)
      .filter((item) => {
        if (this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ImportPortMovements) {
          return item.key == RoundTripType.WithoutReturnTrip || item.key == RoundTripType.WithReturnTrip;
        }
        if (this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ExportPortMovements) {
          return item.key != RoundTripType.WithoutReturnTrip && item.key != RoundTripType.WithReturnTrip;
        }
      })
      .map((item) => {
        const selectItem = new SelectItemDto();
        (selectItem.id as any) = Number(item.key);
        selectItem.displayName = item.value;
        return selectItem;
      });
    // this.step1Dto.roundTripType = Number(this.allRoundTripTypes[0].id);
    // this.ngForm.get('roundTripType').setValue(this.step1Dto.roundTripType);
    // this.step1Form.get('roundTripType').markAsTouched();
    // this.step1Form.get('roundTripType').updateValueAndValidity();
  }

  /**
   * resets step2 inputs if the Route Type Change
   */
  resetShippingInputs() {
    // this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities = [];
    this._TripService.CreateOrEditShippingRequestTripDto.originFacilityId = this._TripService.CreateOrEditShippingRequestTripDto.originCityId =
      undefined;
    // this.originCountry = this.destinationCountry = undefined;
    // this.clearValidation('originCity');
    // this.clearValidation('destinationCity');
    // this.clearValidation('originCountry');
    // this.clearValidation('destinationCountry');
  }

  validateShippingRequestType() {
    if (this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ImportPortMovements) {
      // this.step2Form.get('originFacility').setValidators([Validators.required]);
      // this.step2Form.get('originFacility').updateValueAndValidity();
    }
    //check if user choose local-inside city  but the origin&des same
    if (
      this._TripService.CreateOrEditShippingRequestTripDto.originCityId != null &&
      this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.LocalInsideCity
    ) {
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities = [];
      //local inside city
      this.destinationCountry = this.originCountry;
      var city = new ShippingRequestDestinationCitiesDto();
      city.cityId = this._TripService.CreateOrEditShippingRequestTripDto.originCityId;

      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities.push(city);
    } else if (this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.LocalBetweenCities) {
      // if route type is local betwenn cities check if user select same city in source and destination
      // this.destinationCities = this.sourceCities;
      this.destinationCountry = this.originCountry;

      //if destination city one item selected and equals to origin, while shipping type is between cities
      if (
        isNotNullOrUndefined(this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities) &&
        this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities.length == 1 &&
        this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities.filter(
          (c) => c.cityId == this._TripService.CreateOrEditShippingRequestTripDto.originCityId
        ).length > 0
      ) {
        // this.step2Form.controls['destinationCity'].setErrors({ invalid: true });
        // this.step2Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else if (this.originCountry !== this.destinationCountry) {
        // this.step2Form.controls['originCountry'].setErrors({ invalid: true });
        // this.step2Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        // this.clearValidation('destinationCity');
        // this.clearValidation('destinationCountry');
      }
    } else if (this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.CrossBorderMovements) {
      //if route type is cross border prevent the countries to be the same
      if (this.originCountry === this.destinationCountry) {
        // this.step2Form.controls['originCountry'].setErrors({ invalid: true });
        // this.step2Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        // this.clearValidation('originCountry');
        // this.clearValidation('destinationCountry');
        // this.clearValidation('originFacility');
      }
    }
  }

  loadCitiesByCountryId(countryId: number, type: 'source' | 'destination', isInit = false) {
    if (!isInit) {
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities = [];
    }
    this.destinationCities = [];
    this.citiesLoading = true;
    this._countriesServiceProxy
      .getAllCitiesForTableDropdown(countryId)
      .pipe(
        finalize(() => {
          this.citiesLoading = false;
        })
      )
      .subscribe((res) => {
        type === 'source' ? (this.sourceCities = res) : this.loadDestinationCities(res);
        if (
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.LocalBetweenCities ||
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ImportPortMovements ||
          this._TripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ExportPortMovements
        ) {
          this.sourceCities = res;
          // this.step2Dto.originCityId = this.step2Dto.destinationCityId = null;
          this.loadDestinationCities(res);
        }
      });
  }

  private loadDestinationCities(res: TenantCityLookupTableDto[]) {
    if (isNotNullOrUndefined(res)) {
      res.forEach((element) => {
        var item = new ShippingRequestDestinationCitiesDto();
        item.cityId = Number(element.id);
        item.cityName = element.displayName;
        this.destinationCities.push(item);
      });
    }
  }

  getCityIdFromSelectedPort($event: any) {
    this._TripService.CreateOrEditShippingRequestTripDto.originCityId = this.allOriginPorts?.find((port) => port.id == $event)?.cityId;
    this.PointsComponent.wayPointsList[0].facilityId = $event;
    this.PointsComponent.loadReceivers($event);
  }

  removeValidationForExportPortRequestOnStep2Form() {
    // this.step2Form.get('destinationCountry').clearValidators();
    // this.step2Form.get('destinationCountry').updateValueAndValidity();
    // this.step2Form.get('routeType').clearValidators();
    // this.step2Form.get('routeType').updateValueAndValidity();
    // this.step2Form.get('originFacility').clearValidators();
    // this.step2Form.get('originFacility').updateValueAndValidity();
  }

  removeValidationForImportPortRequestOnStep2Form() {
    // this.step2Form.get('destinationCountry').clearValidators();
    // this.step2Form.get('destinationCountry').updateValueAndValidity();
    // this.step2Form.get('originCity').clearValidators();
    // this.step2Form.get('originCity').updateValueAndValidity();
    // this.step2Form.get('routeType').clearValidators();
    // this.step2Form.get('routeType').updateValueAndValidity();
  }

  loadFacilitiesInPointComponent() {
    if (isNotNullOrUndefined(this.PointsComponent) && !this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.id) {
      this.PointsComponent.loadFacilities();
    }
  }
}
