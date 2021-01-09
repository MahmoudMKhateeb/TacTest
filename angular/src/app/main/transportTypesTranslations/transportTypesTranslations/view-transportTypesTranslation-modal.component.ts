import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTransportTypesTranslationForViewDto, TransportTypesTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTransportTypesTranslationModal',
  templateUrl: './view-transportTypesTranslation-modal.component.html',
})
export class ViewTransportTypesTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTransportTypesTranslationForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTransportTypesTranslationForViewDto();
    this.item.transportTypesTranslation = new TransportTypesTranslationDto();
  }

  show(item: GetTransportTypesTranslationForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
