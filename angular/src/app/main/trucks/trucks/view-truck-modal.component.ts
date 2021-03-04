import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTruckForViewOutput, TruckDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTruckModal',
  templateUrl: './view-truck-modal.component.html',
})
export class ViewTruckModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTruckForViewOutput;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTruckForViewOutput();
    this.item.truck = new TruckDto();
  }

  show(item: GetTruckForViewOutput): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
