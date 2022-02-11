import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { CreateOrEditDriverLicenseTypeDto, DriverLicenseTypesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'createOrEditDriverLicenseTypeModal',
  templateUrl: './create-or-edit-driverLicenseType-modal.component.html',
})
export class CreateOrEditDriverLicenseTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  driverLicenseType: CreateOrEditDriverLicenseTypeDto = new CreateOrEditDriverLicenseTypeDto();

  constructor(injector: Injector, private _driverLicenseTypesServiceProxy: DriverLicenseTypesServiceProxy) {
    super(injector);
  }

  show(driverLicenseTypeId?: number): void {
    if (!driverLicenseTypeId) {
      this.driverLicenseType = new CreateOrEditDriverLicenseTypeDto();
      this.driverLicenseType.id = driverLicenseTypeId;

      this.active = true;
      this.modal.show();
    } else {
      this._driverLicenseTypesServiceProxy.getDriverLicenseTypeForEdit(driverLicenseTypeId).subscribe((result) => {
        this.driverLicenseType = result.driverLicenseType;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._driverLicenseTypesServiceProxy
      .createOrEdit(this.driverLicenseType)
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
