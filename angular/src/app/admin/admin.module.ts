import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { RedemptionCodesComponent } from './redemption/redemptionCodes/redemptionCodes.component';
import { ViewRedemptionCodeModalComponent } from './redemption/redemptionCodes/view-redemptionCode-modal.component';
import { CreateOrEditRedemptionCodeModalComponent } from './redemption/redemptionCodes/create-or-edit-redemptionCode-modal.component';

import { RedeemCodesComponent } from './redemption/redeemCodes/redeemCodes.component';
import { ViewRedeemCodeModalComponent } from './redemption/redeemCodes/view-redeemCode-modal.component';
import { CreateOrEditRedeemCodeModalComponent } from './redemption/redeemCodes/create-or-edit-redeemCode-modal.component';

import { BayanIntegrationResultsComponent } from './bayanIntegration/bayanIntegrationResults/bayanIntegrationResults.component';
import { ViewBayanIntegrationResultModalComponent } from './bayanIntegration/bayanIntegrationResults/view-bayanIntegrationResult-modal.component';
import { CreateOrEditBayanIntegrationResultModalComponent } from './bayanIntegration/bayanIntegrationResults/create-or-edit-bayanIntegrationResult-modal.component';
import { BayanIntegrationResultShippingRequestTripLookupTableModalComponent } from './bayanIntegration/bayanIntegrationResults/bayanIntegrationResult-shippingRequestTrip-lookup-table-modal.component';

import { RegionsComponent } from './regions/regions/regions.component';
import { ViewRegionModalComponent } from './regions/regions/view-region-modal.component';
import { CreateOrEditRegionModalComponent } from './regions/regions/create-or-edit-region-modal.component';

import { VasesComponent } from './vases/vases/vases.component';
import { ViewVasModalComponent } from './vases/vases/view-vas-modal.component';
import { CreateOrEditVasModalComponent } from './vases/vases/create-or-edit-vas-modal.component';

import { TermAndConditionTranslationsComponent } from './termsAndConditions/termAndConditionTranslations/termAndConditionTranslations.component';
import { ViewTermAndConditionTranslationModalComponent } from './termsAndConditions/termAndConditionTranslations/view-termAndConditionTranslation-modal.component';
import { CreateOrEditTermAndConditionTranslationModalComponent } from './termsAndConditions/termAndConditionTranslations/create-or-edit-termAndConditionTranslation-modal.component';

import { ShippingRequestStatusesComponent } from './shippingRequestStatuses/shippingRequestStatuses/shippingRequestStatuses.component';
import { ViewShippingRequestStatusModalComponent } from './shippingRequestStatuses/shippingRequestStatuses/view-shippingRequestStatus-modal.component';
import { CreateOrEditShippingRequestStatusModalComponent } from './shippingRequestStatuses/shippingRequestStatuses/create-or-edit-shippingRequestStatus-modal.component';

import { UnitOfMeasuresComponent } from './unitOfMeasures/unitOfMeasures/unitOfMeasures.component';
import { ViewUnitOfMeasureModalComponent } from './unitOfMeasures/unitOfMeasures/view-unitOfMeasure-modal.component';
import { CreateOrEditUnitOfMeasureModalComponent } from './unitOfMeasures/unitOfMeasures/create-or-edit-unitOfMeasure-modal.component';

import { TruckStatusesComponent } from './trucks/truckStatuses/truckStatuses.component';
import { ViewTruckStatusModalComponent } from './trucks/truckStatuses/view-truckStatus-modal.component';
import { CreateOrEditTruckStatusModalComponent } from './trucks/truckStatuses/create-or-edit-truckStatus-modal.component';

