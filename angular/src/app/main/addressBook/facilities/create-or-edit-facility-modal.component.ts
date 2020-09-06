import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { FacilitiesServiceProxy, CreateOrEditFacilityDto, FacilityCityLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditFacilityModal',
  templateUrl: './create-or-edit-facility-modal.component.html',
})
export class CreateOrEditFacilityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();

  countyDisplayName = '';
  cityDisplayName = '';

  allCitys: FacilityCityLookupTableDto[];

  constructor(injector: Injector, private _facilitiesServiceProxy: FacilitiesServiceProxy) {
    super(injector);
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
}
