import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTransportSubtypeForViewDto, TransportSubtypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTransportSubtypeModal',
  templateUrl: './view-transportSubtype-modal.component.html',
})
export class ViewTransportSubtypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTransportSubtypeForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTransportSubtypeForViewDto();
    this.item.transportSubtype = new TransportSubtypeDto();
  }

  show(item: GetTransportSubtypeForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