import { UtilsModule } from '@shared/utils/utils.module';
import { AddMemberModalComponent } from 'app/admin/organization-units/add-member-modal.component';
import { AddRoleModalComponent } from 'app/admin/organization-units/add-role-modal.component';
import { FileUploadModule } from 'ng2-file-upload';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { EditorModule } from 'primeng/editor';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { InputMaskModule } from 'primeng/inputmask';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TreeModule } from 'primeng/tree';
import { DragDropModule } from 'primeng/dragdrop';
import { TreeDragDropService } from 'primeng/api';
import { ContextMenuModule } from 'primeng/contextmenu';
import { AdminRoutingModule } from './admin-routing.module';
import { AuditLogDetailModalComponent } from './audit-logs/audit-log-detail-modal.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { HostDashboardComponent } from './dashboard/host-dashboard.component';
import { DemoUiComponentsComponent } from './demo-ui-components/demo-ui-components.component';
import { DemoUiDateTimeComponent } from './demo-ui-components/demo-ui-date-time.component';
import { DemoUiEditorComponent } from './demo-ui-components/demo-ui-editor.component';
import { DemoUiFileUploadComponent } from './demo-ui-components/demo-ui-file-upload.component';
import { DemoUiInputMaskComponent } from './demo-ui-components/demo-ui-input-mask.component';
import { DemoUiSelectionComponent } from './demo-ui-components/demo-ui-selection.component';
import { CreateEditionModalComponent } from './editions/create-edition-modal.component';
import { EditEditionModalComponent } from './editions/edit-edition-modal.component';
import { MoveTenantsToAnotherEditionModalComponent } from './editions/move-tenants-to-another-edition-modal.component';
import { EditionsComponent } from './editions/editions.component';
import { InstallComponent } from './install/install.component';
import { CreateOrEditLanguageModalComponent } from './languages/create-or-edit-language-modal.component';
import { EditTextModalComponent } from './languages/edit-text-modal.component';
import { LanguageTextsComponent } from './languages/language-texts.component';
import { LanguagesComponent } from './languages/languages.component';
import { MaintenanceComponent } from './maintenance/maintenance.component';
import { CreateOrEditUnitModalComponent } from './organization-units/create-or-edit-unit-modal.component';
import { OrganizationTreeComponent } from './organization-units/organization-tree.component';
import { OrganizationUnitMembersComponent } from './organization-units/organization-unit-members.component';
import { OrganizationUnitRolesComponent } from './organization-units/organization-unit-roles.component';
import { OrganizationUnitsComponent } from './organization-units/organization-units.component';
import { CreateOrEditRoleModalComponent } from './roles/create-or-edit-role-modal.component';
import { RolesComponent } from './roles/roles.component';
import { HostSettingsComponent } from './settings/host-settings.component';
import { TenantSettingsComponent } from './settings/tenant-settings.component';
import { EditionComboComponent } from './shared/edition-combo.component';
import { FeatureTreeComponent } from './shared/feature-tree.component';
import { OrganizationUnitsTreeComponent } from './shared/organization-unit-tree.component';
import { PermissionComboComponent } from './shared/permission-combo.component';
import { PermissionTreeComponent } from './shared/permission-tree.component';
import { RoleComboComponent } from './shared/role-combo.component';
import { InvoiceComponent } from './subscription-management/invoice/invoice.component';
import { SubscriptionManagementComponent } from './subscription-management/subscription-management.component';
import { CreateTenantModalComponent } from './tenants/create-tenant-modal.component';
import { EditTenantModalComponent } from './tenants/edit-tenant-modal.component';
import { TenantFeaturesModalComponent } from './tenants/tenant-features-modal.component';
import { TenantsComponent } from './tenants/tenants.component';
import { TenantCarriersModel } from './tenants/tenantcarriers/tenant-carriers-model.component';
import { CreateTenantCarriersModel } from './tenants/tenantcarriers/create-tenant-carriers-model.component';
import { UiCustomizationComponent } from './ui-customization/ui-customization.component';
import { DefaultThemeUiSettingsComponent } from './ui-customization/default-theme-ui-settings.component';
import { Theme2ThemeUiSettingsComponent } from './ui-customization/theme2-theme-ui-settings.component';
import { Theme3ThemeUiSettingsComponent } from './ui-customization/theme3-theme-ui-settings.component';
import { Theme4ThemeUiSettingsComponent } from './ui-customization/theme4-theme-ui-settings.component';
import { Theme5ThemeUiSettingsComponent } from './ui-customization/theme5-theme-ui-settings.component';
import { Theme6ThemeUiSettingsComponent } from './ui-customization/theme6-theme-ui-settings.component';
import { Theme7ThemeUiSettingsComponent } from './ui-customization/theme7-theme-ui-settings.component';
import { Theme8ThemeUiSettingsComponent } from './ui-customization/theme8-theme-ui-settings.component';
import { Theme9ThemeUiSettingsComponent } from './ui-customization/theme9-theme-ui-settings.component';
import { Theme10ThemeUiSettingsComponent } from './ui-customization/theme10-theme-ui-settings.component';
import { Theme11ThemeUiSettingsComponent } from './ui-customization/theme11-theme-ui-settings.component';
import { CreateOrEditUserModalComponent } from './users/create-or-edit-user-modal.component';
import { EditUserPermissionsModalComponent } from './users/edit-user-permissions-modal.component';
import { ImpersonationService } from './users/impersonation.service';
import { UsersComponent } from './users/users.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { CountoModule } from 'angular2-counto';
import { TextMaskModule } from 'angular2-text-mask';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { DropdownModule } from 'primeng/dropdown';

