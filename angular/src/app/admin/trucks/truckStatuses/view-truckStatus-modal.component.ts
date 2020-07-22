import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTruckStatusForViewDto, TruckStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTruckStatusModal',
  templateUrl: './view-truckStatus-modal.component.html',
})
export class ViewTruckStatusModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTruckStatusForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTruckStatusForViewDto();
    this.item.truckStatus = new TruckStatusDto();
  }

  show(item: GetTruckStatusForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
