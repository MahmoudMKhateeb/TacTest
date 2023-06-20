import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  AssignDriverAndTruckToShippmentByCarrierInput,
  CreateOrEditShippingRequestTripVasDto,
  DedicatedShippingRequestsServiceProxy,
  GetAllDedicatedDriversOrTrucksForDropDownDto,
  GetShippingRequestForViewOutput,
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestDriverServiceProxy,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestsTripForViewDto,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripFlag,
  ShippingRequestTripStatus,
  TrucksServiceProxy,
  UpdateExpectedDeliveryTimeInput,
  WaybillsServiceProxy,
  DropPaymentMethod,
  ShippingTypeEnum,
  GetAllUploadedFileDto,
  ShippingRequestsServiceProxy,
  RoundTripType,
  TenantRegistrationServiceProxy,
  CountyDto,
  TenantCityLookupTableDto,
  ShippingRequestDestinationCitiesDto,
  FacilitiesServiceProxy,
  SelectFacilityItemDto,
  CreateOrEditShippingRequestTripDto,
  GoodsDetailsServiceProxy,
  GetAllGoodsCategoriesForDropDownOutput,
  GetAllTrucksWithDriversListDto,
  ActorSelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from '@node_modules/rxjs/operators';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import Swal from 'sweetalert2';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { ActivatedRoute } from '@angular/router';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { FileViwerComponent } from '@app/shared/common/file-viwer/file-viwer.component';
