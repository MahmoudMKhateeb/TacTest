import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrEditRoutPointDto,
  DedicatedShippingRequestsServiceProxy,
  FacilityForDropdownDto,
  FacilityType,
  GetAllTrucksWithDriversListDto,
  PickingType,
  ReceiverFacilityLookupTableDto,
  RoundTripType,
  SelectItemDto,
  ShippingRequestsTripServiceProxy,
  TripAppointmentDataDto,
  TripClearancePricesDto,
} from '@shared/service-proxies/service-proxies';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { AppointmentAndClearanceModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/appointment-and-clearance/appointment-and-clearance.component';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { StorageDetailsModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points-for-port-movement/storage-details/storage-details.component';

@Component({
  selector: 'PointsForPortsMovementComponent',
  templateUrl: './points-for-port-movement.component.html',
  styleUrls: ['./points-for-port-movement.component.scss'],
})
export class PointsForPortsMovementComponent extends AppComponentBase implements OnInit {
  PickingType = PickingType;
  activePointIndex: number;
  filteredPickupFacilities: FacilityForDropdownDto[][] = [];
  filteredDropFacilities: FacilityForDropdownDto[][] = [];

  @ViewChild('appointmentAndClearanceModal', { static: true }) appointmentAndClearanceModal: AppointmentAndClearanceModalComponent;
  @ViewChild('StorageDetailsModal', { static: true }) StorageDetailsModal: StorageDetailsModalComponent;
  @Input('isEdit') isEdit = false;
  @Input('wayPointsList') wayPointsList: CreateOrEditRoutPointDto[] = [];
  _pickupFacilities: FacilityForDropdownDto[] = [];
  allfacilities: FacilityForDropdownDto[];
  @Input('pickupFacilities') set pickupFacilities(value: FacilityForDropdownDto[]) {
    this._pickupFacilities = value;
    if (this.wayPointsList.length > 0) {
      for (let i = 0; i < this.wayPointsList.length; i++) {
        this.wayPointsList[i].pickingType === PickingType.Pickup
          ? (this.filteredPickupFacilities[i] = this.filterFacilitiesForDropDown(this.wayPointsList[i].pickingType === PickingType.Pickup, i))
          : (this.filteredDropFacilities[i] = this.filterFacilitiesForDropDown(this.wayPointsList[i].pickingType === PickingType.Pickup, i));
      }
    }
  }
  get pickupFacilities(): FacilityForDropdownDto[] {
    return this._pickupFacilities;
  }

  _dropFacilities: FacilityForDropdownDto[] = [];
  @Input('dropFacilities') set dropFacilities(value: FacilityForDropdownDto[]) {
    this._dropFacilities = value;
    if (this.wayPointsList.length > 0) {
      for (let i = 0; i < this.wayPointsList.length; i++) {
        this.wayPointsList[i].pickingType === PickingType.Pickup
          ? (this.filteredPickupFacilities[i] = this.filterFacilitiesForDropDown(this.wayPointsList[i].pickingType === PickingType.Pickup, i))
          : (this.filteredDropFacilities[i] = this.filterFacilitiesForDropDown(this.wayPointsList[i].pickingType === PickingType.Pickup, i));
      }
    }
  }
  get dropFacilities(): FacilityForDropdownDto[] {
    return this._dropFacilities;
  }

  get isImportWithStorage() {
    let isTripRouteTypeWithStorage = this._tripService.CreateOrEditShippingRequestTripDto?.roundTripType == RoundTripType.WithStorage;
    let isShippingRequestRouteTypeWithStorage = this._tripService.GetShippingRequestForViewOutput?.roundTripType == RoundTripType.WithStorage;
    // console.log(
    //   'isTripRouteTypeWithStorage || isShippingRequestRouteTypeWithStorage',
    //   isTripRouteTypeWithStorage || isShippingRequestRouteTypeWithStorage
    // );
    return isTripRouteTypeWithStorage || isShippingRequestRouteTypeWithStorage;
  }
  @Input('usedIn') usedIn: 'view' | 'createOrEdit';
  @Input('allPointsSendersAndReceivers') allPointsSendersAndReceivers: ReceiverFacilityLookupTableDto[][] = [];
  @Input('facilityLoading') facilityLoading: boolean;
  @Input('receiverLoading') receiverLoading: boolean;
  @Input('isExportRequest') isExportRequest: boolean;
  @Input('isImportWithReturnTrip') isImportWithReturnTrip: boolean;
  @Input('roundTripType') roundTripType: RoundTripType;

  @Output() RouteStepCordSetterEvent = new EventEmitter<{ index: number; facilityId: number }>();
  @Output() wayPointsSetterEvent = new EventEmitter<any>();
  @Output() loadReceiversEvent = new EventEmitter<number>();
  @Output() onChangedWayPointsListEvent = new EventEmitter<any>();
  @Output() createOrEditFacilityModalShowEvent = new EventEmitter<any>();
  @Output() createOrEditReceiverModalShowEvent = new EventEmitter<{ param; facilityId: any }>();
  @Output() createOrEditPointModalShowEvent = new EventEmitter<{ index: number; goodDetails: string; goodsDetailListDto?: any }>();
  @Output('savedAppointmentsAndClearance') savedAppointmentsAndClearance = new EventEmitter<{
    tripAppointment: TripAppointmentDataDto;
    tripClearance: TripClearancePricesDto;
    pointIndex: number;
  }>();

  constructor(
    injector: Injector,
    private _PointsService: PointsService,
    private _shippingRequestsTripServiceProxy: ShippingRequestsTripServiceProxy,
    public _tripService: TripService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.wayPointsList.map((item, index) => (item.pointOrder = index + 1));
  }

  RouteStepCordSetter(index: number, facilityId: number) {
    this.RouteStepCordSetterEvent.emit({ index, facilityId });
    if (
      ((index < 3 && !this.isExportRequest) || (index > 0 && index < 5 && this.isExportRequest)) &&
      this.wayPointsList[index + 1] &&
      this.wayPointsList[index + 1].pickingType === PickingType.Pickup
    ) {
      this.wayPointsList[index + 1].facilityId = facilityId;
    }
    this.wayPointsList[index].dropNeedsAppointment = this.wayPointsList[index].dropNeedsClearance = false;
    // console.log('RouteStepCordSetter', index, facilityId);
  }

  wayPointsSetter() {
    this.wayPointsSetterEvent.emit(true);
    // console.log('wayPointsSetter');
  }

  loadReceivers(facilityId: number) {
    this.loadReceiversEvent.emit(facilityId);
    // console.log('loadReceivers', facilityId);
  }

  onChangedWayPointsList() {
    this.onChangedWayPointsListEvent.emit(true);
    // console.log('onChangedWayPointsList');
  }

  createOrEditFacilityModalShow() {
    this.createOrEditFacilityModalShowEvent.emit(true);
    // console.log('createOrEditFacilityModalShow');
  }

  createOrEditReceiverModalShow(param, facilityId: any) {
    this.createOrEditReceiverModalShowEvent.emit({ param, facilityId });
    // console.log('createOrEditReceiverModalShow', param, facilityId);
  }

  createOrEditPointModalShow(index: number, goodDetails: string, goodsDetailListDto?: any) {
    this._PointsService.currentPointIndex = index;
    this.createOrEditPointModalShowEvent.emit({ index, goodDetails, goodsDetailListDto });
    // console.log('createOrEditPointModalShow', index, goodDetails, goodsDetailListDto);
  }

  showVasModal(index: number) {
    console.log('showVasModal');
    //console.log(this.usedIn, 'this.usedIn');
    this.activePointIndex = index;
    let test = this.usedIn == 'createOrEdit' ? this.wayPointsList[index].dropNeedsClearance : (this.wayPointsList[index] as any).needsClearance;
    let test2 = this.usedIn == 'createOrEdit' ? this.wayPointsList[index].dropNeedsAppointment : (this.wayPointsList[index] as any).needsAppointment;
    console.log('this.wayPointsList[index].dropNeedsClearance', this.wayPointsList[index].dropNeedsClearance);
    this.appointmentAndClearanceModal.show(
      this.wayPointsList[index].id,
      test,
      test2,
      this.wayPointsList[index].appointmentDataDto,
      this.wayPointsList[index].tripClearancePricesDto,
      this.usedIn
    );
  }

  isFacilityDisabled(index: number): boolean {
    // this.wayPointsList[index];
    if (this.isExportRequest && index === 0) {
      return false;
    } else if (this.isImportWithStorage && index === 5) {
      return false;
    } else if (this.isImportWithStorage && index === 4) {
      this.wayPointsList[index].facilityId = this.wayPointsList[index - 1].facilityId;
      return true;
    }
    return index % 2 === 0;
  }

  filterFacilitiesForDropDown(isPickup: boolean, index: number) {
    // console.log('filterFacilitiesForDropDown called with isPickup:', isPickup, 'index:', index, 'isExportRequest:', this.isExportRequest);
    if (this.isExportRequest) {
      return this.filterExportFacilities(isPickup, index);
    } else {
      return this.filterNonExportFacilities(isPickup, index);
    }
  }

  filterExportFacilities(isPickup: boolean, index: number) {
    // console.log('filterExportFacilities called with isPickup:', isPickup, 'index:', index);
    if (isPickup && index === 0) {
      return this.filterPickupFacilities(index, FacilityType.Facility);
    } else {
      return this.filterDropFacilities(index, {
        1: this.roundTripType === RoundTripType.OneWayRoutWithoutPortShuttling ? FacilityType.Port : FacilityType.Facility,
        3: FacilityType.Facility,
        5: FacilityType.Port,
      });
    }
  }

  filterNonExportFacilities(isPickup: boolean, index: number) {
    if (isPickup && index === 0) {
      return this.filterPickupFacilities(index, FacilityType.Port);
    } else if (this.isImportWithStorage && index === 5) {
      this.wayPointsList[5].facilityId = this.wayPointsList[0].facilityId;
      return this.filterPickupFacilities(index, FacilityType.Port);
    } else if (this.isImportWithStorage && index === 4) {
      return this.filterDropFacilities(index, {
        1: FacilityType.Facility,
        2: true,
        3: true,
      });
    } else {
      return this.filterDropFacilities(index, {
        1: FacilityType.Facility,
        2: true,
        3: true,
      });
    }
  }

  filterPickupFacilities(index: number, facilityType: FacilityType) {
    return this.pickupFacilities
      .filter((fac) => this.isCityMatching(fac.cityId))
      .filter((fac) => (index === 0 ? fac.facilityType === facilityType : true));
  }

  filterDropFacilities(index: number, facilityTypeMap: { [key: number]: FacilityType | boolean }) {
    return this.dropFacilities
      .filter((fac) => this.isCityInDestination(fac.cityId))
      .filter((fac) => facilityTypeMap[index] === true || fac.facilityType === facilityTypeMap[index]);
  }

  isCityMatching(cityId: number): boolean {
    const shippingRequest = this._tripService.GetShippingRequestForViewOutput?.shippingRequest;
    const originCityId = this._tripService.CreateOrEditShippingRequestTripDto?.originCityId;
    const originalCityId = this._tripService.GetShippingRequestForViewOutput?.originalCityId;
    return !shippingRequest?.id ? cityId == originCityId : cityId == originalCityId;
  }

  isCityInDestination(cityId: number): boolean {
    const shippingRequest = this._tripService.GetShippingRequestForViewOutput?.shippingRequest;
    console.log(
      'this._tripService.CreateOrEditShippingRequestTripDto?.shippingRequestDestinationCities',
      this._tripService.CreateOrEditShippingRequestTripDto?.shippingRequestDestinationCities
    );
    console.log(
      'this._tripService.GetShippingRequestForViewOutput?.destinationCitiesDtos',
      this._tripService.GetShippingRequestForViewOutput?.destinationCitiesDtos
    );
    const destinationCityIds = !shippingRequest?.id
      ? this._tripService.CreateOrEditShippingRequestTripDto?.shippingRequestDestinationCities?.map((city) => city.cityId)
      : this._tripService.GetShippingRequestForViewOutput?.destinationCitiesDtos?.map((city) => city.cityId);
    console.log('destinationCityIds', destinationCityIds, cityId, destinationCityIds?.includes(cityId));
    return destinationCityIds?.includes(cityId) ?? false;
  }

  selectContact(index: number) {
    if (index % 2 === 0 || this.roundTripType === RoundTripType.WithReturnTrip) {
      return;
    }
    if (((index < 3 && !this.isExportRequest) || (index > 0 && index < 5 && this.isExportRequest)) && this.wayPointsList[index + 1]) {
      this.wayPointsList[index + 1].receiverId = this.wayPointsList[index].receiverId;
    }
  }

  showAppointmentsAndClearanceButton(index): boolean {
    let shippingRequestId = this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.id;
    if (this.isImportWithStorage && (index == 0 || index == 4)) {
      return true;
    }

    // if (
    //   (this.usedIn == 'createOrEdit' && !this.isTachyonDealer) ||
    //   (!shippingRequestId && !this.hasShipperClients && !this.hasCarrierClients && !this.isEdit)
    // ) {
    //   return false;
    // }
    if (this.usedIn != 'createOrEdit') {
      return (this.wayPointsList[index] as any).needsClearance || (this.wayPointsList[index] as any).needsAppointment;
    }

    if (!this.isExportRequest && !this.isImportWithStorage) {
      switch (index) {
        case 3:
        case 0: {
          return true;
        }
        default: {
          return false;
        }
      }
    }
    if (this.isExportRequest) {
      switch (index) {
        case 1: {
          return this.roundTripType === RoundTripType.OneWayRoutWithoutPortShuttling;
        }
        case 5: {
          return this.roundTripType === RoundTripType.TwoWayRoutsWithPortShuttling;
        }
        default: {
          return false;
        }
      }
    }
    return false;
  }

  handleSaveAppointmentsAndClearance(event: { tripAppointment: TripAppointmentDataDto; tripClearance: TripClearancePricesDto }) {
    this.savedAppointmentsAndClearance.emit({ ...event, pointIndex: this.activePointIndex });
  }

  carrierSetClearanceData($event: TripClearancePricesDto) {
    this._shippingRequestsTripServiceProxy.carrierSetClearanceData(this.wayPointsList[this.activePointIndex].id, $event).subscribe((res) => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.wayPointsList[this.activePointIndex].tripClearancePricesDto = $event;
    });
  }

  carrierSetAppointmentData($event: TripAppointmentDataDto) {
    this._shippingRequestsTripServiceProxy.carrierSetAppointmentData(this.wayPointsList[this.activePointIndex].id, $event).subscribe((res) => {
      this.notify.success(this.l('SavedSuccessfully'));
      this.wayPointsList[this.activePointIndex].appointmentDataDto = $event;
    });
  }

  facilitiesDataSource(record, index) {
    let isTwoWayRoutsWithPortShuttling =
      this._tripService.CreateOrEditShippingRequestTripDto.roundTripType == RoundTripType.TwoWayRoutsWithPortShuttling;
    this.allfacilities = [...this.dropFacilities, ...this.pickupFacilities];

    if ((index === 2 || index === 4) && isTwoWayRoutsWithPortShuttling) {
      return this.allfacilities;
    }
    if (index === 5 && this.isImportWithStorage && !isNotNullOrUndefined(this.allfacilities)) {
      //console.log('All Facilities is being returend', [...this.dropFacilities, ...this.pickupFacilities]);
      // this.allfacilities = [...this.dropFacilities, ...this.pickupFacilities];
      return this.allfacilities;
    } else if (index === 4 && this.isImportWithStorage) {
      return this.filteredDropFacilities[index - 1];
    } else {
      // console.log(index);
      // console.log(this.filteredDropFacilities[index]);
      return record.pickingType === PickingType.Pickup ? this.filteredPickupFacilities[index] : this.filteredDropFacilities[index];
    }
  }

  getClassForCreateOrEdit(record: any) {
    const shippingRequestId = this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.id;
    console.log('shippingRequestId && !this.isShipper', shippingRequestId && !this.isShipper);
    // shipping request                                     // direct trip
    if ((shippingRequestId && !this.isShipper) || (this.hasShipperClients && this.hasCarrierClients)) {
      if ((!record.appointmentDataDto || !record.tripClearancePricesDto) && !this.isEdit) {
        return 'fa fa-plus';
      } else {
        return 'fa fa-pen';
      }
    } else {
      return 'fa fa-eye';
    }
  }

  getClassForView(record: any) {
    // Extract the shipping request ID
    const shippingRequestId = this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.id;

    // Condition 1: Shipping request exists and user is not a shipper
    if (shippingRequestId && !this.isShipper) {
      return 'fa fa-pen';
    }

    // Condition 2: No shipping request ID, but both shipper and carrier clients exist
    if (!shippingRequestId && this.hasShipperClients && this.hasCarrierClients) {
      // Check if appointment or clearance is needed but data is missing
      const needsAppointmentWithoutData = record.needsAppointment && !record.appointmentDataDto;
      const needsClearanceWithoutData = record.needsClearance && !record.tripClearancePricesDto;

      if (needsAppointmentWithoutData || needsClearanceWithoutData) {
        return 'fa fa-pen';
      }
    }

    // Default icon for viewing only
    return 'fa fa-eye';
  }

  get StorageDetailsButtonClass(): boolean {
    const point = this.wayPointsList[3];
    return !!(point.driverUserId || point.truckId || point.storageDays || point.storagePricePerDay);
  }
}
