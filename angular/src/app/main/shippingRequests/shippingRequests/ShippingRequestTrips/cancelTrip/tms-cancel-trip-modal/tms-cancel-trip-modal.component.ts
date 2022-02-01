import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CancelTripInput,
  ShippingRequestsTripListDto,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripStatus,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { TripService } from '../../trip.service';

@Component({
  selector: 'app-tms-cancel-trip-modal',
  templateUrl: './tms-cancel-trip-modal.component.html',
  styleUrls: ['./tms-cancel-trip-modal.component.css'],
})
export class TmsCancelTripModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  reason: CancelTripInput;
  active: boolean = false;
  saving: boolean = false;
  applyCancel: boolean = false;
  rejectForm: boolean = false;
  ShippingRequestTripStatusEnum = ShippingRequestTripStatus;

  constructor(injector: Injector, private _tripService: TripService, private _shippingRequestTripsService: ShippingRequestsTripServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}

  ApplyCancel(trip: ShippingRequestsTripListDto) {
    this.reason = new CancelTripInput();
    this.reason.id = trip.id;
    this.applyCancel = true;
    this.active = true;
    this.modal.show();
  }

  cancelTripByTMS() {
    this.saving = true;
    if (this.isTachyonDealer) {
      this.reason.isApproved = true;
      this.reason.rejectedCancelingReason = null;
      this.cancelation(this.reason, 'cancel');
    }
  }

  toggleRejectCancelModal() {
    this.rejectForm = !this.rejectForm;
  }

  rejectCancel() {
    if (this.isTachyonDealer) {
      this.reason.isApproved = false;
      this.cancelation(this.reason, 'reject');
    }
  }

  cancelation(reason: CancelTripInput, type) {
    this._shippingRequestTripsService.cancelTrip(reason).subscribe(() => {
      this.close();
      this.modalSave.emit(null);
      this.saving = false;
      type == 'reject' ? this.notify.info(this.l('SuccessfullyRejectCancelation')) : this.notify.info(this.l('SuccessfullyCanceled'));
    });
  }

  close(): void {
    this.modal.hide();
    this.rejectForm = false;
    this.active = false;
  }
}
