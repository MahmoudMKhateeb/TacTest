/* tslint:disable:triple-equals */
import { ChangeDetectorRef, Component, Injector, OnDestroy, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrEditRoutPointDto,
  FacilityForDropdownDto,
  GetShippingRequestForViewOutput,
  PickingType,
  ReceiverFacilityLookupTableDto,
  ReceiversServiceProxy,
  RoutStepsServiceProxy,
  ShippingRequestRouteType,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { Subscription } from 'rxjs';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'PointsComponent',
  templateUrl: './points.component.html',
  styleUrls: ['./points.component.scss'],
})
export class PointsComponent extends AppComponentBase implements OnInit, OnDestroy {
  cityDestId: number;
  allFacilities: FacilityForDropdownDto[];
  pickupFacilities: FacilityForDropdownDto[];
  dropFacilities: FacilityForDropdownDto[];
  allPointsSendersAndREcivers: ReceiverFacilityLookupTableDto[][] = [];
  receiverLoading: boolean;
  shippingRequestForView: GetShippingRequestForViewOutput;
  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _receiversServiceProxy: ReceiversServiceProxy,
    public _tripService: TripService,
    private _PointsService: PointsService,
    private cdref: ChangeDetectorRef
  ) {
    super(injector);
  }
  activeTripId: number;
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
  private tripSourceFacilitySub$: Subscription;
  private currentActiveTripSubs$: Subscription;
  usedIn: 'view' | 'createOrEdit';
  @Output() SelectedWayPointsFromChild = this.wayPointsList;

  ngOnDestroy() {
    this.pointsServiceSubscription$?.unsubscribe();
    this.tripDestFacilitySub$?.unsubscribe();
    this.tripSourceFacilitySub$?.unsubscribe();
    this.currentActiveTripSubs$?.unsubscribe();
    console.log('Unsubscribed/Destroid from  Point Component');
  }

  ngOnInit() {
    this.loadSharedServices();
    this.loadDropDowns();
  }

  loadDropDowns() {
    this.loadFacilities();
  }

  /**
   * loads Facilities with validation on it related to source and destination in SR
   */
  loadFacilities() {
    if (!this.shippingRequestForView.shippingRequest.id) return;
    if (this.shippingRequestForView.shippingRequest.id != null) {
      this._tripService.currentShippingRequest.subscribe((res) => {
        this.shippingRequestForView = res;
      });
    }
    this._routStepsServiceProxy
      .getAllFacilitiesByCityAndTenantForDropdown(this.shippingRequestForView.shippingRequest.id)
      .pipe(
        finalize(() => {
          this.facilityLoading = false;
        })
      )
      .subscribe((result) => {
        this.allFacilities = result;
        this.pickupFacilities = result.filter((r) => r.cityId == this.shippingRequestForView.originalCityId);
        this.dropFacilities = result.filter((r) => r.cityId == this.shippingRequestForView.destinationCityId);
      });
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
    this.wayPointMapSource = {
      lat: this.wayPointsList[0]?.latitude || undefined,
      lng: this.wayPointsList[0]?.longitude || undefined,
    };
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
    if (this.wayPointsList.length > 1) {
      //set the Dest
      this.wayPointMapDest = {
        lat: this.wayPointsList[this.wayPointsList.length - 1]?.latitude || undefined,
        lng: this.wayPointsList[this.wayPointsList.length - 1]?.longitude || undefined,
      };
    }
  }

  /**
   * creates empty points for the trip based on number of drops
   */
  createEmptyPoints() {
    let numberOfDrops = this.shippingRequestForView.shippingRequest.numberOfDrops;
    //if there is already wayPoints Dont Create Empty Once
    if (this.wayPointsList.length == numberOfDrops + 1) return;
    for (let i = 0; i <= numberOfDrops; i++) {
      let point = new CreateOrEditRoutPointDto();
      //pickup Point
      if (i === 0) {
        point.pickingType = this.PickingType.Pickup;
      } else {
        point.pickingType = this.PickingType.Dropoff;
      }
      this.wayPointsList.push(point);
    } //end of for
  }

  private loadSharedServices() {
    this.pointsServiceSubscription$ = this._PointsService.currentWayPointsList.subscribe((res) => {
      this.wayPointsList = res;
      if (res.length > 0) {
        this.wayPointsSetter();
      }
    });
    //if action is edit trip get active Trip id
    this.currentActiveTripSubs$ = this._tripService.currentActiveTripId.subscribe((res) => (this.activeTripId = res));
    //get some Stuff from ShippingRequest Dto
    this.tripSourceFacilitySub$ = this._tripService.currentShippingRequest.subscribe((res) => {
      if (res.shippingRequest) {
        this.shippingRequestForView = res;
      }
    });

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

  ngAfterContentChecked() {
    this.cdref.detectChanges();
  }
}
