import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetDocumentFileForViewDto, DocumentFileDto, DocumentFilesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'rejectionReasonModal',
  templateUrl: './rejectionReason-modal.component.html',
})
export class RejectionReasonModalComponent extends AppComponentBase {
  @ViewChild('rejectionReasonModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  id: string;
  rejectionReason: string;
  documentName: string;
  tenantName: string;

  isHost: boolean = false;

  constructor(injector: Injector, private _documentFilesServiceProxy: DocumentFilesServiceProxy) {
    super(injector);
  }

  show(id: string, documentName: string, tenantName: string): void {
    this.id = id;
    this.documentName = documentName;
    this.tenantName = tenantName;
    this.active = true;
    this.modal.show();
  }

  rejectDocumentFile() {
    this.saving = true;
    this._documentFilesServiceProxy.reject(this.id, this.rejectionReason).subscribe(() => {
      this.notify.success(this.l('SuccessfullyRejected'));
      this.close();
    });
  }
  close(): void {
    this.active = false;
    this.modalSave.emit(null);
    this.rejectionReason = '';
    this.documentName = '';
    this.tenantName = '';
    this.modal.hide();
  }
}
