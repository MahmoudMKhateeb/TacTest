import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PackingTypesServiceProxy, ShippingTypesServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './shippingTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class ShippingTypesComponent extends AppComponentBase implements OnInit {
  advancedFiltersAreShown = false;
  filterText = '';
  dataSource: any = {};

  constructor(
    injector: Injector,
    private _serviceProxy: ShippingTypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
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
          .getAll(JSON.stringify(loadOptions))
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
        return self._serviceProxy.createOrEdit(values).toPromise();
      },
      update: (key, values) => {
        return self._serviceProxy.createOrEdit(values).toPromise();
      },
      remove: (key) => {
        return self._serviceProxy.delete(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
