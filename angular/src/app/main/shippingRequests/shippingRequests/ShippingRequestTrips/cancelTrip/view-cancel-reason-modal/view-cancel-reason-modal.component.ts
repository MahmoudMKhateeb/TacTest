import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CancelTripInput,
  ShippingRequestTripCancelStatus,
  ShippingRequestTripStatus,
  ShippingRequestType,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-view-cancel-reason-modal',
  templateUrl: './view-cancel-reason-modal.component.html',
  styleUrls: ['./view-cancel-reason-modal.component.css'],
})
export class ViewCancelReasonModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  reason: CancelTripInput = new CancelTripInput();
  ShippingRequestTripStatusEnum = ShippingRequestTripStatus;
  constructor(injector: Injector) {
    super(injector);
  }
  ngOnInit(): void {}

  show(status: undefined, canceledReason: undefined, rejectedCancelingReason: undefined, cancelStatus: ShippingRequestTripCancelStatus): void {
    if (status == this.ShippingRequestTripStatusEnum.Canceled || cancelStatus == ShippingRequestTripCancelStatus.WaitingForTMSApproval) {
      this.reason.canceledReason = canceledReason;
    } else if (cancelStatus == ShippingRequestTripCancelStatus.Rejected) {
      this.reason.canceledReason = rejectedCancelingReason;
    }

    this.modal.show();
  }

  close(): void {
    this.modal.hide();
  }
}
