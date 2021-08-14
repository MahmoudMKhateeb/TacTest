import { Component, Injector, Input, OnInit } from '@angular/core';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PackingTypesServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-translations-template',
  templateUrl: './translations-template.component.html',
  styleUrls: ['./translations-template.component.css'],
})
export class TranslationsTemplateComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _packingTypesServiceProxy: PackingTypesServiceProxy) {
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
        return self._packingTypesServiceProxy
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
        return self._packingTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._packingTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      remove: (key) => {
        return self._packingTypesServiceProxy.deleteTranslation(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
