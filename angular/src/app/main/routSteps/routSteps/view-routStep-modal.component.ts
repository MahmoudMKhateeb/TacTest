import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetRoutStepForViewOutput, RoutStepDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewRoutStepModal',
  templateUrl: './view-routStep-modal.component.html',
})
export class ViewRoutStepModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetRoutStepForViewOutput;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetRoutStepForViewOutput();
    this.item.routStep = new RoutStepDto();
  }

  show(item: GetRoutStepForViewOutput): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
