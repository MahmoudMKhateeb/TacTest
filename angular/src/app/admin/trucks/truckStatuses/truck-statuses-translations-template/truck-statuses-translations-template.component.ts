import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TruckStatusesServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-truck-statuses-translations-template',
  templateUrl: './truck-statuses-translations-template.component.html',
  styleUrls: ['./truck-statuses-translations-template.component.css'],
})
export class TruckStatusesTranslationsTemplateComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _serviceProxy: TruckStatusesServiceProxy) {
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
        return self._serviceProxy
          .getAllTranslations(JSON.stringify(loadOptions), self.CoreId)
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
        return self._serviceProxy.createOrEditTranslation(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._serviceProxy.createOrEditTranslation(values).toPromise();
      },
      remove: (key) => {
        return self._serviceProxy.deleteTranslation(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