// Metronic
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { PermissionTreeModalComponent } from './shared/permission-tree-modal.component';
import { WebhookSubscriptionComponent } from './webhook-subscription/webhook-subscription.component';
import { CreateOrEditWebhookSubscriptionModalComponent } from './webhook-subscription/create-or-edit-webhook-subscription-modal.component';
import { WebhookSubscriptionDetailComponent } from './webhook-subscription/webhook-subscription-detail.component';
import { WebhookEventDetailComponent } from './webhook-subscription/webhook-event-detail.component';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';
import { DynamicParameterComponent } from './dynamic-entity-parameters/dynamic-parameter/dynamic-parameter.component';
import { CreateOrEditDynamicParameterModalComponent } from './dynamic-entity-parameters/dynamic-parameter/create-or-edit-dynamic-parameter-modal.component';
import { DynamicParameterDetailComponent } from './dynamic-entity-parameters/dynamic-parameter/dynamic-parameter-detail.component';
import { DynamicParameterValueComponent } from './dynamic-entity-parameters/dynamic-parameter/dynamic-parameter-value/dynamic-parameter-value.component';
import { CreateOrEditDynamicParameterValueModalComponent } from './dynamic-entity-parameters/dynamic-parameter/dynamic-parameter-value/create-or-edit-dynamic-parameter-value-modal.component';
import { EntityDynamicParameterComponent } from './dynamic-entity-parameters/entity-dynamic-parameter/entity-dynamic-parameter.component';
import { CreateEntityDynamicParameterModalComponent } from './dynamic-entity-parameters/entity-dynamic-parameter/create-entity-dynamic-parameter-modal.component';
import { EntityDynamicParameterValueComponent } from './dynamic-entity-parameters/entity-dynamic-parameter/entity-dynamic-parameter-value/entity-dynamic-parameter-value.component';
import { ManageEntityDynamicParameterValuesModalComponent } from './dynamic-entity-parameters/entity-dynamic-parameter/entity-dynamic-parameter-value/manage-entity-dynamic-parameter-values-modal.component';
import { EntityDynamicParameterValueManagerComponent } from './dynamic-entity-parameters/entity-dynamic-parameter/entity-dynamic-parameter-value/entity-dynamic-parameter-value-manager/entity-dynamic-parameter-value-manager.component';
import { RequiredDocumentFilesComponent } from './required-document-files/required-document-files.component';
import { NgbModule } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DriversComponent } from './users/drivers/drivers.component';
import { CreateOrEditDriverModalComponent } from '@app/admin/users/drivers/create-or-edit-driver-modal.component';
import { ViewOrEditEntityDocumentsModalComponent } from '@app/main/documentFiles/documentFiles/documentFilesViewComponents/view-or-edit-entity-documents-modal.componant';
import { waybillsComponent } from './waybills/waybills';
import { NotRequiredDocumentFilesComponent } from '@app/admin/not-required-document-files/not-required-document-files.component';
import { TruckStatusesTranslationsTemplateComponent } from './trucks/truckStatuses/truck-statuses-translations-template/truck-statuses-translations-template.component';
import { VasesTranslationsTemplateComponent } from './vases/vases/vases-translations-template/vases-translations-template.component';
import { RatingModule } from '@node_modules/primeng/rating';
import { DriverTrackingModalComponent } from './users/drivers/driver-tracking-modal/driver-tracking-modal.component';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { UnitOfMeasureTranslationsComponent } from './unitOfMeasures/unitOfMeasures/UnitOfMeasureTranslations/unit-of-measure-translations/unit-of-measure-translations.component';
// import { ActorInvoiceDemandModelComponent } from './actors/Actor-Invoices-ondemand-model.component';
import { MultiSelectModule } from '@node_modules/primeng/multiselect';
import { DriverFilterModalComponent } from '@app/admin/users/drivers/driver-filter/driver-filter-modal.component';

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
import { DriversCommissionComponent } from '@app/admin/users/drivers/driver-commisions/driver-Commission.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  // suppressScrollX: true
};

