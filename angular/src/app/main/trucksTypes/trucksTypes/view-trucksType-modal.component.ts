import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTrucksTypeForViewDto, TrucksTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTrucksTypeModal',
  templateUrl: './view-trucksType-modal.component.html',
})
export class ViewTrucksTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTrucksTypeForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTrucksTypeForViewDto();
    this.item.trucksType = new TrucksTypeDto();
  }

  show(item: GetTrucksTypeForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
