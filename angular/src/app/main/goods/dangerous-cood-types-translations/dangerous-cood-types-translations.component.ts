import { Component, Injector, Input, OnInit } from '@angular/core';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DangerousGoodTypesServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-dangerous-cood-types-translations',
  templateUrl: './dangerous-cood-types-translations.component.html',
  styleUrls: ['./dangerous-cood-types-translations.component.css'],
})
export class DangerousCoodTypesTranslationsComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _dangerousGoodTypesServiceProxy: DangerousGoodTypesServiceProxy) {
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
        return self._dangerousGoodTypesServiceProxy
          .getAllTranslation(JSON.stringify(loadOptions), self.CoreId)
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
        return self._dangerousGoodTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._dangerousGoodTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      remove: (key) => {
        return self._dangerousGoodTypesServiceProxy.deleteTranslation(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
