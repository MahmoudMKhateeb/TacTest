/* tslint:disable:triple-equals */
import { AfterContentChecked, ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrEditRoutPointDto,
  DropPaymentMethod,
  FacilityForDropdownDto,
  FacilityType,
  PickingType,
  ReceiverFacilityLookupTableDto,
  ReceiversServiceProxy,
  RoundTripType,
  RoutStepsServiceProxy,
  ShippingRequestDestinationCitiesDto,
  ShippingRequestDto,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingTypeEnum,
  TripAppointmentDataDto,
  TripClearancePricesDto,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { Subscription } from 'rxjs';
import { finalize } from '@node_modules/rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'PointsComponent',
  templateUrl: './points.component.html',
  styleUrls: ['./points.component.scss'],
})
export class PointsComponent extends AppComponentBase implements OnInit, OnDestroy, AfterContentChecked {
  // @Output() SelectedWayPointsFromChild = this.wayPointsList;
  @Input('isPortMovement') isPortMovement = false;
  @Input('isEdit') isEdit = false;
  @Input() shippingRequest: ShippingRequestDto;
  @Output() wayPointsListChanged: EventEmitter<any> = new EventEmitter<any>();
  shippingRequestId: number;
  DestCitiesDtos: ShippingRequestDestinationCitiesDto[];
  SRDestionationCity: number;
  allFacilities: FacilityForDropdownDto[] = [];
  pickupFacilities: FacilityForDropdownDto[] = [];
  dropFacilities: FacilityForDropdownDto[] = [];
  allPointsSendersAndREcivers: ReceiverFacilityLookupTableDto[][] = [];
  receiverLoading: boolean;
  //shippingRequestForView: GetShippingRequestForViewOutput;
  //activeTripId: number;
  RouteTypes = ShippingRequestRouteType;
  wayPointsList: CreateOrEditRoutPointDto[] = [];

  PickingType = PickingType;
  active = false;
  saving = false;
  facilityLoading = false;
  loading = false;
  zoom: Number = 13; //map zoom
  //this dir is for Single Route Step Map Route Draw
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  dir = { point: { lat: undefined, lng: undefined } };
  wayPoints = [];
  wayPointMapSource = undefined;
  wayPointMapDest = undefined;
  id: number;
  Point: CreateOrEditRoutPointDto;
  private pointsServiceSubscription$: Subscription;
  private tripDestFacilitySub$: Subscription;
  //private tripSourceFacilitySub$: Subscription;
  private currentActiveTripSubs$: Subscription;
  private pointServiceSubs$: Subscription;
  usedIn: 'view' | 'createOrEdit';
  @Output() SelectedWayPointsFromChild = this.wayPointsList;
  @Input('isHomeDelivery') isHomeDelivery: boolean;
  shippingRequestFlagEnum = ShippingRequestFlag;
  paymentMethodsArray = [];
  RoundTripType = RoundTripType;
  ShippingTypeEnum = ShippingTypeEnum;

  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _receiversServiceProxy: ReceiversServiceProxy,
    public _tripService: TripService,
    private _PointsService: PointsService,
    private cdref: ChangeDetectorRef,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit() {
    this.loadSharedServices();
    this.loadDropDowns();
    this.paymentMethodsArray = this.enumToArray.transform(DropPaymentMethod);
    this.pointServiceSubs$ = this._PointsService.currentSingleWayPoint.subscribe((res) => {
      this.onChangedWayPointsList();
    });
  }

  loadDropDowns() {
    this.loadFacilities();
  }

