import { AfterViewChecked, Component, ElementRef, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
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
  CreateOrEditDocumentFileDto,
  DocumentFilesServiceProxy,
  UpdateDocumentFileInput,
  SelectItemDto,
  NationalitiesServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'createOrEditDriverModal',
  templateUrl: './create-or-edit-driver-modal.component.html',
  styleUrls: ['create-or-edit-driver-modal.component.less'],
  providers: [DateFormatterService],
})
export class CreateOrEditDriverModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('userForm', { static: false }) userForm: NgForm;

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
  user: UserEditDto = new UserEditDto();
  roles: UserRoleDto[];
  sendActivationEmail = true;
  setRandomPassword = true;
  passwordComplexityInfo = '';
  profilePicture: string;
  createOrEditDocumentFileDtos!: CreateOrEditDocumentFileDto[];
  hasValidationErorr = false;
  fileFormateIsInvalideIndexList: number[] = [];
  fileisDuplicateList: number[] = [];
  datesInValidList: boolean[] = [];

  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  /**
   * required documents fileUploader options
   * @private
   */
  private _DocsUploaderOptions: FileUploaderOptions = {};
  /**
   * required documents fileUploader
   */
  public DocsUploader: FileUploader;

  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;

  selectedDateTypeHijri = DateType.Hijri; // or DateType.Gregorian
  selectedDateTypeGregorian = DateType.Gregorian; // or DateType.Gregorian
  nationalities: SelectItemDto[] = [];
  allOrganizationUnits: OrganizationUnitDto[];
  memberedOrganizationUnits: string[];
  userPasswordRepeat = '';
  isUserNameValid = false;
  isWaintingUserNameValidation = false;
  CheckingIfDriverPhoneNumberIsValid = false;

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _profileService: ProfileServiceProxy,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _tokenService: TokenService,
    private _nationalitiesServiceProxy: NationalitiesServiceProxy
  ) {
    super(injector);
  }

  show(userId?: number): void {
    if (!userId) {
      //RequiredDocuments
      this._documentFilesServiceProxy.getDriverRequiredDocumentFiles('').subscribe((result) => {
        this.createOrEditDocumentFileDtos = result;
        this.intilizedates();
      });

      this.active = true;
      this.setRandomPassword = true;
      this.sendActivationEmail = true;
    }
    this.getDriverNationalites();
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
          this.setRandomPassword = true;
        }, 0);

        this.sendActivationEmail = false;
      }

      this._profileService.getPasswordComplexitySetting().subscribe((passwordComplexityResult) => {
        this.passwordComplexitySetting = passwordComplexityResult.setting;
        this.setPasswordComplexityInfo();
        this.modal.show();
      });
    });
    this.initDocsUploader();
  }

  onShown(): void {
    document.getElementById('Name').focus();
  }

  close(): void {
    this.active = false;
    this.userPasswordRepeat = '';
    this.docProgress = 0;
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

    if (!this.isAllFileFormatValid() || !this.isAllFileDuplicatePass()) {
      this.notify.error(this.l('makeSureThatYouFillAllRequiredFields'));
      return;
    }

    if (!this.user.id && this.user.isDriver) {
      if (this.DocsUploader.queue?.length > 0) {
        this.DocsUploader.uploadAll();
      } else {
        this.saveInternal();
      }
    } else {
      this.saveInternal();
    }
  }

  private saveInternal(): void {
    let input = new CreateOrUpdateUserInput();

    input.user = this.user;
    input.setRandomPassword = this.setRandomPassword;
    input.sendActivationEmail = this.sendActivationEmail;
    input.assignedRoleNames = _.map(_.filter(this.roles, { isAssigned: true, inheritedFromOrganizationUnit: false }), (role) => role.roleName);
    input.user.phoneNumber = input.user.userName;
    //#616
    input.user.emailAddress = input.user.userName + '@' + this.appSession.tenancyName + '.com';

    //docs

    input.createOrEditDocumentFileDtos = this.createOrEditDocumentFileDtos;

    if (!this.user.id) {
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
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
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

  getAssignedRoleCount(): number {
    return _.filter(this.roles, { isAssigned: true }).length;
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto, index: number): void {
    item.extn = event.target.files[0].type;
    item.name = event.target.files[0].name;

    //size validation
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      item.name = '';
      return;
    }

    //extention validation
    if (item.extn !== 'image/jpeg' && item.extn !== 'image/png' && item.extn !== 'application/pdf') {
      this.fileFormateIsInvalideIndexList.push(index);
      this.message.warn(this.l('PleaseChooseAvalidFormat'));
      item.name = '';
      return;
    } else {
      this.fileFormateIsInvalideIndexList = this.fileFormateIsInvalideIndexList.filter((x) => x !== index);
    }

    //not allow uploading the same document twice
    for (let i = 0; i < this.createOrEditDocumentFileDtos.length; i++) {
      const element = this.createOrEditDocumentFileDtos[i];
      if (element.name === item.name && element.extn === item.extn && i !== index) {
        item.name = '';
        item.extn = '';
        this.fileisDuplicateList.push(index);
        this.message.warn(this.l('DuplicateFileUploadMsg', element.name, element.extn));
        return;
      } else {
        //remove it if exist in validation list
        this.fileisDuplicateList = this.fileisDuplicateList.filter((x) => x !== index);
      }
    }

    this.DocsUploader.addToQueue(event.target.files);
  }

  /**
   * initialize required documents fileUploader
   */
  initDocsUploader(): void {
    this.DocsUploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
    this._DocsUploaderOptions.autoUpload = false;
    this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._DocsUploaderOptions.removeAfterUpload = true;

    this.DocsUploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.DocsUploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', fileItem.file.name);
      form.append('FileToken', this.guid());
    };

    this.DocsUploader.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);

      if (resp.success) {
        //attach each fileToken to his CreateOrEditDocumentFileDto

        this.createOrEditDocumentFileDtos.find(
          (x) => x.name === item.file.name && x.extn === item.file.type
        ).updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploader.onCompleteAll = () => {
      this.saveInternal();
    };

    //for progressBar
    this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.docProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploader.setOptions(this._DocsUploaderOptions);
  }

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
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

  // CheckIfDriverMobileNumberIsValid(mobileNumber: string) {
  //   this.isWaintingUserNameValidation = true;
  //   this._userService.checkIfPhoneNumberValid(mobileNumber, this.user.id).subscribe((res) => {
  //     this.isWaintingUserNameValidation = false;
  //     this.isPhoneNumberValid = res;
  //   });
  // }

  getDriverNationalites() {
    this._nationalitiesServiceProxy.getAllNationalityForDropdown().subscribe((res) => {
      this.nationalities = res;
    });
  }

  intilizedates() {
    this.createOrEditDocumentFileDtos.forEach((element) => {
      if (element.documentTypeDto.hasExpirationDate) {
        element.expirationDate = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
      }
    });
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

  isFileFormatValid(i: number): boolean {
    return this.fileFormateIsInvalideIndexList.every((x) => x !== i);
  }
  isFileDuplicate(i: number): boolean {
    return !this.fileisDuplicateList.every((x) => x !== i);
  }
  isAllFileFormatValid(): boolean {
    return this.fileFormateIsInvalideIndexList?.length === 0;
  }
  isAllFileDuplicatePass(): boolean {
    return this.fileisDuplicateList?.length === 0;
  }
}
