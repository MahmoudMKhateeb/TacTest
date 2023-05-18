import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportDesingerComponent } from './report-desinger/report-desinger.component';
import { ReportsRoutingModule } from '@app/main/reports/reports-routing.module';
import { ReportViewerComponent } from './report-viewer/report-viewer.component';
import { DxReportDesignerModule, DxReportViewerModule } from '@node_modules/devexpress-reporting-angular';
import { AppCommonModule } from '@app/shared/common/app-common.module';

@NgModule({
  declarations: [ReportDesingerComponent, ReportViewerComponent],
  imports: [CommonModule, ReportsRoutingModule, DxReportViewerModule, AppCommonModule, DxReportDesignerModule],
})
export class ReportsModule {}
