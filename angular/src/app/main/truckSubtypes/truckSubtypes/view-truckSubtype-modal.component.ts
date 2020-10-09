import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTruckSubtypeForViewDto, TruckSubtypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTruckSubtypeModal',
  templateUrl: './view-truckSubtype-modal.component.html',
})
export class ViewTruckSubtypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTruckSubtypeForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTruckSubtypeForViewDto();
    this.item.truckSubtype = new TruckSubtypeDto();
  }

  show(item: GetTruckSubtypeForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
