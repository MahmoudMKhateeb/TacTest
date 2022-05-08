import { Component, Injector, Input, OnInit } from '@angular/core';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TrucksTypesServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'trucksType-translationsTemplate',
  templateUrl: './trucks-type-translation-template.component.html',
  // styleUrls: ['./trucks-type-translation-template.component.css'],
})
export class TrucksTypeTranslationTemplateComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _trucksTypesServiceProxy: TrucksTypesServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.DxGetAll();
  }

  DxGetAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._trucksTypesServiceProxy
          .getAllTranslations(self.CoreId, JSON.stringify(loadOptions))
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
        return self._trucksTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._trucksTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
