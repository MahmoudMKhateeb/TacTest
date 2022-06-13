import { Component, ElementRef, EventEmitter, Injector, NgZone, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CityPolygonLookupTableDto,
  CountyDto,
  CreateOrEditFacilityDto,
  CreateOrEditFacilityWorkingHourDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  TenantRegistrationServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { NgForm } from '@angular/forms';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { Pokedex, styleObject } from '@app/main/addressBook/facilities/facilites-helper';
import { WeekDay } from '@angular/common';
import { CreateOrEditWorkingHoursComponent } from '@app/shared/common/workingHours/create-or-edit-working-hours/create-or-edit-working-hours.component';

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
  @ViewChild('CreateOrEditWorkingHoursComponent', { static: true }) CreateOrEditWorkingHoursComponent: CreateOrEditWorkingHoursComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  zoom = 6;
  active = false;
  saving = false;
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  cities: any;
  allCities: CityPolygonLookupTableDto[];
  countriesLoading: boolean;
  citiesLoading: boolean;
  selectedCountryId: number;

  selectedCityJson: Pokedex;
  Bounds: google.maps.LatLngBounds;
  countries: CountyDto[];
  private geoCoder: google.maps.Geocoder;
  polygonStyle = styleObject;
  days = WeekDay;
  FacilityWorkingHours: any[];

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

  private get SelectedCountryCode(): string {
    return this.countries?.find((x) => x.id == this.selectedCountryId)?.code;
  }
  getEnumsAsList() {
    const result = [];
    for (const [propertyKey, propertyValue] of Object.entries(this.days)) {
      if (!Number.isNaN(Number(propertyKey))) {
        continue;
      }
      result.push({ dayOfWeek: this.days[propertyKey], name: propertyKey });
    }
    return result;
  }

  getEnumsAsFillList(list2: CreateOrEditFacilityWorkingHourDto[]) {
    const result = [];
    let list = this.getEnumsAsList();
    for (const propertyKey of list) {
      if (!Number.isNaN(Number(propertyKey.dayOfWeek)) && result.filter((r) => r.dayOfWeek == propertyKey.dayOfWeek).length > 1) {
        continue;
      }
      var item = list2.find((r) => r.dayOfWeek === propertyKey.dayOfWeek);

      if (item && result.filter((r) => r.dayOfWeek === propertyKey.dayOfWeek).length == 0) {
        result.push({
          dayOfWeek: propertyKey.dayOfWeek,
          name: this.days[propertyKey.dayOfWeek],
          startTime: item.startTime,
          endTime: item.endTime,
          hasTime: true,
          id: item.id,
          facilityId: item.facilityId,
        });
      } else {
        result.push({ dayOfWeek: propertyKey.dayOfWeek, name: this.days[propertyKey.dayOfWeek], hasTime: false, facilityId: list2[0].facilityId });
      }
    }
    return result;
  }

  show(facilityId?: number): void {
    this.active = true;
    if (!facilityId) {
      this.facility = new CreateOrEditFacilityDto();
      this.facility.id = facilityId;
      this.facility.latitude = 24.67911662122269;
      this.facility.longitude = 46.6355543345471;
      this.FacilityWorkingHours = this.getEnumsAsList();
      this.modal.show();
    } else {
      this._facilitiesServiceProxy.getFacilityForEdit(facilityId).subscribe((result) => {
        this.facility = result.facility;
        this.FacilityWorkingHours = [];
        if (result.facility.facilityWorkingHours.length == 0) {
          this.FacilityWorkingHours = this.getEnumsAsList();
        } else {
          this.FacilityWorkingHours = this.getEnumsAsFillList(result.facility.facilityWorkingHours);
        }
        this.selectedCountryId = result.countryId;
        this.loadCitiesByCountryId(result.countryId);
      });
    }
    this.modal.show();
  }

  /**
   * CreateOrUpdate Facility
   */
  save(): void {
    this.saving = true;
    this.facility.facilityWorkingHours = this.FacilityWorkingHours.filter((r) => r.startTime && r.endTime && r.hasTime).map(
      (fh) =>
        new CreateOrEditFacilityWorkingHourDto({
          dayOfWeek: fh.dayOfWeek,
          startTime: fh.startTime,
          endTime: fh.endTime,
          facilityId: this.facility.id == undefined ? null : fh.facilityId,
          id: this.facility.id == undefined ? null : fh.id,
        })
    );
    if (this.facility.facilityWorkingHours.length == 0) {
      this.notify.error(this.l('PleaseEnterfacilityWorkingHours'));
      this.saving = false;
      return;
    }

    if (this.facility.cityId == undefined) {
      this.notify.error(this.l('PleaseEnterCity'));
      this.saving = false;
      return;
    }

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
    this.FacilityWorkingHours = null;
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
      if (this.selectedCityJson) {
        this.selectedCityJson.geometry.coordinates[0].forEach((x) => {
          let lng: number = x[0];
          let lat: number = x[1];
          Bounds.extend(new google.maps.LatLng(lat, lng, false));
        });
      }
      this.Bounds = Bounds;
      let options = {
        borderRestriction: {
          bounds: Bounds,
          strictBounds: true,
        },
        countryRestriction: {
          componentRestrictions: {
            country: this.SelectedCountryCode,
          },
        },
      };

      let autocomplete = new google.maps.places.Autocomplete(
        this.searchElementRef.nativeElement,
        isNotNullOrUndefined(this.selectedCityJson) ? options.borderRestriction : options.countryRestriction
      );
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
          this.zoom = 10;
          this.facility.address = results[0].formatted_address;
        }
      }
    });
  }

  /**
   * Getting The Map Marker Cordinates from The Click
   * @param $event
   */
  mapClicked($event) {
    let position;
    if (isNotNullOrUndefined(this.selectedCityJson)) {
      position = { lat: $event.latLng.lat(), lng: $event.latLng.lng() };
    } else {
      position = { lat: $event.coords.lat, lng: $event.coords.lng };
    }
    console.log(position);
    this.facility.latitude = position.lat;
    this.facility.longitude = position.lng;
    this.getAddress(Number(this.facility.latitude), Number(this.facility.longitude));
  }

  /**
   * Allow the User to Add a Manual Coordinates and Apply Boundaries Validation
   */
  manualCords(): void {
    //check if manual Cord are within the selected city
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
    this._countriesServiceProxy.getAllCitiesWithPolygonsByCountryId(countryId).subscribe((res) => {
      this.allCities = res;
      this.citiesLoading = false;
      this.handleCityPolygon();
    });
  }

  /**
   * Gets the Selected City Polygons
   */
  handleCityPolygon() {
    let Json = this.allCities[this.allCities.findIndex((x) => x.id === this.facility.cityId.toString())].polygon;
    this.selectedCityJson = JSON.parse(Json);
    //empty old address
    if (!this.facility.id) {
      this.facility.longitude = null;
      this.facility.latitude = null;
      this.facility.address = null;
    }
    // Load Map Api
    this.loadMapApi();
  }
}
