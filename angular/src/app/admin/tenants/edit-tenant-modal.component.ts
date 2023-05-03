import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild, OnInit } from '@angular/core';
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
export class EditTenantModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('editModal', { static: true }) modal: ModalDirective;
  @ViewChild('SubscriptionEndDateUtc') subscriptionEndDateUtc: ElementRef;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  today = new Date();
  active = false;
  saving = false;
  isUnlimited = false;
  subscriptionEndDateUtcIsValid = false;
  cityLoading: boolean;
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
  ngOnInit() {
    this.GetAllCountries();
  }

  show(tenantId: number): void {
    this._tenantService.getTenantForEdit(tenantId).subscribe((tenantResult) => {
      this.tenant = tenantResult;
      this.currentConnectionString = tenantResult.connectionString;
      // this.tenant.editionId = this.tenant.editionId || 0;
      this.isUnlimited = !this.tenant.subscriptionEndDateUtc;
      this.subscriptionEndDateUtcIsValid = this.isUnlimited || this.tenant.subscriptionEndDateUtc !== undefined;
      this.cityLoading = true;
      this._tenantService
        .getAllCitiesForTableDropdown(tenantResult.countryId)
        .pipe(
          finalize(() => {
            this.cityLoading = false;
          })
        )
        .subscribe((result) => {
          this.allCities = result.map((item) => {
            (item.id as any) = Number(item.id);
            return item;
          });
          this.allCities.push(TenantCityLookupTableDto.fromJS({ id: -1, displayName: this.l('Other') }));
          this.cityLoading = false;
        });
      this.active = true;
      this.modal.show();
      // this.toggleSubscriptionFields();
    });
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
    this.allCities = undefined;
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
    if (event.selectedItem.id == -2) {
      this.isCountySelected = false;
    } else {
      this.isCountySelected = true;
    }

    this._tenantService.getAllCitiesForTableDropdown(event.selectedItem.id).subscribe((result) => {
      this.allCities = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
      this.allCities.push(TenantCityLookupTableDto.fromJS({ id: -1, displayName: this.l('Other') }));
    });
  }

  checkIfIsCompanyUniqueMoiNumber() {
    this._tenantRegistrationService
      .isCompanyUniqueMoiNumber(this.tenant.moiNumber, undefined)
      .subscribe((result) => (this.isMoiNumberAvailable = result));
  }

  GetAllCountries() {
    this._tenantService.getAllCountryForTableDropdown().subscribe((result) => {
      this.allCountries = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
}
