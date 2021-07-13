import { Component, EventEmitter, Injector, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditRoutPointDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GoodsDetailDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailsServiceProxy,
  PickingType,
  RoutesServiceProxy,
  RoutStepCityLookupTableDto,
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  WaybillsServiceProxy,
  ReceiversServiceProxy,
} from '@shared/service-proxies/service-proxies';
import Swal from 'sweetalert2';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';

@Component({
  selector: 'PointsComponent',
  templateUrl: './points.component.html',
  styleUrls: ['./points.component.scss'],
})
export class PointsComponent extends AppComponentBase implements OnInit, OnDestroy {
  constructor(
    injector: Injector,
    private _routesServiceProxy: RoutesServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _tripService: TripService,
    private _PointsService: PointsService
  ) {
    super(injector);
  }
  @ViewChild('createOrEditFacilityModal') public createOrEditFacilityModal: ModalDirective;
  @ViewChild('createRouteStepModal') public createRouteStepModal: ModalDirective;
  MainGoodsCategory: number;
  NumberOfDrops: number;
  sourceFacility: number;
  destFacility: number;
  activeTripId: number;
  @Output() SelectedWayPointsFromChild: EventEmitter<CreateOrEditRoutPointDto[]> = new EventEmitter<CreateOrEditRoutPointDto[]>();

  RouteType: number;

  wayPointsList: CreateOrEditRoutPointDto[] = [];
  goodsDetail: GoodsDetailDto = new GoodsDetailDto();
  allFacilities: FacilityForDropdownDto[];

  PickingType = PickingType;

  active = false;
  saving = false;
  allCitys: RoutStepCityLookupTableDto[];
  facilityLoading = false;
  editRouteId: number = undefined;

  routeStepIdForEdit: number = undefined;
  zoom: Number = 13; //map zoom
  //this dir is for Single Route Step Map Route Draw
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  dir = { point: { lat: undefined, lng: undefined } };
  wayPoints = [];
  wayPointMapSource = undefined;
  wayPointMapDest = undefined;
  wayPointValidationSets = {
    singlePoint: {
      allowedPoints: 2,
      numberOfPickUps: 1,
      numberOfDrops: 1,
    },
    twoWay: {
      allowedPoints: 4,
      numberOfPickUps: 1,
      numberOfDrops: 3,
    },
    multiDrops: {
      allowedPoints: 1,
      numberOfPickUps: 1,
      numberOfDrops: 1,
    },
  };
  //TODO : to change this line when twoWay Type Become Active
  activeValidator: any;
  singleWayPoint = new CreateOrEditRoutPointDto();
  wayPointsSubscription: any;
  shippingRequestSubscription: any;
  tripSubscription: any;
  ngOnInit() {
    //in case of edit trip
    //and found that already there is a way point
    //and the way Points count is greater than 0
    //Draw on map wayPointsSetter()
    this.wayPointsSubscription = this._PointsService.currentWayPointsList.subscribe((res) => {
      this.wayPointsList = res;
      if (res.length > 0) {
        this.wayPointsSetter();
      }
    });
    this.shippingRequestSubscription = this._tripService.currentShippingRequest.subscribe((res) => {
      this.RouteType = res.shippingRequest.routeTypeId;

      this.NumberOfDrops = res.shippingRequest.numberOfDrops;
      this.MainGoodsCategory = res.shippingRequest.goodCategoryId;
    });
    this.tripSubscription = this._tripService.currentActiveTrip.subscribe((res) => {
      // this.activeTripId = res.id;
      this.sourceFacility = res.originFacilityId;
      this.destFacility = res.destinationFacilityId;
      console.log('Source Facility/Dest Facility Changes ');
      if (this.sourceFacility) {
        //for singleDrop
        this.drawFirstPoint();
      }
      if (this.destFacility) {
        //For Single Drop
        this.drawSecondPoint();
      }
    });

    this.wayPointValidationSets.multiDrops.allowedPoints = this.NumberOfDrops + 1;
    this.wayPointValidationSets.multiDrops.numberOfDrops = this.NumberOfDrops + 1;
    this.activeValidator = this.RouteType == 1 ? this.wayPointValidationSets.singlePoint : this.wayPointValidationSets.multiDrops;
    this.loadDropDowns();
  }

