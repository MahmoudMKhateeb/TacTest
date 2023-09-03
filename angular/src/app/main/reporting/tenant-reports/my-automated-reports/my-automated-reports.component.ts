import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-my-automated-reports',
  templateUrl: './my-automated-reports.component.html',
  styleUrls: ['./my-automated-reports.component.scss'],
  animations: [appModuleAnimation()],
})
export class TenantMyAutomatedReportsComponent extends AppComponentBase implements OnInit {
  allAutomatedReportsDataSource: any = {};

  constructor(injector: Injector, private _router: Router) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllReportTypes();
  }

  /**
   * send api to get all report types
   */
  getAllReportTypes() {
    let self = this;
    this.allAutomatedReportsDataSource = {};
    this.allAutomatedReportsDataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return new Promise((resolve) => {
          resolve([
            {
              id: 2313,
              reportType: 'ttedsdd',
              reportName: 'amj',
              roleAccess: 'access',
              generationDate: '19/05/2023',
              format: 'PDF',
              period: 'EveryWeek',
              lastReport: '20/05/2023',
              upcomingReport: '30/05/2023',
              active: true,
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

  duplicateReport(id) {}

  deactivateReport(report) {
    report.active = false;
  }

  deleteReport(id) {}

  editReport(id) {}

  editReportRoleAccess(id) {}

  activateReport(report) {
    report.active = true;
  }
}
