import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { VasPricesServiceProxy, CreateOrEditVasPriceDto, VasPriceVasLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditVasPriceModal',
  templateUrl: './create-or-edit-vasPrice-modal.component.html',
})
export class CreateOrEditVasPriceModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  vasPrice: CreateOrEditVasPriceDto = new CreateOrEditVasPriceDto();

  vasName = '';

  allVass: VasPriceVasLookupTableDto[];

  constructor(injector: Injector, private _vasPricesServiceProxy: VasPricesServiceProxy) {
    super(injector);
  }

  show(vasPriceId?: number): void {
    if (!vasPriceId) {
      this.vasPrice = new CreateOrEditVasPriceDto();
      this.vasPrice.id = vasPriceId;
      this.vasName = '';

      this.active = true;
      this.modal.show();
    } else {
      this._vasPricesServiceProxy.getVasPriceForEdit(vasPriceId).subscribe((result) => {
        this.vasPrice = result.vasPrice;

        this.vasName = result.vasName;

        this.active = true;
        this.modal.show();
      });
    }
    this._vasPricesServiceProxy.getAllVasForTableDropdown().subscribe((result) => {
      this.allVass = result;
    });
  }

  save(): void {
    this.saving = true;

    this._vasPricesServiceProxy
      .createOrEdit(this.vasPrice)
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
