import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetPackingTypeForViewDto, PackingTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewPackingTypeModal',
  templateUrl: './view-packingType-modal.component.html',
})
export class ViewPackingTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetPackingTypeForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetPackingTypeForViewDto();
    this.item.packingType = new PackingTypeDto();
  }

  show(item: GetPackingTypeForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
