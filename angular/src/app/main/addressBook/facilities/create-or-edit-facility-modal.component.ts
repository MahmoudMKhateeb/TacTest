import { Component, ElementRef, EventEmitter, Injector, Input, NgZone, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  CityPolygonLookupTableDto,
  CountyDto,
  CreateOrEditFacilityDto,
  CreateOrEditFacilityWorkingHourDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  FacilityType,
  PenaltiesServiceProxy,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestsServiceProxy,
  TenantRegistrationServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { NgForm } from '@angular/forms';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { Pokedex, styleObject } from '@app/main/addressBook/facilities/facilites-helper';
import { WeekDay } from '@angular/common';
import { CreateOrEditWorkingHoursComponent } from '@app/shared/common/workingHours/create-or-edit-working-hours/create-or-edit-working-hours.component';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

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
  @Input('isForDedicated') isForDedicated: boolean;
  @Input('isHomeDelivery') isHomeDelivery: boolean;

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
  AllActorsShippers: SelectItemDto[];
  days = WeekDay;
  FacilityWorkingHours: any[];
  mapCenterLat: number;
  mapCenterLng: number;
  AllTenants: ShippersForDropDownDto[];

  callbacks: any[] = [];
  adapterConfig = {
    getValue: () => {
      return this.validateWorkingHours();
    },
    applyValidationResults: (e) => {
      this.isWorkingHoursInvalid = !e.isValid;
    },
    validationRequestsCallbacks: this.callbacks,
  };
  isWorkingHoursInvalid = false;
  private cityId: number;
  allFacilityTypes: { key: number; value: string }[] = [];

  get isRequiredFacilityName(): boolean {
    if (!this.isHomeDelivery) {
      return true;
    }
    return !isNotNullOrUndefined(this.facility.address) || this.facility.address === '';
  }
  get isRequiredAddress(): boolean {
    if (!this.isHomeDelivery) {
      return true;
    }
    return !isNotNullOrUndefined(this.facility.name) || this.facility.name === '';
  }

  constructor(
    injector: Injector,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _countriesServiceProxy: TenantRegistrationServiceProxy,
    private ngZone: NgZone,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private _penaltiesServiceProxy: PenaltiesServiceProxy,
    private _enumService: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit() {
    console.log('isHomeDelivery', this.isHomeDelivery);
    this.loadAllFacilityTypes();
    this.loadAllCountries();
    this.loadAllCompaniesForDropDown();
    if (this.feature.isEnabled('App.ShipperClients')) {
      this._shippingRequestsServiceProxy.getAllShippersActorsForDropDown().subscribe((result) => {
        this.AllActorsShippers = result;
        // let defaultItem = new SelectItemDto();
        // defaultItem.id = null;
        // defaultItem.displayName = this.l('Myself');
        // this.AllActorsShippers.unshift(defaultItem);
      });
    }
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
        result.push({
          dayOfWeek: propertyKey.dayOfWeek,
          name: this.days[propertyKey.dayOfWeek],
          hasTime: false,
          facilityId: list2[0].facilityId,
        });
      }
    }
    return result;
  }

  show(facilityId?: number): void {
    console.log('isHomeDelivery', this.isHomeDelivery);
    this.active = true;

    if (!facilityId) {
      this.facility = new CreateOrEditFacilityDto();
      this.facility.id = facilityId;
      this.facility.latitude = 24.67911662122269;
      this.facility.longitude = 46.6355543345471;
      this.FacilityWorkingHours = this.getEnumsAsList();
      this.facility.isForHomeDelivery = this.isHomeDelivery;
      this.modal.show();
    } else {
      this._facilitiesServiceProxy.getFacilityForEdit(facilityId).subscribe((result) => {
        this.cityId = result.facility.cityId;
        this.facility = result.facility;
        this.FacilityWorkingHours = [];
        if (result.facility.facilityWorkingHours.length == 0) {
          this.FacilityWorkingHours = this.getEnumsAsList();
        } else {
          this.FacilityWorkingHours = this.getEnumsAsFillList(result.facility.facilityWorkingHours);
        }
        this.selectedCountryId = result.countryId;
        this.loadCitiesByCountryId(result.countryId);
        if (isNotNullOrUndefined(this.facility.shipperActorId)) {
          (this.facility.shipperActorId as any) = this.facility.shipperActorId?.toString();
        }

        (this.facility.cityId as any) = this.facility.cityId.toString();
      });
    }
    this.modal.show();
  }

  /**
   * CreateOrUpdate Facility
   */
  save(): void {
    this.revalidateWorkingHours();
    this.saving = true;
    console.log(this.facility.shipperId);

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
    if (this.facility.facilityWorkingHours.length == 0 && !this.isHomeDelivery) {
      this.notify.error(this.l('PleaseEnterfacilityWorkingHours'));
      this.saving = false;
      return;
    }

    if (this.facility.cityId == undefined && this.isRequiredAddress) {
      this.notify.error(this.l('PleaseEnterCity'));
      this.saving = false;
      return;
    }

    console.log('this.facility', this.facility);
    if (!this.isRequiredAddress && !this.facility.address) {
      this.facility.latitude = null;
      this.facility.longitude = null;
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
    this.facility = new CreateOrEditFacilityDto();
    this.selectedCountryId = null;
    this.FacilityWorkingHours = null;
    this.isWorkingHoursInvalid = false;
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
        this.getMapCenter();
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
      position = { lat: $event?.latLng?.lat(), lng: $event?.latLng?.lng() };
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

  loadAllCompaniesForDropDown() {
    this._penaltiesServiceProxy.getAllCompanyForDropDown().subscribe((result) => {
      this.AllTenants = result;
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

      if (this.cityId != null) {
        this.facility.cityId = this.cityId;
        this.cityId = null;
      } else {
        this.facility.cityId = null;
      }
      //
      // let selectedCity = this.allCities.find((x) => x.id == this.facility.cityId.toString());
      // if (isNotNullOrUndefined(countryId) && selectedCity.countryId != countryId) {
      //   this.facility.cityId = null;
      // }
      this.handleCityPolygon();
    });
  }

  /**
   * Gets the Selected City Polygons
   */
  handleCityPolygon() {
    if (!isNotNullOrUndefined(this.facility.cityId)) {
      return;
    }
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

  /**
   * gets map center from polygons json
   * @private
   */
  private getMapCenter() {
    if (typeof this.selectedCityJson.geometry.coordinates[0][0][0] === 'number') {
      this.mapCenterLng = this.selectedCityJson.geometry.coordinates[0][0][0];
      this.mapCenterLat = this.selectedCityJson.geometry.coordinates[0][0][1];
    } else {
      this.mapCenterLng = this.selectedCityJson.geometry.coordinates[0][0][0][0];
      this.mapCenterLat = this.selectedCityJson.geometry.coordinates[0][0][0][1];
    }
    this.zoom = 10;
  }

  private validateWorkingHours() {
    const facilityWorkingHours = this.FacilityWorkingHours.filter((r) => r.startTime && r.endTime && r.hasTime);
    if (facilityWorkingHours.length === 0 && !this.isHomeDelivery) {
      return false;
    }
    return true;
  }

  revalidateWorkingHours() {
    this.callbacks.forEach((func) => {
      func();
    });
  }

  /**
   * Loads All Facility types for Facilities CRUD
   */
  loadAllFacilityTypes() {
    this.allFacilityTypes = this._enumService.transform(FacilityType).map((item) => {
      item.key = Number(item.key);
      return item;
    });
  }
}
