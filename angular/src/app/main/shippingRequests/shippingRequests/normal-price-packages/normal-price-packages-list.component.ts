import { Component, ViewChild, Injector, Input } from '@angular/core';
import { NormalPricePackagesServiceProxy, SelectItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';

import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';

@Component({
  selector: 'normal-price-packages-list',
  styleUrls: ['./normal-peice-packages-list.component.scss'],
  templateUrl: './normal-price-packages-list.component.html',
})
export class NormalPricePackagesList extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Input() shippingRequestId: number;
  IsStartSearch: boolean = false;
  carrierId?: number;
  allCarriers: SelectItemDto[];

  constructor(injector: Injector, private _normalPricePackages: NormalPricePackagesServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this._normalPricePackages.getCarriers().subscribe((result) => {
      this.allCarriers = result;
    });
  }
  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._normalPricePackages
      .getAllPricePackagesForShippingRequest(
        this.carrierId,
        this.shippingRequestId,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
}
