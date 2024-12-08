import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { AppComponentBase, DatesFormats } from '@shared/common/app-component-base';
import {
  CommonLookupServiceProxy,
  CreateTenantInput,
  PasswordComplexitySetting,
  ProfileServiceProxy,
  SubscribableEditionComboboxItemDto,
  TenantCityLookupTableDto,
  TenantCountryLookupTableDto,
  TenantRegistrationServiceProxy,
  TenantServiceProxy,
} from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';

@Component({
  selector: 'createTenantModal',
  templateUrl: './create-tenant-modal.component.html',
  providers: [DateFormatterService],
})
export class CreateTenantModalComponent extends AppComponentBase {
  @ViewChild('createModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  dates: DatesFormats;
  active = false;
  saving = false;
  setRandomPassword = false;
  useHostDb = true;
  editions: SubscribableEditionComboboxItemDto[] = [];
  tenant: CreateTenantInput = new CreateTenantInput();
  passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
  isUnlimited = false;
  isSubscriptionFieldsVisible = false;
  isSelectedEditionFree = false;
  tenantAdminPasswordRepeat = '';
  allCountries: TenantCountryLookupTableDto[];
  allCities: TenantCityLookupTableDto[];
  isCountySelected = false;
  isCompanyNameAvailable = true;
  isEmailAvailable = true;
  isEmailValid = true;
  isMoiNumberAvailable = true;
  subscriptionEndDateUtc1: NgbDateStruct;
  hijriDateNow = this.dateFormatterService.GetTodayHijri();
  hDate = this.hijriDateNow.split('-');
  gregDateNow = this.dateFormatterService.GetTodayGregorian();
  minGreg: NgbDateStruct = { day: this.gregDateNow.day, month: this.gregDateNow.month, year: this.gregDateNow.year };
  minHijri: NgbDateStruct = { day: parseInt(this.hDate[2]), month: parseInt(this.hDate[1]), year: parseInt(this.hDate[0]) };
  @Input() parentForm: NgForm;
  @ViewChild('userForm', { static: false }) userForm: NgForm;
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  constructor(
    injector: Injector,
    private _tenantService: TenantServiceProxy,
    private _tenantRegistrationService: TenantRegistrationServiceProxy,
    private _commonLookupService: CommonLookupServiceProxy,
    private _profileService: ProfileServiceProxy
  ) {
    super(injector);
  }

  show() {
    this.active = true;
    this.init();

    this._profileService.getPasswordComplexitySetting().subscribe((result) => {
      this.passwordComplexitySetting = result.setting;
      this.modal.show();
    });
  }

  onShown(): void {
    document.getElementById('TenancyName').focus();
  }

  init(): void {
    this.tenant = new CreateTenantInput();
    this.tenant.isActive = true;
    this.tenant.shouldChangePasswordOnNextLogin = true;
    this.tenant.sendActivationEmail = false;
    this.tenant.editionId = 0;
    this.tenant.isInTrialPeriod = false;
    //this.subscriptionEndDateUtc1 = this.dateFormatterService.MomentToNgbDateStruct(this.dates.GregorianDate);

    this._commonLookupService.getEditionsForCombobox(false).subscribe((result) => {
      this.editions = result.items;

      let notAssignedItem = new SubscribableEditionComboboxItemDto();
      notAssignedItem.value = '';
      notAssignedItem.displayText = this.l('NotAssigned');

      this.editions.unshift(notAssignedItem);

      this._commonLookupService.getDefaultEditionName().subscribe((getDefaultEditionResult) => {
        let defaultEdition = _.filter(this.editions, { displayText: getDefaultEditionResult.name });
        if (defaultEdition && defaultEdition[0]) {
          this.tenant.editionId = parseInt(defaultEdition[0].value);
          this.toggleSubscriptionFields();
        }
      });
    });
    this.GetAllCountries();
    this.tenant.cityId = null;
    this.tenant.countryId = null;
  }

  getEditionValue(item): number {
    return parseInt(item.value);
  }

  selectedEditionIsFree(): boolean {
    let selectedEditions = _.filter(this.editions, { value: this.tenant.editionId.toString() }).map((u) =>
      Object.assign(new SubscribableEditionComboboxItemDto(), u)
    );

    if (selectedEditions.length !== 1) {
      this.isSelectedEditionFree = true;
    }

    let selectedEdition = selectedEditions[0];
    this.isSelectedEditionFree = selectedEdition.isFree;
    return this.isSelectedEditionFree;
  }

  subscriptionEndDateIsValid(): boolean {
    if (this.tenant.editionId <= 0) {
      return true;
    }

    if (this.isUnlimited) {
      return true;
    }

    if (!this.tenant.subscriptionEndDateUtc) {
      return false;
    }

    return this.tenant.subscriptionEndDateUtc !== undefined;
  }

  save(): void {
    this.tenant.tenancyName = this.tenant.companyName.trim().replace(' ', '_');
    if (this.isEmailAvailable == false || this.isCompanyNameAvailable == false || this.isEmailValid == false) {
      this.notify.error('PleaseMakeSureYouProvideValidDetails!');
      return;
    }
    this.saving = true;

    if (this.setRandomPassword) {
      this.tenant.adminPassword = null;
    }

    if (this.tenant.editionId === 0) {
      this.tenant.editionId = null;
    }

    if (this.isUnlimited) {
      this.tenant.isInTrialPeriod = false;
    }

    if (this.isUnlimited || this.tenant.editionId <= 0) {
      this.tenant.subscriptionEndDateUtc = null;
    } else {
      if (this.subscriptionEndDateUtc1 != null && this.subscriptionEndDateUtc1 != undefined)
        this.tenant.subscriptionEndDateUtc = this.GetGregorianAndhijriFromDatepickerChange(this.subscriptionEndDateUtc1).GregorianDate;
    }

    this._tenantService
      .createTenant(this.tenant)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.subscriptionEndDateUtc1 = undefined;
    this.tenantAdminPasswordRepeat = '';
    this.modal.hide();
  }

  onEditionChange(): void {
    this.tenant.isInTrialPeriod = this.tenant.editionId > 0 && !this.selectedEditionIsFree();
    this.toggleSubscriptionFields();
  }

  toggleSubscriptionFields() {
    this.isSelectedEditionFree = this.selectedEditionIsFree();
    if (this.tenant.editionId <= 0 || this.isSelectedEditionFree) {
      this.isSubscriptionFieldsVisible = false;

      if (this.isSelectedEditionFree) {
        this.isUnlimited = true;
      } else {
        this.isUnlimited = false;
      }
    } else {
      this.isSubscriptionFieldsVisible = true;
    }
  }

  onIsUnlimitedChange() {
    if (this.isUnlimited) {
      this.tenant.isInTrialPeriod = false;
    }
  }

  CountryChanged(event) {
    console.log('event', event);
    if (event.selectedItem.id == -2) {
      this.isCountySelected = false;
    } else {
      this.isCountySelected = true;
    }

    this._tenantService.getAllCitiesForTableDropdown(event.selectedItem.id).subscribe((result) => {
      this.allCities = result;
      this.allCities.push(TenantCityLookupTableDto.fromJS({ id: -1, displayName: this.l('Other') }));
    });
  }

  GetAllCountries() {
    this._tenantService.getAllCountryForTableDropdown().subscribe((result) => {
      this.allCountries = result;
    });
  }

  checkIfIsCompanyUniqueName() {
    this.tenant.tenancyName = this.tenant.companyName.trim().replace(' ', '_');
    this._tenantRegistrationService.checkIfCompanyUniqueNameisAvailable(this.tenant.tenancyName).subscribe((result) => {
      this.isCompanyNameAvailable = result;
    });
  }

  checkIfIsEmailAvailable() {
    this._tenantRegistrationService
      .checkIfEmailisAvailable(this.tenant.adminEmailAddress == null ? ' ' : this.tenant.adminEmailAddress)
      .subscribe((result) => {
        this.isEmailAvailable = result;
      });
  }
  removeWhiteSpacesFromEmail() {
    var exp = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/g;
    var result = exp.test(this.tenant.adminEmailAddress);
    if (!result) {
      this.isEmailValid = false;
    } else {
      this.isEmailValid = true;
    }
    this.checkIfIsEmailAvailable();
  }
  checkIfIsCompanyUniqueMoiNumber() {
    // this._tenantRegistrationService
    //   .isCompanyUniqueMoiNumber(this.tenant.moiNumber, undefined)
    //   .subscribe((result) => (this.isMoiNumberAvailable = result));
  }
}
