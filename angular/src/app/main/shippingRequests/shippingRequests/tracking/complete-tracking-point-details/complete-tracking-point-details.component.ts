import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { TrackingPODModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-pod-model.component';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';
import { ConfirmReceiverCodeInput, TrackingServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'complete-tracking-point-details',
  templateUrl: './complete-tracking-point-details.component.html',
  styleUrls: ['./complete-tracking-point-details.component.css'],
})
export class CompleteTrackingPointDetailsComponent extends AppComponentBase {
  @ViewChild('completeTrackingPointDetails', { static: true }) modal: ModalDirective;
  @ViewChild('uploadPodModal') uploadPodModal: TrackingPODModalComponent;
  // @ViewChild('pointConfirmationComp') pointConfirmationComp: TrackingConfirmModalComponent;
  input: ConfirmReceiverCodeInput = new ConfirmReceiverCodeInput();

  trip: any;
  currentPoint: any;
  active = 1;
  checking = false;
  isCodeCorrect: boolean;
  constructor(injector: Injector, private _Service: TrackingServiceProxy) {
    super(injector);
  }

  show() {
    //  this.active = true;
    this.modal.show();
  }

  close() {
    //this.active = false;
    this.modal.hide();
  }

  checkIfCodeCorrect(): void {
    this.checking = true;
    this._Service
      .confirmReceiverCode(this.input)
      .pipe(finalize(() => (this.checking = false)))
      .subscribe(
        () => {
          //this.notify.info(this.l('SuccessfullyConfirmed'));
          //this.modalConfirm.emit(null);
          //this.close();
          //console.log(res);
        },
        (error) => {
          console.log(error);
        }
      );
  }
}
