import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  TruckSubtypesServiceProxy,
  CreateOrEditTruckSubtypeDto,
  TruckSubtypeTrucksTypeLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTruckSubtypeModal',
  templateUrl: './create-or-edit-truckSubtype-modal.component.html',
})
export class CreateOrEditTruckSubtypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  truckSubtype: CreateOrEditTruckSubtypeDto = new CreateOrEditTruckSubtypeDto();

  trucksTypeDisplayName = '';

  allTrucksTypes: TruckSubtypeTrucksTypeLookupTableDto[];

  constructor(injector: Injector, private _truckSubtypesServiceProxy: TruckSubtypesServiceProxy) {
    super(injector);
  }

  show(truckSubtypeId?: number): void {
    if (!truckSubtypeId) {
      this.truckSubtype = new CreateOrEditTruckSubtypeDto();
      this.truckSubtype.id = truckSubtypeId;
      this.truckSubtype.trucksTypeId = null;
      this.trucksTypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._truckSubtypesServiceProxy.getTruckSubtypeForEdit(truckSubtypeId).subscribe((result) => {
        this.truckSubtype = result.truckSubtype;

        this.trucksTypeDisplayName = result.trucksTypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._truckSubtypesServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
  }

  save(): void {
    this.saving = true;
    if (this.truckSubtype.trucksTypeId == -1) {
      this.notify.error(this.l('PleaseChooseATruckType'));
      return;
    }
    this._truckSubtypesServiceProxy
      .createOrEdit(this.truckSubtype)
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
