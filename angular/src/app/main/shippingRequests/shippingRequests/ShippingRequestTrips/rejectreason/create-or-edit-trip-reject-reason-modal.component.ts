import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShippingRequestTripRejectReasonServiceProxy, CreateOrEditShippingRequestTripRejectReasonDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'trip-reject-reason-modal',
  templateUrl: './create-or-edit-trip-reject-reason-modal.component.html',
})
export class TripRejectReasonModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('nameInput', { static: false }) nameInput: ElementRef;

  reason: CreateOrEditShippingRequestTripRejectReasonDto;
  active: boolean = false;
  saving: boolean = false;

  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: ShippingRequestTripRejectReasonServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}

  public show(reason: CreateOrEditShippingRequestTripRejectReasonDto | null): void {
    if (reason == null) {
      this.reason = new CreateOrEditShippingRequestTripRejectReasonDto();
    } else {
      this.reason = reason;
    }
    this.active = true;
    this.modal.show();
  }

  onShown(): void {
    this.nameInput.nativeElement.focus();
  }

  save(): void {
    this.saving = true;

    this._Service
      .createOrEdit(this.reason)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.reason);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
