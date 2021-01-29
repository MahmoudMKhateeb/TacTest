import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetDocumentFileForViewDto,
  DocumentFileDto,
  DocumentTypeDto,
  UserInGetDocumentFileForViewDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewDocumentFileModal',
  templateUrl: './view-documentFile-modal.component.html',
})
export class ViewDocumentFileModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetDocumentFileForViewDto;
  entityType: string = '';
  isHost: boolean = false;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetDocumentFileForViewDto();
    this.item.documentFile = new DocumentFileDto();
    this.item.documentType = new DocumentTypeDto();
    this.item.user = new UserInGetDocumentFileForViewDto();
  }

  show(item: GetDocumentFileForViewDto, entityType: string, isHost: boolean): void {
    this.item = item;
    this.isHost = isHost;
    this.entityType = entityType;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
