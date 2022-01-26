/* tslint:disable:triple-equals */
import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditRoutPointDto,
  FacilitiesServiceProxy,
  PickingType,
  RoutesServiceProxy,
  RoutStepsServiceProxy,
  ShippingRequestRouteType,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { Subscription } from 'rxjs';

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
    public _tripService: TripService,
    private _PointsService: PointsService
  ) {
    super(injector);
  }
  @ViewChild('createOrEditFacilityModal') public createOrEditFacilityModal: ModalDirective;
  @ViewChild('createRouteStepModal') public createRouteStepModal: ModalDirective;
  // @ViewChild('PointGoodDetailsComponent') public PointGoodDetailsComponent: GoodDetailsComponent;
  @Input() usedIn: 'view' | 'createOrEdit';
  @Output() SelectedWayPointsFromChild: EventEmitter<CreateOrEditRoutPointDto[]> = new EventEmitter<CreateOrEditRoutPointDto[]>();

  MainGoodsCategory: number;
  NumberOfDrops: number;
  sourceFacility: number;
  destFacility: number;
  activeTripId: number;
  RouteType: number;
  RouteTypes = ShippingRequestRouteType;
  wayPointsList: CreateOrEditRoutPointDto[] = [];

  //allFacilities: FacilityForDropdownDto[];
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

  ngOnDestroy() {
    this.pointsServiceSubscription$.unsubscribe();
    this.tripDestFacilitySub$?.unsubscribe();
    this.tripSourceFacilitySub$.unsubscribe();
    this.currentActiveTripSubs$.unsubscribe();
    console.log('Unsubscribed/Destroid from  Point Component');
  }

  ngOnInit() {
    this.loadDropDowns();
    //in case of edit trip
    //and found that already there is a way point
    //and the way Points count is greater than 0
    //Draw on map wayPointsSetter()
    this.pointsServiceSubscription$ = this._PointsService.currentWayPointsList.subscribe((res) => {
      this.wayPointsList = res;
      if (res.length > 0) {
        this.wayPointsSetter();
      }
      //validate if point Limit Reached For Multible Drops And Show Success Message
      if (!this.activeTripId && this.RouteType == ShippingRequestRouteType.MultipleDrops && res.length - 1 == this.NumberOfDrops) {
        //  Swal.fire(this.l('GoodJob'), this.l('AllDropPointsAddedSuccessfully'), 'success');
      }
    });
    //Tell the Service Where this Component is Being Used
    this._PointsService.updateCurrentUsedIn(this.usedIn);
    //if action is edit trip get active Trip id
    this.currentActiveTripSubs$ = this._tripService.currentActiveTripId.subscribe((res) => (this.activeTripId = res));
    //get some Stuff from ShippingRequest Dto
    this.tripSourceFacilitySub$ = this._tripService.currentShippingRequest.subscribe((res) => {
      if (res.shippingRequest) {
        this.RouteType = res.shippingRequest.routeTypeId;
        this.NumberOfDrops = res.shippingRequest.numberOfDrops;
        this.MainGoodsCategory = res.shippingRequest.goodCategoryId;
      }
    });
    //Take Trip Source Facility From Trip Service
    this.tripSourceFacilitySub$ = this._tripService.currentSourceFacility.subscribe((res) => {
      this.sourceFacility = res;
      if (this.sourceFacility) {
        this.drawPointForSingleDropTrip('pickup');
      }
    });

    //in case of the Shipping Request Route Type is Single Drop -- Create the Drop Point From Dest Trip Facility
    if (this.RouteType == ShippingRequestRouteType.SingleDrop) {
      //Take Trip Dest Facility From Trip Service
      this.tripDestFacilitySub$ = this._tripService.currentDestFacility.subscribe((res) => {
        this.destFacility = res;
        if (this.destFacility) {
          this.drawPointForSingleDropTrip('drop');
        }
      });
    }
  }

  //Load DropDowns For Shipper Only
  loadDropDowns() {
    this.feature.isEnabled('App.Shipper') ? this.loadFacilities() : 0;
  }
  //for SingleDrop Trip Only
  //draws the points and sets them
  drawPointForSingleDropTrip(pointType: 'pickup' | 'drop') {
    console.log('Before From the Points Component:', this.wayPointsList);
    //if create Make New Dto
    if (!this.activeTripId) {
      //this is for create Trip
      this.Point = new CreateOrEditRoutPointDto();
    } else {
      //if edit
      this.Point = pointType == 'drop' ? this.wayPointsList[1] : this.wayPointsList[0];
    }
    this.Point.pickingType = pointType == 'drop' ? PickingType.Dropoff : PickingType.Pickup;
    this.Point.facilityId = pointType == 'drop' ? this.destFacility : this.sourceFacility;
    // this.Point.latitude = this.allFacilities.find((x) => (pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility))?.lat;
    // this.Point.longitude = this.allFacilities.find((x) => (pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility))?.long;
    this.Point.latitude =
      this._tripService.currentSourceFacilitiesItems.find((x) => (pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility))
        ?.lat == null
        ? this._tripService.currentDestinationFacilitiesItems.find((x) =>
            pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility
          )?.lat
        : this._tripService.currentSourceFacilitiesItems.find((x) => (pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility))
            ?.lat;
    this.Point.longitude =
      this._tripService.currentSourceFacilitiesItems.find((x) => (pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility))
        ?.long == null
        ? this._tripService.currentDestinationFacilitiesItems.find((x) =>
            pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility
          )?.long
        : this._tripService.currentSourceFacilitiesItems.find((x) => (pointType == 'drop' ? x.id == this.destFacility : x.id == this.sourceFacility))
            ?.long;
    //sets the long and lat of the point
    this.wayPointsList[pointType == 'drop' ? 1 : 0] = this.Point;
    //sync points list with the cloud
    this._PointsService.updateWayPoints(this.wayPointsList);
    //Points Drawer
    this.wayPointsSetter();
    console.log('after:', this.wayPointsList);
    this.createDropPointsForMultiDrops();
  }

  //for MultiBleDrops
  createDropPointsForMultiDrops() {
    if (this.RouteType === this.RouteTypes.MultipleDrops && this.wayPointsList.length === 1) {
      for (let i = 0; i < this.NumberOfDrops; i++) {
        let item = new CreateOrEditRoutPointDto();
        item.pickingType = PickingType.Dropoff;
        this.wayPointsList.push(item);
      }
    }
  }

  delete(index: number) {
    this.wayPointsList.splice(index, 1);
    this.notify.info(this.l('SuccessfullyDeleted'));
  }

  loadFacilities() {
    console.log('Facilites Loaded From Points');
    this.facilityLoading = true;
  }

  getFacilityNameByid(id: number) {
    return this._tripService.currentSourceFacilitiesItems?.find((x) => x.id == id) == null
      ? this._tripService.currentDestinationFacilitiesItems?.find((x) => x.id == id)?.displayName
      : this._tripService.currentSourceFacilitiesItems?.find((x) => x.id == id)?.displayName;
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
    this.id = i;
    this.loading = true;
    this._waybillsServiceProxy.getMultipleDropWaybillPdf(i).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.loading = false;
    });
  }

  getTotalWeightOfDropPoint(record: CreateOrEditRoutPointDto): number {
    let weight = 0;
    record.goodsDetailListDto.forEach((x) => {
      weight += x.weight * x.amount;
    });
    return weight;
  }
}
