import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { CommonLookupServiceProxy, ISelectItemDto, PenaltiesServiceProxy, PenaltyStatus, PenaltyType } from '@shared/service-proxies/service-proxies';
import { DxDataGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';
import { finalize } from 'rxjs/operators';

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
  PenaltyTypeEnum = PenaltyStatus;
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

  Cancel(id: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._PenaltiesServiceProxy
          .cancelPenalty(id)
          .pipe(
            finalize(() => {
              this.refreshDataGrid();
            })
          )
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyCanceld'));
          });
      }
    });
  }

  Confirm(id: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._PenaltiesServiceProxy
          .confirmPenalty(id)
          .pipe(
            finalize(() => {
              this.refreshDataGrid();
            })
          )
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyConfirmed'));
          });
      }
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
