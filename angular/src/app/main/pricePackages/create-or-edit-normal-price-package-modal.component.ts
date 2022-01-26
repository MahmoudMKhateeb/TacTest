import { Component, ViewChild, Injector, Output, EventEmitter, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { NormalPricePackagesServiceProxy, CreateOrEditNormalPricePackageDto, SelectItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditNormalPricePackageModal',
  templateUrl: './create-or-edit-normal-price-package-modal.component.html',
})
export class CreateOrEditNormalPricePackageModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  isNameAvilable = true;
  isTranspotTypesSelected = true;
  normalPricePackageId: number;
  allCitys: SelectItemDto[];
  allTranspotTypes: SelectItemDto[];
  allTruckTypes: SelectItemDto[];
  normalPricePackage: CreateOrEditNormalPricePackageDto = new CreateOrEditNormalPricePackageDto();

  constructor(injector: Injector, private _normalPricePackagesServiceProxy: NormalPricePackagesServiceProxy) {
    super(injector);
  }

  show(normalPricePackageId?: number): void {
    if (!normalPricePackageId) {
      this.normalPricePackage = new CreateOrEditNormalPricePackageDto();
      this.normalPricePackage.id = normalPricePackageId;
    } else {
      this._normalPricePackagesServiceProxy.getNormalPricePackageForEdit(normalPricePackageId).subscribe((result) => {
        this.normalPricePackage = result;
        this.fillAllTruckTypes(result.transportTypeId);
      });
    }
    this.fillAllCities();
    this.fillTranspotTypes();
    this.active = true;
    this.modal.show();
  }

  TranspotTypesChanged(event) {
    if (event.target.value == -2) {
      this.isTranspotTypesSelected = false;
    } else {
      this.isTranspotTypesSelected = true;
    }

    this._normalPricePackagesServiceProxy.getAllTruckTypesForTableDropdown(event.target.value).subscribe((result) => {
      this.allTruckTypes = result;
    });
  }
  checkPricePerExtraDrop(event) {
    debugger;
    if (!event.target.checked) {
      this.normalPricePackage.pricePerExtraDrop = undefined;
    }
  }
  save(): void {
    this.saving = true;

    this._normalPricePackagesServiceProxy
      .createOrEdit(this.normalPricePackage)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.clearListsItems();
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.clearListsItems();
    this.active = false;
    this.modal.hide();
  }
  numberOnly(event): boolean {
    if (event.target.value.length >= 9) {
      return false;
    }
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
  fillAllTruckTypes(transportTypeId: number) {
    this._normalPricePackagesServiceProxy.getAllTruckTypesForTableDropdown(transportTypeId).subscribe((result) => {
      this.allTruckTypes = result;
    });
  }
  fillAllCities() {
    this._normalPricePackagesServiceProxy.getAllCitiesForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
  }
  fillTranspotTypes() {
    this._normalPricePackagesServiceProxy.getAllTranspotTypesForTableDropdown().subscribe((result) => {
      this.allTranspotTypes = result;
    });
  }
  clearListsItems() {
    this.allCitys = [];
    this.allTruckTypes = [];
    this.allTranspotTypes = [];
  }
}