import { Moment } from '@node_modules/moment';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'viewTripModal',
  templateUrl: './viewTripModal.component.html',
  styleUrls: ['./viewTripModal.component.scss'],
})
export class ViewTripModalComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('viewTripDetails', { static: false }) modal: ModalDirective;
  @ViewChild('TripNotesModal', { static: false }) TripNotesModal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter();
  @ViewChild('fileViwerComponent', { static: false }) fileViwerComponent: FileViwerComponent;
  @Input('isPortMovement') isPortMovement = false;

  canAssignTrucksAndDrivers: boolean;
  fromTime: string;
  toTime: string;
  Vases: CreateOrEditShippingRequestTripVasDto[];
  trip: ShippingRequestsTripForViewDto = new ShippingRequestsTripForViewDto();
  assignDriverAndTruck: AssignDriverAndTruckToShippmentByCarrierInput = new AssignDriverAndTruckToShippmentByCarrierInput();
  allDrivers: SelectItemDto[] = [];
  allTrucks: SelectItemDto[] | GetAllTrucksWithDriversListDto[] = [];
  saving = false;
  loading = true;
  //currentTripId: number;
  wayBillIsDownloading = false;
  isResetTripLoading = false;
  private TruckTypeId: number;
  pickUpPointSender: string;
  activeTripId: any;
  type = 'Trip';
  shippingRequestTripStatusEnum = ShippingRequestTripStatus;
  expectedDeliveryTime: moment.Moment;
  originalExpectedDeliveryTime: Moment;
  expectedDeliveryTimeLoading: boolean;
  shippingRequestForView: GetShippingRequestForViewOutput;
  allDedicatedDrivers: GetAllDedicatedDriversOrTrucksForDropDownDto[];
  allDedicatedTrucks: GetAllDedicatedDriversOrTrucksForDropDownDto[];
  routeTypes: any[] = [];
  RouteTypesEnum = ShippingRequestRouteType;
  ShippingRequestFlagEnum = ShippingRequestFlag;
  ShippingRequestTripFlagEnum = ShippingRequestTripFlag;
  ShippingRequestTripFlagArray = [];
  AllActorsCarriers: ActorSelectItemDto[] = [];
  AllActorsShippers: ActorSelectItemDto[] = [];
  allShippingTypes: SelectItemDto[] = [];
  ShippingTypeEnum = ShippingTypeEnum;
  allRoundTripTypes: SelectItemDto[] = [];
  originCountry;
  destinationCountry;
  allCountries: CountyDto[] = [];
  sourceCities: TenantCityLookupTableDto[] = [];
  destinationCities: ShippingRequestDestinationCitiesDto[] = [];
  allOriginPorts: SelectFacilityItemDto[] = [];
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  allpackingTypes: SelectItemDto[];

  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy,
    public _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _PointsService: PointsService,
    public _TripService: TripService,
    private _Router: ActivatedRoute,
    private _changeDetectorRef: ChangeDetectorRef,
    private enumToArray: EnumToArrayPipe,
    private _countriesServiceProxy: TenantRegistrationServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.TruckTypeId = this._TripService.GetShippingRequestForViewOutput?.truckTypeId;
    this.activeTripId = this._Router.snapshot.queryParams['tripId'];
    this.fillData();
  }

  private fillData() {
    this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType);
    this.ShippingRequestTripFlagArray = this.enumToArray.transform(ShippingRequestTripFlag);
    this.getActors();
    this.allShippingTypes = this.enumToArray.transform(ShippingTypeEnum).map((item) => {
      const selectItem = new SelectItemDto();
      (selectItem.id as any) = Number(item.key);
      selectItem.displayName = item.value;
      return selectItem;
    });
    this.allRoundTripTypes = this.enumToArray.transform(RoundTripType).map((item) => {
      const selectItem = new SelectItemDto();
      (selectItem.id as any) = Number(item.key);
      selectItem.displayName = item.value;
      return selectItem;
    });

    this._countriesServiceProxy.getAllCountriesWithCode().subscribe((res) => {
      this.allCountries = res;
    });
    this._facilitiesServiceProxy.getAllPortsForTableDropdown().subscribe((result) => {
      this.allOriginPorts = result;
    });
    this.getAllGoodCategories();

    this._shippingRequestsServiceProxy.getAllPackingTypesForDropdown().subscribe((result) => {
      this.allpackingTypes = result;
    });
  }

  ngAfterViewInit() {
    if (isNotNullOrUndefined(this.activeTripId)) {
      this.show(this.activeTripId, this.shippingRequestForView);
    }
    this._changeDetectorRef.detectChanges();
  }

  show(id, shippingRequestForView?: GetShippingRequestForViewOutput): void {
    this.shippingRequestForView = shippingRequestForView;
    this._PointsService.currentShippingRequest = this.shippingRequestForView;
    if (isNotNullOrUndefined(shippingRequestForView) && shippingRequestForView.shippingRequestFlag === this.ShippingRequestFlagEnum.Dedicated) {
      this.getAllDedicatedDriversForDropDown();
      this.getAllDedicateTrucksForDropDown();
    }
    this.loading = true;

    //update the active trip id in TripsService
    this._TripService.activeTripId = id;
    if (this.shippingRequestForView) {
      this.getAllTrucks(this.TruckTypeId);
      this.getAllDrivers();
    }

    if (!this._TripService?.GetShippingRequestForViewOutput?.shippingRequest?.id) {
      this._TripService.GetShippingRequestForViewOutput = undefined;
      this.getAllDriversForDirectShipment();
      this.getAllTrucksForDirectShipment();
      this._shippingRequestTripsService.getShippingRequestTripForEdit(id).subscribe((res) => {
        this._TripService.CreateOrEditShippingRequestTripDto = res;
        (this.originCountry as any) = res.countryId;
        if (!shippingRequestForView) {
          this.loadCitiesByCountryId(this.originCountry, 'source', true);
          if (res.shippingTypeId != ShippingTypeEnum.CrossBorderMovements) {
            this.loadCitiesByCountryId(this.originCountry, 'destination', true);
          }
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
          this.isPortMovement = true;
        }
      });
    }

    this._shippingRequestTripsService
      .getShippingRequestTripForView(id)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.trip = res;
        this.fromTime = res.supposedPickupDateFrom?.format('HH:mm');
        this.toTime = res.supposedPickupDateTo?.format('HH:mm');
        if (res.shippingTypeId === ShippingTypeEnum.ExportPortMovements || res.shippingTypeId === ShippingTypeEnum.ImportPortMovements) {
          this.isPortMovement = true;
          this.trip.routPoints = this.trip.routPoints.sort((a, b) => a.pointOrder - b.pointOrder);
        }
        //Get The Points From The View Service and send them to the Points Service To Draw Them
        this._PointsService.updateWayPoints(this.trip.routPoints);
        this._PointsService.updateCurrentUsedIn('view');
        this.pickUpPointSender = res.routPoints.length > 0 ? res.routPoints[0].senderOrReceiverContactName : null;
        this.assignDriverAndTruck.assignedTruckId = this.trip.assignedTruckId;
        this.assignDriverAndTruck.assignedDriverUserId = this.trip.assignedDriverUserId;
        this.assignDriverAndTruck.containerNumber = this.trip.containerNumber;
        this.assignDriverAndTruck.sealNumber = this.trip.sealNumber;
        this.expectedDeliveryTime = this.trip.expectedDeliveryTime;
        this._changeDetectorRef.detectChanges();
        this.canAssignTrucksAndDrivers = res.canAssignDriversAndTrucks;
      });

    this.modal.show();
  }

  close(): void {
    this.trip = new ShippingRequestsTripForViewDto();
    //this.wayPointsComponent.wayPointsList = [];
    this.allDrivers = [];
    this.allTrucks = [];
    this.loading = true;
    this._PointsService.updateWayPoints([]);
    this.modal.hide();
    this._changeDetectorRef.detectChanges();
    this._TripService.CreateOrEditShippingRequestTripDto = undefined;
  }

  checkData(category) {
    if (category === 'truck' && this.allTrucks.length === 0) {
      this.getAlert(this.l('NoMatchingTrucks'));
    }
    if (category === 'driver' && this.allDrivers.length === 0) {
      this.getAlert(this.l('NoMatchingDrivers'));
    }
  }

  getAlert(msg: string) {
    Swal.fire({
      title: msg,
      icon: 'warning',
      confirmButtonText: this.l('Ok'),
    });
  }

  DownloadSingleDropWaybillPdf(id: number): void {
    this.wayBillIsDownloading = true;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.fileViwerComponent.show(this._fileDownloadService.downloadTempFile(result), 'pdf');
      this.wayBillIsDownloading = false;
    });
  }

  /**
   * Driver Assignation Section
   * this method is for Getting All Carriers Drivers For DD
   */
  getAllDrivers() {
    if (this.isTachyonDealer || this.isCarrier || this.hasCarrierClients) {
      this._trucksServiceProxy.getDriversByShippingRequestId(this.shippingRequestForView.shippingRequest.id).subscribe((result) => {
        this.allDrivers = result;
      });
    }
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks(truckTypeId) {
    if (this.isTachyonDealer || this.isCarrier || this.hasCarrierClients) {
      this._trucksServiceProxy.getTrucksByShippingRequestId(truckTypeId, this.shippingRequestForView.shippingRequest.id).subscribe((result) => {
        this.allTrucks = result;
      });
    }
  }

  /**
   * this method is for Getting All Drivers For Direct Shipment
   */
  getAllDriversForDirectShipment() {
    this._trucksServiceProxy.getAllDriversForDropDown(undefined).subscribe((result) => {
      this.allDrivers = result;
    });
  }

  /**
   * this method is for Getting All Trucks For Direct Shipment
   */
  getAllTrucksForDirectShipment() {
    this._dedicatedShippingRequestsServiceProxy.getAllTrucksWithDriversList(undefined, undefined).subscribe((res) => {
      this.allTrucks = res;
    });
  }

  getNotes() {
    this.TripNotesModal.show();
  }
  /**
   * this function is to assign Driver And Truck To shipping Request Trip
   */
  assignDriverandTruck() {
    this.saving = true;
    this.assignDriverAndTruck.id = this._TripService.activeTripId;
    this._shippingRequestTripsService
      .assignDriverAndTruckToShippmentByCarrier(this.assignDriverAndTruck)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.success('driverandTrucksAssignedSuccessfully');
      });
  }

  /**
   * Resets Shipping Request Trip
   * @param tripId
   */
  ResetTrip(tripId: number) {
    Swal.fire({
      title: this.l('areYouSure'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.isResetTripLoading = true;
        this._shippingRequestDriverServiceProxy.reset(tripId).subscribe(() => {
          this.isResetTripLoading = false;
          this.notify.info(this.l('SuccessfullyReseated'));
          this.modalSave.emit('');
        });
      } //end of if
    });
  }

  /**
   * update the expected Delivery Time Of the Trip
   */
  updateTripExpectedDeliveryTime() {
    if (!isNotNullOrUndefined(this._TripService.activeTripId)) {
      return;
    }
    if (this.expectedDeliveryTime === this.originalExpectedDeliveryTime) return;
    // console.log('Trip Expected Delivery time Was Updated');
    this.expectedDeliveryTimeLoading = true;
    let body = new UpdateExpectedDeliveryTimeInput();
    body.id = this._TripService.activeTripId;
    body.expectedDeliveryTime = this.expectedDeliveryTime;
    this._shippingRequestTripsService.updateExpectedDeliveryTimeForTrip(body).subscribe((res) => {
      this.expectedDeliveryTimeLoading = false;
      this.notify.success(this.l('TripExpectedDateWasUpdated'));
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

  DriverOrTruckSelected(driverUserId?: number, truckId?: number) {
    if (isNotNullOrUndefined(driverUserId)) {
      this._trucksServiceProxy.getTruckByDriverId(driverUserId, this.shippingRequestForView.truckTypeId).subscribe((result) => {
        if (!isNotNullOrUndefined(this.assignDriverAndTruck.assignedTruckId)) this.assignDriverAndTruck.assignedTruckId = result;
      });
    } else if (isNotNullOrUndefined(truckId)) {
      this._trucksServiceProxy.getDriverByTruckId(truckId).subscribe((result) => {
        if (!isNotNullOrUndefined(this.assignDriverAndTruck.assignedDriverUserId)) this.assignDriverAndTruck.assignedDriverUserId = result;
      });
    }
  }

  downloadTripManifest() {
    let image = this._fileDownloadService.downloadFileByBinary(
      this.trip.tripManifestDataDto.documentId,
      this.trip.tripManifestDataDto.documentName,
      this.trip.tripManifestDataDto.documentContentType
    );
    this.fileViwerComponent.show(image, this.trip.tripManifestDataDto.documentContentType == 'application/pdf' ? 'pdf' : 'img');
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

  loadCitiesByCountryId(countryId: number, type: 'source' | 'destination', isInit = false) {
    if (!isInit) {
      this._TripService.CreateOrEditShippingRequestTripDto.shippingRequestDestinationCities = [];
    }
    this.destinationCities = [];
    this._countriesServiceProxy.getAllCitiesForTableDropdown(countryId).subscribe((res) => {
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

  private getAllGoodCategories() {
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });
  }
}
