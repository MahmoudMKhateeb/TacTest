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
  GetUserForEditOutput,
  CreateOrEditDocumentFileDto,
  DocumentFilesServiceProxy,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { IOrganizationUnitsTreeComponentData, OrganizationUnitsTreeComponent } from '../../shared/organization-unit-tree.component';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { DateType } from '@app/admin/required-document-files/hijri-gregorian-datepicker/consts';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import * as moment from '@node_modules/moment';
import { DateFormatterService } from '@app/admin/required-document-files/hijri-gregorian-datepicker/date-formatter.service';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';

@Component({
  selector: 'createOrEditDriverModal',
  templateUrl: './create-or-edit-driver-modal.component.html',
  styleUrls: ['create-or-edit-driver-modal.component.less'],
  providers: [DateFormatterService],
})
export class CreateOrEditDriverModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() creatDriver: boolean;

  active = false;
  saving = false;
  canChangeUserName = true;
  isTwoFactorEnabled: boolean = this.setting.getBoolean('Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled');
  isLockoutEnabled: boolean = this.setting.getBoolean('Abp.Zero.UserManagement.UserLockOut.IsEnabled');
  passwordComplexitySetting: PasswordComplexitySetting = new PasswordComplexitySetting();

  user: UserEditDto = new UserEditDto();
  roles: UserRoleDto[];
  sendActivationEmail = true;
  setRandomPassword = true;
  passwordComplexityInfo = '';
  profilePicture: string;
  createOrEditDocumentFileDtos!: CreateOrEditDocumentFileDto[];

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

  allOrganizationUnits: OrganizationUnitDto[];
  memberedOrganizationUnits: string[];
  userPasswordRepeat = '';

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _profileService: ProfileServiceProxy,
    private dateFormatterService: DateFormatterService,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _tokenService: TokenService
  ) {
    super(injector);
  }

  show(userId?: number): void {
    if (!userId) {
      //RequiredDocuments
      this._documentFilesServiceProxy.getDriverRequiredDocumentFiles().subscribe((result) => {
        this.createOrEditDocumentFileDtos = result;
      });

      this.active = true;
      this.setRandomPassword = true;
      this.sendActivationEmail = true;
    }

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
    this.initDocsUploader();
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
    document.getElementById('Name').focus();
  }

  save(): void {
    if (!this.user.id && this.user.isDriver) {
      this.DocsUploader.uploadAll();
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

    //docs
    input.createOrEditDocumentFileDtos = this.createOrEditDocumentFileDtos;

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
    this.modal.hide();
  }

  getAssignedRoleCount(): number {
    return _.filter(this.roles, { isAssigned: true }).length;
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      return;
    }
    this.DocsUploader.addToQueue(event.target.files);

    item.extn = event.target.files[0].type;
    item.name = event.target.files[0].name;
  }

  selectedDateChange($event: NgbDateStruct, item: CreateOrEditDocumentFileDto) {
    if ($event != null && $event.year < 2000) {
      this.dateFormatterService.SetFormat('DD/MM/YYYY', 'iDD/iMM/iYYYY');
      const incomingDate = this.dateFormatterService.ToGregorian($event);
      item.expirationDate = moment(incomingDate.month + '/' + incomingDate.day + '/' + incomingDate.year, 'MM/DD/YYYY');
    } else if ($event != null && $event.year > 2000) {
      item.expirationDate = moment($event.month + '/' + $event.day + '/' + $event.year, 'MM/DD/YYYY');
    }
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
        console.log(item.file);
        this.createOrEditDocumentFileDtos.find(
          (x) => x.name === item.file.name && x.extn === item.file.type
        ).updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      console.log(resp);
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
}
