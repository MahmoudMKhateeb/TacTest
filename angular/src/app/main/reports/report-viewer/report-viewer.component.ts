import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { localize } from '@node_modules/@devexpress/analytics-core/property-grid/localization/_localization';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: '',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './report-viewer.component.html',
  styleUrls: [
    './report-viewer.component.css',
    '../../../../../node_modules/jquery-ui/themes/base/all.css',
    '../../../../../node_modules/devextreme/dist/css/dx.common.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
  ],
  animations: [appModuleAnimation()],
})
export class ReportViewerComponent extends AppComponentBase implements OnInit {
  title = 'DXReportViewerSample';
  reportUrl = '';
  hostUrl = 'http://localhost:44301/';
  invokeAction = 'DXXRDV';

  constructor(injector: Injector, private _router: ActivatedRoute) {
    super(injector);
    this.reportUrl = _router.snapshot.queryParams['reportName'];
  }

  ngOnInit(): void {
    this.reportUrl = this._router.snapshot.queryParams['reportName'];
  }
}
