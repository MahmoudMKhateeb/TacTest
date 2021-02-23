import { Component, ViewChild, Injector, Output, EventEmitter, OnChanges, SimpleChanges, NgZone, ElementRef, OnInit, Input } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditFacilityDto,
  CreateOrEditGoodsDetailDto,
  CreateOrEditRoutPointDto,
  CreateOrEditRoutStepDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GoodsDetailDto,
  GoodsDetailGoodCategoryLookupTableDto,
  GoodsDetailsServiceProxy,
  ISelectItemDto,
  RoutesServiceProxy,
  RoutStepCityLookupTableDto,
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import Swal from 'sweetalert2';

@Component({
  selector: 'RouteStepsForCreateShippingRequest',
  templateUrl: './RouteStepsForCreateShippingRequest.html',
  styleUrls: ['./RouteStepsForCreateShippingRequest.scss'],
})
export class RouteStepsForCreateShippingRequstComponent extends AppComponentBase implements OnInit {
  @ViewChild('createFacilityModal') public createFacilityModal: ModalDirective;
  @ViewChild('createRouteStepModal') public createRouteStepModal: ModalDirective;
  @ViewChild('createOrEditGoodDetail') public createOrEditGoodDetail: ModalDirective;

  @ViewChild('search') public searchElementRef: ElementRef;
  @Input() MainGoodsCategory: number;
  @Input() WayPointListFromFatherForShippingRequestEdit: CreateOrEditRoutPointDto[];
  @Output() SelectedWayPointsFromChild: EventEmitter<CreateOrEditRoutPointDto[]> = new EventEmitter<CreateOrEditRoutPointDto[]>();
  wayPointsList: CreateOrEditRoutPointDto[] = this.WayPointListFromFatherForShippingRequestEdit || [];
  singleWayPoint: CreateOrEditRoutPointDto = new CreateOrEditRoutPointDto();
  goodsDetail: GoodsDetailDto = new GoodsDetailDto();

  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  allFacilities: FacilityForDropdownDto[];

  private geoCoder;
  active = false;
  saving = false;
  allCitys: RoutStepCityLookupTableDto[];
  facilityLoading = false;
  editRouteId: number = undefined;
  //
  Address: string;
  State: string;
  Postal: string;
  City: string;
  Country: string;
  selectedCountryCode = 'SA';
  routeStepIdForEdit: number = undefined;

  zoom: Number = 13; //map zoom
  //this dir is for Single Route Step Map Route Draw

  lat: Number = 24.717942;
  lng: Number = 46.675761;
  dir = {
    point: { lat: undefined, lng: undefined },
  };

  //wayPoints map
  wayPoints = [];
  wayPointMapSource = undefined;
  wayPointMapDest = undefined;
  //end of way points

