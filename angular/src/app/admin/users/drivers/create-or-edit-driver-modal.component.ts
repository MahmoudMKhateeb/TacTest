import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarriersForDropDownDto,
  CreateOrEditDocumentFileDto,
  CreateOrUpdateUserInput,
  DocumentFilesServiceProxy,
  DriverLicenseTypesServiceProxy,
  GetLicenseTypeForDropDownOutput,
  NationalitiesServiceProxy,
  OrganizationUnitDto,
  PasswordComplexitySetting,
  PriceOfferServiceProxy,
  ProfileServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  UserEditDto,
  UserServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { NgForm } from '@angular/forms';
import { RequiredDocumentFormChildComponent } from '@app/shared/common/required-document-form-child/required-document-form-child.component';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';

@Component({
  //changeDetection: ChangeDetectionStrategy.Default,
  selector: 'createOrEditDriverModal',
  templateUrl: './create-or-edit-driver-modal.component.html',
  styleUrls: ['create-or-edit-driver-modal.component.less'],
  providers: [DateFormatterService],
})
export class CreateOrEditDriverModalComponent extends AppComponentBase {
  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _profileService: ProfileServiceProxy,
    private _nationalitiesServiceProxy: NationalitiesServiceProxy,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _driverLicenseTypesServiceProxy: DriverLicenseTypesServiceProxy,

    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _priceOfferService: PriceOfferServiceProxy
  ) {
    super(injector);
    this.getDriverRequiredDocumentFiles();
  }
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('userForm', { static: false }) userForm: NgForm;
  //@ViewChild(RequiredDocumentFormChildComponent) requiredDocumentFormChildComponent: RequiredDocumentFormChildComponent;
  @ViewChild('requiredDocumentFormChildComponent', { static: false }) requiredDocumentFormChildComponent: RequiredDocumentFormChildComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() creatDriver: boolean;
  @Input() placeholder: string;

  active = false;
  saving = false;
  canChangeUserName = true;
  isTwoFactorEnabled: boolean = this.setting.getBoolean('Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled');
  isLockoutEnabled: boolean = this.setting.getBoolean('Abp.Zero.UserManagement.UserLockOut.IsEnabled');
  passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
  isEmailAvailable = true;
  isEmailValid = true;
  isPhoneNumberAvilable = true;
  user: UserEditDto = new UserEditDto();
  sendActivationEmail = true;
  passwordComplexityInfo = '';
  profilePicture: string;
  createOrEditDocumentFileDtos!: CreateOrEditDocumentFileDto[];
  hasValidationErorr = false;
  datesInValidList: boolean[] = [];

  nationalities: SelectItemDto[] = [];
  carriers: CarriersForDropDownDto[] = [];
  allOrganizationUnits: OrganizationUnitDto[];
  memberedOrganizationUnits: string[];
  userPasswordRepeat = '';
  isUserNameValid = false;
  isWaintingUserNameValidation = false;
  CheckingIfDriverPhoneNumberIsValid = false;

  driverLicenseTypes: GetLicenseTypeForDropDownOutput[] = [];
  AllActorsCarriers: SelectItemDto[];

  // CheckIfDriverMobileNumberIsValid(mobileNumber: string) {
  //   this.isWaintingUserNameValidation = true;
  //   this._userService.checkIfPhoneNumberValid(mobileNumber, this.user.id).subscribe((res) => {
  //     this.isWaintingUserNameValidation = false;
  //     this.isPhoneNumberValid = res;
  //   });
  // }
  selectedDate: NgbDateStruct;
  hijriDateNow = this.dateFormatterService.GetTodayHijri();
  hDate = this.hijriDateNow.split('-');
  gregDateNow = this.dateFormatterService.GetTodayGregorian();
  minGreg: NgbDateStruct = { day: 1, month: 1, year: 1900 };
  minHijri: NgbDateStruct = { day: 1, month: 1, year: 1342 };
  maxGreg: NgbDateStruct = { day: this.gregDateNow.day, month: this.gregDateNow.month, year: this.gregDateNow.year };

  maxHijri: NgbDateStruct = { day: parseInt(this.hDate[2]), month: parseInt(this.hDate[1]), year: parseInt(this.hDate[0]) };

  private getDriverRequiredDocumentFiles() {
    //RequiredDocuments
    this._documentFilesServiceProxy.getDriverRequiredDocumentFiles('').subscribe((result) => {
      this.createOrEditDocumentFileDtos = result;
    });
  }

  show(userId?: number): void {
    this.CheckingIfDriverPhoneNumberIsValid = true;
    if (!userId) {
      this.active = true;
      this.sendActivationEmail = true;
    }
    this.getDriverNationalites();
    this.GetDriverLicenseTypes();
    this._userService.getUserForEdit(userId).subscribe((userResult) => {
      this.user = userResult.user;
      this.user.isDriver = this.creatDriver;
      this.canChangeUserName = this.user.userName !== AppConsts.userManagement.defaultAdminUserName;
      this.allOrganizationUnits = userResult.allOrganizationUnits;
      this.memberedOrganizationUnits = userResult.memberedOrganizationUnits;

      this.getProfilePicture(userId);

      if (userId) {
        this.active = true;

        this.sendActivationEmail = false;
        this.selectedDate = this.dateFormatterService.MomentToNgbDateStruct(userResult.user.dateOfBirth);
      }
      if (this.isUserTenantRequired) {
        this._shippingRequestServiceProxy.getAllCarriersForDropDown().subscribe((result) => (this.carriers = result));
      }

      this._priceOfferService.getAllCarrierActorsForDropDown().subscribe((result) => {
        this.AllActorsCarriers = result;
        let defaultItem = new SelectItemDto();
        defaultItem.id = null;
        defaultItem.displayName = this.l('Myself');
        this.AllActorsCarriers.unshift(defaultItem);
      });

      this._profileService.getPasswordComplexitySetting().subscribe((passwordComplexityResult) => {
        this.passwordComplexitySetting = passwordComplexityResult.setting;
        this.setPasswordComplexityInfo();
        this.modal.show();
      });
    });
  }

  onShown(): void {
    this.getDriverRequiredDocumentFiles();
    document.getElementById('Name').focus();
  }

  close(): void {
    this.createOrEditDocumentFileDtos = [];
    this.active = false;
    this.userPasswordRepeat = '';
    this.user = undefined;
    this.selectedDate = undefined;
    this.modal.hide();
  }

  save(): void {
    if (this.userForm.invalid) {
      return;
    }
    this.saving = true;

    if (this.isEmailAvailable === false || this.isEmailValid === false) {
      this.notify.error('PleaseMakeSureYouProvideValidDetails!');
      return;
    }

    // if (!this.isAllFileFormatValid() || !this.isAllFileDuplicatePass()) {
    //   this.notify.error(this.l('makeSureThatYouFillAllRequiredFields'));
    //   return;
    // }

    if (!this.user.id && this.user.isDriver) {
      if (this.requiredDocumentFormChildComponent.DocsUploader.queue?.length > 0) {
        this.requiredDocumentFormChildComponent.DocsUploader.uploadAll();
      } else {
        this.saveInternal();
      }
    } else {
      this.saveInternal();
    }
  }

  public saveInternal(): void {
    let input = new CreateOrUpdateUserInput();

    input.user = this.user;
    console.log(input);
    input.setRandomPassword = this.user.id == null;
    input.sendActivationEmail = this.sendActivationEmail;
    input.user.phoneNumber = input.user.userName;
    //#616
    if (!input.user.emailAddress) {
      var filteredEmailAddress = this.appSession.tenancyName.replace(/[^a-zA-Z ]/g, '');
      input.user.emailAddress = input.user.userName + '@' + filteredEmailAddress + '.com';
    }

    //docs

    if (!this.user.id) {
      input.createOrEditDocumentFileDtos = this.createOrEditDocumentFileDtos;

      input.createOrEditDocumentFileDtos.forEach((element) => {
        let date = this.dateFormatterService.MomentToNgbDateStruct(element.expirationDate);
        let hijriDate = this.dateFormatterService.ToHijri(date);
        element.hijriExpirationDate = this.dateFormatterService.ToString(hijriDate);
      });
    }

    this._userService
      .createOrUpdateUser(input)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.close();
        this.notify.info(this.l('SavedSuccessfully'));
        this.modalSave.emit(null);
      });
  }

  setPasswordComplexityInfo(): void {
    this.passwordComplexityInfo = '<ul>';

    if (this.passwordComplexitySetting.requireDigit) {
      this.passwordComplexityInfo += '<li>' + this.l('PasswordComplexity_RequireDigit_Hint') + '</li>';
    }

    if (this.passwordComplexitySetting.requireLowercase) {
      this.passwordComplexityInfo += '<li>' + this.l('PasswordComplexity_RequireLowercase_Hint') + '</li>';
    }

    if (this.passwordComplexitySetting.requireUppercase) {
      this.passwordComplexityInfo += '<li>' + this.l('PasswordComplexity_RequireUppercase_Hint') + '</li>';
    }

    if (this.passwordComplexitySetting.requireNonAlphanumeric) {
      this.passwordComplexityInfo += '<li>' + this.l('PasswordComplexity_RequireNonAlphanumeric_Hint') + '</li>';
    }

    if (this.passwordComplexitySetting.requiredLength) {
      this.passwordComplexityInfo +=
        '<li>' + this.l('PasswordComplexity_RequiredLength_Hint', this.passwordComplexitySetting.requiredLength) + '</li>';
    }

    this.passwordComplexityInfo += '</ul>';
  }

  getProfilePicture(userId: number): void {
    if (!userId) {
      this.profilePicture = this.appRootUrl() + 'assets/common/images/default-profile-picture.png';
      return;
    }

    this._profileService.getProfilePictureByUser(userId).subscribe((result) => {
      if (result && result.profilePicture) {
        this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
      } else {
        this.profilePicture = this.appRootUrl() + 'assets/common/images/default-profile-picture.png';
      }
    });
  }

  CheckIfDriverPhoneNumberIsValid(phoneNumber: string, id: number) {
    if (phoneNumber.trim().length === 9) {
      this.CheckingIfDriverPhoneNumberIsValid = true;
      this._userService.checkIfPhoneNumberValid(phoneNumber, id == null ? 0 : id).subscribe((res) => {
        this.isPhoneNumberAvilable = res;
        this.CheckingIfDriverPhoneNumberIsValid = false;
      });
    }
  }

  removeWhiteSpacesFromEmail() {
    this.user.emailAddress.trim();
    let exp = /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$/i;

    let result = exp.test(this.user.emailAddress);
    if (!result) {
      this.isEmailValid = false;
    } else {
      this.isEmailValid = true;
    }
    this.checkIfIsEmailAvailable();
  }

  checkIfIsEmailAvailable() {
    this._userService.checkIfEmailisAvailable(this.user.emailAddress).subscribe((result) => {
      this.isEmailAvailable = result;
    });
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

  getDriverNationalites() {
    this._nationalitiesServiceProxy.getAllNationalityForDropdown().subscribe((res) => {
      this.nationalities = res;
    });
  }
  GetDriverLicenseTypes() {
    this._driverLicenseTypesServiceProxy.getForDropDownList().subscribe((res) => {
      this.driverLicenseTypes = res;
    });
  }

  get isUserTenantRequired(): boolean {
    return (this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) && !this.user.id;
  }
  /**
   * do not delete the function dateSelected() below >
   */

  // dateSelected() {
  //   for (let index = 0; index < this.createOrEditDocumentFileDtos.filter((x) => x.documentTypeDto.hasExpirationDate).length; index++) {
  //     const element = this.createOrEditDocumentFileDtos[index];
  //     if (element.documentTypeDto.hasExpirationDate) {
  //       this.datesInValidList[index] = element.expirationDate == null;
  //     }
  //   }
  //   if (
  //     this.datesInValidList.every((x) => x === false) &&
  //     this.createOrEditDocumentFileDtos.filter((x) => x.documentTypeDto.hasExpirationDate).length == this.datesInValidList.length
  //   ) {
  //     this.allDatesValid = true;
  //   } else {
  //     this.allDatesValid = false;
  //   }

  //   console.log(this.createOrEditDocumentFileDtos.filter((x) => x.documentTypeDto.hasExpirationDate));
  //   console.log(this.datesInValidList.every((x) => x === false));
  //   console.log(this.datesInValidList);
  // }

  dateOfBirthSelectedDateChange($event: NgbDateStruct, user: UserEditDto) {
    if ($event != null && $event.year < 1900) {
      const ngDate = this.dateFormatterService.ToGregorian($event);
      user.dateOfBirth = this.dateFormatterService.NgbDateStructToMoment(ngDate);
      user.hijriDateOfBirth = this.dateFormatterService.ToString($event);
    } else if ($event != null && $event.year > 1900) {
      user.dateOfBirth = this.dateFormatterService.NgbDateStructToMoment($event);
      const ngDate = this.dateFormatterService.ToHijri($event);
      user.hijriDateOfBirth = this.dateFormatterService.ToString(ngDate);
    }

    console.log(user.dateOfBirth, user.hijriDateOfBirth);
  }
}
