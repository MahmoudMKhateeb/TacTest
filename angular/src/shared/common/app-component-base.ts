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
import { HttpClient } from '@angular/common/http';
import { map } from '@node_modules/rxjs/internal/operators';
import { DevExtremeDataGridHelper } from '@shared/helpers/DevExtremeDataGridHelper';
import * as rtlDetect from 'rtl-detect';

const capitalize = (s) => {
  if (typeof s !== 'string') return '';
  return s.charAt(0).toUpperCase() + s.slice(1);
};

export class DatesFormats {
  hijriDate: string | undefined;
  GregorianDate!: moment.Moment | undefined;
}
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
  devExtremeDataGridHelper: DevExtremeDataGridHelper;
  ui: AppUiCustomizationService;
  appUrlService: AppUrlService;
  spinnerService: NgxSpinnerService;
  private ngxSpinnerTextService: NgxSpinnerTextService;
  dateFormatterService: DateFormatterService;
  http: HttpClient;
  /**
   * max file size that  user can upload
   */
  public maxDocumentFileBytesUserFriendlyValue = 4;
  isRtl = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);

  iconList = [
    // array of icon class list based on type
    { type: 'xlsx', icon: 'fas fa-file-excel' },
    { type: 'pdf', icon: 'fas fa-file-pdf' },
    { type: 'jpg', icon: 'fas fa-file-image' },
  ];

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
    this.http = injector.get(HttpClient);
  }

  flattenDeep(array) {
    return array.reduce((acc, val) => (Array.isArray(val) ? acc.concat(this.flattenDeep(val)) : acc.concat(val)), []);
  }

  l(key: string, ...args: any[]): string {
    args.unshift(key);
    args.unshift(this.localizationSourceName);
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

  GetGregorianAndhijriFromDatepickerChange($event: NgbDateStruct) {
    var item = new DatesFormats();
    if ($event != null && $event.year < 1900) {
      const ngDate = this.dateFormatterService.ToGregorian($event);
      item.GregorianDate = this.dateFormatterService.NgbDateStructToMoment(ngDate);
      item.hijriDate = this.dateFormatterService.ToString($event);
    } else if ($event != null && $event.year > 1900) {
      item.GregorianDate = this.dateFormatterService.NgbDateStructToMoment($event);
      const ngDate = this.dateFormatterService.ToHijri($event);
      item.hijriDate = this.dateFormatterService.ToString(ngDate);
    }
    return item;
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

  getFileExtension(filename: string) {
    if (!filename) {
      return '';
    }
    // this will give you icon class name
    let ext = filename.split('.').pop();
    let obj = this.iconList.filter((row) => {
      if (row.type === ext) {
        return true;
      }
    });
    if (obj.length > 0) {
      let icon = obj[0].icon;
      return icon;
    } else {
      return '';
    }
  }

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }

  checkImportedList(ImportedVasesList) {
    return ImportedVasesList
      ? ImportedVasesList.filter((e) => e.exception != undefined && e.exception != null && e.exception != '').length > 0
      : false;
  }

  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
  get isShipper(): boolean {
    return this.feature.isEnabled('App.Shipper');
  }
  get isTachyonDealer(): boolean {
    return this.feature.isEnabled('App.TachyonDealer');
  }
  get isTachyonDealerOrHost(): boolean {
    return this.isTachyonDealer || !this.appSession.tenantId;
  }
  get isCarrierSaas(): boolean {
    return this.feature.isEnabled('App.CarrierAsASaas');
  }

  get hasCarrierClients(): boolean {
    return this.feature.isEnabled('App.CarrierClients');
  }

  get hasShipperClients(): boolean {
    return this.feature.isEnabled('App.ShipperClients');
  }

  IfOther(items: any, id: any) {
    // id return string or number
    if (id != undefined) return items?.filter((x) => x.id == id && x.isOther).length > 0;
    else return false;
  }

  cannotContainSpace(input: string) {
    if (input) {
      if (input.includes(' ') && input.trim().length == 0) return false;
      else return true;
    }
  }
  get isSaaS(): boolean {
    return this.feature.isEnabled('App.CarrierAsASaas');
  }
}
