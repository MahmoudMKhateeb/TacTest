﻿import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTransportTypeForViewDto, TransportTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTransportTypeModal',
  templateUrl: './view-transportType-modal.component.html',
})
export class ViewTransportTypeModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTransportTypeForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTransportTypeForViewDto();
    this.item.transportType = new TransportTypeDto();
  }

  show(item: GetTransportTypeForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}