  SourceGoodDetailValueslist = [];
  DestGoodDetailValueslist = [];
  allSubGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  allUnitOfMeasure: SelectItemDto[];
  constructor(
    injector: Injector,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _routesServiceProxy: RoutesServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy
  ) {
    super(injector);
  }

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
      //draw the waypointsForEdit
      this.wayPointsSetter();
    }
    this.refreshFacilities();
    //this.Tester();
  }
  //to Select PickUp Point
  showPickUpModal() {
    if (!this.MainGoodsCategory) {
      return Swal.fire(this.l('ERROR'), this.l('pleaseSelectMainGoodCategoryFirst'), 'info');
    }
    this.singleWayPoint = new CreateOrEditRoutPointDto();
    this.singleWayPoint.pickingTypeId = 1;
    this.createRouteStepModal.show();
  }
  //to Select DropDown point
  showDropPointUpModal() {
    this.singleWayPoint = new CreateOrEditRoutPointDto();
    this.singleWayPoint.pickingTypeId = 2;
    this.createRouteStepModal.show();
  }

  openCreateFacilityModal() {
    this.active = true;
    //load Places Autocomplete
    this.loadMapApi();
    this.facility.latitude = 24.67911662122269;
    this.facility.longitude = 46.6355543345471;
    this.zoom = 14;
    this.createFacilityModal.show();
  }

  EditRouteStep(id) {
    //if there is an id for the RouteStep then update the Record Don't Create A new one
    console.log(`Save Edits Fired ${id}`);
    this.RouteStepCordSetter();
    // this.routStep.createOrEditSourceRoutPointInputDto.routPointGoodsDetailListDto = this.SourceGoodDetailValueslist;
    // this.routStep.createOrEditDestinationRoutPointInputDto.routPointGoodsDetailListDto = this.DestGoodDetailValueslist;
    this.wayPointsList[id] = this.singleWayPoint;
    this.createRouteStepModal.hide();
    this.notify.info(this.l('UpdatedSuccessfully'));
    this.EmitToFather();
  }
  AddRouteStep(id?: number) {
    if (id !== undefined) {
      //view
      //if there is an id open the modal and display the date
      this.routeStepIdForEdit = id;
      this.singleWayPoint = this.wayPointsList[id];
      // this.SourceGoodDetailValueslist = this.routStep.createOrEditSourceRoutPointInputDto.routPointGoodsDetailListDto;
      // this.DestGoodDetailValueslist = this.routStep.createOrEditDestinationRoutPointInputDto.routPointGoodsDetailListDto;
      this.createRouteStepModal.show();
      console.log('this is show: ', this.singleWayPoint);
    } else {
      //create new route Step
      this.RouteStepCordSetter();
      // this.routStep.createOrEditSourceRoutPointInputDto.routPointGoodsDetailListDto = this.SourceGoodDetailValueslist;
      // this.routStep.createOrEditDestinationRoutPointInputDto.routPointGoodsDetailListDto = this.DestGoodDetailValueslist;
      console.log(this.wayPointsList);
      this.wayPointsList.push(this.singleWayPoint);
      this.createRouteStepModal.hide();
      this.notify.info(this.l('SuccessfullyAdded'));

      this.EmitToFather();
    }
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
    this.createFacilityModal.hide();
  }
  loadMapApi() {
    this.mapsAPILoader.load().then(() => {
      this.geoCoder = new google.maps.Geocoder();

      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement, {
        componentRestrictions: {
          country: this.selectedCountryCode,
        },
      });
      autocomplete.addListener('place_changed', () => {
        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocomplete.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude and zoom
          this.facility.latitude = place.geometry.location.lat();
          this.facility.longitude = place.geometry.location.lng();
          this.getAddress(place.geometry.location.lat(), place.geometry.location.lng());
          this.zoom = 12;
        });
      });
    });
  }
  mapClicked($event: MouseEvent) {
    this.Address = undefined;
    this.City = undefined;
    this.State = undefined;
    this.Country = undefined;
    // @ts-ignore
    this.facility.latitude = $event.coords.lat;
    // @ts-ignore
    this.facility.longitude = $event.coords.lng;
    // @ts-ignore
    this.getAddress($event.coords.lat, $event.coords.lng);
  }
  getAddress(latitude, longitude) {
    this.geoCoder.geocode({ location: { lat: latitude, lng: longitude } }, (results, status) => {
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 14;
          let Spleted = results[0].formatted_address.split(',');
          console.log('Address Should Be Changed By Now');
          this.addressFormater(Spleted);
        } else {
          window.alert('No results found');
        }
      } else {
        window.alert('Geocoder failed due to: ' + status);
      }
    });
  }
  addressFormater(Spleted) {
    switch (Spleted.length) {
      case 4:
        this.Address = Spleted[0];
        this.State = Spleted[1];
        this.City = Spleted[2];
        this.Country = Spleted[3];
        break;
      case 5:
        this.Address = Spleted[0] + ' ' + Spleted[1];
        this.State = Spleted[2];
        this.City = Spleted[3];
        this.Country = Spleted[4];
        break;
      default:
        this.Address = undefined;
        this.State = undefined;
        this.City = undefined;
        this.Country = undefined;
        break;
    }
  }

  refreshFacilities() {
    this.facilityLoading = true;
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
      this.facilityLoading = false;
    });
  }
  createFacility() {
    this.saving = true;
    this.facility.address = this.Address;
    //to be Changed later cause it takes an id not a string for the city
    this.facility.cityId = 3;
    this._facilitiesServiceProxy
      .createOrEdit(this.facility)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.createFacilityModal.hide();
        this.refreshFacilities();
      });
  }

  //this function is to update the CORD of the RouteSteps and set the Dir of map Direction
  //by searching in the allFacilites array for the id of the facility and get the long/Lat of Each one
  //triggerd when Facility DD is clicked
  RouteStepCordSetter() {
    //facility Coordinates --> set the Coordinates in create RouteStep
    //source
    this.singleWayPoint.latitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.lat;
    this.singleWayPoint.longitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.long;

    //end of each Facility Cordinates
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
      // console.log('this is waypointlist length: ', this.wayPointsList.length);
      // console.log('this is i ', i);
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

    console.log('this is my source points :', this.wayPointMapSource);
    console.log('this is my waypoint:', this.wayPoints);
    console.log('this is my Dest points :', this.wayPointMapDest);
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
    console.log(this.goodsDetail);
    this.singleWayPoint.goodsDetailListDto ? 0 : (this.singleWayPoint.goodsDetailListDto = []);
    this.singleWayPoint.goodsDetailListDto.push(this.goodsDetail);
    this.createOrEditGoodDetail.hide();
  }

  DeleteGoodDetail(id) {
    this.singleWayPoint.goodsDetailListDto.splice(id, 1);
  }

  getFacilityNameByid(id: number) {
    return this.allFacilities?.find((x) => x.id == id)?.displayName;
  }
}
