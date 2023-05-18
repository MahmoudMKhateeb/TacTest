import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { DxReportDesignerComponent } from '@node_modules/devexpress-reporting-angular';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ajaxSetup } from '@node_modules/@devexpress/analytics-core/core/internal/ajaxSetup';
import * as ko from 'knockout';

@Component({
  selector: 'app-report-desinger',
  templateUrl: './report-desinger.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    './report-desinger.component.css',
    '../../../../../node_modules/jquery-ui/themes/base/all.css',
    '../../../../../node_modules/devexpress-richedit/dist/dx.richedit.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css',
    '../../../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css',
  ],
})
export class ReportDesingerComponent extends AppComponentBase implements OnInit {
  @ViewChild('designer', { static: false }) designer: DxReportDesignerComponent;

  title = 'DXReportViewerSample';
  getDesignerModelAction = `/DXXRD/GetDesignerModel`;
  hostUrl = 'http://localhost:44301/';
  koReportUrl = ko.observable('');
  get reportUrl() {
    return this.koReportUrl();
  }

  set reportUrl(newUrl) {
    this.koReportUrl(newUrl);
  }

  constructor(private injector: Injector) {
    super(injector);
    ajaxSetup.ajaxSettings = {
      headers: {
        Authorization: 'Bearer ' + abp.auth.getToken(),
      },
    };
  }

  ngOnInit(): void {
    this.reportUrl = 'TripDetailsReport';
  }

  open() {
    this.designer.bindingSender.open(this.reportUrl);
  }
}
