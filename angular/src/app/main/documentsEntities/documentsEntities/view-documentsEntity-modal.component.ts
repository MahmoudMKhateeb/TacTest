import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetDocumentsEntityForViewDto, DocumentsEntityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewDocumentsEntityModal',
  templateUrl: './view-documentsEntity-modal.component.html',
})
export class ViewDocumentsEntityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetDocumentsEntityForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetDocumentsEntityForViewDto();
    this.item.documentsEntity = new DocumentsEntityDto();
  }

  show(item: GetDocumentsEntityForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
