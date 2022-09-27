import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  NormalPricePackagesServiceProxy,
  CheckIfPricePackageNameAvailableDto,
  CreateOrEditNormalPricePackageDto,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

let that;
@Component({
  selector: 'createOrEditNormalPricePackageModal',
  templateUrl: './create-or-edit-normal-price-package-modal.component.html',
  styleUrls: ['./create-or-edit-normal-price-package-modal.component.scss'],
})
export class CreateOrEditNormalPricePackageModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  isNameAvilable = true;
  normalPricePackageId: number;
  allCitys: SelectItemDto[];
  allTranspotTypes: SelectItemDto[];
  allTruckTypes: SelectItemDto[];
  normalPricePackage: CreateOrEditNormalPricePackageDto = new CreateOrEditNormalPricePackageDto();

  constructor(injector: Injector, private _normalPricePackagesServiceProxy: NormalPricePackagesServiceProxy) {
    super(injector);
    that = this;
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
    if (!isNotNullOrUndefined(event.selectedItem)) {
      return;
    }

    this._normalPricePackagesServiceProxy.getAllTruckTypesForTableDropdown(Number(event.selectedItem.id)).subscribe((result) => {
      this.allTruckTypes = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
  checkPricePerExtraDrop(event) {
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
    this.isNameAvilable = true;
    this.modal.hide();
  }
  checkIfIsPricePackageUniqueName() {
    var modal = new CheckIfPricePackageNameAvailableDto();
    modal.id = this.normalPricePackage.id;
    modal.name = this.normalPricePackage.displayName;
    this._normalPricePackagesServiceProxy.checkIfPricePackageNameAvailable(modal).subscribe((result) => {
      this.isNameAvilable = result;
    });
  }
  fillAllTruckTypes(transportTypeId: number) {
    this._normalPricePackagesServiceProxy.getAllTruckTypesForTableDropdown(transportTypeId).subscribe((result) => {
      this.allTruckTypes = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
  fillAllCities() {
    this._normalPricePackagesServiceProxy.getAllCitiesForTableDropdown().subscribe((result) => {
      this.allCitys = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
  fillTranspotTypes() {
    this._normalPricePackagesServiceProxy.getAllTranspotTypesForTableDropdown().subscribe((result) => {
      this.allTranspotTypes = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
  clearListsItems() {
    this.allCitys = [];
    this.allTruckTypes = [];
    this.allTranspotTypes = [];
  }

  checkIfIsPricePackageUniqueNameAsync(params) {
    let modal = new CheckIfPricePackageNameAvailableDto();
    modal.id = that.normalPricePackage.id;
    modal.name = params.value;
    return new Promise((resolve) => resolve(that._normalPricePackagesServiceProxy.checkIfPricePackageNameAvailable(modal).toPromise()));
  }

  minPriceComparison(e) {
    return new Promise((resolve) => resolve(e.value >= 1));
  }
  minPriceComparisonZero(e) {
    return new Promise((resolve) => resolve(e.value >= 0));
  }
}
