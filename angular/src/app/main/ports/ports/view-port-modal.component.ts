import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetPortForViewDto, PortDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewPortModal',
  templateUrl: './view-port-modal.component.html',
})
export class ViewPortModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetPortForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetPortForViewDto();
    this.item.port = new PortDto();
  }

  show(item: GetPortForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
