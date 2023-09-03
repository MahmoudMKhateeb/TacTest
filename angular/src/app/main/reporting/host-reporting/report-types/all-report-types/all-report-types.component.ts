import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NavigationExtras, Router } from '@angular/router';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { ReportDefinitionServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-all-report-types',
  templateUrl: './all-report-types.component.html',
  styleUrls: ['./all-report-types.component.scss'],
  animations: [appModuleAnimation()],
})
export class AllReportTypesComponent extends AppComponentBase implements OnInit {
  allReportTypesDataSource: any = {};

  constructor(injector: Injector, private _router: Router, private _reportDefinitionService: ReportDefinitionServiceProxy) {
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
        return self._reportDefinitionService
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  goToCreateNewReportType() {
    this._router.navigate(['app/main/reporting/create-report-type']);
  }

  cloneReportType(id) {
    const data = {
      clonedReportDefinitionId: id,
    };
    this._router.navigate(['app/main/reporting/create-report-type'], { queryParams: data });
  }

  activateReportType(id) {
    this._reportDefinitionService.activate(id).subscribe(
      () => {
        this.notify.success(this.l('ActivatedSuccessfully'));
        this.getAllReportTypes();
      },
      (error) => {
        this.notify.error(this.l('AnErrorOccurred'));
        console.log(error);
      }
    );
  }
  deactivateReportType(id) {
    this._reportDefinitionService.deactivate(id).subscribe(
      () => {
        this.notify.success(this.l('DeactivatedSuccessfully'));
        this.getAllReportTypes();
      },
      (error) => {
        this.notify.error(this.l('AnErrorOccurred'));
        console.log(error);
      }
    );
  }
}
