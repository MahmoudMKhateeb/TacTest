import { Component, Injector, OnChanges, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  CreateOrEditRoutPointDto,
  FacilityForDropdownDto,
  GoodsDetailDto,
  PickingType,
  ReceiverFacilityLookupTableDto,
  ReceiversServiceProxy,
  RoutStepsServiceProxy,
  ShippingRequestRouteType,
} from '@shared/service-proxies/service-proxies';
import { DropDownMenu, TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { GoodDetailsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/good-details/good-details.component';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'createOrEditPointModal',
  styleUrls: ['./createOrEditPointModal.component.css'],
  templateUrl: './createOrEditPointModal.component.html',
})
export class CreateOrEditPointModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  constructor(
    injector: Injector,
    private _tripService: TripService,
    private _PointService: PointsService,
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
  isAdditionalReceiverEnabled: boolean;
  pointIdForEdit = null;

  tripServiceSubscription$: any;
  pointsServiceSubscription$: any;
  ngOnDestroy() {
    this.tripServiceSubscription$.unsubscribe();
    this.pointsServiceSubscription$.unsubscribe();
    console.log('Unsubscribed/Destroid from CreateOrEdit Point Modal');
  }

  ngOnInit(): void {
    this.feature.isEnabled('App.Shipper') ? this.loadFacilities() : 0;

    //take the Route Type From the Shared Service
    this.tripServiceSubscription$ = this._tripService.currentShippingRequest.subscribe((res) => (this.RouteType = res.shippingRequest.routeTypeId));
    //take the PointsList From The Shared Service
    this.pointsServiceSubscription$ = this._PointService.currentWayPointsList.subscribe((res) => {
      this.wayPointsList = res;
    });

    this._PointService.currentSingleWayPoint.subscribe((res) => {
      this.Point = res;
      this.wayPointVariableForTesting = res;
    });

    // setInterval(() => {
    //   console.log('current Point', this.Point);
    //   console.log('current way Point', this.wayPointVariableForTesting);
    //   console.log('way point length:', this.wayPointsList.length);
    //   console.log('Current Way Points ', this.wayPointsList);
    // }, 5000);
  }

  show(id?) {
    this.active = true;
    //this.singleWayPoint = new CreateOrEditRoutPointDto();
    if (id) {
      this.pointIdForEdit = id;
      //this is edit point action
      this.Point = this.wayPointsList[id];
    }
    //tell the service that i have this SinglePoint Active Right Now
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
    this.Point.latitude = this.allFacilities.find((x) => x.id == this.Point.facilityId)?.lat;
    this.Point.longitude = this.allFacilities.find((x) => x.id == this.Point.facilityId)?.long;
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
   * loads Facilities
   */
  loadFacilities() {
    this.facilityLoading = true;
    // this._shippingRequestDDService.allFacilities.subscribe((res) => (this.allFacilities = res));
    this._tripService.currentFacilitiesItems.subscribe((res: DropDownMenu) => {
      this.facilityLoading = res.isLoading;
      this.allFacilities = res.items;
    });
  }

  /**
   * resets Additional Receiver if the Additional Receiver checkbox is not selected
   */
  resetAdditionalReviverFields() {
    if (!this.isAdditionalReceiverEnabled) {
      this.Point.receiverAddress = undefined;
      this.Point.receiverCardIdNumber = undefined;
      this.Point.receiverEmailAddress = undefined;
      this.Point.receiverPhoneNumber = undefined;
      this.Point.receiverFullName = undefined;
    }
  }
}