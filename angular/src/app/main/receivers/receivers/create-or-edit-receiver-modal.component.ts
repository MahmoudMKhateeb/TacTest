import { Component, ViewChild, Injector, Output, EventEmitter, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  ReceiversServiceProxy,
  CreateOrEditReceiverDto,
  ReceiverFacilityLookupTableDto,
  ShippersForDropDownDto,
  PenaltiesServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'createOrEditReceiverModal',
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
  AllTenants: ShippersForDropDownDto[];

  receiver: CreateOrEditReceiverDto = new CreateOrEditReceiverDto();
  // facilityName = '';
  allFacilitys: ReceiverFacilityLookupTableDto[];
  isShippersActorsLoading: boolean;
  isFacilitiesLoading: boolean;
  shipperActorId: number;
  shipperActors: SelectItemDto[];
  constructor(
    injector: Injector,
    private _receiversServiceProxy: ReceiversServiceProxy,
    private _penaltiesServiceProxy: PenaltiesServiceProxy,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy
  ) {
    super(injector);
    this.isShippersActorsLoading = false;
    this.isFacilitiesLoading = false;
  }

  show(receiverId?: number, facilityIdFromTrip?: number): void {
    this.facilityIdFromTrips = facilityIdFromTrip;
    if (!receiverId) {
      this.receiver = new CreateOrEditReceiverDto();
      this.receiver.id = receiverId;
    } else {
      this._receiversServiceProxy.getReceiverForEdit(receiverId).subscribe((result) => {
        this.receiver = result.receiver;
        this.shipperActorId = result.shipperActorId;
        this.loadFacilitiesByActor(this.shipperActorId);
      });
    }
    if (this.feature.isEnabled('App.ShipperClients')) {
      this.loadShippersActors();
    } else {
      this.isFacilitiesLoading = true;
      this._receiversServiceProxy
        .getAllFacilityForTableDropdown()
        .pipe(finalize(() => (this.isFacilitiesLoading = false)))
        .subscribe((result) => {
          this.allFacilitys = result;
          this.receiver.facilityId = this.facilityIdFromTrips;
        });
    }

    this.loadAllCompaniesForDropDown();
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
    this.receiver = new CreateOrEditReceiverDto();
    this.allFacilitys = undefined;
    this.shipperActors = undefined;
    this.active = false;
    this.modal.hide();
  }

  CheckIfReciverPhoneNumberIsValid(phoneNumber: string, id: number) {
    if (phoneNumber.trim().length === 9) {
      this.CheckingIfReciverPhoneNumberIsValid = true;
      this._receiversServiceProxy.checkIfPhoneNumberValid(phoneNumber, id == null ? 0 : id).subscribe((res) => {
        this.isPhoneNumberAvilable = res;
        this.CheckingIfReciverPhoneNumberIsValid = false;
      });
    }
  }

  loadAllCompaniesForDropDown() {
    this._penaltiesServiceProxy.getAllCompanyForDropDown().subscribe((result) => {
      this.AllTenants = result;
    });
  }

  loadShippersActors() {
    this.isShippersActorsLoading = true;
    this._shippingRequestServiceProxy
      .getAllShippersActorsForDropDown()
      .pipe(
        finalize(() => {
          this.isShippersActorsLoading = false;
        })
      )
      .subscribe((result) => {
        this.shipperActors = result;
      });
  }

  loadFacilitiesByActor(actorId) {
    if (isNotNullOrUndefined(actorId)) {
      this.isFacilitiesLoading = true;
      this._receiversServiceProxy
        .getAllFacilitiesByActorId(actorId)
        .pipe(finalize(() => (this.isFacilitiesLoading = false)))
        .subscribe((result) => {
          this.allFacilitys = result;
        });
    }
  }
}
