import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportDesingerComponent } from '@app/main/reports/report-desinger/report-desinger.component';

const routes: Routes = [
  {
    path: 'designer',
    component: ReportDesingerComponent,
    //data: { permission: undefined } // todo add permission
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRoutingModule {}
