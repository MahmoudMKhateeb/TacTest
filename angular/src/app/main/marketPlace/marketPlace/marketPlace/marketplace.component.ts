import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  TokenAuthServiceProxy,
  ShippingRequestBidsServiceProxy,
  TenantDashboardServiceProxy,
  TrucksServiceProxy,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { ViewDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/view-documentFile-modal.component';
import { Table } from '@node_modules/primeng/table';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Paginator } from 'primeng/paginator';
import { ViewAllCarrierBidsComponent } from '@app/main/marketPlace/marketPlace/ViewAllCarrierBidsModal/ViewAllCarrierBids.component';
@Component({
  templateUrl: './marketPlace.component.html',
  styleUrls: ['./marketPlaceStyling.css'],
  animations: [appModuleAnimation()],
})
export class MarketplaceComponent extends AppComponentBase implements OnInit {
  @ViewChild('ViewShippingRequestDetailsModal', { static: true }) viewDocumentFileModal: ViewDocumentFileModalComponent;
  @ViewChild('ViewAllCarrierBidsModal', { static: true }) ViewAllCarrierBidsModal: ViewAllCarrierBidsComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  //inputs
  advancedFiltersAreShown = false;

  filterText = '';
  MatchingBidOnlyFilter = true;
  MyBidsOnlyFilter: false;
  TruckTypeIdFilter: number;
  TransportTypeFilter: number;
  CapacityIdFilter: number;

  allTransportTypes: SelectItemDto[];
  allTruckTypesByTransportType: SelectItemDto[];
  allTrucksCapByTruckTypeId: SelectItemDto[];

  constructor(
    injector: Injector,
    private _tokenAuth: TokenAuthServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy,
    private _tenantDashboardServiceProxy: TenantDashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.initTransportDropDownList();
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  GetAllShippingRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._shippingRequestBidsServiceProxy
      .getAllBidShippingRequestsForCarrier(
        this.filterText,
        this.MatchingBidOnlyFilter,
        this.MyBidsOnlyFilter,
        this.TruckTypeIdFilter,
        this.TransportTypeFilter,
        this.CapacityIdFilter,
        false,
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
  transportTypeSelectChange(transportTypeId?: number) {
    if (transportTypeId > 0) {
      this._trucksServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(transportTypeId).subscribe((result) => {
        this.allTruckTypesByTransportType = result;
        this.TruckTypeIdFilter = null;
      });
    } else {
      this.TruckTypeIdFilter = null;
      this.allTruckTypesByTransportType = null;
      this.allTrucksCapByTruckTypeId = null;
    }
  }

  trucksTypeSelectChange(trucksTypeId?: number) {
    if (trucksTypeId > 0) {
      this._trucksServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(trucksTypeId).subscribe((result) => {
        this.allTrucksCapByTruckTypeId = result;
        this.CapacityIdFilter = null;
      });
    } else {
      this.CapacityIdFilter = null;
      this.allTrucksCapByTruckTypeId = null;
    }
  }

  initTransportDropDownList() {
    this.allTransportTypes = null;
    this.allTruckTypesByTransportType = null;
    this.allTrucksCapByTruckTypeId = null;
    this._trucksServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });
  }
}
