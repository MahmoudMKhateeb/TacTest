import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PenaltiesServiceProxy, PenaltyComplaintDto, RejectComplaintDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'viewComplaint',
  templateUrl: './view-complaint-modal.component.html',
  styleUrls: ['./view-complaint-modal.component.css'],
})
export class ViewComplaintModalComponent extends AppComponentBase {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving: boolean;
  needEnterRejectReason: boolean;
  penaltyComplaint: PenaltyComplaintDto;
  rejectComplaint: RejectComplaintDto;

  constructor(inject: Injector, private _PenaltiesServiceProxy: PenaltiesServiceProxy) {
    super(inject);
  }

  reject() {
    this.saving = true;
    this._PenaltiesServiceProxy
      .rejectComplaint(this.rejectComplaint)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.notify.success('RejectedSuccessfully');
          this.modalSave.emit();
        })
      )
      .subscribe(() => {
        this.close();
      });
  }

  accept(id: number) {
    this.saving = true;
    this._PenaltiesServiceProxy
      .acceptComplaint(id)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.notify.success('AcceptedSuccessfully');
          this.modalSave.emit();
        })
      )
      .subscribe(() => {
        this.close();
      });
  }

  show(id: number) {
    this.active = true;
    this.rejectComplaint = new RejectComplaintDto();
    this.rejectComplaint.id = id;
    this._PenaltiesServiceProxy.getPenaltyComplaintForView(id).subscribe((result) => {
      this.penaltyComplaint = result;
    });
    this.modal.show();
  }

  close() {
    this.active = false;
    this.modal.hide();
  }
}
