import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CapacitiesServiceProxy, CapacityTruckTypeLookupTableDto, CreateOrEditCapacityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditCapacityModal',
  templateUrl: './create-or-edit-capacity-modal.component.html',
})
export class CreateOrEditCapacityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  capacity: CreateOrEditCapacityDto = new CreateOrEditCapacityDto();

  truckTypeDisplayName = '';

  allTruckTypes: CapacityTruckTypeLookupTableDto[];

  constructor(injector: Injector, private _capacitiesServiceProxy: CapacitiesServiceProxy) {
    super(injector);
  }

  show(capacityId?: number): void {
    if (!capacityId) {
      this.capacity = new CreateOrEditCapacityDto();
      this.capacity.id = capacityId;
      this.capacity.trucksTypeId = null;
      this.truckTypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._capacitiesServiceProxy.getCapacityForEdit(capacityId).subscribe((result) => {
        this.capacity = result.capacity;

        this.truckTypeDisplayName = result.truckTypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._capacitiesServiceProxy.getAllTruckTypeForTableDropdown().subscribe((result) => {
      this.allTruckTypes = result;
    });
  }

  save(): void {
    this.saving = true;
    if (this.capacity.trucksTypeId == null) {
      this.notify.error(this.l('PleaseChooseATruckType'));
      return;
    }
    this._capacitiesServiceProxy
      .createOrEdit(this.capacity)
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
