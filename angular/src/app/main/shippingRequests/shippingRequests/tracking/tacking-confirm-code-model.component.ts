import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TrackingServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'tacking-confirm-code-model',
  templateUrl: './tacking-confirm-code-model.component.html',
})
export class TrackingConfirmModalComponent extends AppComponentBase {
  @Output() modalConfirm: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  //
  active: boolean = false;
  saving: boolean = false;
  // reasonId: any = '';
  // input: ConfirmReceiverCodeInput = new ConfirmReceiverCodeInput();
  // Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: TrackingServiceProxy) {
    super(injector);
  }
  //
  public show(id: number): void {
    //this.input = ;
    // this.input.id = id;
    this.active = true;
    this.modal.show();
  }
  //
  save(): void {
    this.saving = true;

    // this._Service
    //   .confirmReceiverCode(this.input)
    //   .pipe(finalize(() => (this.saving = false)))
    //   .subscribe(() => {
    //     this.notify.info(this.l('SuccessfullyConfirmed'));
    //     this.modalConfirm.emit(null);
    //     this.close();
    //   });
  }
  //
  close(): void {
    this.modal.hide();
    this.modalConfirm.emit(null);
    this.active = false;
  }
}
