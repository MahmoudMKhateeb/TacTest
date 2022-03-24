import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UnitOfMeasuresServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  selector: 'app-unit-of-measure-translations',
  templateUrl: './unit-of-measure-translations.component.html',
  styleUrls: ['./unit-of-measure-translations.component.css'],
})
export class UnitOfMeasureTranslationsComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _unitOfMeasuresServiceProxy: UnitOfMeasuresServiceProxy) {
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
        return self._unitOfMeasuresServiceProxy
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
        return self._unitOfMeasuresServiceProxy.createOrEditTranslation(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._unitOfMeasuresServiceProxy.createOrEditTranslation(values).toPromise();
      },
      remove: (key) => {
        return self._unitOfMeasuresServiceProxy.deleteTranslation(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