  /**
   * loads Facilities with validation on it related to source and destination in SR
   */
  loadFacilities() {
    if (!isNotNullOrUndefined(this._tripService.GetShippingRequestForViewOutput)) {
      // direct trip
      this._routStepsServiceProxy
        .getAllFacilitiesForDirectTrip()
        .pipe(
          finalize(() => {
            this.facilityLoading = false;
          })
        )
        .subscribe((result) => {
          this.allFacilities = result;

          let portMovement =
            this._tripService?.CreateOrEditShippingRequestTripDto?.shippingTypeId == ShippingTypeEnum.ExportPortMovements ||
            this._tripService?.CreateOrEditShippingRequestTripDto?.shippingTypeId == ShippingTypeEnum.ImportPortMovements;

          if (portMovement) {
            this.dropFacilities = result;
          } else {
            this.dropFacilities = result.filter((fac) => {
              return this._tripService.CreateOrEditShippingRequestTripDto?.shippingRequestDestinationCities
                ?.map((city) => city.cityId)
                .includes(fac.cityId);
            });
          }

          if (portMovement) {
            this.pickupFacilities = result;
          } else {
            this.pickupFacilities = result.filter((r) => {
              return r.cityId == this._tripService.CreateOrEditShippingRequestTripDto?.originCityId;
            });
            if (this._tripService.CreateOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.LocalInsideCity) {
              this.dropFacilities = this.pickupFacilities;
            }
          }
        });

      return;
    }
    this.DestCitiesDtos = this._tripService.GetShippingRequestForViewOutput.destinationCitiesDtos;

    if (!this.isCarrier) {
      this._routStepsServiceProxy
        .getAllFacilitiesByCityAndTenantForDropdown(this._tripService.GetShippingRequestForViewOutput?.shippingRequest?.id)
        .pipe(
          finalize(() => {
            this.facilityLoading = false;
          })
        )
        .subscribe((result) => {
          this.allFacilities = result;
          this.dropFacilities = result;
          this.pickupFacilities = result.filter((r) => {
            if (isNotNullOrUndefined(this._tripService.GetShippingRequestForViewOutput)) {
              return this._tripService.GetShippingRequestForViewOutput?.shippingRequestFlag === 0
                ? r.cityId == this._tripService.GetShippingRequestForViewOutput?.originalCityId
                : this.DestCitiesDtos.some((y) => y.cityId == r.cityId);
            } else {
              return r.cityId == this._tripService.CreateOrEditShippingRequestTripDto?.originCityId;
            }
          });
        });
    }
  }

  /**
   * loads a list of Receivers by facility Id
   * @param facilityId
   */
  loadReceivers(facilityId, bulkload?: boolean) {
    this.receiverLoading = true;
    if (facilityId) {
      this._receiversServiceProxy.getAllReceiversByFacilityForTableDropdown(facilityId).subscribe((result) => {
        this.allPointsSendersAndREcivers[facilityId] = result;
      });
    }

    if (bulkload) {
      this.wayPointsList.forEach((x) => {
        let id = x.facilityId;
        this._receiversServiceProxy.getAllReceiversByFacilityForTableDropdown(id).subscribe((result) => {
          this.allPointsSendersAndREcivers[id] = result;
        });
      });
    }
    this.receiverLoading = false;
  }

  wayPointsSetter() {
    this.wayPointMapSource = undefined;
    this.wayPoints = [];
    this.wayPointMapDest = undefined;
    //take the first Point in the List and Set it As The source
    if (
      isNotNullOrUndefined(this.wayPointsList) &&
      this.wayPointsList.length > 0 &&
      isNotNullOrUndefined(this.wayPointsList[0]?.latitude) &&
      isNotNullOrUndefined(this.wayPointsList[0]?.longitude)
    ) {
      this.wayPointMapSource = {
        lat: this.wayPointsList[0]?.latitude || undefined,
        lng: this.wayPointsList[0]?.longitude || undefined,
      };
    }
    //Take Any Other Points but the First And last one in the List and set them to way points
    for (let i = 1; i < this.wayPointsList.length - 1; i++) {
      this.wayPoints.push({
        location: {
          lat: this.wayPointsList[i].latitude,
          lng: this.wayPointsList[i].longitude,
        },
      });
    }
    //to avoid the source and Dest from becoming the Same when place the First Elem in wayPointsList
    if (
      isNotNullOrUndefined(this.wayPointsList) &&
      this.wayPointsList.length > 1 &&
      isNotNullOrUndefined(this.wayPointsList[this.wayPointsList.length - 1]?.latitude) &&
      isNotNullOrUndefined(this.wayPointsList[this.wayPointsList.length - 1]?.longitude)
    ) {
      //set the Dest
      this.wayPointMapDest = {
        lat: this.wayPointsList[this.wayPointsList.length - 1]?.latitude || undefined,
        lng: this.wayPointsList[this.wayPointsList.length - 1]?.longitude || undefined,
      };
    }
  }

  onChangedWayPointsList() {
    this.wayPointsListChanged.emit(null);
  }

