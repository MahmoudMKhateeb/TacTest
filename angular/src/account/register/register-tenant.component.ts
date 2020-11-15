import { AfterViewInit, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  EditionSelectDto,
  PasswordComplexitySetting,
  ProfileServiceProxy,
  RegisterTenantOutput,
  TenantRegistrationServiceProxy,
  PaymentPeriodType,
  SubscriptionPaymentGatewayType,
  SubscriptionStartType,
  EditionPaymentType,
  TenantCountryLookupTableDto,
  TenantCityLookupTableDto,
  TenantServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { RegisterTenantModel } from './register-tenant.model';
import { TenantRegistrationHelperService } from './tenant-registration-helper.service';
import { finalize, catchError } from 'rxjs/operators';
import { ReCaptchaV3Service } from 'ngx-captcha';

@Component({
  templateUrl: './register-tenant.component.html',
  animations: [accountModuleAnimation()],
})
export class RegisterTenantComponent extends AppComponentBase implements OnInit, AfterViewInit {
  model: RegisterTenantModel = new RegisterTenantModel();
  passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
  subscriptionStartType = SubscriptionStartType;
  editionPaymentType: EditionPaymentType;
  paymentPeriodType = PaymentPeriodType;
  selectedPaymentPeriodType: PaymentPeriodType = PaymentPeriodType.Monthly;
  subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
  paymentId = '';
  recaptchaSiteKey: string = AppConsts.recaptchaSiteKey;
  allCountries: TenantCountryLookupTableDto[];
  allCities: TenantCityLookupTableDto[];
  saving = false;
  isCountySelected = false;
  isCompanyNameAvailable = true;
  isEmailAvailable = true;
  isEmailValid = true;
  constructor(
    injector: Injector,
    private _tenantRegistrationService: TenantRegistrationServiceProxy,
    private _router: Router,
    private _profileService: ProfileServiceProxy,
    private _tenantRegistrationHelper: TenantRegistrationHelperService,
    private _activatedRoute: ActivatedRoute,
    private _reCaptchaV3Service: ReCaptchaV3Service
  ) {
    super(injector);
  }

  ngOnInit() {
    this.model.editionId = this._activatedRoute.snapshot.queryParams['editionId'];
    this.editionPaymentType = this._activatedRoute.snapshot.queryParams['editionPaymentType'];

    if (this.model.editionId) {
      this.model.subscriptionStartType = this._activatedRoute.snapshot.queryParams['subscriptionStartType'];
    }

    //Prevent to create tenant in a tenant context
    if (this.appSession.tenant != null) {
      this._router.navigate(['account/login']);
      return;
    }

    this._profileService.getPasswordComplexitySetting().subscribe((result) => {
      this.passwordComplexitySetting = result.setting;
    });
    this.GetAllCountries();
    this.model.cityId = -2;
    this.model.countryId = -2;
  }

  ngAfterViewInit() {
    if (this.model.editionId) {
      this._tenantRegistrationService.getEdition(this.model.editionId).subscribe((result: EditionSelectDto) => {
        this.model.edition = result;
      });
    }
  }

  get useCaptcha(): boolean {
    return this.setting.getBoolean('App.TenantManagement.UseCaptchaOnRegistration');
  }

  save(): void {
    if (this.isEmailAvailable == false || this.isCompanyNameAvailable == false || this.isEmailValid == false) {
      this.notify.error('PleaseMakeSureYouProvideValidDetails!');
      return;
    }
    if (this.model.countryId == -2 || this.model.cityId == -2) {
      this.notify.error('please make sure you choose the country and the city!');
      return;
    }
    let recaptchaCallback = (token: string) => {
      this.saving = true;
      this.model.captchaResponse = token;
      this._tenantRegistrationService
        .registerTenant(this.model)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe((result: RegisterTenantOutput) => {
          this.notify.success(this.l('SuccessfullyRegistered'));
          this._tenantRegistrationHelper.registrationResult = result;
          if (parseInt(this.model.subscriptionStartType.toString()) === SubscriptionStartType.Paid) {
            this._router.navigate(['account/buy'], {
              queryParams: {
                tenantId: result.tenantId,
                editionId: this.model.editionId,
                subscriptionStartType: this.model.subscriptionStartType,
                editionPaymentType: this.editionPaymentType,
              },
            });
          } else {
            this._router.navigate(['account/register-tenant-result']);
          }
        });
    };

    if (this.useCaptcha) {
      this._reCaptchaV3Service.execute('6LeEZ-kUAAAAAGdgiM9BoWiRKBZOeULch73OlyZP', 'register_tenant', (token) => {
        recaptchaCallback(token);
      });
    } else {
      recaptchaCallback(null);
    }
  }

  CountryChanged(event) {
    if (event.target.value == -2) {
      this.isCountySelected = false;
    } else {
      this.isCountySelected = true;
    }

    this._tenantRegistrationService.getAllCitiesForTableDropdown(event.target.value).subscribe((result) => {
      this.allCities = result;
    });
  }

  GetAllCountries() {
    this._tenantRegistrationService.getAllCountryForTableDropdown().subscribe((result) => {
      this.allCountries = result;
    });
  }

  removeWhiteSpacesFromEmail() {
    console.log(this.model.adminEmailAddress);
    // this.model.adminEmailAddress.trim();
    console.log(this.model.adminEmailAddress);

    var exp = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/g;

    var result = exp.test(this.model.adminEmailAddress);

    // console.log(result);

    if (!result) {
      this.isEmailValid = false;
    } else {
      this.isEmailValid = true;
    }
    this.checkIfIsEmailAvailable();
  }

  checkIfIsCompanyUniqueName() {
    this._tenantRegistrationService.checkIfCompanyUniqueNameisAvailable(this.model.tenancyName).subscribe((result) => {
      this.isCompanyNameAvailable = result;
    });
  }
  checkIfIsEmailAvailable() {
    this._tenantRegistrationService.checkIfEmailisAvailable(this.model.adminEmailAddress).subscribe((result) => {
      this.isEmailAvailable = result;
    });
  }
}
