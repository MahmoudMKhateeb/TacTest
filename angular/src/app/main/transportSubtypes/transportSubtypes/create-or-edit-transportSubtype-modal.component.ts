import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  TransportSubtypesServiceProxy,
  CreateOrEditTransportSubtypeDto,
  TransportSubtypeTransportTypeLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTransportSubtypeModal',
  templateUrl: './create-or-edit-transportSubtype-modal.component.html',
})
export class CreateOrEditTransportSubtypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  transportSubtype: CreateOrEditTransportSubtypeDto = new CreateOrEditTransportSubtypeDto();

  transportTypeDisplayName = '';

  allTransportTypes: TransportSubtypeTransportTypeLookupTableDto[];

  constructor(injector: Injector, private _transportSubtypesServiceProxy: TransportSubtypesServiceProxy) {
    super(injector);
  }

  show(transportSubtypeId?: number): void {
    if (!transportSubtypeId) {
      this.transportSubtype = new CreateOrEditTransportSubtypeDto();
      this.transportSubtype.id = transportSubtypeId;
      this.transportSubtype.transportTypeId = null;
      this.transportTypeDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._transportSubtypesServiceProxy.getTransportSubtypeForEdit(transportSubtypeId).subscribe((result) => {
        this.transportSubtype = result.transportSubtype;

        this.transportTypeDisplayName = result.transportTypeDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._transportSubtypesServiceProxy.getAllTransportTypeForTableDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });
  }

  save(): void {
    this.saving = true;
    if (this.transportSubtype.transportTypeId == null) {
      this.notify.error(this.l('PleaseChooseATransportType'));
      return;
    }
    this._transportSubtypesServiceProxy
      .createOrEdit(this.transportSubtype)
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
