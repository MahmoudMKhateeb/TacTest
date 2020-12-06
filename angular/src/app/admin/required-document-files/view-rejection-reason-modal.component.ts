import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetDocumentFileForViewDto, DocumentFileDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewRejectionReasonModal',
  templateUrl: './view-rejection-reason-modal.component.html',
})
export class ViewRejectionReasonModalComponent extends AppComponentBase {
  @ViewChild('viewRejectionReasonModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  reaosn = '';
  documentName = '';
  saving = false;
  constructor(injector: Injector) {
    super(injector);
  }

  show(documentName: string, reason: string): void {
    this.reaosn = reason;
    this.documentName = documentName;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
