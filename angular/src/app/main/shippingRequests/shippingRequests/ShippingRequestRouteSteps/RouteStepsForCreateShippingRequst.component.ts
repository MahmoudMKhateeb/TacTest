import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditFacilityDto,
  CreateOrEditRoutPointDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GoodsDetailDto,
  GoodsDetailGoodCategoryLookupTableDto,
  GoodsDetailsServiceProxy,
  PickingType,
  RoutesServiceProxy,
  RoutStepCityLookupTableDto,
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { MapsAPILoader } from '@node_modules/@agm/core';
import Swal from 'sweetalert2';

@Component({
  selector: 'RouteStepsForCreateShippingRequest',
  templateUrl: './RouteStepsForCreateShippingRequest.html',
  styleUrls: ['./RouteStepsForCreateShippingRequest.scss'],
})
export class RouteStepsForCreateShippingRequstComponent extends AppComponentBase implements OnInit, OnChanges {
  constructor(
    injector: Injector,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _routesServiceProxy: RoutesServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy
  ) {
    super(injector);
  }
  @ViewChild('createOrEditFacilityModal') public createOrEditFacilityModal: ModalDirective;
  @ViewChild('createRouteStepModal') public createRouteStepModal: ModalDirective;
  @ViewChild('createOrEditGoodDetail', { static: false }) public createOrEditGoodDetail: ModalDirective;
  @Input() MainGoodsCategory: number;
  @Input() RouteType: number;
  @Input() NumberOfDrops: number;

  @Input() WayPointListFromFatherForShippingRequestEdit: CreateOrEditRoutPointDto[];
  @Output() SelectedWayPointsFromChild: EventEmitter<CreateOrEditRoutPointDto[]> = new EventEmitter<CreateOrEditRoutPointDto[]>();
  wayPointsList: CreateOrEditRoutPointDto[] = this.WayPointListFromFatherForShippingRequestEdit || [];
  singleWayPoint: CreateOrEditRoutPointDto = new CreateOrEditRoutPointDto();
  goodsDetail: GoodsDetailDto = new GoodsDetailDto();

