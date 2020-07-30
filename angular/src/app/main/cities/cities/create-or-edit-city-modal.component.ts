import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CitiesServiceProxy, CreateOrEditCityDto, CityCountyLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditCityModal',
  styles: [
    `
      agm-map {
        height: 300px;
      }
    `,
  ],
  templateUrl: './create-or-edit-city-modal.component.html',
})
export class CreateOrEditCityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  city: CreateOrEditCityDto = new CreateOrEditCityDto();

  countyDisplayName = '';

  allCountys: CityCountyLookupTableDto[];
  zoom = 8;

  constructor(injector: Injector, private _citiesServiceProxy: CitiesServiceProxy) {
    super(injector);
  }

  show(cityId?: number): void {
    if (!cityId) {
      this.city = new CreateOrEditCityDto();
      this.city.id = cityId;
      this.countyDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._citiesServiceProxy.getCityForEdit(cityId).subscribe((result) => {
        this.city = result.city;

        this.countyDisplayName = result.countyDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._citiesServiceProxy.getAllCountyForTableDropdown().subscribe((result) => {
      this.allCountys = result;
    });
  }

  save(): void {
    this.saving = true;

    this._citiesServiceProxy
      .createOrEdit(this.city)
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
  mapClicked($event: MouseEvent) {
    // @ts-ignore
    this.city.latitude = $event.coords.lat.toString();
    // @ts-ignore
    this.city.longitude = $event.coords.lng.toString();
  }
}
