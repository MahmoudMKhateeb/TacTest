import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NormalPricePackagesServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditNormalPricePackageModalComponent } from './create-or-edit-normal-price-package-modal.component';

import { ViewNormalPricePackageModalComponent } from './view-normal-price-package-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import CustomStore from '@node_modules/devextreme/data/custom_store';

@Component({
  templateUrl: './normal-price-package.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class NormalPricePackageComponent extends AppComponentBase {
  @ViewChild('createOrEditNormalPricePackageModal', { static: true })
  createOrEditNormalPricePackageModal: CreateOrEditNormalPricePackageModalComponent;
  @ViewChild('viewNormalPricePackageModalComponent', { static: true }) viewNormalPricePackageModal: ViewNormalPricePackageModalComponent;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  dataSource: any = {};

  advancedFiltersAreShown = false;
  filterText = '';
  destinationFilter = '';
  originFilter = '';
  truckTypeFilter = '';
  pricePackageIdFilter = '';

  constructor(
    injector: Injector,
    private _normalPricePackagesServiceProxy: NormalPricePackagesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getAllNormalPricePackages();
  }
  getAllNormalPricePackages() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._normalPricePackagesServiceProxy
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
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createNormalPricePackage(): void {
    this.createOrEditNormalPricePackageModal.show();
  }

  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
  deleteNormalPricePackage(normalPricePackageId: number): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._normalPricePackagesServiceProxy.delete(normalPricePackageId).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
