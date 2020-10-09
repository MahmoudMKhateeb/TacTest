import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CapacitiesServiceProxy, CreateOrEditCapacityDto, CapacityTruckSubtypeLookupTableDto } from '@shared/service-proxies/service-proxies';
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

  truckSubtypeDisplayName = '';

  allTruckSubtypes: CapacityTruckSubtypeLookupTableDto[];

  constructor(injector: Injector, private _capacitiesServiceProxy: CapacitiesServiceProxy) {
    super(injector);
  }

  show(capacityId?: number): void {
    if (!capacityId) {
      this.capacity = new CreateOrEditCapacityDto();
      this.capacity.id = capacityId;
      this.truckSubtypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._capacitiesServiceProxy.getCapacityForEdit(capacityId).subscribe((result) => {
        this.capacity = result.capacity;

        this.truckSubtypeDisplayName = result.truckSubtypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._capacitiesServiceProxy.getAllTruckSubtypeForTableDropdown().subscribe((result) => {
      this.allTruckSubtypes = result;
    });
  }

  save(): void {
    this.saving = true;

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
