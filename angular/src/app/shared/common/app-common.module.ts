import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppLocalizationService } from '@app/shared/common/localization/app-localization.service';
import { AppNavigationService } from '@app/shared/layout/nav/app-navigation.service';
import { TACHYONCommonModule } from '@shared/common/common.module';
import { UtilsModule } from '@shared/utils/utils.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { AppAuthService } from './auth/app-auth.service';
import { AppRouteGuard } from './auth/auth-route-guard';
import { CommonLookupModalComponent } from './lookup/common-lookup-modal.component';
import { EntityTypeHistoryModalComponent } from './entityHistory/entity-type-history-modal.component';
import { EntityChangeDetailModalComponent } from './entityHistory/entity-change-detail-modal.component';
import { DateRangePickerInitialValueSetterDirective } from './timing/date-range-picker-initial-value.directive';
import { DatePickerInitialValueSetterDirective } from './timing/date-picker-initial-value.directive';
import { DateTimeService } from './timing/date-time.service';
import { TimeZoneComboComponent } from './timing/timezone-combo.component';
import { CustomizableDashboardComponent } from './customizable-dashboard/customizable-dashboard.component';
import { WidgetGeneralStatsComponent } from './customizable-dashboard/widgets/widget-general-stats/widget-general-stats.component';
import { DashboardViewConfigurationService } from './customizable-dashboard/dashboard-view-configuration.service';
import { GridsterModule } from 'angular-gridster2';
import { WidgetDailySalesComponent } from './customizable-dashboard/widgets/widget-daily-sales/widget-daily-sales.component';
import { WidgetEditionStatisticsComponent } from './customizable-dashboard/widgets/widget-edition-statistics/widget-edition-statistics.component';
import { WidgetHostTopStatsComponent } from './customizable-dashboard/widgets/widget-host-top-stats/widget-host-top-stats.component';
import { WidgetIncomeStatisticsComponent } from './customizable-dashboard/widgets/widget-income-statistics/widget-income-statistics.component';
import { WidgetMemberActivityComponent } from './customizable-dashboard/widgets/widget-member-activity/widget-member-activity.component';
import { WidgetProfitShareComponent } from './customizable-dashboard/widgets/widget-profit-share/widget-profit-share.component';
import { WidgetRecentTenantsComponent } from './customizable-dashboard/widgets/widget-recent-tenants/widget-recent-tenants.component';
import { WidgetRegionalStatsComponent } from './customizable-dashboard/widgets/widget-regional-stats/widget-regional-stats.component';
import { WidgetSalesSummaryComponent } from './customizable-dashboard/widgets/widget-sales-summary/widget-sales-summary.component';
import { WidgetSubscriptionExpiringTenantsComponent } from './customizable-dashboard/widgets/widget-subscription-expiring-tenants/widget-subscription-expiring-tenants.component';
import { WidgetTopStatsComponent } from './customizable-dashboard/widgets/widget-top-stats/widget-top-stats.component';
import { FilterDateRangePickerComponent } from './customizable-dashboard/filters/filter-date-range-picker/filter-date-range-picker.component';
import { AddWidgetModalComponent } from './customizable-dashboard/add-widget-modal/add-widget-modal.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { CountoModule } from 'angular2-counto';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';
import { SingleLineStringInputTypeComponent } from './input-types/single-line-string-input-type/single-line-string-input-type.component';
import { ComboboxInputTypeComponent } from './input-types/combobox-input-type/combobox-input-type.component';
import { CheckboxInputTypeComponent } from './input-types/checkbox-input-type/checkbox-input-type.component';
import { MultipleSelectComboboxInputTypeComponent } from './input-types/multiple-select-combobox-input-type/multiple-select-combobox-input-type.component';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PasswordInputWithShowButtonComponent } from './password-input-with-show-button/password-input-with-show-button.component';
import { KeyValueListManagerComponent } from './key-value-list-manager/key-value-list-manager.component';
import { SubHeaderComponent } from '@app/shared/common/sub-header/sub-header.component';
import { HijriGregorianDatepickerComponent } from '@app/shared/common/hijri-gregorian-datepicker/hijri-gregorian-datepicker.component';
import { HijriDatepickerComponent } from '@app/shared/common/hijri-gregorian-datepicker/hijri-datepicker/hijri-datepicker.component';
import { NgbModule } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { RequiredDocumentFormChildComponent } from './required-document-form-child/required-document-form-child.component';
import { FileUploadModule } from 'primeng/fileupload';
import { DriverSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/drivers-submitted-documents/driver-submited-documents-list/driver-submited-documents-list.component';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { ViewDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/view-documentFile-modal.component';
import { RejectionReasonModalComponent } from '@app/main/documentFiles/documentFiles/rejectionReason-modal.component';
import { ViewRejectionReasonModalComponent } from '@app/admin/required-document-files/view-rejection-reason-modal.component';
import { TruckSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/trucks-submitted-documents/truck-submited-documents-list/truck-submited-documents-list.component';
import { ChartModule } from '@node_modules/primeng/chart';
import { NgApexchartsModule } from '@node_modules/ng-apexcharts';
import { WidgetsModule } from '@app/shared/common/customizable-dashboard/widgets/widgets.module';
import { EntityLogComponent } from './entity-log/entity-log.component';
import { CollapseModule } from '@node_modules/ngx-bootstrap/collapse';
import { PriceSARComponent } from './price-sar/price-sar.component';
import { FileViwerComponent } from './file-viwer/file-viwer.component';
import { PdfJsViewerModule } from '@node_modules/ng2-pdfjs-viewer';
import { CreateOrEditWorkingHoursComponent } from './workingHours/create-or-edit-working-hours/create-or-edit-working-hours.component';
import { ViewWorkingHoursComponent } from './workingHours/view-working-hours/view-working-hours.component';

// devextreme imports
import { DxDataGridModule } from '@node_modules/devextreme-angular/ui/data-grid';
import { DxPopoverModule } from '@node_modules/devextreme-angular/ui/popover';
import { DxValidationGroupModule } from '@node_modules/devextreme-angular/ui/validation-group';
import { DxValidatorModule } from '@node_modules/devextreme-angular/ui/validator';
import { DxValidationSummaryModule } from '@node_modules/devextreme-angular/ui/validation-summary';
import { DxDateBoxModule } from '@node_modules/devextreme-angular/ui/date-box';
import { DxTextBoxModule } from '@node_modules/devextreme-angular/ui/text-box';
import { DxFileUploaderModule } from '@node_modules/devextreme-angular/ui/file-uploader';
import { DxCheckBoxModule } from '@node_modules/devextreme-angular/ui/check-box';
import { DxNumberBoxModule } from '@node_modules/devextreme-angular/ui/number-box';
import { DxLoadPanelModule } from '@node_modules/devextreme-angular/ui/load-panel';
import { DxButtonModule } from '@node_modules/devextreme-angular/ui/button';
import { DxTreeListModule } from '@node_modules/devextreme-angular/ui/tree-list';
import { DxSelectBoxModule } from '@node_modules/devextreme-angular/ui/select-box';
import { DxDropDownBoxModule } from '@node_modules/devextreme-angular/ui/drop-down-box';
import { DxSchedulerModule } from '@node_modules/devextreme-angular/ui/scheduler';
import { DxPopupModule } from '@node_modules/devextreme-angular/ui/popup';
import { DxScrollViewModule } from '@node_modules/devextreme-angular/ui/scroll-view';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ModalModule.forRoot(),
    UtilsModule,
    TACHYONCommonModule,
    TableModule,
    PaginatorModule,
    GridsterModule,
    TabsModule.forRoot(),
    BsDropdownModule.forRoot(),
    NgxChartsModule,
    BsDatepickerModule.forRoot(),
    PerfectScrollbarModule,
    CountoModule,
    AppBsModalModule,
    AutoCompleteModule,
    NgbModule,
    FileUploadModule,
    ChartModule,
    NgApexchartsModule,
    WidgetsModule,
    CollapseModule,
    PdfJsViewerModule,
    DxDataGridModule,
    DxPopoverModule,
    DxValidationGroupModule,
    DxValidatorModule,
    DxValidationSummaryModule,
    DxDateBoxModule,
    DxTextBoxModule,
    DxFileUploaderModule,
    DxCheckBoxModule,
    DxNumberBoxModule,
    DxLoadPanelModule,
    DxButtonModule,
    DxTreeListModule,
    DxSelectBoxModule,
    DxDropDownBoxModule,
    DxSchedulerModule,
    DxPopupModule,
    DxScrollViewModule,
  ],
  declarations: [
    TimeZoneComboComponent,
    CommonLookupModalComponent,
    EntityTypeHistoryModalComponent,
    EntityChangeDetailModalComponent,
    DateRangePickerInitialValueSetterDirective,
    DatePickerInitialValueSetterDirective,
    CustomizableDashboardComponent,
    WidgetGeneralStatsComponent,
    WidgetDailySalesComponent,
    WidgetEditionStatisticsComponent,
    WidgetHostTopStatsComponent,
    WidgetIncomeStatisticsComponent,
    WidgetMemberActivityComponent,
    WidgetProfitShareComponent,
    WidgetRecentTenantsComponent,
    WidgetRegionalStatsComponent,
    WidgetSalesSummaryComponent,
    WidgetSubscriptionExpiringTenantsComponent,
    WidgetTopStatsComponent,
    FilterDateRangePickerComponent,
    AddWidgetModalComponent,
    SingleLineStringInputTypeComponent,
    ComboboxInputTypeComponent,
    CheckboxInputTypeComponent,
    MultipleSelectComboboxInputTypeComponent,
    PasswordInputWithShowButtonComponent,
    KeyValueListManagerComponent,
    SubHeaderComponent,
    HijriGregorianDatepickerComponent,
    HijriDatepickerComponent,
    RequiredDocumentFormChildComponent,
    DriverSubmitedDocumentsListComponent,
    CreateOrEditDocumentFileModalComponent,
    ViewDocumentFileModalComponent,
    RejectionReasonModalComponent,
    ViewRejectionReasonModalComponent,
    TruckSubmitedDocumentsListComponent,
    EntityLogComponent,
    PriceSARComponent,
    FileViwerComponent,
    CreateOrEditWorkingHoursComponent,
    ViewWorkingHoursComponent,
  ],
  exports: [
    TimeZoneComboComponent,
    CommonLookupModalComponent,
    EntityTypeHistoryModalComponent,
    EntityChangeDetailModalComponent,
    DateRangePickerInitialValueSetterDirective,
    DatePickerInitialValueSetterDirective,
    CustomizableDashboardComponent,
    NgxChartsModule,
    PasswordInputWithShowButtonComponent,
    KeyValueListManagerComponent,
    SubHeaderComponent,
    HijriGregorianDatepickerComponent,
    RequiredDocumentFormChildComponent,
    DriverSubmitedDocumentsListComponent,
    CreateOrEditDocumentFileModalComponent,
    ViewDocumentFileModalComponent,
    RejectionReasonModalComponent,
    ViewRejectionReasonModalComponent,
    TruckSubmitedDocumentsListComponent,
    EntityLogComponent,
    EntityLogComponent,
    PriceSARComponent,
    FileViwerComponent,
    CreateOrEditWorkingHoursComponent,
    ViewWorkingHoursComponent,
  ],
  providers: [
    DateTimeService,
    AppLocalizationService,
    AppNavigationService,
    DashboardViewConfigurationService,
    { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
    { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
    { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
  ],

  entryComponents: [
    WidgetGeneralStatsComponent,
    WidgetDailySalesComponent,
    WidgetEditionStatisticsComponent,
    WidgetHostTopStatsComponent,
    WidgetIncomeStatisticsComponent,
    WidgetMemberActivityComponent,
    WidgetProfitShareComponent,
    WidgetRecentTenantsComponent,
    WidgetRegionalStatsComponent,
    WidgetSalesSummaryComponent,
    WidgetSubscriptionExpiringTenantsComponent,
    WidgetTopStatsComponent,
    FilterDateRangePickerComponent,
    SingleLineStringInputTypeComponent,
    ComboboxInputTypeComponent,
    CheckboxInputTypeComponent,
    MultipleSelectComboboxInputTypeComponent,
  ],
})
export class AppCommonModule {
  static forRoot(): ModuleWithProviders<AppCommonModule> {
    return {
      ngModule: AppCommonModule,
      providers: [AppAuthService, AppRouteGuard],
    };
  }
}
