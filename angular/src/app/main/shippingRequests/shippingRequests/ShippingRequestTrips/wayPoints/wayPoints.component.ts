import { Component, EventEmitter, Injector, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
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
} from '@shared/service-proxies/service-proxies';
import Swal from 'sweetalert2';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'wayPointsComponent',
  templateUrl: './wayPoints.component.html',
  styleUrls: ['./wayPoints.component.scss'],
})
export class WayPointsComponent extends AppComponentBase implements OnInit, OnChanges {
  constructor(
    injector: Injector,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _routesServiceProxy: RoutesServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy
  ) {
    super(injector);
  }
  @ViewChild('createOrEditFacilityModal') public createOrEditFacilityModal: ModalDirective;
  @ViewChild('createRouteStepModal') public createRouteStepModal: ModalDirective;
  @ViewChild('createOrEditGoodDetail', { static: false }) public createOrEditGoodDetail: ModalDirective;
  @Input() MainGoodsCategory: number;
  @Input() RouteType: number;
  @Input() NumberOfDrops: number;

  @Input() WayPointListFromFatherForShippingRequestEdit: [];
  @Output() SelectedWayPointsFromChild: EventEmitter<CreateOrEditRoutPointDto[]> = new EventEmitter<CreateOrEditRoutPointDto[]>();

  wayPointsList: CreateOrEditRoutPointDto[] = this.WayPointListFromFatherForShippingRequestEdit || [];
  singleWayPoint: CreateOrEditRoutPointDto = new CreateOrEditRoutPointDto();
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
  allSubGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
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
      allowedPoints: this.NumberOfDrops + 1,
      numberOfPickUps: 1,
      numberOfDrops: this.NumberOfDrops,
    },
  };
  //TODO : to change this line when twoWay Type Become Active
  activeValidator = this.RouteType == 1 ? this.wayPointValidationSets.singlePoint : this.wayPointValidationSets.multiDrops;
  sourceTripFacilityId: number;
  desTripFacilityId: number;
  ngOnInit() {
    this.loadDropDowns();
    //check if ShippingRequest is in Edit Mode
    if (this.WayPointListFromFatherForShippingRequestEdit) {
      this.wayPointsList = this.WayPointListFromFatherForShippingRequestEdit;
      this.wayPointsSetter();
    }
  }
  ngOnChanges(changes: SimpleChanges) {
    //if RouteType Was MultipleDrops/twoWays And Changed To SomeThing Else i want to keep the First 2 Point of the wayPoints
    const Route = changes.RouteType;
    if (Route?.currentValue !== Route?.previousValue) {
      this.wayPointsList.length = 0;
    }
    //in case of single Drop allow only 2 points
    // setInterval((_) => {
    //   return console.log(this.activeValidator, this.RouteType, this.NumberOfDrops);
    // }, 1000);
    if (this.RouteType == 1) {
      this.activeValidator = this.wayPointValidationSets.singlePoint;
    } else if (this.RouteType == 2) {
      this.activeValidator = this.wayPointValidationSets.twoWay;
    } else if (this.RouteType == 3) {
      this.activeValidator = this.wayPointValidationSets.multiDrops;
    }
    //console.log('Changes Happend');
    this.EmitToFather();
  }
  //Load DropDowns For Shipper Only
  loadDropDowns() {
    if (this.feature.isEnabled('App.Shipper')) {
      this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
        this.allCitys = result;
      });
      this._shippingRequestsServiceProxy.getAllUnitOfMeasuresForDropdown().subscribe((result) => {
        this.allUnitOfMeasure = result;
      });
      this.refreshFacilities();
    }
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
    this.refreshFacilities();
  }
  //to Select DropDown point
  showDropPointUpModal() {
    this.singleWayPoint = new CreateOrEditRoutPointDto();
    this.singleWayPoint.pickingType = PickingType.Dropoff;
    this.createRouteStepModal.show();
    this.refreshFacilities();
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
        //Swal.fire(this.l('Warning'), this.l('wayPointsLimitReched'), 'warning');
      }
    }
  }

  /**
   * this Methoud Controlls the Validation Of Creating New WayPoint/sourcePoint/DropPoint
   */
  validateAddRoutePoint() {
    //if the user Didnt not Select a Source Trip Facility
    if (this.sourceTripFacilityId === undefined) {
      Swal.fire(this.l('Warning'), this.l('pleaseSelectASourceTripFacilityFirst'), 'warning');
      return false;
    }
    //if point type is pick up ...Validate if Trip Source Facility == the PickUp Point Facility
    if (this.singleWayPoint.pickingType === PickingType.Pickup && this.singleWayPoint.facilityId !== this.sourceTripFacilityId) {
      Swal.fire(this.l('Warning'), this.l('pickupPointFacilityAndSourceTripFacilityareNotTheSame'), 'warning');
      return false;
      // Validate the Allowed Number Of Drops
    } else if (this.wayPointsList.length === this.activeValidator.allowedPoints) {
      Swal.fire(this.l('Warning'), this.l('pointsLimitReached'), 'warning');
      return false;
      //validate if dropPoint Has Goods Or No
    } else if (this.singleWayPoint.pickingType === PickingType.Dropoff && this.singleWayPoint.goodsDetailListDto === undefined) {
      Swal.fire(this.l('Warning'), this.l('goodDetailsCantBeEmptyInDropPoint'), 'warning');
      return false;
    }
    return true;
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

  downloadDropWayBill(i: number) {
    this._waybillsServiceProxy.getMultipleDropWaybillPdf(i).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
