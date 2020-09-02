import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetPickingTypeForViewDto, PickingTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewPickingTypeModal',
  templateUrl: './view-pickingType-modal.component.html',
})
export class ViewPickingTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetPickingTypeForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetPickingTypeForViewDto();
    this.item.pickingType = new PickingTypeDto();
  }

  show(item: GetPickingTypeForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
