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
  AllTenants: ShippersForDropDownDto[];

  receiver: CreateOrEditReceiverDto = new CreateOrEditReceiverDto();
  // facilityName = '';
  allFacilitys: ReceiverFacilityLookupTableDto[];
  isShippersActorsLoading: boolean;
  isFacilitiesLoading: boolean;
  shipperActorId: number;
  shipperActors: SelectItemDto[];
  canManageShipperClients: boolean;
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
    this.facilityIdFromTrips = isNotNullOrUndefined(facilityIdFromTrip) ? Number(facilityIdFromTrip) : null;
    this.canManageShipperClients = this.feature.isEnabled('App.ShipperClients');
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
        this.shipperActorId = result.shipperActorId;
        this.loadFacilitiesByActor(this.shipperActorId);
      });
    }
    if (this.canManageShipperClients && !facilityIdFromTrip) {
      this.loadShippersActors();
    }

    this.loadAllFacilities();
    if (this.isTachyonDealerOrHost) {
      this.loadAllCompaniesForDropDown();
    }
    this.active = true;
    this.modal.show();
  }

  private loadAllFacilities() {
    // Important Note: There's some cases we must not load all facilities
    // 1- if the user is a broker and he is creating a receiver from the contact management page. hint: not from create trip
    // 2- if the user is a broker and he is updating an internal receiver (actor has value) from the contact management page

    if (!this.facilityIdFromTrips && this.canManageShipperClients) {
      if (!this.receiver.id || isNotNullOrUndefined(this.shipperActorId)) {
        return;
      }
    }

    this.isFacilitiesLoading = true;
    this._receiversServiceProxy
      .getAllFacilityForTableDropdown(null)
      .pipe(finalize(() => (this.isFacilitiesLoading = false)))
      .subscribe((result) => {
        this.allFacilitys = result;
        if (isNotNullOrUndefined(this.facilityIdFromTrips)) {
          this.receiver.facilityId = this.facilityIdFromTrips;
        }
      });
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
    this.shipperActorId = undefined;
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
    } else {
      this.loadAllFacilities();
    }
  }

  isActorRequired(): boolean {
    // broker in this cases need to fill actor
    return !this.receiver.id || isNotNullOrUndefined(this.shipperActorId);
  }
}
