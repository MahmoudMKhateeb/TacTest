import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrUpdateUserInput,
  OrganizationUnitDto,
  PasswordComplexitySetting,
  ProfileServiceProxy,
  UserEditDto,
  UserRoleDto,
  UserServiceProxy,
  SelectItemDto,
  NationalitiesServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { IOrganizationUnitsTreeComponentData, OrganizationUnitsTreeComponent } from '../shared/organization-unit-tree.component';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { TabsetComponent } from 'ngx-bootstrap/tabs';

/**
 * Required vars and functions for devextreme validation
 **/
let that: any;
const emailAsyncValidation = function (value) {
  return new Promise(async (resolve) => {
    resolve(await that._userService.checkIfEmailisAvailable(value).toPromise());
  });
};
const phoneNumberAsyncValidation = function (value) {
  return new Promise(async (resolve) => {
    resolve(
      await that._userService.checkIfPhoneNumberValid(value, isNotNullOrUndefined(that.user) && that.user.id == null ? 0 : that.user.id).toPromise()
    );
  });
};
const userNameAsyncValidation = function (value) {
  return new Promise(async (resolve) => {
    resolve(
      await that._userService.checkIfUserNameValid(value, isNotNullOrUndefined(that.user) && that.user.id == null ? 0 : that.user.id).toPromise()
    );
  });
};

