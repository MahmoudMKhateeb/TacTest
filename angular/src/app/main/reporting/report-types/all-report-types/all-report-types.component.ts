import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-all-report-types',
  templateUrl: './all-report-types.component.html',
  styleUrls: ['./all-report-types.component.scss'],
  animations: [appModuleAnimation()],
})
export class AllReportTypesComponent extends AppComponentBase implements OnInit {
  allReportTypesDataSource: any = {};

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
    this.allReportTypesDataSource = {};
    this.allReportTypesDataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return new Promise((resolve) => {
          resolve([
            {
              id: 2313,
              reportType: 'ttedsdd',
              reportName: 'amj',
              creationDate: '15/05/2023',
              accessEdition: 'tttt',
              accessException: 'dddd',
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

  goToCreateNewReportType() {
    this._router.navigate(['app/main/reporting/create-report-type']);
  }

  viewReportType(id) {}

  cloneReportType(id) {}

  deleteReportType(id) {}

  editAccessReportType(id) {}

  editReportType(id) {}
}
