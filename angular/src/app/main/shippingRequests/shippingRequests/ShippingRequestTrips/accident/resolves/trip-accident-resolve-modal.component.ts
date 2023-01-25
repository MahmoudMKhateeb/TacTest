import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import {
  CreateOrEditShippingRequestTripAccidentResolveDto,
  SelectItemDto,
  ShippingRequestTripAccidentListDto,
  ShippingRequestTripAccidentServiceProxy,
  TripAccidentResolveType,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'trip-accident-resolve-modal',
  templateUrl: 'trip-accident-resolve-modal.component.html',
})
export class TripAccidentResolveModalComponent extends AppComponentBase implements OnInit {
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
  changeDriverEnabled: boolean;
  changeTruckEnabled: boolean;
  cancelTripEnabled: boolean;
  resolveWithoutActionEnabled: boolean;
  noActionNeededEnabled: boolean;

  constructor(injector: Injector, private _accidentServiceProxy: ShippingRequestTripAccidentServiceProxy) {
    super(injector);
  }

  save() {
    this.saving = true;
    this.resolve.accidentId = this.accident.id;
    this.resolve.id = this.accident?.resolveListDto?.id;
    if (this.changeDriverEnabled && this.changeTruckEnabled) {
      this.resolve.resolveType = TripAccidentResolveType.ChangeDriverAndTruck;
    } else if (this.changeTruckEnabled) {
      this.resolve.resolveType = TripAccidentResolveType.ChangeTruck;
    } else if (this.changeDriverEnabled) {
      this.resolve.resolveType = TripAccidentResolveType.ChangeDriver;
    } else if (this.cancelTripEnabled) {
      this.resolve.resolveType = TripAccidentResolveType.CancelTrip;
    } else if (this.resolveWithoutActionEnabled) {
      this.resolve.resolveType = TripAccidentResolveType.ResolveWithoutAction;
    } else {
      this.resolve.resolveType = TripAccidentResolveType.NoActionNeeded;
    }

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
    this.getResolveForEdit(accident.resolveListDto.id);
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
    this.resolve = new CreateOrEditShippingRequestTripAccidentResolveDto();
    this.accident = undefined;
    this.resolveType = undefined;
    this.viewModeEnabled = false;
  }

  show(accident: ShippingRequestTripAccidentListDto) {
    this.accident = accident;
    this.active = true;
    this.modal.show();

    if (accident.resolveListDto.id) {
      this.getResolveForEdit(accident.resolveListDto.id);
    } else {
      this.resolve = new CreateOrEditShippingRequestTripAccidentResolveDto();
    }
  }

  private getResolveForEdit(resolveId: number) {
    this._accidentServiceProxy.getAccidentResolveForEdit(resolveId).subscribe((result) => {
      this.resolve = result;
      switch (this.resolve.resolveType) {
        case TripAccidentResolveType.ChangeDriver:
          this.changeDriverEnabled = true;
          break;
        case TripAccidentResolveType.ChangeTruck:
          this.changeTruckEnabled = true;
          break;
        case TripAccidentResolveType.ChangeDriverAndTruck:
          this.changeDriverEnabled = true;
          this.changeTruckEnabled = true;
          break;
        case TripAccidentResolveType.NoActionNeeded:
          this.noActionNeededEnabled = true;
          break;
        case TripAccidentResolveType.CancelTrip:
          this.cancelTripEnabled = true;
          break;
        case TripAccidentResolveType.ResolveWithoutAction:
          this.resolveWithoutActionEnabled = true;
          break;
      }
    });
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

  ngOnInit(): void {
    this.changeTruckEnabled = false;
    this.changeDriverEnabled = false;
    this.cancelTripEnabled = false;
    this.resolveWithoutActionEnabled = false;
    this.noActionNeededEnabled = false;
    this.driversLoading = false;
    this.trucksLoading = false;
    this.resolve = new CreateOrEditShippingRequestTripAccidentResolveDto();
  }

  validateChangeDriverOption() {
    if (this.changeDriverEnabled) {
      this.cancelTripEnabled = false;
      this.resolveWithoutActionEnabled = false;
      this.noActionNeededEnabled = false;

      if (!isNotNullOrUndefined(this.drivers)) {
        this.getAllDriversForDropdown();
      }

      if (!this.changeTruckEnabled) {
        this.resolve.truckId = undefined;
      }
      this.resolve.description = undefined;
    } else {
      this.resolve.driverId = undefined;
    }
  }

  validateChangeTruckOption() {
    if (this.changeTruckEnabled) {
      this.cancelTripEnabled = false;
      this.resolveWithoutActionEnabled = false;
      this.noActionNeededEnabled = false;

      if (!isNotNullOrUndefined(this.trucks)) {
        this.getAllTrucksForDropdown();
      }

      if (!this.changeDriverEnabled) {
        this.resolve.driverId = undefined;
      }
      this.resolve.description = undefined;
    } else {
      this.resolve.truckId = undefined;
    }
  }

  validateCancelTripOption() {
    if (this.cancelTripEnabled) {
      this.changeDriverEnabled = false;
      this.changeTruckEnabled = false;
      this.resolveWithoutActionEnabled = false;
      this.resolve.truckId = undefined;
      this.resolve.driverId = undefined;
      this.resolve.description = undefined;
      this.noActionNeededEnabled = false;
    }
  }

  validateResolveWithoutActionOption() {
    if (this.resolveWithoutActionEnabled) {
      this.changeDriverEnabled = false;
      this.changeTruckEnabled = false;
      this.cancelTripEnabled = false;
      this.noActionNeededEnabled = false;
      this.resolve.truckId = undefined;
      this.resolve.driverId = undefined;
    } else {
      this.resolve.description = undefined;
    }
  }

  validateNoActionNeededOption() {
    if (this.noActionNeededEnabled) {
      this.changeDriverEnabled = false;
      this.changeTruckEnabled = false;
      this.resolveWithoutActionEnabled = false;
      this.cancelTripEnabled = false;
      this.resolve.truckId = undefined;
      this.resolve.driverId = undefined;
      this.resolve.description = undefined;
    }
  }
}