  /**
   * creates empty points for the trip based on number of drops
   */
  createEmptyPoints(selectedPaymentMethodId?: number) {
    let numberOfDrops = 0;
    if (!this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.id) {
      numberOfDrops = this._tripService.CreateOrEditShippingRequestTripDto?.numberOfDrops;
    } else {
      numberOfDrops = this._tripService.GetShippingRequestForViewOutput?.shippingRequest?.numberOfDrops;
    }

    //if there is already wayPoints Dont Create Empty Once
    if (this.wayPointsList.length == numberOfDrops + 1) return;
    // for (let i = 0; i <= numberOfDrops; i++) {
    //   let point = new CreateOrEditRoutPointDto();
    //   //pickup Point
    //   if (i === 0) {
    //     point.pickingType = this.PickingType.Pickup;
    //   } else {
    //     point.pickingType = this.PickingType.Dropoff;
    //   }
    //   point.dropPaymentMethod = selectedPaymentMethodId;
    //   point.needsPOD = false;
    //   point.needsReceiverCode = false;
    //   this.wayPointsList.push(point);
    // } //end of for
    if (!this.isPortMovement) {
      this.addPointsToWayPointList(numberOfDrops, selectedPaymentMethodId);
      return;
    } else {
      this.wayPointsList.map((item, index) => (item.pointOrder = index + 1));
    }
    for (let i = 0; i < numberOfDrops; i++) {
      this.addPointsToWayPointList(1, selectedPaymentMethodId);
    }
    if (
      (this._tripService?.CreateOrEditShippingRequestTripDto?.shippingTypeId === ShippingTypeEnum.ImportPortMovements ||
        this._tripService?.GetShippingRequestForViewOutput?.shippingRequest?.shippingTypeId === ShippingTypeEnum.ImportPortMovements) &&
      !this.isCarrier
    ) {
      this.wayPointsList[0].facilityId = this._tripService.GetShippingRequestForViewOutput.shippingRequest.originFacilityId;
      this.loadReceivers(this._tripService.GetShippingRequestForViewOutput.shippingRequest.originFacilityId);
    }
  }

  private addPointsToWayPointList(numberOfDrops: number, selectedPaymentMethodId: number) {
    for (let i = 0; i <= numberOfDrops; i++) {
      let point = new CreateOrEditRoutPointDto();
      //pickup Point
      if (i === 0) {
        point.pickingType = this.PickingType.Pickup;
      } else {
        point.pickingType = this.PickingType.Dropoff;
      }
      point.dropPaymentMethod = selectedPaymentMethodId;
      point.needsPOD = false;
      point.needsReceiverCode = false;
      this.wayPointsList.push(point);
    }
  }

  private loadSharedServices() {
    this.pointsServiceSubscription$ = this._PointsService.currentWayPointsList.subscribe((res) => {
      this.wayPointsList = res;
      if (res.length > 0) {
        this.wayPointsSetter();
      }
    });
    //if action is edit trip get active Trip id
    //this.currentActiveTripSubs$ = this._tripService.currentActiveTripId.subscribe((res) => (this.activeTripId = res));
    //get some Stuff from ShippingRequest Dto
    // this._tripService.currentShippingRequest.subscribe((res) => {

    //   if (res?.shippingRequest) {
    //     this._tripService.GetShippingRequestForViewOutput = res;
    //   }
    // });

    this._PointsService.currentUsedIn.subscribe((res) => {
      this.usedIn = res;
    });
    this.createEmptyPoints();
  }

  /**
   * Extracts Facility Coordinates for single Point (Map Drawing)
   */
  RouteStepCordSetter(pointIndex: number, facilityId: number) {
    this.wayPointsList[pointIndex].latitude = this.allFacilities.find((x) => x.id == facilityId)?.lat;
    this.wayPointsList[pointIndex].longitude = this.allFacilities.find((x) => x.id == facilityId)?.long;
  }

  savedAppointmentsAndClearance($event: { tripAppointment: TripAppointmentDataDto; tripClearance: TripClearancePricesDto; pointIndex: number }) {
    this.wayPointsList[$event.pointIndex].appointmentDataDto = $event.tripAppointment;
    this.wayPointsList[$event.pointIndex].tripClearancePricesDto = $event.tripClearance;
  }

  filterFacilitiesForDropDown(isPickup: boolean) {
    return isPickup
      ? this.pickupFacilities.filter((fac) => fac.facilityType === FacilityType.Facility)
      : this.dropFacilities.filter((fac) => fac.facilityType === FacilityType.Facility);
  }

  ngOnDestroy() {
    this.pointServiceSubs$?.unsubscribe();
    this.pointsServiceSubscription$?.unsubscribe();
    this.tripDestFacilitySub$?.unsubscribe();
    // this.tripSourceFacilitySub$?.unsubscribe();
    this.currentActiveTripSubs$?.unsubscribe();
  }

  ngAfterContentChecked() {
    // this.wayPointsList.forEach((x) => {
    //   return ((x.receiverId as any) = x.receiverId.toString());
    // });
    this.cdref.detectChanges();
  }
}
