import { Component, Injector, ViewEncapsulation, ViewChild, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CitiesTranslationsServiceProxy, CitiesTranslationDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditCitiesTranslationModalComponent } from './create-or-edit-citiesTranslation-modal.component';

import { ViewCitiesTranslationModalComponent } from './view-citiesTranslation-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  selector: 'cities-translations-template',
  templateUrl: './citiesTranslations.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class CitiesTranslationsComponent extends AppComponentBase {
  @ViewChild('createOrEditCitiesTranslationModal', { static: true }) createOrEditCitiesTranslationModal: CreateOrEditCitiesTranslationModalComponent;
  @ViewChild('viewCitiesTranslationModalComponent', { static: true }) viewCitiesTranslationModal: ViewCitiesTranslationModalComponent;

  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _citiesTranslationsServiceProxy: CitiesTranslationsServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAll();
  }

  getAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._citiesTranslationsServiceProxy
          .getAll(JSON.stringify(loadOptions), self.CoreId)
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
        values = { ...values, ...{ coreId: self.CoreId } };
        return self._citiesTranslationsServiceProxy.createOrEdit(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._citiesTranslationsServiceProxy.createOrEdit(values).toPromise();
      },
      remove: (key) => {
        return self._citiesTranslationsServiceProxy.delete(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
