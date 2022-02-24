import { Component, ElementRef, EventEmitter, Injector, NgZone, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CityPolygonLookupTableDto,
  CreateOrEditFacilityDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  TenantRegistrationServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'createOrEditFacilityModal',
  templateUrl: './create-or-edit-facility-modal.component.html',
  styleUrls: ['./create-or-edit-facility-modal.component.css'],
})
export class CreateOrEditFacilityModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditFacilityModal', { static: true }) modal: ModalDirective;
  @ViewChild('search') public searchElementRef: ElementRef;
  @ViewChild('createFacilityForm') public createFacilityForm: NgForm;
  @ViewChild('secountInput') public secountInput: ElementRef;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  zoom = 14;
  active = false;
  saving = false;
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  countries: any;
  cities: any;
  private geoCoder;
  allCities: CityPolygonLookupTableDto[];
  countriesLoading: boolean;
  citiesLoading: boolean;
  selectedCountryId: number;
  data: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  public styleObject = {
    clickable: true,
    fillColor: 'rgba(143,75,75,0.92)',
    strokeWeight: 1,
  };
  selectedCityJson: Pokedex;
  Bounds: google.maps.LatLngBounds;

  constructor(
    injector: Injector,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _countriesServiceProxy: TenantRegistrationServiceProxy,
    private ngZone: NgZone,
    private mapsAPILoader: MapsAPILoader
  ) {
    super(injector);
  }

  ngOnInit() {
    this.loadAllCountries();
  }

  show(facilityId?: number): void {
    this.active = true;
    if (!facilityId) {
      this.facility = new CreateOrEditFacilityDto();
      this.facility.id = facilityId;
      this.facility.latitude = 24.67911662122269;
      this.facility.longitude = 46.6355543345471;
      this.modal.show();
    } else {
      this._facilitiesServiceProxy.getFacilityForEdit(facilityId).subscribe((result) => {
        this.facility = result.facility;
        this.selectedCountryId = result.countryId;
        this.loadCitiesByCountryId(result.countryId);
        this.data = result.facility;
        this.modal.show();
      });
    }
  }

  /**
   * CreateOrUpdate Facility
   */
  save(): void {
    this.saving = true;
    this._facilitiesServiceProxy
      .createOrEdit(this.facility)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe((id) => {
        console.log(id);
        let facilitycallback: FacilityForDropdownDto = new FacilityForDropdownDto();
        facilitycallback.id = id;
        facilitycallback.displayName = this.facility.name;
        facilitycallback.lat = this.facility.latitude;
        facilitycallback.long = this.facility.longitude;
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(facilitycallback);
      });
  }

  close(): void {
    this.facility = null;
    this.active = false;
    this.modal.hide();
  }

  /**
   * loads Google Map API
   * Setting Up AutoComplete For Facility Address Lookup
   */
  loadMapApi() {
    this.mapsAPILoader.load().then(() => {
      this.geoCoder = new google.maps.Geocoder();
      let Bounds = new google.maps.LatLngBounds();
      console.log('this.selectedCityJson', this.selectedCityJson);
      this.selectedCityJson.geometry.coordinates[0].forEach((x) => {
        let lng: number = x[0];
        let lat: number = x[1];
        Bounds.extend(new google.maps.LatLng(lat, lng, false));
      });
      this.Bounds = Bounds;
      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement, {
        bounds: Bounds,
        strictBounds: true,
      });
      autocomplete.addListener('place_changed', () => {
        this.ngZone.run(() => {
          let place: google.maps.places.PlaceResult = autocomplete.getPlace();
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }
          this.facility.latitude = place.geometry.location.lat();
          this.facility.longitude = place.geometry.location.lng();
          this.getAddress(place.geometry.location.lat(), place.geometry.location.lng());
        });
      });
    });
  }

  /**
   * Converts The Cordinates into Real Address and Assighn it to facility.address
   * @param latitude
   * @param longitude
   */
  getAddress(latitude: number, longitude: number) {
    this.geoCoder.geocode({ location: { lat: latitude, lng: longitude } }, (results, status) => {
      if (status === 'OK') {
        console.log(results);
        if (results[0]) {
          //this.zoom = 14;
          this.facility.address = results[0].formatted_address;
        }
      }
    });
    this.geoCoder.zoom = 16;
  }

  /**
   * Getting The Map Marker Cordinates from The Click
   * @param $event
   */
  mapClicked(event) {
    let position = { lat: event.latLng.lat(), lng: event.latLng.lng() };
    console.log(position);
    this.facility.latitude = position.lat;
    this.facility.longitude = position.lng;
    this.getAddress(Number(this.facility.latitude), Number(this.facility.longitude));
  }

  /**
   * Allow the User To Add Cord Manulay
   * @param $event
   */
  manualCords() {
    /**
     * if manual coordinate is outside the bounds exit
     */
    if (this.Bounds.contains(new google.maps.LatLng(this.facility.latitude, this.facility.longitude))) {
      this.getAddress(Number(this.facility.latitude), Number(this.facility.longitude));
    } else {
      this.createFacilityForm.controls['long'].setErrors({ invalid: true });
      this.createFacilityForm.controls['lat'].setErrors({ invalid: true });
    }
  }

  /**
   * Loads All Countires for Facilities CRUD
   */
  loadAllCountries() {
    console.log('Countries Loaded');
    this.countriesLoading = true;
    this._countriesServiceProxy.getAllCountriesWithCode().subscribe((res) => {
      this.countries = res;
      this.countriesLoading = false;
    });
  }

  /**
   * Loads All Cities After Selecting The Country
   * @Input selectedCountryId
   */

  loadCitiesByCountryId(countryId): any {
    this.citiesLoading = true;
    this.facility.cityId = undefined;
    this._countriesServiceProxy.getAllCitiesWithPolygonsByCountryId(countryId).subscribe((res) => {
      this.allCities = res;
      this.citiesLoading = false;
    });
  }

  /**
   * handle a city select change
   */
  cityChangeEvent() {
    let Json = this.allCities[this.allCities.findIndex((x) => x.id === this.facility.cityId.toString())].polygon;
    this.selectedCityJson = JSON.parse(Json);
    //empty old address
    this.facility.longitude = null;
    this.facility.latitude = null;
    this.facility.address = null;
    // Load Map Api
    this.loadMapApi();
  }
}
export interface Pokedex {
  type: string;
  geometry: Geometry;
  properties: Properties;
}

export interface Geometry {
  type: string;
  coordinates: Array<Array<number[]>>;
}

export interface Properties {
  name: string;
  description: string;
}
