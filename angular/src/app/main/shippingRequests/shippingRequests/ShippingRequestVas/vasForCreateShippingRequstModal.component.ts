import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'VasForCreateShippingRequstModalComponent',
  templateUrl: './Vas-for-create-shipping-request-modal.html',
})
export class VasForCreateShippingRequstModalComponent extends AppComponentBase {
  @ViewChild('VasForCreateShippingRequstModalComponent', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  constructor(injector: Injector) {
    super(injector);
  }

  show(facilityId?: number): void {}

  save(): void {
    this.saving = true;
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
