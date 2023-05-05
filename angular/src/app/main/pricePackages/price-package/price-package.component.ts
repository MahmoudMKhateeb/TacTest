import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import {PricePackageServiceProxy, ShippingRequestRouteType} from '@shared/service-proxies/service-proxies';
import {EnumToArrayPipe} from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-price-package',
  templateUrl: './price-package.component.html',
  styleUrls: ['./price-package.component.css'],
})
export class PricePackageComponent extends AppComponentBase implements OnInit {
  dataSource: any;
  routeTypes = this._enumToArrayPipe.transform(ShippingRequestRouteType);

  constructor(private injector: Injector, private _pricePackageProxy: PricePackageServiceProxy, private _enumToArrayPipe: EnumToArrayPipe) {
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

    getRouteTypeTitle(routeType): string {
        let routeTypeTitle = this.routeTypes.find((item) => item.key === routeType?.toString())?.value;

        return routeTypeTitle ?? '';
    }
}
