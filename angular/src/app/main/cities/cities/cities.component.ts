import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CitiesServiceProxy, CityDto, LoadOptionsInput } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCityModalComponent } from './create-or-edit-city-modal.component';

import { ViewCityModalComponent } from './view-city-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './cities.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class CitiesComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditCityModal', { static: true }) createOrEditCityModal: CreateOrEditCityModalComponent;
  @ViewChild('viewCityModalComponent', { static: true }) viewCityModal: ViewCityModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  codeFilter = '';
  latitudeFilter = '';
  longitudeFilter = '';
  countyDisplayNameFilter = '';

  _entityTypeFullName = 'TACHYON.Cities.City';
  entityHistoryEnabled = false;
  dataSource: any = {};
  countries: any;
  constructor(injector: Injector, private _citiesServiceProxy: CitiesServiceProxy, private _fileDownloadService: FileDownloadService) {
    super(injector);
  }

  ngOnInit(): void {
    this._citiesServiceProxy.getAllCountyForTableDropdown().subscribe((result) => {
      this.countries = result;
    });
    this.getCities();
  }

  getCities() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        let Input = new LoadOptionsInput();
        Input.loadOptions = JSON.stringify(loadOptions);
        return self._citiesServiceProxy
          .dxGetAll(Input)
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
          });
      },
      insert: (values) => {
        return self._citiesServiceProxy.createOrEdit(values).toPromise();
      },
      update: (key, values) => {
        return self._citiesServiceProxy.createOrEdit(values).toPromise();
      },
      remove: (key) => {
        return self._citiesServiceProxy.delete(key).toPromise();
      },
    });
  }

  createCity(): void {
    this.createOrEditCityModal.show();
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }

  exportToExcel(): void {
    this._citiesServiceProxy
      .getCitiesToExcel(
        this.filterText,
        this.displayNameFilter,
        this.codeFilter,
        this.latitudeFilter,
        this.longitudeFilter,
        this.countyDisplayNameFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
