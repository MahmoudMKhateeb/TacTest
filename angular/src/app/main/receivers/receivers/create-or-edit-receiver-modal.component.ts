import { Component, ViewChild, Injector, Output, EventEmitter, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ReceiversServiceProxy, CreateOrEditReceiverDto, ReceiverFacilityLookupTableDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'createOrEditReceiverModal',
  styleUrls: ['./create-or-edit-receiver-modal.component.css'],
  templateUrl: './create-or-edit-receiver-modal.component.html',
})
export class CreateOrEditReceiverModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Input() facilityIdFromTrips: number;
  @Input() agentType: 'sender' | 'receiver';
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;
  CheckingIfReciverPhoneNumberIsValid = false;
  isPhoneNumberAvilable = true;

  receiver: CreateOrEditReceiverDto = new CreateOrEditReceiverDto();
  // facilityName = '';
  allFacilitys: ReceiverFacilityLookupTableDto[];
  constructor(injector: Injector, private _receiversServiceProxy: ReceiversServiceProxy) {
    super(injector);
  }

  show(receiverId?: number, facilityIdFromTrip?: number): void {
    console.log('facilityIdFromTrip', facilityIdFromTrip);
    this.facilityIdFromTrips = isNotNullOrUndefined(facilityIdFromTrip) ? Number(facilityIdFromTrip) : null;
    if (!receiverId) {
      //  {
      //           id: null,
      //           fullName: '',
      //           email: '',
      //           phoneNumber: '',
      //           facilityId: null
      //       }
      this.receiver = new CreateOrEditReceiverDto();
      this.receiver.id = receiverId;
    } else {
      this._receiversServiceProxy.getReceiverForEdit(receiverId).subscribe((result) => {
        this.receiver = result.receiver;
      });
    }
    this._receiversServiceProxy.getAllFacilityForTableDropdown().subscribe((result) => {
      this.allFacilitys = result;
      this.receiver.facilityId = this.facilityIdFromTrips;
    });
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;

    this._receiversServiceProxy
      .createOrEdit(this.receiver)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.facilityIdFromTrips);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  CheckIfReciverPhoneNumberIsValid(phoneNumber, id: number) {
    console.log('phoneNumber', phoneNumber);
    if (isNotNullOrUndefined(phoneNumber) && phoneNumber.trim().length === 9) {
      this.CheckingIfReciverPhoneNumberIsValid = true;
      this._receiversServiceProxy.checkIfPhoneNumberValid(phoneNumber, id == null ? 0 : id).subscribe((res) => {
        this.isPhoneNumberAvilable = res;
        this.CheckingIfReciverPhoneNumberIsValid = false;
      });
    }
  }
}
