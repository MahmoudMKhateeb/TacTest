import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { ReportFormat, ReportServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-all-report',
  templateUrl: './all-report.component.html',
  styleUrls: ['./all-report.component.scss'],
  animations: [appModuleAnimation()],
})
export class TenantAllReportComponent extends AppComponentBase implements OnInit {
  allReportsDataSource: any = {};

  constructor(
    injector: Injector,
    private _router: Router,
    private _reportService: ReportServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllReports();
  }

  /**
   * send api to get all report types
   */
  getAllReports() {
    let self = this;
    this.allReportsDataSource = {};
    this.allReportsDataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._reportService
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((result) => {
            return {
              data: result.data,
              totalCount: result.totalCount,
              summary: result.summary,
              groupCount: result.groupCount,
            };
          })
          .catch(() => {
            throw new Error('Error when loading data from server');
          });
      },
    });
  }

  deleteReport(id) {
    this._reportService.delete(id).subscribe(() => {
      this.notify.success(this.l('DeletedSuccessfully'));
      this.getAllReports();
    });
  }

  downloadReport(report) {
    console.log('report', report);

    const fileId = report?.generatedFileId;

    if (!isNotNullOrUndefined(fileId)) {
      this.notify.error(this.l('ReportFileNotFound'));
      return;
    }

    this._fileDownloadService.downloadFileByBinary(fileId, report.displayName, this.getContentTypeByFormat(report.format));
  }

  private getContentTypeByFormat(reportFormat: ReportFormat) {
    switch (reportFormat) {
      case ReportFormat.Pdf:
        return 'application/pdf';
      case ReportFormat.Excel:
        return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
      case ReportFormat.Word:
        return 'application/vnd.openxmlformats-officedocument.wordprocessingml.document';
      case ReportFormat.Html:
        return 'text/html';
      case ReportFormat.Image:
        return 'image/png';
    }
  }

  goToCreateNewReportType() {
    this._router.navigate(['app/main/reporting/generate-report']);
  }
}
