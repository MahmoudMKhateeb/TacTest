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
import { ViewAllCarrierBidsComponent } from '@app/main/marketPlace/marketPlace/ViewAllCarrierBids.component';
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
  advancedFiltersAreShown = true;

  filterText = '';
  MatchingBidOnlyFilter = true;
  MyBidsOnlyFilter: false;
  TruckTypeIdFilter: number;
  TruckSubTypeIdFilter: number;
  TransportTypeFilter: number;
  TransportSubTypeFilter: number;
  CapacityIdFillter: number;

  allTransportTypes: SelectItemDto[];
  allTransportSubTypes: SelectItemDto[];
  allTruckTypesByTransportSubtype: SelectItemDto[];
  allTruckSubTypesByTruckTypeId: SelectItemDto[];
  allTrucksCapByTruckSubTypeId: SelectItemDto[];

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
    this.GetTransportDropDownList();
  }

  test() {
    console.log(this.MatchingBidOnlyFilter);
  }

  reloadPage(): void {
    console.log('reload page');
    this.paginator.changePage(this.paginator.getPage());
  }
  GetAllShippingRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._shippingRequestBidsServiceProxy
      .getAllMarketPlaceSRForCarrier(
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event),
        this.primengTableHelper.getSorting(this.dataTable),
        this.filterText,
        this.MatchingBidOnlyFilter,
        this.MyBidsOnlyFilter,
        this.TruckTypeIdFilter,
        this.TruckSubTypeIdFilter,
        this.TransportTypeFilter,
        this.TransportSubTypeFilter,
        this.CapacityIdFillter
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }
  ClearAllTransPortDropDowns() {
    this.allTransportTypes = null;
    this.allTransportSubTypes = null;
    this.allTruckTypesByTransportSubtype = null;
    this.allTruckSubTypesByTruckTypeId = null;
    this.allTrucksCapByTruckSubTypeId = null;
  }
  GetTransportDropDownList(mode?: string, value?: number) {
    switch (mode) {
      case 'GetAllTransportSubTypes':
        if (value > 0) {
          this._trucksServiceProxy.getAllTransportSubtypesByTransportTypeIdForDropdown(value).subscribe((result) => {
            this.allTransportSubTypes = result;
          });
        } else {
          this.allTransportSubTypes = null;
          this.allTruckTypesByTransportSubtype = null;
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllTruckTypesByTransportSubTypes':
        if (value > 0) {
          this._trucksServiceProxy.getAllTruckTypesByTransportSubtypeIdForDropdown(value).subscribe((result) => {
            this.allTruckTypesByTransportSubtype = result;
          });
        } else {
          this.allTruckTypesByTransportSubtype = null;
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllTruckSubTypesByTruckTypeId':
        if (value > 0) {
          this._trucksServiceProxy.getAllTruckSubTypesByTruckTypeIdForDropdown(value).subscribe((result) => {
            this.allTruckSubTypesByTruckTypeId = result;
          });
        } else {
          this.allTruckSubTypesByTruckTypeId = null;
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      case 'GetAllCapByTruckSybTypeId':
        if (value > 0) {
          this._trucksServiceProxy.getAllTuckCapacitiesByTuckSubTypeIdForDropdown(value).subscribe((result) => {
            this.allTrucksCapByTruckSubTypeId = result;
          });
        } else {
          this.allTrucksCapByTruckSubTypeId = null;
        }
        break;
      default:
        this.ClearAllTransPortDropDowns();
        if (this.allTransportTypes == null) {
          this._trucksServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
            this.allTransportTypes = result;
            console.log('if true fired');
            console.log(this.allTransportTypes);
          });
        }
        break;
    }
  }
}
