import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CommonLookupServiceProxy, ISelectItemDto, PenaltiesServiceProxy } from '@shared/service-proxies/service-proxies';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  selector: 'app-penalties-list',
  templateUrl: './penalties-list.component.html',
})
export class PenaltiesListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  Tenants: ISelectItemDto[];
  dataSource: any = {};
  advancedFiltersAreShown = false;
  constructor(injector: Injector, private _PenaltiesServiceProxy: PenaltiesServiceProxy, private _CommonServ: CommonLookupServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    if (this.appSession.tenantId) {
      this.advancedFiltersAreShown = true;
    }
    this.getAllInvoices();
  }
  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, '').subscribe((result) => {
      this.Tenants = result;
    });
  }
  reloadPage(): void {
    this.refreshDataGrid();
  }
  getAllInvoices() {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._PenaltiesServiceProxy
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
  refreshDataGrid() {
    this.dataGrid.instance
      .refresh()
      .then(function () {
        // ...
      })
      .catch(function (error) {
        // ...
      });
  }
}
