import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetCapacityForViewDto, CapacityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewCapacityModal',
  templateUrl: './view-capacity-modal.component.html',
})
export class ViewCapacityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetCapacityForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetCapacityForViewDto();
    this.item.capacity = new CapacityDto();
  }

  show(item: GetCapacityForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
