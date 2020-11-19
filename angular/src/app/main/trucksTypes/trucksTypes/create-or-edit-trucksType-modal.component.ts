import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  TrucksTypesServiceProxy,
  CreateOrEditTrucksTypeDto,
  TransportSubtypeTransportTypeLookupTableDto,
  TransportSubtypesServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

@Component({
  selector: 'createOrEditTrucksTypeModal',
  templateUrl: './create-or-edit-trucksType-modal.component.html',
})
export class CreateOrEditTrucksTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  trucksType: CreateOrEditTrucksTypeDto = new CreateOrEditTrucksTypeDto();
  allTransportSubTypes: TransportSubtypeTransportTypeLookupTableDto[];
  constructor(
    injector: Injector,
    private _trucksTypesServiceProxy: TrucksTypesServiceProxy,
    private _transportSubtypesServiceProxy: TransportSubtypesServiceProxy
  ) {
    super(injector);
  }

  show(trucksTypeId?: number): void {
    if (!trucksTypeId) {
      this.trucksType = new CreateOrEditTrucksTypeDto();
      this.trucksType.id = trucksTypeId;
      this.trucksType.transportSubtypeId = null;
      this.active = true;
      this.modal.show();
    } else {
      this._trucksTypesServiceProxy.getTrucksTypeForEdit(trucksTypeId).subscribe((result) => {
        this.trucksType = result.trucksType;

        this.active = true;
        this.modal.show();
      });
    }
    this._transportSubtypesServiceProxy.getAllTransportTypeForTableDropdown().subscribe((result) => {
      this.allTransportSubTypes = result;
    });
  }

  save(): void {
    this.saving = true;
    if (this.trucksType.transportSubtypeId == -1) {
      this.notify.error(this.l('PleaseChooseATransportSubType'));
      return;
    }
    this._trucksTypesServiceProxy
      .createOrEdit(this.trucksType)
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
