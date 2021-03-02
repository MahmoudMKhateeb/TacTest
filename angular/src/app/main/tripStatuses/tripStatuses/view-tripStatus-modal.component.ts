import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTripStatusForViewDto, TripStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTripStatusModal',
  templateUrl: './view-tripStatus-modal.component.html',
})
export class ViewTripStatusModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTripStatusForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTripStatusForViewDto();
    this.item.tripStatus = new TripStatusDto();
  }

  show(item: GetTripStatusForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
