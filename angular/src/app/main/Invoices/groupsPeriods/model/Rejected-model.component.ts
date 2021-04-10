import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GroupPeriodServiceProxy, SubmitInvoiceRejectedInput } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'submitinvoice-rejected-modal',
  templateUrl: './Rejected-model.component.html',
})
export class SubmitInvoiceRejectedModelComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  Reason: SubmitInvoiceRejectedInput;
  active: boolean = false;
  saving: boolean = false;
  constructor(injector: Injector, private _Service: GroupPeriodServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}

  public show(id: number): void {
    this.Reason = new SubmitInvoiceRejectedInput();
    this.Reason.id = id;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;

    this._Service
      .rejected(this.Reason)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
