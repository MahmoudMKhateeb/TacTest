import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { TokenAuthServiceProxy, ShippingRequestBidsServiceProxy, TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { ViewDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/view-documentFile-modal.component';
import { interval } from '@node_modules/rxjs';
import { Table } from '@node_modules/primeng/table';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Paginator } from 'primeng/paginator';
@Component({
  templateUrl: './marketPlace.component.html',
  styleUrls: ['./marketPlaceStyling.css'],
  animations: [appModuleAnimation()],
})
export class MarketplaceComponent extends AppComponentBase implements OnInit {
  @ViewChild('ViewShippingRequestDetailsModal', { static: true }) viewDocumentFileModal: ViewDocumentFileModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  maxPriceFilterEmpty: number;
  minPriceFilter: number;
  totalSales: number;
  revenue: number;
  expenses: number;
  growth: number;

  constructor(
    injector: Injector,
    private _tokenAuth: TokenAuthServiceProxy,
    private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy
  ) {
    super(injector);
    interval(3000).subscribe((x) => {
      console.log('fired');
      //this.GetCarrierSummary();
    });
  }

  ngOnInit() {}

  GetAllShippingRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._shippingRequestBidsServiceProxy.getAllbidsRequestDetailsForView(0, 10, 'id').subscribe((result) => {
      //console.log(result);
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      //console.log(result.items);
      this.primengTableHelper.hideLoadingIndicator();
    });
  }

  reloadPage(): void {
    console.log('reload page');
    this.paginator.changePage(this.paginator.getPage());
  }
}
