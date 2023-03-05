import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { PricePackageServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-price-package',
  templateUrl: './price-package.component.html',
  styleUrls: ['./price-package.component.css'],
})
export class PricePackageComponent extends AppComponentBase implements OnInit {
  dataSource: any;

  constructor(private injector: Injector, private _pricePackageProxy: PricePackageServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllPricePackages();
  }

  getAllPricePackages() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._pricePackageProxy
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
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