  allFacilities: FacilityForDropdownDto[];

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
  allSubGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  allUnitOfMeasure: SelectItemDto[];
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
      allowedPoints: this.NumberOfDrops + 1 || 20,
      numberOfPickUps: 1,
      numberOfDrops: this.NumberOfDrops || 15,
    },
  };
  activeValidator = this.wayPointValidationSets.singlePoint;

  ngOnInit() {
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
    this._shippingRequestsServiceProxy.getAllUnitOfMeasuresForDropdown().subscribe((result) => {
      this.allUnitOfMeasure = result;
    });
    //check if ShippingRequest is in Edit Mode
    if (this.WayPointListFromFatherForShippingRequestEdit) {
      this.wayPointsList = this.WayPointListFromFatherForShippingRequestEdit;
      this.wayPointsSetter();
    }
    this.refreshFacilities();
  }
  ngOnChanges(changes: SimpleChanges) {
    //if RouteType Was MultipleDrops/twoWays And Changed To SomeThing Else i want to keep the First 2 Point of the wayPoints
    const Route = changes.RouteType;
    if (Route?.currentValue !== Route?.previousValue) {
      this.wayPointsList.length = 0;
    }
    //in case of single Drop allow only 2 points
    if (this.RouteType == 1) {
      this.activeValidator = this.wayPointValidationSets.singlePoint;
    } else if (this.RouteType == 2) {
      this.activeValidator = this.wayPointValidationSets.twoWay;
    } else if (this.RouteType == 3) {
      this.activeValidator = this.wayPointValidationSets.multiDrops;
    }
    console.log('Changes Happend');
    this.EmitToFather();
  }
  //to Select PickUp Point
  showPickUpModal() {
    // if (!this.MainGoodsCategory) {
    //   return Swal.fire(this.l('Warning'), this.l('pleaseSelectMainGoodCategoryFirst'), 'warning');
    // }
    // if (!this.RouteType) {
    //   return Swal.fire(this.l('Warning'), this.l('pleaseSelectaRouteTypeFirst'), 'warning');
    // }
    this.singleWayPoint = new CreateOrEditRoutPointDto();
    this.singleWayPoint.pickingType = PickingType.Pickup;
    this.createRouteStepModal.show();
  }
  //to Select DropDown point
  showDropPointUpModal() {
    this.singleWayPoint = new CreateOrEditRoutPointDto();
    this.singleWayPoint.pickingType = PickingType.Dropoff;
    this.createRouteStepModal.show();
  }

  openCreateFacilityModal() {
    this.active = true;
    //load Places Autocomplete
    // this.zoom = 14;
    this.createOrEditFacilityModal.show();
  }

  EditRouteStep(id) {
    //if there is an id for the RouteStep then update the Record Don't Create A new one
    this.RouteStepCordSetter();
    this.wayPointsList[id] = this.singleWayPoint;
    this.createRouteStepModal.hide();
    this.notify.info(this.l('UpdatedSuccessfully'));
    this.EmitToFather();
  }
  AddRouteStep(id?: number) {
    if (id !== undefined) {
      //view
      //if there is an id open the modal and display the data
      this.routeStepIdForEdit = id;
      this.singleWayPoint = this.wayPointsList[id];
      this.createRouteStepModal.show();
      console.log('this is show: ', this.singleWayPoint);
    } else {
      //create new route Step
      this.RouteStepCordSetter();
      //console.log(this.wayPointsList);
      if (this.validateAddRoutePoint()) {
        this.wayPointsList.push(this.singleWayPoint);
        this.createRouteStepModal.hide();
        this.notify.info(this.l('SuccessfullyAdded'));
        this.EmitToFather();
      } else {
        this.createRouteStepModal.hide();
        Swal.fire(this.l('Warning'), this.l('wayPointsLimitReched'), 'warning');
      }
    }
  }
  validateAddRoutePoint() {
    return this.wayPointsList.length === this.activeValidator.allowedPoints ? false : true;
  }
  delete(index: number) {
    this.wayPointsList.splice(index, 1);
    this.notify.info(this.l('SuccessfullyDeleted'));
    this.EmitToFather();
  }

  EmitToFather() {
    this.routeStepIdForEdit = undefined;
    this.SelectedWayPointsFromChild.emit(this.wayPointsList);
    this.wayPointsSetter();
    this.singleWayPoint = new CreateOrEditRoutPointDto();
  }
  refreshFacilities() {
    this.facilityLoading = true;
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
      this.facilityLoading = false;
    });
  }
  getFacilityNameByid(id: number) {
    return this.allFacilities?.find((x) => x.id == id)?.displayName;
  }
  RouteStepCordSetter() {
    this.singleWayPoint.latitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.lat;
    this.singleWayPoint.longitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.long;
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
  //GootDetails Section

  GetAllSubCat(FatherID) {
    //Get All Sub-Good Category
    if (FatherID) {
      this.allSubGoodCategorys = undefined;
      this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(FatherID).subscribe((result) => {
        this.allSubGoodCategorys = result;
      });
    }
  }

  getGoodSubDisplayname(id) {
    return this.allSubGoodCategorys ? this.allSubGoodCategorys.find((x) => x.id == id)?.displayName : 0;
  }

  openAddNewGoodDetailModal() {
    this.GetAllSubCat(this.MainGoodsCategory);
    this.createOrEditGoodDetail.show();
  }
  AddGoodDetail() {
    if (!this.singleWayPoint.goodsDetailListDto) {
      this.singleWayPoint.goodsDetailListDto = [];
    }
    this.singleWayPoint.goodsDetailListDto.push(this.goodsDetail);
    this.goodsDetail = new GoodsDetailDto();
    this.createOrEditGoodDetail.hide();
  }

  DeleteGoodDetail(id) {
    this.singleWayPoint.goodsDetailListDto.splice(id, 1);
  }
}