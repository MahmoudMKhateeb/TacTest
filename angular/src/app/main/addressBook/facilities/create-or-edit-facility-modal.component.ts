import { Component, ViewChild, Injector, Output, EventEmitter, NgZone, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { FacilitiesServiceProxy, CreateOrEditFacilityDto, FacilityCityLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MapsAPILoader } from '@node_modules/@agm/core';

@Component({
  selector: 'createOrEditFacilityModal',
  templateUrl: './create-or-edit-facility-modal.component.html',
})
export class CreateOrEditFacilityModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditFacilityModal', { static: true }) modal: ModalDirective;
  @ViewChild('search') public searchElementRef: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  zoom = 14;
  active = false;
  saving = false;
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  private geoCoder;

  selectedCountryCode = 'SA';
  countyDisplayName = '';
  cityDisplayName = '';

  Address: string;
  State: string;
  Postal: string;
  City: string;
  Country: string;
  allCitys: FacilityCityLookupTableDto[];

  constructor(
    injector: Injector,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private ngZone: NgZone,
    private mapsAPILoader: MapsAPILoader
  ) {
    super(injector);
  }
  ngOnInit() {
    this.loadMapApi();
    this.facility.latitude = 24.67911662122269;
    this.facility.longitude = 46.6355543345471;
  }

  show(facilityId?: number): void {
    if (!facilityId) {
      this.facility = new CreateOrEditFacilityDto();
      this.facility.id = facilityId;
      this.countyDisplayName = '';
      this.cityDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._facilitiesServiceProxy.getFacilityForEdit(facilityId).subscribe((result) => {
        this.facility = result.facility;

        this.cityDisplayName = result.cityDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._facilitiesServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
    console.log('From Facility Modal');
  }

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
  //Facility Map Functions
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
}
