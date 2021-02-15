import { Component, ViewChild, Injector, Output, EventEmitter, OnChanges, SimpleChanges, NgZone, ElementRef, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditFacilityDto,
  CreateOrEditRoutStepDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  RoutesServiceProxy,
  RoutPointGoodsDetailDto,
  RoutStepCityLookupTableDto,
  RoutStepsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { MapsAPILoader } from '@node_modules/@agm/core';

@Component({
  selector: 'RouteStepsForCreateShippingRequest',
  templateUrl: './RouteStepsForCreateShippingRequest.html',
  styleUrls: ['./RouteStepsForCreateShippingRequest.scss'],
})
export class RouteStepsForCreateShippingRequstComponent extends AppComponentBase implements OnInit {
  @ViewChild('createFacilityModal') public createFacilityModal: ModalDirective;
  @ViewChild('createRouteStepModal') public createRouteStepModal: ModalDirective;
  @ViewChild('search') public searchElementRef: ElementRef;
  @Output() SelectedRouteStepsFromChild: EventEmitter<CreateOrEditRoutStepDto[]> = new EventEmitter<CreateOrEditRoutStepDto[]>();
  routeStepsDetails: CreateOrEditRoutStepDto[] = [];
  routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();
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

  zoom: Number = 14; //map zoom
  //this dir is for Single Route Step Map Route Draw

  lat: Number = 24.67911662122269;
  lng: Number = 46.6355543345471;
  dir = {
    origin: { lat: undefined, lng: undefined },
    destination: { lat: undefined, lng: undefined },
  };

  //wayPoints map
  wayPoints = [];
  wayPointMapSource = undefined;
  wayPointMapDest = undefined;

  constructor(
    injector: Injector,
    private _routesServiceProxy: RoutesServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private _routStepsServiceProxy: RoutStepsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });

    this.refreshFacilities();
    //this.Tester();
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
    this.routeStepsDetails[id] = this.routStep;
    this.createRouteStepModal.hide();
    this.notify.info(this.l('UpdatedSuccessfully'));
    this.EmitToFather();
  }
  AddRouteStep(id?: number) {
    if (id !== undefined) {
      //view
      //if there is an id open the modal and display the date
      this.routeStepIdForEdit = id;
      this.routStep = this.routeStepsDetails[id];
      this.createRouteStepModal.show();
    } else {
      //create new route Step
      this.RouteStepCordSetter();
      this.routeStepsDetails.push(this.routStep);
      this.createRouteStepModal.hide();
      this.notify.info(this.l('SuccessfullyAdded'));

      this.EmitToFather();
    }
    console.log(this.routeStepsDetails);
  }
  delete(index: number) {
    this.routeStepsDetails.splice(index, 1);
    this.notify.info(this.l('SuccessfullyDeleted'));
    this.EmitToFather();
  }
  EmitToFather() {
    this.routeStepIdForEdit = undefined;
    this.SelectedRouteStepsFromChild.emit(this.routeStepsDetails);
    this.wayPointsSetter();
    this.routStep = new CreateOrEditRoutStepDto();
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
    this.facility.adress = this.Address;
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
    this.routStep.createOrEditSourceRoutPointInputDto.latitude = this.allFacilities.find(
      (x) => x.id == this.routStep.createOrEditSourceRoutPointInputDto.facilityId
    )?.lat;
    console.log('Source Lat ==> ', this.routStep.createOrEditSourceRoutPointInputDto.latitude);
    this.routStep.createOrEditSourceRoutPointInputDto.longitude = this.allFacilities.find(
      (x) => x.id == this.routStep.createOrEditSourceRoutPointInputDto.facilityId
    )?.long;
    console.log('Source Long ==> ', this.routStep.createOrEditSourceRoutPointInputDto.longitude);
    //Dest
    this.routStep.createOrEditDestinationRoutPointInputDto.latitude = this.allFacilities.find(
      (x) => x.id == this.routStep.createOrEditDestinationRoutPointInputDto.facilityId
    )?.lat;
    console.log('Des Lat ==> ', this.routStep.createOrEditDestinationRoutPointInputDto.latitude);
    this.routStep.createOrEditDestinationRoutPointInputDto.longitude = this.allFacilities.find(
      (x) => x.id == this.routStep.createOrEditDestinationRoutPointInputDto.facilityId
    )?.long;
    console.log('Des Long ==> ', this.routStep.createOrEditDestinationRoutPointInputDto.longitude);

    //end of each Facility Cordinates

    return (this.dir = {
      origin: {
        lat: this.routStep.createOrEditSourceRoutPointInputDto.latitude,
        lng: this.routStep.createOrEditSourceRoutPointInputDto.longitude,
      },
      destination: {
        lat: this.routStep.createOrEditDestinationRoutPointInputDto.latitude,
        lng: this.routStep.createOrEditDestinationRoutPointInputDto.longitude,
      },
    });
  }

  wayPointsSetter() {
    this.wayPointMapSource = undefined;
    this.wayPoints = [];
    this.wayPointMapDest = undefined;
    //Source is done
    this.wayPointMapSource = {
      lat: this.routeStepsDetails[0].createOrEditSourceRoutPointInputDto?.latitude || undefined,
      lng: this.routeStepsDetails[0].createOrEditSourceRoutPointInputDto?.longitude || undefined,
    };

    //set the way points
    for (let i = 1; i < this.routeStepsDetails.length; i++) {
      this.wayPoints.push(
        {
          location: {
            lat: this.routeStepsDetails[i].createOrEditSourceRoutPointInputDto.latitude,
            lng: this.routeStepsDetails[i].createOrEditSourceRoutPointInputDto.longitude,
          },
        },
        {
          location: {
            lat: this.routeStepsDetails[i].createOrEditDestinationRoutPointInputDto.latitude,
            lng: this.routeStepsDetails[i].createOrEditDestinationRoutPointInputDto.longitude,
          },
        }
      );
    }
    //set the Dest
    this.wayPointMapDest = {
      lat: this.routeStepsDetails[this.routeStepsDetails.length - 1].createOrEditDestinationRoutPointInputDto?.latitude || undefined,
      lng: this.routeStepsDetails[this.routeStepsDetails.length - 1].createOrEditDestinationRoutPointInputDto?.longitude || undefined,
    };
  }
}
