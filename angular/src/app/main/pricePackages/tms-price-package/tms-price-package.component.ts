import { Component, Injector, OnInit, Pipe, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { TmsPricePackageServiceProxy } from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { CreateTmsPricePackageModalComponent } from '@app/main/pricePackages/tms-price-package/create-tms-price-package-modal/create-tms-price-package-modal.component';

@Component({
  selector: 'app-tms-price-package',
  templateUrl: './tms-price-package.component.html',
  styleUrls: ['./tms-price-package.component.css'],
})
export class TmsPricePackageComponent extends AppComponentBase implements OnInit {
  dataSource: any;
  @ViewChild('TmsPricePackageModal', { static: true }) tmsPricePackageModal: CreateTmsPricePackageModalComponent;

  constructor(private injector: Injector, private _tmsPricePackageProxy: TmsPricePackageServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllTmsPricePackages();
  }

  getAllTmsPricePackages() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._tmsPricePackageProxy
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
