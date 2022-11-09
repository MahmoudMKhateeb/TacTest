import { Component, ViewChild, Injector, Output, EventEmitter, Input, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { SubmitInvoiceClaimCreateInput, ActorSubmitInvoiceServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'client-demand-model',
  templateUrl: './client-demand-model.component.html',
})
export class ClientDemandModelComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  active = false;
  saving = false;
  File: SubmitInvoiceClaimCreateInput;
  constructor(injector: Injector, private _Service: ActorSubmitInvoiceServiceProxy) {
    super(injector);
  }

  show(groupid: number): void {
    this.File = new SubmitInvoiceClaimCreateInput();
    this.File.id = groupid;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
    this._Service
      .claim(this.File)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  async fileChangeEvent(event: any): Promise<void> {
    let file = event.target.files[0];
    this.File.documentBase64 = String(await this.toBase64(file));
    this.File.documentContentType = file.type;
    this.File.documentName = file.name;
  }
}
