import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TrucksTypesServiceProxy, CreateOrEditTrucksTypeDto, SelectItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

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
  allTransportTypes: SelectItemDto[];
  constructor(injector: Injector, private _trucksTypesServiceProxy: TrucksTypesServiceProxy) {
    super(injector);
  }

  show(trucksTypeId?: number): void {
    if (!trucksTypeId) {
      this.trucksType = new CreateOrEditTrucksTypeDto();
      this.trucksType.id = trucksTypeId;
      this.trucksType.transportTypeId = null;
      this.active = true;
      this.modal.show();
    } else {
      this._trucksTypesServiceProxy.getTrucksTypeForEdit(trucksTypeId).subscribe((result) => {
        this.trucksType = result.trucksType;

        this.active = true;
        this.modal.show();
      });
    }
    this._trucksTypesServiceProxy.getAllTransportTypeForTableDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });
  }

  save(): void {
    this.saving = true;
    if (this.trucksType.transportTypeId == -1) {
      this.notify.error(this.l('PleaseChooseATransportType'));
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
