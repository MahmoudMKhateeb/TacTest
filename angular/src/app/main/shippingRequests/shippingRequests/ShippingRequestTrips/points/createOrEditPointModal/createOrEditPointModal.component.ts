/* tslint:disable:triple-equals */
import { Component, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
    CreateOrEditRoutPointDto,
    FacilitiesServiceProxy,
    FacilityForDropdownDto,
    PickingType,
    ReceiverFacilityLookupTableDto,
    ReceiversServiceProxy,
    RoutStepsServiceProxy,
    ShippingRequestRouteType,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'createOrEditPointModal',
  styleUrls: ['./createOrEditPointModal.component.css'],
  templateUrl: './createOrEditPointModal.component.html',
})
export class CreateOrEditPointModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  constructor(
    injector: Injector,
    public _tripService: TripService,
    private _PointService: PointsService,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _receiversServiceProxy: ReceiversServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy
  ) {
    super(injector);
  }
  @ViewChild('createRouteStepModal', { static: true }) modal: ModalDirective;
  // @ViewChild('PointGoodDetailsComponent') public PointGoodDetailsComponent: GoodDetailsComponent;
  @ViewChild('createOrEditPintForm') public createOrEditPintForm: NgForm;

  allFacilities: FacilityForDropdownDto[];
  allReceivers: ReceiverFacilityLookupTableDto[];

  RouteType: number; //filled in onInit from the Trip Shared Service
  PickingType = PickingType;
  RouteTypes = ShippingRequestRouteType;

  zoom: Number = 13; //map zoom
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  wayPointsList: CreateOrEditRoutPointDto[];
  Point: CreateOrEditRoutPointDto;

  wayPointVariableForTesting: any;
  active = false;
  facilityLoading = false;
  receiversLoading = false;
  shippingRequestId: number;
  citySourceId: number;
  cityDestId: number;
  pointsCount: number;
  isAdditionalReceiverEnabled: boolean;
  pointIdForEdit = null;
  usedIn: 'view' | 'createOrEdit';

  tripServiceSubscription$: any;
  pointsServiceSubscription$: any;
  usedInSubscription$: any;
  ngOnDestroy() {
    this.tripServiceSubscription$.unsubscribe();
    this.pointsServiceSubscription$.unsubscribe();
    this.usedInSubscription$.unsubscribe();
    console.log('Unsubscribed/Destroid from CreateOrEdit Point Modal');
  }

  ngOnInit(): void {
    //take the Route Type From the Shared Service
    this.tripServiceSubscription$ = this._tripService.currentShippingRequest.subscribe((res) => {
      this.RouteType = res.shippingRequest.routeTypeId;
      this.shippingRequestId = res.shippingRequest.id;
    });
    //take the PointsList From The Shared Service
    this.pointsServiceSubscription$ = this._PointService.currentWayPointsList.subscribe((res) => (this.wayPointsList = res));
    //Where the using of this Component is coming from
    this.usedInSubscription$ = this._PointService.currentUsedIn.subscribe((res) => (this.usedIn = res));
    console.log('used in from Create Or Edit Point ', this.usedIn);

    this._PointService.currentSingleWayPoint.subscribe((res) => {
      this.Point = res;
      this.wayPointVariableForTesting = res;
    });
    //this.feature.isEnabled('App.Shipper') ? this.loadFacilities() : 0;

    // setInterval(() => {
    //   console.log('current Point', this.Point);
    //   console.log('current way Point', this.wayPointVariableForTesting);
    //   console.log('way point length:', this.wayPointsList.length);
    //   console.log('Current Way Points ', this.wayPointsList);
    // }, 5000);
  }

  show(id?) {
    //if view disable the form otherwise enable it
    // this.usedIn == 'view' ? this.createOrEditPintForm.form.disable() : this.createOrEditPintForm.form.enable();
    this.active = true;
    this.loadFacilities();

    //this.singleWayPoint = new CreateOrEditRoutPointDto();
    if (id) {
      this.pointIdForEdit = id;
      //this is edit point action
      this.Point = this.wayPointsList[id];
    }

    //tell the service that i have this SinglePoint Active Right Now
    this.isAdditionalReceiverEnabled = this.Point.receiverFullName ? true : false;

    this._PointService.updateSinglePoint(this.Point);
    this.loadReceivers(this.Point.facilityId);
    this.modal.show();
  }

  /**
   * Add Route Step
   */
  AddRouteStep() {
    //this is edit
    if (this.pointIdForEdit) {
      this.wayPointsList[this.pointIdForEdit] = this.Point;
      //in case of edit add GoodDetails List
      // this.PointGoodDetailsComponent.goodsDetailList = this.Point.goodsDetailListDto;
    } else {
      if (this.RouteType == this.RouteTypes.MultipleDrops) {
        this.Point.pickingType = PickingType.Dropoff;
        // this.Point.goodsDetailListDto = this.PointGoodDetailsComponent.goodsDetailList;
        this.wayPointsList.push(this.Point);
      }
    }
    //Route Type Single Drop ? dont add new point Edit the index 1 directly
    this._PointService.updateWayPoints(this.wayPointsList);
    // this._PointService.currentWayPointsList
    this.close();
  }

  close() {
    this.Point = new CreateOrEditRoutPointDto();
    this._PointService.updateSinglePoint(this.Point);
    this.pointIdForEdit = null;
    this.isAdditionalReceiverEnabled = false;
    this.active = false;
    this.modal.hide();
  }

  /**
   * Extracts Facility Coordinates for single Point (Map Drawing)
   */
  RouteStepCordSetter() {
    this.Point.latitude =
      this._tripService.currentSourceFacilitiesItems.find((x) => x.id == this.Point.facilityId)?.lat == undefined
        ? this._tripService.currentDestinationFacilitiesItems.find((x) => x.id == this.Point.facilityId)?.lat
        : this._tripService.currentSourceFacilitiesItems.find((x) => x.id == this.Point.facilityId)?.lat;
    this.Point.longitude =
      this._tripService.currentSourceFacilitiesItems.find((x) => x.id == this.Point.facilityId)?.long == undefined
        ? this._tripService.currentDestinationFacilitiesItems.find((x) => x.id == this.Point.facilityId)?.long
        : this._tripService.currentSourceFacilitiesItems.find((x) => x.id == this.Point.facilityId)?.long;
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
   * loads Facilities with validation on it related to source and destination in SR
   */
  loadFacilities() {
    this.facilityLoading = true;
    if (this.shippingRequestId != null) {
      this._tripService.currentShippingRequest.subscribe((res) => {
        this.citySourceId = res.originalCityId;
        this.cityDestId = res.destinationCityId;
        this.pointsCount = res.shippingRequest.numberOfDrops;
      });
    }
    if (this.wayPointsList.length > 0) {
      // if point is last of drop off
      if (this.wayPointsList.filter((r) => r.pickingType == 2).length + 1 == this.pointsCount) {
        this.getFacilities(true);
        return;
      }
      // hide facilities of souurce city SR if any point put in destinaton city of SR
      this.wayPointsList.forEach((element) => {
        this._facilitiesServiceProxy.getFacilityForView(element.facilityId).subscribe((r) => {
          if (r.facility.cityId != null) {
            if (r.facility.cityId == this.cityDestId) {
              this.getFacilities(true);
              return;
            }
          }
        });
      });

      this.getFacilities(false);
    }
  }

  /**
   * get all facilities or get facilities without source SR facilities
   */
  getFacilities(hideSource: boolean) {
    if (this.shippingRequestId != null) {
      this._routStepsServiceProxy.getAllFacilitiesByCityAndTenantForDropdown(this.shippingRequestId).subscribe((result) => {
        if (hideSource) {
          this.allFacilities = result.filter((r) => r.cityId != this.citySourceId);
        } else {
          this.allFacilities = result;
        }
      });
    }
    this.facilityLoading = false;
  }

  /**
   * resets Additional Receiver if the Additional Receiver checkbox is not selected
   */
  resetAdditionalReviverFields() {
    if (!this.isAdditionalReceiverEnabled) {
      this.Point.receiverCardIdNumber = undefined;
      this.Point.receiverEmailAddress = undefined;
      this.Point.receiverPhoneNumber = undefined;
      this.Point.receiverFullName = undefined;
    }
  }
}
