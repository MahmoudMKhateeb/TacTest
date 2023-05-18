import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { CreateReportTypeComponent } from '@app/main/reporting/report-types/create-report-type/create-report-type.component';
import { AllReportTypesComponent } from '@app/main/reporting/report-types/all-report-types/all-report-types.component';

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
  { path: '', redirectTo: '/app/main/page-not-found', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportingRouteModule {}
