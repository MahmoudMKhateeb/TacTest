import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { LazyLoadEvent } from '@node_modules/primeng/api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'profile-carrier-normal-price-package',
  templateUrl: './carrier-normal-price-package.component.html',
})
export class CarrierNormalPricePackagesComponent extends AppComponentBase implements OnInit {
  @Input() giverUserId: number;
  @ViewChild('dataTableNormalPricePackages', { static: true }) dataTableNormalPricePackages: Table;
  @ViewChild('paginatorNormalPricePackages', { static: true }) paginatorNormalPricePackages: Paginator;
  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  /**
   * get All NormalPricePackages
   */
  getAllNormalPricePackages(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorNormalPricePackages.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._profileServiceProxy
      .getNormalPricePackages(
        this.giverUserId,
        this.primengTableHelper.getSorting(this.dataTableNormalPricePackages),
        this.primengTableHelper.getSkipCount(this.paginatorNormalPricePackages, event),
        this.primengTableHelper.getMaxResultCount(this.paginatorNormalPricePackages, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }
}
