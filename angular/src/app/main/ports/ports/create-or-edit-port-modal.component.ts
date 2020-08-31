import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PortsServiceProxy, CreateOrEditPortDto, PortCityLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditPortModal',
  templateUrl: './create-or-edit-port-modal.component.html',
})
export class CreateOrEditPortModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  port: CreateOrEditPortDto = new CreateOrEditPortDto();

  cityDisplayName = '';

  allCitys: PortCityLookupTableDto[];

  constructor(injector: Injector, private _portsServiceProxy: PortsServiceProxy) {
    super(injector);
  }

  show(portId?: number): void {
    if (!portId) {
      this.port = new CreateOrEditPortDto();
      this.port.id = portId;
      this.cityDisplayName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._portsServiceProxy.getPortForEdit(portId).subscribe((result) => {
        this.port = result.port;

        this.cityDisplayName = result.cityDisplayName;

        this.active = true;
        this.modal.show();
      });
    }
    this._portsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
  }

  save(): void {
    this.saving = true;

    this._portsServiceProxy
      .createOrEdit(this.port)
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
