import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrEditRoutPointDto,
  FacilityForDropdownDto,
  FacilityType,
  PickingType,
  ReceiverFacilityLookupTableDto,
  RoundTripType,
  ShippingRequestsTripServiceProxy,
  TripAppointmentDataDto,
  TripClearancePricesDto,
} from '@shared/service-proxies/service-proxies';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { AppointmentAndClearanceModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/appointment-and-clearance/appointment-and-clearance.component';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

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
  @Input('isEdit') isEdit = false;
  @Input('wayPointsList') wayPointsList: CreateOrEditRoutPointDto[] = [];
  _pickupFacilities: FacilityForDropdownDto[] = [];
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
    console.log('PointsForPortsMovementComponent');
    this.wayPointsList.map((item, index) => (item.pointOrder = index + 1));
    console.log('wayPointsList', this.wayPointsList);
    console.log('usedIn', this.usedIn);
    console.log('pickupFacilities', this.pickupFacilities);
    console.log('dropFacilities', this.dropFacilities);
    console.log('allPointsSendersAndReceivers', this.allPointsSendersAndReceivers);
    console.log('facilityLoading', this.facilityLoading);
    console.log('receiverLoading', this.receiverLoading);
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
    console.log('RouteStepCordSetter', index, facilityId);
  }

  wayPointsSetter() {
    this.wayPointsSetterEvent.emit(true);
    console.log('wayPointsSetter');
  }

  loadReceivers(facilityId: number) {
    this.loadReceiversEvent.emit(facilityId);
    console.log('loadReceivers', facilityId);
  }

  onChangedWayPointsList() {
    this.onChangedWayPointsListEvent.emit(true);
    console.log('onChangedWayPointsList');
  }

  createOrEditFacilityModalShow() {
    this.createOrEditFacilityModalShowEvent.emit(true);
    console.log('createOrEditFacilityModalShow');
  }

  createOrEditReceiverModalShow(param, facilityId: any) {
    this.createOrEditReceiverModalShowEvent.emit({ param, facilityId });
    console.log('createOrEditReceiverModalShow', param, facilityId);
  }

  createOrEditPointModalShow(index: number, goodDetails: string, goodsDetailListDto?: any) {
    this._PointsService.currentPointIndex = index;
    this.createOrEditPointModalShowEvent.emit({ index, goodDetails, goodsDetailListDto });
    console.log('createOrEditPointModalShow', index, goodDetails, goodsDetailListDto);
  }

  showVasModal(index: number) {
    console.log('showVasModal');
    this.activePointIndex = index;
    this.appointmentAndClearanceModal.show(
      this.wayPointsList[index].id,
      this.usedIn == 'createOrEdit' ? this.wayPointsList[index].dropNeedsClearance : (this.wayPointsList[index] as any).needsClearance,
      this.usedIn == 'createOrEdit' ? this.wayPointsList[index].dropNeedsAppointment : (this.wayPointsList[index] as any).needsAppointment,
      this.wayPointsList[index].appointmentDataDto,
      this.wayPointsList[index].tripClearancePricesDto,
      this.usedIn
    );
  }

  isFacilityDisabled(index: number): boolean {
    // this.wayPointsList[index];
    if (this.isExportRequest && index === 0) {
      return false;
    }
    return index % 2 === 0;
  }

  filterFacilitiesForDropDown(isPickup: boolean, index: number) {
    if (!this.isExportRequest) {
      return isPickup && index === 0
        ? this.pickupFacilities
            .filter((fac) => {
              if (!this._tripService.GetShippingRequestForViewOutput?.shippingRequest?.id) {
                return fac.cityId == this._tripService.CreateOrEditShippingRequestTripDto?.originCityId;
              }
              return fac.cityId == this._tripService.GetShippingRequestForViewOutput?.originalCityId;
            })
            .filter((fac) => {
              switch (index) {
                case 0: {
                  return fac.facilityType === FacilityType.Port;
                }
                default: {
                  return true;
                }
              }
            })
        : this.dropFacilities
            .filter((fac) => {
              if (!this._tripService.GetShippingRequestForViewOutput?.shippingRequest?.id) {
                return this._tripService.CreateOrEditShippingRequestTripDto?.shippingRequestDestinationCities
                  .map((city) => city.cityId)
                  .includes(fac.cityId);
              }
              return this._tripService.GetShippingRequestForViewOutput?.destinationCitiesDtos.map((city) => city.cityId).includes(fac.cityId);
            })
            .filter((fac) => {
              switch (index) {
                case 1: {
                  return fac.facilityType === FacilityType.Facility;
                }
                default:
                case 2:
                case 3: {
                  return true;
                }
              }
            });
    }
    return isPickup && index === 0
      ? this.pickupFacilities
          .filter((fac) => {
            if (!this._tripService.GetShippingRequestForViewOutput?.shippingRequest?.id) {
              return fac.cityId == this._tripService.CreateOrEditShippingRequestTripDto?.originCityId;
            }
            return fac.cityId == this._tripService.GetShippingRequestForViewOutput?.originalCityId;
          })
          .filter((fac) => {
            switch (index) {
              case 0: {
                return fac.facilityType === FacilityType.Facility;
              }
              default: {
                return true;
              }
            }
          })
      : this.dropFacilities
          .filter((fac) => {
            if (!this._tripService.GetShippingRequestForViewOutput?.shippingRequest?.id) {
              return this._tripService.CreateOrEditShippingRequestTripDto?.shippingRequestDestinationCities
                .map((city) => city.cityId)
                .includes(fac.cityId);
            }
            return this._tripService.GetShippingRequestForViewOutput?.destinationCitiesDtos.map((city) => city.cityId).includes(fac.cityId);
          })
          .filter((fac) => {
            switch (index) {
              case 1: {
                return this.roundTripType === RoundTripType.OneWayRoutWithoutPortShuttling
                  ? fac.facilityType === FacilityType.Port
                  : fac.facilityType === FacilityType.Facility;
              }
              default:
              case 3: {
                return fac.facilityType === FacilityType.Facility;
              }
              case 5: {
                return fac.facilityType === FacilityType.Port;
              }
            }
          });
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
    if (
      this.usedIn == 'createOrEdit' &&
      ((isNotNullOrUndefined(this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.id) && !this.isTachyonDealer) ||
        (!this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.id && !this.hasShipperClients && !this.hasCarrierClients)) &&
      !this.isEdit
    ) {
      return false;
    }
    if (this.usedIn != 'createOrEdit') {
      return (this.wayPointsList[index] as any).needsClearance || (this.wayPointsList[index] as any).needsAppointment;
    }
    if (!this.isExportRequest) {
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
}
