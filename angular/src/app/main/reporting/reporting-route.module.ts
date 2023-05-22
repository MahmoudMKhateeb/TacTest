import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { CreateReportTypeComponent } from '@app/main/reporting/host-reporting/report-types/create-report-type/create-report-type.component';
import { AllReportTypesComponent } from '@app/main/reporting/host-reporting/report-types/all-report-types/all-report-types.component';
import { GenerateReportByCompanyComponent } from '@app/main/reporting/tenant-reports/generate-report-by-company/generate-report-by-company.component';
import { TenantAllReportComponent } from '@app/main/reporting/tenant-reports/all-report/all-report.component';
import { TenantMyAutomatedReportsComponent } from '@app/main/reporting/tenant-reports/my-automated-reports/my-automated-reports.component';

const routes: Routes = [
  {
    path: 'create-report-type',
    component: CreateReportTypeComponent,
    data: {
      permission: '',
    },
  },
  {
    path: 'report-types',
    component: AllReportTypesComponent,
    data: {
      permission: '',
    },
  },
  {
    path: 'generate-report',
    component: GenerateReportByCompanyComponent,
    data: {
      permission: '',
    },
  },
  {
    path: 'all-reports',
    component: TenantAllReportComponent,
    data: {
      permission: '',
    },
  },
  {
    path: 'my-automated-reports',
    component: TenantMyAutomatedReportsComponent,
    data: {
      permission: '',
    },
  },
  { path: '', redirectTo: '/app/main/page-not-found', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportingRouteModule {}
