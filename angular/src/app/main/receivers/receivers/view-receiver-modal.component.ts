import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetReceiverForViewDto, ReceiverDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewReceiverModal',
  templateUrl: './view-receiver-modal.component.html',
})
export class ViewReceiverModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetReceiverForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetReceiverForViewDto();
    this.item.receiver = new ReceiverDto();
  }

  show(item: GetReceiverForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
