import { Component, ViewChild, Injector, Output, EventEmitter, NgZone, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  FacilitiesServiceProxy,
  CreateOrEditFacilityDto,
  FacilityCityLookupTableDto,
  CountiesServiceProxy,
  TenantRegistrationServiceProxy,
  TenantCityLookupTableDto,
  FacilityForDropdownDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MapsAPILoader } from '@node_modules/@agm/core';

@Component({
  selector: 'createOrEditFacilityModal',
  templateUrl: './create-or-edit-facility-modal.component.html',
})
export class CreateOrEditFacilityModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditFacilityModal', { static: true }) modal: ModalDirective;
  @ViewChild('search') public searchElementRef: ElementRef;
  @ViewChild('secountInput') public secountInput: ElementRef;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  zoom = 14;
  active = false;
  saving = false;
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  countries: any;
  cities: any;
  private geoCoder;
  allCities: TenantCityLookupTableDto[];
  countriesLoading: boolean;
  citiesLoading: boolean;
  selectedCountryId: number;
  selectedCountryCode = 'SA';
  data: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();

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
    this.loadMapApi();
    this.loadAllCountries();
    this.facility.latitude = 24.67911662122269;
    this.facility.longitude = 46.6355543345471;
  }

  show(facilityId?: number): void {
    if (!facilityId) {
      this.facility = new CreateOrEditFacilityDto();
      this.facility.id = facilityId;
      this.active = true;
      this.modal.show();
    } else {
      this._facilitiesServiceProxy.getFacilityForEdit(facilityId).subscribe((result) => {
        this.facility = result.facility;
        this.selectedCountryId = result.countryId;
        this.loadCitiesByCountryId(result.countryId);
        this.data = result.facility;
        this.active = true;
        this.modal.show();
      });
    }
    // this._facilitiesServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
    //   this.allCities = result;
    // });
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
    // this.facility = new CreateOrEditFacilityDto();
    this.facility.cityId = null;
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
      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement, {
        componentRestrictions: {
          country: this.selectedCountryCode,
        },
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
          this.zoom = 12;
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
        if (results[0]) {
          this.zoom = 14;
          let address = results[0].formatted_address;
          let pin = results[0].address_components[results[0].address_components.length - 1].long_name;
          let country = results[0].address_components[results[0].address_components.length - 2].long_name;
          let state = results[0].address_components[results[0].address_components.length - 3].long_name;
          let city = results[0].address_components[results[0].address_components.length - 4].long_name;
          this.facility.cityId = null;
          this.selectedCountryId = null;
          console.log('address : ', address);
          console.log('pin : ', pin);
          console.log('country : ', country);
          console.log('state : ', state);
          console.log('city : ', city);
          this.facility.address = results[0].formatted_address;
        } else {
          window.alert('No results found');
        }
      } else {
        window.alert('Geocoder failed due to: ' + status);
      } //
    });
  }

  /**
   * Getting The Map Marker Cordinates from The Click
   * @param $event
   */
  mapClicked($event) {
    // @ts-ignore
    this.facility.latitude = $event.coords.lat;
    // @ts-ignore
    this.facility.longitude = $event.coords.lng;
    // @ts-ignore
    this.getAddress(Number(this.facility.latitude), Number(this.facility.longitude));
  }

  /**
   * Allow the User To Add Cord Manulay
   * @param $event
   */
  manualCords() {
    // @ts-ignore
    this.getAddress(Number(this.facility.latitude), Number(this.facility.longitude));
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
    this.selectedCountryCode = this.countries.find((x) => x.id == this.selectedCountryId).code;
    this.loadMapApi();
    this.citiesLoading = true;
    this.facility.cityId = undefined;
    this._countriesServiceProxy
      .getAllCitiesForTableDropdown(countryId)
      .pipe(
        finalize(() => {
          this.facility = this.data;
        })
      )
      .subscribe((res) => {
        this.allCities = res;
        this.citiesLoading = false;
      });
  }
}