@NgModule({
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    FileUploadModule,
    ModalModule.forRoot(),
    TabsModule.forRoot(),
    TooltipModule.forRoot(),
    PopoverModule.forRoot(),
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    AdminRoutingModule,
    UtilsModule,
    AppCommonModule,
    TableModule,
    TreeModule,
    DragDropModule,
    ContextMenuModule,
    PaginatorModule,
    PrimeNgFileUploadModule,
    AutoCompleteModule,
    EditorModule,
    InputMaskModule,
    NgxChartsModule,
    CountoModule,
    TextMaskModule,
    ImageCropperModule,
    PerfectScrollbarModule,
    DropdownModule,
    AppBsModalModule,
    NgbModule,
    RatingModule,
    AgmCoreModule,
    MultiSelectModule,
    AgmCoreModule.forRoot({
      apiKey: 'dummy',
      libraries: ['places'],
    }),
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
		RedemptionCodesComponent,

		ViewRedemptionCodeModalComponent,
		CreateOrEditRedemptionCodeModalComponent,
		RedeemCodesComponent,

		ViewRedeemCodeModalComponent,
		CreateOrEditRedeemCodeModalComponent,
    BayanIntegrationResultsComponent,

    ViewBayanIntegrationResultModalComponent,
    CreateOrEditBayanIntegrationResultModalComponent,
    BayanIntegrationResultShippingRequestTripLookupTableModalComponent,
    RegionsComponent,

    ViewRegionModalComponent,
    CreateOrEditRegionModalComponent,
    VasesComponent,
    waybillsComponent,
    ViewVasModalComponent,
    CreateOrEditVasModalComponent,
    TermAndConditionTranslationsComponent,

    ViewTermAndConditionTranslationModalComponent,
    CreateOrEditTermAndConditionTranslationModalComponent,
    ViewOrEditEntityDocumentsModalComponent,
    ShippingRequestStatusesComponent,
    ViewShippingRequestStatusModalComponent,
    CreateOrEditShippingRequestStatusModalComponent,
    UnitOfMeasuresComponent,

    ViewUnitOfMeasureModalComponent,
    CreateOrEditUnitOfMeasureModalComponent,
    TruckStatusesComponent,

    ViewTruckStatusModalComponent,
    CreateOrEditTruckStatusModalComponent,
    UsersComponent,
    PermissionComboComponent,
    RoleComboComponent,
    CreateOrEditUserModalComponent,
    EditUserPermissionsModalComponent,
    PermissionTreeComponent,
    FeatureTreeComponent,
    OrganizationUnitsTreeComponent,
    RolesComponent,
    CreateOrEditRoleModalComponent,
    AuditLogsComponent,
    AuditLogDetailModalComponent,
    HostSettingsComponent,
    InstallComponent,
    MaintenanceComponent,
    EditionsComponent,
    CreateEditionModalComponent,
    EditEditionModalComponent,
    MoveTenantsToAnotherEditionModalComponent,
    LanguagesComponent,
    LanguageTextsComponent,
    CreateOrEditLanguageModalComponent,
    TenantsComponent,
    TenantCarriersModel,
    CreateTenantCarriersModel,
    CreateTenantModalComponent,
    EditTenantModalComponent,
    TenantFeaturesModalComponent,
    CreateOrEditLanguageModalComponent,
    EditTextModalComponent,
    OrganizationUnitsComponent,
    OrganizationTreeComponent,
    OrganizationUnitMembersComponent,
    OrganizationUnitRolesComponent,
    CreateOrEditUnitModalComponent,
    TenantSettingsComponent,
    HostDashboardComponent,
    EditionComboComponent,
    InvoiceComponent,
    SubscriptionManagementComponent,
    AddMemberModalComponent,
    AddRoleModalComponent,
    DemoUiComponentsComponent,
    DemoUiDateTimeComponent,
    DemoUiSelectionComponent,
    DemoUiFileUploadComponent,
    DemoUiInputMaskComponent,
    DemoUiEditorComponent,
    UiCustomizationComponent,
    DefaultThemeUiSettingsComponent,
    Theme2ThemeUiSettingsComponent,
    Theme3ThemeUiSettingsComponent,
    Theme4ThemeUiSettingsComponent,
    Theme5ThemeUiSettingsComponent,
    Theme6ThemeUiSettingsComponent,
    Theme7ThemeUiSettingsComponent,
    Theme8ThemeUiSettingsComponent,
    Theme9ThemeUiSettingsComponent,
    Theme10ThemeUiSettingsComponent,
    Theme11ThemeUiSettingsComponent,
    PermissionTreeModalComponent,
    WebhookSubscriptionComponent,
    CreateOrEditWebhookSubscriptionModalComponent,
    WebhookSubscriptionDetailComponent,
    WebhookEventDetailComponent,
    DynamicParameterComponent,
    CreateOrEditDynamicParameterModalComponent,
    DynamicParameterDetailComponent,
    DynamicParameterValueComponent,
    CreateOrEditDynamicParameterValueModalComponent,
    EntityDynamicParameterComponent,
    CreateEntityDynamicParameterModalComponent,
    EntityDynamicParameterValueComponent,
    ManageEntityDynamicParameterValuesModalComponent,
    EntityDynamicParameterValueManagerComponent,
    RequiredDocumentFilesComponent,
    NotRequiredDocumentFilesComponent,
    DriversComponent,
    CreateOrEditDriverModalComponent,
    VasesTranslationsTemplateComponent,
    VasesTranslationsTemplateComponent,
    TruckStatusesTranslationsTemplateComponent,
    DriverTrackingModalComponent,
    UnitOfMeasureTranslationsComponent,
    // ActorInvoiceDemandModelComponent
    DriverFilterModalComponent,
    DriversCommissionComponent,
  ],

  exports: [AddMemberModalComponent, AddRoleModalComponent, ViewOrEditEntityDocumentsModalComponent],
  providers: [
    ImpersonationService,
    TreeDragDropService,
    { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
    { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
    { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
    { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG },
  ],
})
export class AdminModule {}
