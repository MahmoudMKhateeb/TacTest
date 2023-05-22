import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-all-report',
  templateUrl: './all-report.component.html',
  styleUrls: ['./all-report.component.scss'],
  animations: [appModuleAnimation()],
})
export class TenantAllReportComponent extends AppComponentBase implements OnInit {
  allReportsDataSource: any = {};

  constructor(injector: Injector, private _router: Router) {
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
        return new Promise((resolve) => {
          resolve([
            {
              id: 2313,
              reportType: 'ttedsdd',
              reportName: 'amj',
              edition: 'Edition',
              generationDate: '15/05/2023',
              reportFile: 'tttt',
              generationType: 'dddd',
            },
          ]);
        }).then((res) => {
          return {
            data: res,
            totalCount: 1,
          };
        });
        /*self._unitOfMeasuresServiceProxy
                    .getAll(JSON.stringify(loadOptions))
                    .toPromise()
                    .then((response) => {
                        return {
                            data: response.data,
                            totalCount: response.totalCount,
                        };
                    })
                    .catch((error) => {
                        console.log(error);
                        throw new Error('Data Loading Error');
                    });*/
      },
    });
  }

  viewReport(id) {}

  cloneReport(id) {}

  deleteReport(id) {}

  editReport(id) {}

  downloadReport(report) {
    console.log('report', report);
  }
}
