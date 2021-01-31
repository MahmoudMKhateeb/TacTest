import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetFacilityForViewOutput, FacilityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewFacilityModal',
  templateUrl: './view-facility-modal.component.html',
})
export class ViewFacilityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetFacilityForViewOutput;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetFacilityForViewOutput();
    this.item.facility = new FacilityDto();
  }

  show(item: GetFacilityForViewOutput): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
