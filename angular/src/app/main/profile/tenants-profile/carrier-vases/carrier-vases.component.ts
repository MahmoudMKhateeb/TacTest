import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { LazyLoadEvent } from '@node_modules/primeng/api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'profile-carrier-vases',
  templateUrl: './carrier-vases.component.html',
  styleUrls: ['./carrier-vases.component.css'],
})
export class CarrierVasesComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTableVases', { static: true }) dataTableVases: Table;
  @ViewChild('paginatorVases', { static: true }) paginatorVases: Paginator;
  @Input() giverUserId: number;
  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  /**
   * get All Vases
   */
  getAllVases(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorVases.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._profileServiceProxy
      .getAvailableVases(
        this.giverUserId,
        this.primengTableHelper.getSorting(this.dataTableVases),
        this.primengTableHelper.getSkipCount(this.paginatorVases, event),
        this.primengTableHelper.getMaxResultCount(this.paginatorVases, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }
}
