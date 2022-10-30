import { Component, ViewChild, Injector, Input, OnInit } from '@angular/core';
import {
  PriceOfferServiceProxy,
  PriceOfferChannel,
  TmsPricePackageServiceProxy,
  TmsPricePackageForViewDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';

import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { Router } from '@angular/router';

@Component({
  selector: 'shipping-request-offers-list',
  styleUrls: ['./shipping-request-offers-list.component.scss'],
  templateUrl: './shipping-request-offers-list.component.html',
})
export class ShippingRequestOffersList extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Input() shippingRequestId: number;
  @Input() Channel: PriceOfferChannel;
  @Input() isTachyonDeal: boolean;
  @Input() isForDedicated: boolean;
  IsStartSearch: boolean = false;
  matchingTmsPricePkgs: TmsPricePackageForViewDto;

  constructor(
    injector: Injector,
    private _currentServ: PriceOfferServiceProxy,
    private _router: Router,
    private _tmsPricePkgServiceproxy: TmsPricePackageServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    if (this.isTachyonDealer) {
      this.getTMSMatchingPricePackage();
    }
  }

  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._currentServ
      .getAll(
        this.shippingRequestId,
        undefined,
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

  Reject() {
    this.reloadPage();
  }

  openProfileInNewTab(tenantId: number) {
    // Converts the route into a string that can be used
    // with the window.open() function
    const url = this._router.serializeUrl(this._router.createUrlTree([`/app/main/profile`, tenantId]));

    window.open(url, '_blank');
  }

  /**
   * checks if there a Matching TMS Price Package and if yes Allow the Tms To acknowledge
   */
  getTMSMatchingPricePackage(): void {
    //service
    this._tmsPricePkgServiceproxy.getMatchingPricePackage(this.shippingRequestId).subscribe((res) => {
      this.matchingTmsPricePkgs = res;
    });
  }

  tmsPPAcknowledge(shippingRequestId: number, pricePackageId: number): void {
    this._tmsPricePkgServiceproxy.acknowledgePricePackage(shippingRequestId, pricePackageId).subscribe(() => {
      this.notify.success(this.l('Success'));
    });
  }
}
