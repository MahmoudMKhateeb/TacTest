import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  TenantCityLookupTableDto,
  TenantCountryLookupTableDto,
  TenantEditDto,
  TenantRegistrationServiceProxy,
  TenantServiceProxy,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'editTenantModal',
  templateUrl: './edit-tenant-modal.component.html',
})
export class EditTenantModalComponent extends AppComponentBase {
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('editModal', { static: true }) modal: ModalDirective;
  @ViewChild('SubscriptionEndDateUtc') subscriptionEndDateUtc: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  today = new Date();
  active = false;
  saving = false;
  isUnlimited = false;
  subscriptionEndDateUtcIsValid = false;

  tenant: TenantEditDto = undefined;
  currentConnectionString: string;
  // editions: SubscribableEditionComboboxItemDto[] = [];
  isSubscriptionFieldsVisible = false;

  isCountySelected = false;
  allCities: TenantCityLookupTableDto[];
  allCountries: TenantCountryLookupTableDto[];
  isMoiNumberAvailable = true;

  constructor(
    injector: Injector,
    private _tenantService: TenantServiceProxy,
    // private _commonLookupService: CommonLookupServiceProxy,
    private _tenantRegistrationService: TenantRegistrationServiceProxy
  ) {
    super(injector);
  }

  show(tenantId: number): void {
    this.active = true;

    this._tenantService.getTenantForEdit(tenantId).subscribe((tenantResult) => {
      this.tenant = tenantResult;
      this.currentConnectionString = tenantResult.connectionString;
      // this.tenant.editionId = this.tenant.editionId || 0;
      this.isUnlimited = !this.tenant.subscriptionEndDateUtc;
      this.subscriptionEndDateUtcIsValid = this.isUnlimited || this.tenant.subscriptionEndDateUtc !== undefined;
      this.modal.show();
      // this.toggleSubscriptionFields();
    });
    this.GetAllCountries();
  }

  onShown(): void {
    document.getElementById('Name').focus();

    if (this.tenant.subscriptionEndDateUtc) {
      (this.subscriptionEndDateUtc.nativeElement as any).value = moment(this.tenant.subscriptionEndDateUtc).format('L');
    }
  }

  subscriptionEndDateChange(e): void {
    this.subscriptionEndDateUtcIsValid = (e && e.date !== false) || this.isUnlimited;
  }

  // selectedEditionIsFree(): boolean {
  //   if (!this.tenant.editionId) {
  //     return true;
  //   }
  //
  //   let selectedEditions = _.filter(this.editions, { value: this.tenant.editionId + '' });
  //   if (selectedEditions.length !== 1) {
  //     return true;
  //   }
  //
  //   let selectedEdition = selectedEditions[0];
  //   return selectedEdition.isFree;
  // }

  save(): void {
    this.saving = true;
    // if (this.tenant.editionId === 0) {
    //   this.tenant.editionId = null;
    // }

    if (this.isUnlimited) {
      this.tenant.isInTrialPeriod = false;
    }

    //take selected date as UTC
    if (this.isUnlimited) {
      this.tenant.subscriptionEndDateUtc = null;
    }

    this._tenantService
      .updateTenant(this.tenant)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    // this.editions = [];
    this.active = false;
    this.modal.hide();
  }

  // onEditionChange(): void {
  //   if (this.selectedEditionIsFree()) {
  //     this.tenant.isInTrialPeriod = false;
  //   }
  //
  //    this.toggleSubscriptionFields();
  // }

  onUnlimitedChange(): void {
    if (this.isUnlimited) {
      this.tenant.subscriptionEndDateUtc = null;
      this.subscriptionEndDateUtcIsValid = true;
      this.tenant.isInTrialPeriod = false;
    } else {
      if (!this.tenant.subscriptionEndDateUtc) {
        this.subscriptionEndDateUtcIsValid = false;
      }
    }
  }

  // toggleSubscriptionFields() {
  //   if (this.tenant.editionId > 0) {
  //     this.isSubscriptionFieldsVisible = true;
  //   } else {
  //     this.isSubscriptionFieldsVisible = false;
  //   }
  // }

  CountryChanged(event) {
    if (event.target.value == -2) {
      this.isCountySelected = false;
    } else {
      this.isCountySelected = true;
    }

    this._tenantService.getAllCitiesForTableDropdown(event.target.value).subscribe((result) => {
      this.allCities = result;
    });
  }

  checkIfIsCompanyUniqueMoiNumber() {
    this._tenantRegistrationService
      .isCompanyUniqueMoiNumber(this.tenant.moiNumber, undefined)
      .subscribe((result) => (this.isMoiNumberAvailable = result));
  }

  GetAllCountries() {
    this._tenantService.getAllCountryForTableDropdown().subscribe((result) => {
      this.allCountries = result;
    });
  }
}
