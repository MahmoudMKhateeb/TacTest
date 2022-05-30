import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PenaltiesServiceProxy, RegisterPenaltyComplaintDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'registerComplaint',
  templateUrl: './register-complaint-modal.component.html',
  styleUrls: ['./register-complaint-modal.component.css'],
})
export class RegisterComplaintModalComponent extends AppComponentBase {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving: boolean;
  form: RegisterPenaltyComplaintDto;

  constructor(inject: Injector, private _PenaltiesServiceProxy: PenaltiesServiceProxy) {
    super(inject);
  }

  save() {
    this.saving = true;
    this._PenaltiesServiceProxy
      .registerComplaint(this.form)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.notify.success('SavedSuccessfully');
          this.modalSave.emit();
        })
      )
      .subscribe(() => {
        this.close();
      });
  }
  show(id: number) {
    this.active = true;
    this.form = new RegisterPenaltyComplaintDto();
    this.form.penaltyId = id;
    this.modal.show();
  }
  close() {
    this.active = false;
    this.modal.hide();
  }
}
