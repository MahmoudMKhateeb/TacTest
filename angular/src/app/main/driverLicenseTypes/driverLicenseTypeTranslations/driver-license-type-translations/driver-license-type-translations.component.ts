import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DriverLicenseTypesServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  selector: 'app-driver-license-type-translations',
  templateUrl: './driver-license-type-translations.component.html',
  styleUrls: ['./driver-license-type-translations.component.css'],
})
export class DriverLicenseTypeTranslationsComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _driverLicenseTypesServiceProxy: DriverLicenseTypesServiceProxy) {
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
        return self._driverLicenseTypesServiceProxy
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
        return self._driverLicenseTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      update: (key, values) => {
        values = { ...values, ...{ id: key } };
        return self._driverLicenseTypesServiceProxy.createOrEditTranslation(values).toPromise();
      },
      remove: (key) => {
        return self._driverLicenseTypesServiceProxy.deleteTranslation(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
