import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
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
import * as moment from 'moment';

@Component({
  templateUrl: './normal-price-package.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class NormalPricePackageComponent extends AppComponentBase {
  @ViewChild('createOrEditNormalPricePackageModal', { static: true })
  createOrEditNormalPricePackageModal: CreateOrEditNormalPricePackageModalComponent;
  @ViewChild('viewNormalPricePackageModalComponent', { static: true }) viewNormalPricePackageModal: ViewNormalPricePackageModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

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

  ngOnInit(): void {}

  getNormalPricePackages(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._normalPricePackagesServiceProxy
      .getAll(
        null,
        this.filterText,
        this.destinationFilter,
        this.originFilter,
        this.truckTypeFilter,
        this.pricePackageIdFilter,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
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
