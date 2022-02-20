import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ShippingRequestsTripServiceProxy,
  CancelTripInput,
  ShippingRequestsTripListDto,
  ShippingRequestTripStatus,
  ShippingRequestType,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '../../trip.service';

@Component({
  selector: 'app-cancel-trip-modal',
  templateUrl: './cancel-trip-modal.component.html',
  styleUrls: ['./cancel-trip-modal.component.css'],
})
export class CancelTripModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  reason: CancelTripInput;
  active: boolean = false;
  saving: boolean = false;
  isTMS: boolean = false;
  view: boolean = false;
  applyCancel: boolean = false;
  rejectForm: boolean = false;
  requestType: number;
  ShippingRequestTripStatusEnum = ShippingRequestTripStatus;
  constructor(injector: Injector, private _tripService: TripService, private _shippingRequestTripsService: ShippingRequestsTripServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}

  public show(tripId: undefined, status: undefined, canceledReason: undefined, requestType: undefined): void {
    this.reason = new CancelTripInput();
    this.reason.id = tripId;
    this.requestType = requestType;
    if (status == this.ShippingRequestTripStatusEnum.Canceled) {
      // Canceled Trip => show reason
      this.view = true;
      this.reason.canceledReason = canceledReason;
    }
    this.active = true;
    this.modal.show();
  }

  cancelTrip() {
    this.saving = true;
    if (this.requestType == ShippingRequestType.TachyonManageService) this.isTMS = true;

    if (this.isTachyonDealer) {
      this.reason.isApproved = true;
      this.reason.rejectedCancelingReason = null;
      this.cancelation(this.reason, 'tms');
    } else this.cancelation(this.reason, '');
  }

  cancelation(reason: CancelTripInput, type) {
    this._shippingRequestTripsService.cancelTrip(reason).subscribe(() => {
      this.close();
      this.modalSave.emit(null);
      this.saving = false;
      type == 'tms'
        ? this.notify.info(!this.isTMS ? this.l('SuccessfullyCanceled') : this.l('WaitingApproveFromTMS'))
        : this.notify.info(this.l('SuccessfullyCanceled'));
    });
  }

  close(): void {
    this.modal.hide();
    this.rejectForm = false;
    this.active = false;
  }
}
