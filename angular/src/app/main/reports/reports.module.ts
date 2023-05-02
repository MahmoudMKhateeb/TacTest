import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportDesingerComponent } from './report-desinger/report-desinger.component';
import { ReportsRoutingModule } from '@app/main/reports/reports-routing.module';
import {DxReportDesignerModule} from '@node_modules/devexpress-reporting-angular';

@NgModule({
  declarations: [ReportDesingerComponent],
  imports: [CommonModule, ReportsRoutingModule, DxReportDesignerModule],
})
export class ReportsModule {}
