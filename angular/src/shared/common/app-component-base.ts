import {
  PermissionCheckerService,
  FeatureCheckerService,
  LocalizationService,
  MessageService,
  AbpMultiTenancyService,
  NotifyService,
  SettingService,
} from 'abp-ng2-module';
import { Component, Injector } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { CreateOrEditDocumentFileDto, UiCustomizationSettingsDto } from '@shared/service-proxies/service-proxies';
import { NgxSpinnerService } from 'ngx-spinner';
import { NgxSpinnerTextService } from '@app/shared/ngx-spinner-text.service';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNumeric } from '@node_modules/rxjs/internal/util/isNumeric';
import { TerminologieServiceProxy } from 'shared/service-proxies/terminologies-ervice-proxy';
import { HttpClient } from '@angular/common/http';
import { map } from '@node_modules/rxjs/internal/operators';

const capitalize = (s) => {
  if (typeof s !== 'string') return '';
  return s.charAt(0).toUpperCase() + s.slice(1);
};

export abstract class AppComponentBase {
  localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

  localization: LocalizationService;
  permission: PermissionCheckerService;
  feature: FeatureCheckerService;
  notify: NotifyService;
  setting: SettingService;
  message: MessageService;
  multiTenancy: AbpMultiTenancyService;
  appSession: AppSessionService;
  primengTableHelper: PrimengTableHelper;
  ui: AppUiCustomizationService;
  appUrlService: AppUrlService;
  spinnerService: NgxSpinnerService;
  private terminologieServiceProxy: TerminologieServiceProxy;
  private ngxSpinnerTextService: NgxSpinnerTextService;
  dateFormatterService: DateFormatterService;
  http: HttpClient;
  /**
   * max file size that  user can upload
   */
  public maxDocumentFileBytesUserFriendlyValue = 4;
  constructor(injector: Injector) {
    this.localization = injector.get(LocalizationService);
    this.permission = injector.get(PermissionCheckerService);
    this.feature = injector.get(FeatureCheckerService);
    this.notify = injector.get(NotifyService);
    this.setting = injector.get(SettingService);
    this.message = injector.get(MessageService);
    this.multiTenancy = injector.get(AbpMultiTenancyService);
    this.appSession = injector.get(AppSessionService);
    this.ui = injector.get(AppUiCustomizationService);
    this.appUrlService = injector.get(AppUrlService);
    this.primengTableHelper = new PrimengTableHelper();
    this.spinnerService = injector.get(NgxSpinnerService);
    this.ngxSpinnerTextService = injector.get(NgxSpinnerTextService);
    this.dateFormatterService = injector.get(DateFormatterService);
    this.terminologieServiceProxy = injector.get(TerminologieServiceProxy);
    this.http = injector.get(HttpClient);
  }

  flattenDeep(array) {
    return array.reduce((acc, val) => (Array.isArray(val) ? acc.concat(this.flattenDeep(val)) : acc.concat(val)), []);
  }

  l(key: string, ...args: any[]): string {
    key = capitalize(key);
    args.unshift(key);
    args.unshift(this.localizationSourceName);
    this.terminologieServiceProxy.Add(key);
    return this.ls.apply(this, args);
  }

  ls(sourcename: string, key: string, ...args: any[]): string {
    let localizedText = this.localization.localize(key, sourcename);

    if (!localizedText) {
      localizedText = key;
    }

    if (!args || !args.length) {
      return localizedText;
    }

    args.unshift(localizedText);
    return abp.utils.formatString.apply(this, this.flattenDeep(args));
  }

  isGranted(permissionName: string): boolean {
    return this.permission.isGranted(permissionName);
  }

  isGrantedAny(...permissions: string[]): boolean {
    if (!permissions) {
      return false;
    }

    for (const permission of permissions) {
      if (this.isGranted(permission)) {
        return true;
      }
    }

    return false;
  }

  s(key: string): string {
    return abp.setting.get(key);
  }

  appRootUrl(): string {
    return this.appUrlService.appRootUrl;
  }

  get currentTheme(): UiCustomizationSettingsDto {
    return this.appSession.theme;
  }

  get containerClass(): string {
    if (this.appSession.theme.baseSettings.layout.layoutType === 'fluid') {
      return 'container-fluid';
    }

    return 'container';
  }

  showMainSpinner(text?: string): void {
    this.ngxSpinnerTextService.currentText = text;
    this.spinnerService.show();
  }

  hideMainSpinner(text?: string): void {
    this.spinnerService.hide();
  }

  Number(string: string) {
    return Number(string);
  }

  /**
   * Forces the User To Enter Numbers Only in inputs
   * recommend to be used with keypress
   * @param $event
   */
  numberOnly($event: KeyboardEvent) {
    return isNumeric($event.key);
  }

  /**
   * is used to handle hijri gregorian datepicker change event
   * @param $event
   * @param item
   */
  hijriDatepickerSelectedDateChange($event: NgbDateStruct, item: CreateOrEditDocumentFileDto) {
    if ($event != null && $event.year < 1900) {
      const ngDate = this.dateFormatterService.ToGregorian($event);
      item.expirationDate = this.dateFormatterService.NgbDateStructToMoment(ngDate);
      item.hijriExpirationDate = this.dateFormatterService.ToString($event);
    } else if ($event != null && $event.year > 1900) {
      item.expirationDate = this.dateFormatterService.NgbDateStructToMoment($event);
      const ngDate = this.dateFormatterService.ToHijri($event);
      item.hijriExpirationDate = this.dateFormatterService.ToString(ngDate);
    }
  }

  toBase64 = (file) =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = (error) => reject(error);
    });

  downloadFile(url: string): any {
    return this.http.get(url, { responseType: 'blob' }).pipe(
      map((result: any) => {
        return result;
      })
    );
  }
}
