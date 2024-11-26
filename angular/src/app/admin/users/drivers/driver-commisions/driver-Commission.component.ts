import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';
import { TripDriversServiceProxy } from '@shared/service-proxies/service-proxies';
import { DxDataGridComponent } from 'devextreme-angular';

@Component({
  selector: 'app-drivers-Commission',
  templateUrl: './driver-Commission.component.html',
  styleUrls: ['./driver-Commission.component.css'],
})
export class DriversCommissionComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataGrid', { static: false }) dataGrid: DxDataGridComponent;
  dataSource: any = {};

  constructor(injector: Injector, private _tripAppService: TripDriversServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.initializeDataSource();
  }

  initializeDataSource() {
    let self = this;

    this.dataSource = {
      store: new CustomStore({
        key: 'id',
        load(loadOptions: LoadOptions) {
          return self._tripAppService
            .getAllDX(JSON.stringify(loadOptions))
            .toPromise()
            .then((response) => {
              return {
                data: response.data,
                totalCount: response.totalCount,
              };
            })
            .catch((error) => {
              console.error('Data Loading Error', error);
              throw new Error('Data Loading Error');
            });
        },
        update: (key, values) => {
          return self._tripAppService.updateTripDriver(values).toPromise();
        },
      }),
    };
  }

  updateRow(e) {
    e.newData = Object.assign({}, e.oldData, e.newData);
  }

  getMonthYear(rowData) {
    if (!rowData || !rowData.creationTime) {
      return '';
    }
    const date = new Date(rowData.creationTime);
    const month = date.getUTCMonth() + 1; // Months are zero-based in JavaScript
    const year = date.getUTCFullYear();
    return `${year}-${month < 10 ? '0' : ''}${month}`;
  }

  onRowClick(e) {
    // Toggle the expanded state of the group when clicking on a group row
    if (e.rowType === 'group') {
      if (e.isExpanded) {
        this.dataGrid.instance.collapseRow(e.key);
      } else {
        this.dataGrid.instance.expandRow(e.key);
      }
    }
  }
}