  //Load DropDowns For Shipper Only
  loadDropDowns() {
    if (this.feature.isEnabled('App.Shipper')) {
      this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
        this.allCitys = result;
      });

      this.loadFacilities();
    }
  }
  //to Select PickUp Point
  //for single Drop only
  drawFirstPoint() {
    //if create Make New Dto
    if (!this.activeTripId) {
      this.singleWayPoint = new CreateOrEditRoutPointDto();
    } else {
      //if edit
      this.singleWayPoint = this.wayPointsList[0];
    }
    this.singleWayPoint.pickingType = PickingType.Pickup;
    this.singleWayPoint.facilityId = this.sourceFacility;
    this.singleWayPoint.latitude = this.allFacilities.find((x) => x.id == this.sourceFacility)?.lat;
    this.singleWayPoint.longitude = this.allFacilities.find((x) => x.id == this.sourceFacility)?.long;
    //sets the long and lat of the point
    this.wayPointsList[0] = this.singleWayPoint;

    // this.wayPointsList.length == 0 ? this.AddRouteStep() : this.EditRouteStep(0);
    this.AddRouteStep();
  }
  //for singleDrop  Draw Secound Point
  drawSecondPoint() {
    //if create Make New Dto
    if (!this.activeTripId) {
      this.singleWayPoint = new CreateOrEditRoutPointDto();
    } else {
      //if edit
      this.singleWayPoint = this.wayPointsList[1];
    }
    this.singleWayPoint.pickingType = PickingType.Dropoff;
    this.singleWayPoint.facilityId = this.destFacility;
    this.singleWayPoint.latitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.lat;
    this.singleWayPoint.longitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.long;
    //sets the long and lat of the point
    this.wayPointsList[1] = this.singleWayPoint;
    // this.wayPointsList.length == 0 ? this.AddRouteStep() : this.EditRouteStep(0);
    this.AddRouteStep();
  }
  /**
   * pish the New Point to the Points List
   *
   */
  AddRouteStep() {
    if (this.activeTripId) {
      this._PointsService.updateSinglePoint(this.singleWayPoint);
    }
    this._PointsService.updateWayPoints(this.wayPointsList);
    //Points Drawer
    this.wayPointsSetter();
  }

  /**
   * this Methoud Controlls the Validation Of Creating New WayPoint/sourcePoint/DropPoint
   */
  validateAddRoutePoint() {
    //if the user Didnt not Select a Source Trip Facility
    if (this.sourceFacility === undefined) {
      Swal.fire(this.l('Warning'), this.l('pleaseSelectASourceTripFacilityFirst'), 'warning');
      return false;
    } else if (this.wayPointsList.length === this.activeValidator.allowedPoints) {
      Swal.fire(this.l('Warning'), this.l('pointsLimitReached'), 'warning');
      return false;
      //validate if dropPoint Has Goods Or No
    }
    return true;
  }
  delete(index: number) {
    this.wayPointsList.splice(index, 1);
    this.notify.info(this.l('SuccessfullyDeleted'));
  }

  loadFacilities() {
    this.facilityLoading = true;
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
      this.facilityLoading = false;
    });
  }

  getFacilityNameByid(id: number) {
    return this.allFacilities?.find((x) => x.id == id)?.displayName;
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

  downloadDropWayBill(i: number) {
    this._waybillsServiceProxy.getMultipleDropWaybillPdf(i).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  ngOnDestroy() {
    this.wayPointsSubscription.unsubscribe();
    this.shippingRequestSubscription.unsubscribe();
    this.tripSubscription.unsubscribe();
  }
}
