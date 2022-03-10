import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { CommonLookupServiceProxy, ISelectItemDto, PenaltiesServiceProxy, PenaltyType } from '@shared/service-proxies/service-proxies';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  selector: 'app-penalties-list',
  templateUrl: './penalties-list.component.html',
  providers: [EnumToArrayPipe],
})
export class PenaltiesListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  Tenants: ISelectItemDto[];
  dataSource: any = {};
  PenaltyType: any;
  advancedFiltersAreShown = false;
  constructor(
    injector: Injector,
    private _PenaltiesServiceProxy: PenaltiesServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
    this.PenaltyType = this.enumToArray.transform(PenaltyType);
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
  StyleStatus(Status: PenaltyType): string {
    switch (Status) {
      case PenaltyType.NotLogged:
        return 'label label-success label-inline m-1';
      case PenaltyType.Delay:
        return 'label label-danger label-inline m-1';
      case PenaltyType.Cancelation:
        return 'label label-info label-inline m-1';
      default:
        return 'label label-default label-inline m-1';
    }
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
