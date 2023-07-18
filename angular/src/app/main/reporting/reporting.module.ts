import { NgModule } from '@angular/core';
import { UtilsModule } from '@shared/utils/utils.module';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
// import { TabsModule } from 'ngx-bootstrap/tabs';
// import { TooltipModule } from 'ngx-bootstrap/tooltip';
// import { PopoverModule } from 'ngx-bootstrap/popover';

import { DxDataGridModule } from '@node_modules/devextreme-angular/ui/data-grid';
import { DxPopoverModule } from '@node_modules/devextreme-angular/ui/popover';
import { DxPopupModule } from '@node_modules/devextreme-angular/ui/popup';
import { DxValidationGroupModule } from '@node_modules/devextreme-angular/ui/validation-group';
import { DxDateBoxModule } from '@node_modules/devextreme-angular/ui/date-box';
import { DxValidatorModule } from '@node_modules/devextreme-angular/ui/validator';
import { DxValidationSummaryModule } from '@node_modules/devextreme-angular/ui/validation-summary';
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
import { DxScrollViewModule } from '@node_modules/devextreme-angular/ui/scroll-view';
import { DxTextAreaModule } from '@node_modules/devextreme-angular/ui/text-area';
import { CreateReportTypeComponent } from '@app/main/reporting/host-reporting/report-types/create-report-type/create-report-type.component';
import { ReportingRouteModule } from '@app/main/reporting/reporting-route.module';
import { AllReportTypesComponent } from '@app/main/reporting/host-reporting/report-types/all-report-types/all-report-types.component';
import { DxReportDesignerModule, DxReportViewerModule } from '@node_modules/devexpress-reporting-angular';
import { GenerateReportByCompanyComponent } from '@app/main/reporting/tenant-reports/generate-report-by-company/generate-report-by-company.component';
import { AutomationSetupModalComponent } from '@app/main/reporting/tenant-reports/generate-report-by-company/automation-setup-modal/automation-setup-modal.component';
import { TenantAllReportComponent } from '@app/main/reporting/tenant-reports/all-report/all-report.component';
import { TenantMyAutomatedReportsComponent } from '@app/main/reporting/tenant-reports/my-automated-reports/my-automated-reports.component';
import { DxListModule } from '@node_modules/devextreme-angular';
import { MultiSelectModule } from '@node_modules/primeng/multiselect';

const imports = [
  UtilsModule,
  AppCommonModule,
  CommonModule,
  FormsModule,
  ModalModule,
  BsDropdownModule,
  ReportingRouteModule,
  MultiSelectModule,
  ReactiveFormsModule,
];

const dxImports = [
  DxDataGridModule,
  DxPopoverModule,
  DxPopupModule,
  DxValidationGroupModule,
  DxDateBoxModule,
  DxValidatorModule,
  DxValidationSummaryModule,
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
  DxScrollViewModule,
  DxTextAreaModule,
  DxReportDesignerModule,
  DxListModule,
];

@NgModule({
  imports: [...imports, ...dxImports, ReportingRouteModule, DxReportDesignerModule, DxListModule, MultiSelectModule, DxReportViewerModule],
  declarations: [
    CreateReportTypeComponent,
    AllReportTypesComponent,
    GenerateReportByCompanyComponent,
    AutomationSetupModalComponent,
    TenantAllReportComponent,
    TenantMyAutomatedReportsComponent,
  ],
  providers: [],
})
export class ReportingModule {}