@Component({
  selector: 'createOrEditUserModal',
  templateUrl: './create-or-edit-user-modal.component.html',
  styleUrls: ['create-or-edit-user-modal.component.less'],
})
export class CreateOrEditUserModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('organizationUnitTree') organizationUnitTree: OrganizationUnitsTreeComponent;
  @ViewChild('staticTabs', { static: false }) staticTabs: TabsetComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() creatDriver: boolean;

  active = false;
  saving = false;
  canChangeUserName = true;
  isTwoFactorEnabled: boolean = this.setting.getBoolean('Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled');
  isLockoutEnabled: boolean = this.setting.getBoolean('Abp.Zero.UserManagement.UserLockOut.IsEnabled');
  passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();
  isEmailAvailable = true;
  isEmailValid = true;
  isPhoneNumberAvilable = true;
  isUserNameAvilable = true;
  user: UserEditDto = new UserEditDto();
  roles: UserRoleDto[];
  sendActivationEmail = true;
  setRandomPassword = true;
  passwordComplexityInfo = '';
  profilePicture: string;
  allOrganizationUnits: OrganizationUnitDto[];
  memberedOrganizationUnits: string[];
  userPasswordRepeat = '';
  nationalities: SelectItemDto[] = [];
  customPasswordMessage = '';

  numberOfRolesStyleClass = 'checkbox-inline';
  callbacks = [];
  adapterConfig = {
    getValue: () => {
      return this.getAssignedRoleCount() > 0;
    },
    applyValidationResults: (e) => {
      this.numberOfRolesStyleClass = e.isValid ? 'checkbox-inline' : 'checkbox-inline custom-invalid';
    },
    validationRequestsCallbacks: this.callbacks,
  };

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _profileService: ProfileServiceProxy,
    private _nationalitiesServiceProxy: NationalitiesServiceProxy
  ) {
    super(injector);
    that = this;
  }

  show(userId?: number): void {
    if (!userId) {
      this.active = true;
      this.setRandomPassword = true;
      this.sendActivationEmail = true;
      this.user = new UserEditDto();
    }
    this.getUserNationalites();
    this._userService.getUserForEdit(userId).subscribe((userResult) => {
      this.user = userResult.user;
      this.user.isDriver = this.creatDriver;
      this.roles = userResult.roles;
      this.canChangeUserName = this.user.userName !== AppConsts.userManagement.defaultAdminUserName;

      this.allOrganizationUnits = userResult.allOrganizationUnits;
      this.memberedOrganizationUnits = userResult.memberedOrganizationUnits;

      this.getProfilePicture(userId);

      if (userId) {
        this.active = true;

        setTimeout(() => {
          this.setRandomPassword = false;
        }, 0);

        this.sendActivationEmail = false;
      }

      this._profileService.getPasswordComplexitySetting().subscribe((passwordComplexityResult) => {
        this.passwordComplexitySetting = passwordComplexityResult.setting;
        this.setPasswordComplexityInfo();
        this.modal.show();
      });
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

  onShown(): void {
    this.organizationUnitTree.data = <IOrganizationUnitsTreeComponentData>{
      allOrganizationUnits: this.allOrganizationUnits,
      selectedOrganizationUnits: this.memberedOrganizationUnits,
    };

    document.getElementById('Name').focus();
  }

  save(): void {
    let input = new CreateOrUpdateUserInput();

    input.user = this.user;
    input.setRandomPassword = this.setRandomPassword;
    input.sendActivationEmail = this.sendActivationEmail;
    input.assignedRoleNames = _.map(_.filter(this.roles, { isAssigned: true, inheritedFromOrganizationUnit: false }), (role) => role.roleName);

    input.organizationUnits = this.organizationUnitTree.getSelectedOrganizations();

    this.saving = true;
    this._userService
      .createOrUpdateUser(input)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.userPasswordRepeat = '';
    this.numberOfRolesStyleClass = 'checkbox-inline';
    this.modal.hide();
    this.user = null;
  }

  getAssignedRoleCount(): number {
    return _.filter(this.roles, { isAssigned: true }).length;
  }

  removeWhiteSpacesFromEmail() {
    this.user.emailAddress.trim();
    let exp = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/g;

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
  CheckIfDriverPhoneNumberIsValid(phoneNumber: string, id: number) {
    if (phoneNumber.length > 9) {
      return false;
    }
    this._userService.checkIfPhoneNumberValid(phoneNumber, id == null ? 0 : id).subscribe((res) => {
      this.isPhoneNumberAvilable = res;
    });
  }
  CheckIfUserNameIsValid(userName: string, id: number) {
    this._userService.checkIfUserNameValid(userName, id == null ? 0 : id).subscribe((res) => {
      this.isUserNameAvilable = res;
    });
  }

  getUserNationalites() {
    this._nationalitiesServiceProxy.getAllNationalityForDropdown().subscribe((res) => {
      this.nationalities = res;
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

  removeWhiteSpacesFromEmailAsync(params) {
    params.value.trim();
    let exp = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/g;
    let result = exp.test(params.value);
    if (!result) {
      return new Promise((resolve) => resolve(false));
    }
    return emailAsyncValidation(params.value);
  }

  checkIfDriverPhoneNumberIsValidAsync(params) {
    if (params?.value?.length > 9) {
      return new Promise((resolve) => resolve(false));
    }
    return phoneNumberAsyncValidation(params.value);
  }

  checkIfCanChangeUserNameAsync() {
    return new Promise((resolve) => {
      resolve(that.canChangeUserName);
    });
  }

  checkIfIsUserNameAvailableAsync(params) {
    if (params?.value?.length > 9) {
      return new Promise((resolve) => resolve(false));
    }
    return userNameAsyncValidation(params.value);
  }

  validatePassword(params) {
    return new Promise((resolve) => {
      const givenPassword = params.value;
      const requireDigit = that.passwordComplexitySetting.requireDigit;
      if (requireDigit && givenPassword && !/[0-9]/.test(givenPassword)) {
        that.customPasswordMessage = that.l('PasswordComplexity_RequireDigit_Hint');
        resolve(false);
      }

      const requireUppercase = that.passwordComplexitySetting.requireUppercase;
      if (requireUppercase && givenPassword && !/[A-Z]/.test(givenPassword)) {
        that.customPasswordMessage = that.l('PasswordComplexity_RequireUppercase_Hint');
        resolve(false);
      }

      const requireLowercase = that.passwordComplexitySetting.requireLowercase;
      if (requireLowercase && givenPassword && !/[a-z]/.test(givenPassword)) {
        that.customPasswordMessage = that.l('PasswordComplexity_RequireLowercase_Hint');
        resolve(false);
      }

      const requiredLength = that.passwordComplexitySetting.requiredLength;
      if (requiredLength && givenPassword && givenPassword.length < requiredLength) {
        that.customPasswordMessage = that.l('PasswordComplexity_RequiredLength_Hint', that.passwordComplexitySetting.requiredLength);
        resolve(false);
      }

      const requireNonAlphanumeric = that.passwordComplexitySetting.requireNonAlphanumeric;
      if (requireNonAlphanumeric && givenPassword && /^[0-9a-zA-Z]+$/.test(givenPassword)) {
        that.customPasswordMessage = that.l('PasswordComplexity_RequireNonAlphanumeric_Hint');
        resolve(false);
      }
      resolve(true);
    });
  }

  passwordComparison = () => that.user.password;

  revalidateRoles() {
    this.callbacks.forEach((func) => {
      func();
    });
  }

  validationSummaryItemClicked($event: any) {
    if (($event.itemData.text as string).search(this.l('Roles')) > -1) {
      this.staticTabs.tabs[1].active = true;
      return;
    }
    this.staticTabs.tabs[0].active = true;
  }
}
