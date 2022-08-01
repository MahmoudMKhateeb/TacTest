import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import {
  CreateOrEditShippingRequestTripAccidentResolveDto,
  SelectItemDto,
  ShippingRequestTripAccidentListDto,
  ShippingRequestTripAccidentServiceProxy,
  TripAccidentResolveType,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';

@Component({
  selector: 'trip-accident-resolve-modal',
  templateUrl: 'trip-accident-resolve-modal.component.html',
})
export class TripAccidentResolveModalComponent extends AppComponentBase {
  description: any;
  saving: boolean;
  active: boolean;
  drivers: SelectItemDto[];
  trucks: SelectItemDto[];
  resolve = new CreateOrEditShippingRequestTripAccidentResolveDto();
  resolveType: TripAccidentResolveType;
  trucksLoading = false;
  driversLoading = false;
  @ViewChild('resolveModal') private modal: ModalDirective;
  @Output() private modalSave: EventEmitter<any> = new EventEmitter<any>();
  accident: ShippingRequestTripAccidentListDto;
  viewModeEnabled = false;

  constructor(injector: Injector, private _accidentServiceProxy: ShippingRequestTripAccidentServiceProxy) {
    super(injector);
  }

  save() {
    this.saving = true;
    this.resolve.accidentId = this.accident.id;
    this.resolve.id = this.accident.resolveListDto.id;
    this._accidentServiceProxy
      .createOrEditResolve(this.resolve)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.modalSave.emit();
        this.closeModal();
      });
  }

  showForView(accident: ShippingRequestTripAccidentListDto) {
    this.accident = accident;
    this.active = true;
    this.viewModeEnabled = true;
    this.resolveType = accident.resolveListDto.resolveType;
    this._accidentServiceProxy.getAccidentResolveForEdit(accident.resolveListDto.id).subscribe((result) => {
      this.resolve = result;
    });
    this.modal.show();
    switch (this.resolveType) {
      case TripAccidentResolveType.ChangeDriver:
        this.getAllDriversForDropdown();
        break;
      case TripAccidentResolveType.ChangeTruck:
        this.getAllTrucksForDropdown();
        break;

      case TripAccidentResolveType.ChangeDriverAndTruck:
        this.getAllDriversForDropdown();
        this.getAllTrucksForDropdown();
        break;
    }
  }

  closeModal() {
    this.modal.hide();
    this.active = false;
    this.resolve = undefined;
    this.accident = undefined;
    this.resolveType = undefined;
    this.viewModeEnabled = false;
  }

  show(resolveType: TripAccidentResolveType, accident: ShippingRequestTripAccidentListDto) {
    this.accident = accident;
    this.active = true;
    this.modal.show();
    this.resolveType = resolveType;

    switch (resolveType) {
      case TripAccidentResolveType.ChangeDriver:
        this.getAllDriversForDropdown();
        break;
      case TripAccidentResolveType.ChangeTruck:
        this.getAllTrucksForDropdown();
        break;

      case TripAccidentResolveType.ChangeDriverAndTruck:
        this.getAllDriversForDropdown();
        this.getAllTrucksForDropdown();
        break;
    }

    if (accident.resolveListDto.id && accident.resolveListDto.resolveType === resolveType) {
      this._accidentServiceProxy.getAccidentResolveForEdit(accident.resolveListDto.id).subscribe((result) => {
        this.resolve = result;
      });
    } else {
      this.resolve = new CreateOrEditShippingRequestTripAccidentResolveDto();
    }
    this.resolve.resolveType = resolveType;
  }

  approveResolve(accident: ShippingRequestTripAccidentListDto) {
    if (accident.resolveListDto.id) {
      this.saving = true;
      this._accidentServiceProxy
        .applyResolveChanges(accident.resolveListDto.id)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.modalSave.emit();
          this.closeModal();
          this.notify.info(this.l('SavedSuccessfully'));
        });
    }
  }

  isDriverRequired(): boolean {
    return this.resolveType === TripAccidentResolveType.ChangeDriver || this.resolveType === TripAccidentResolveType.ChangeDriverAndTruck;
  }

  isTruckRequired(): boolean {
    return this.resolveType === TripAccidentResolveType.ChangeTruck || this.resolveType === TripAccidentResolveType.ChangeDriverAndTruck;
  }

  getAllDriversForDropdown() {
    this.driversLoading = true;
    this._accidentServiceProxy
      .getAllDriversByAccidentId(this.accident.id)
      .pipe(
        finalize(() => {
          this.driversLoading = false;
        })
      )
      .subscribe((result) => {
        this.drivers = result;
      });
  }

  getAllTrucksForDropdown() {
    this.trucksLoading = true;
    this._accidentServiceProxy
      .getAllTrucksByAccidentId(this.accident.id)
      .pipe(
        finalize(() => {
          this.trucksLoading = false;
        })
      )
      .subscribe((result) => {
        this.trucks = result;
      });
  }
}
