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
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
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
  getAddress(latitude, longitude) {
    this.geoCoder.geocode({ location: { lat: latitude, lng: longitude } }, (results, status) => {
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 14;
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
   * @param $event: MouseEvent
   */
  mapClicked($event: MouseEvent) {
    // @ts-ignore
    this.facility.latitude = $event.coords.lat;
    // @ts-ignore
    this.facility.longitude = $event.coords.lng;
    // @ts-ignore
    this.getAddress($event.coords.lat, $event.coords.lng);
  }

  /**
   * Loads All Countires for Facilities CRUD
   */
  loadAllCountries() {
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

  loadCitiesByCountryId() {
    this.selectedCountryCode = this.countries.find((x) => x.id == this.selectedCountryId).code;
    this.loadMapApi();
    this.citiesLoading = true;
    this._countriesServiceProxy.getAllCitiesForTableDropdown(this.selectedCountryId).subscribe((res) => {
      this.allCities = res;
      this.citiesLoading = false;
    });
  }
}
