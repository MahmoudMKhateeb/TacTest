import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  TokenAuthServiceProxy,
  ShippingRequestStatus,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { AppSessionService } from '@shared/common/session/app-session.service';

@Component({
  templateUrl: './shippingRequests.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class ShippingRequestsComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  maxVasFilter: number;
  maxVasFilterEmpty: number;
  minVasFilter: number;
  minVasFilterEmpty: number;
  // trucksTypeDisplayNameFilter = '';
  // trailerTypeDisplayNameFilter = '';
  // goodsDetailNameFilter = '';
  // routeDisplayNameFilter = '';
  isBid: boolean;
  ShowPriceAcceptedNoTrips = undefined;
  isTachyonDeal: boolean;
  noOfPostPriceWithoutTrips: number;

  constructor(
    injector: Injector,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService,
    private _router: Router,
    private _sessionService: AppSessionService
  ) {
    super(injector);
    console.log(this._sessionService);
  }

  getShippingRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._shippingRequestsServiceProxy
      .getAll(
        this.filterText,
        this.maxVasFilter == null ? this.maxVasFilterEmpty : this.maxVasFilter,
        this.minVasFilter == null ? this.minVasFilterEmpty : this.minVasFilter,
        this.isBid,
        this.isTachyonDeal,
        undefined,
        undefined,
        this.ShowPriceAcceptedNoTrips,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.noOfPostPriceWithoutTrips = result.noOfPostPriceWithoutTrips;
        this.primengTableHelper.totalRecordsCount = result.data.totalCount;
        this.primengTableHelper.records = result.data.items;
        this.primengTableHelper.hideLoadingIndicator();
        console.log(result.data.items);
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createShippingRequest(): void {
    this._router.navigate(['/app/main/shippingRequests/shippingRequests/createOrEdit']);
  }

  deleteShippingRequest(id): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._shippingRequestsServiceProxy.delete(id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  // exportToExcel(): void {
  //   this._shippingRequestsServiceProxy
  //     .getShippingRequestsToExcel(
  //       this.filterText,
  //       this.maxVasFilter == null ? this.maxVasFilterEmpty : this.maxVasFilter,
  //       this.minVasFilter == null ? this.minVasFilterEmpty : this.minVasFilter,
  //       this.trucksTypeDisplayNameFilter,
  //       this.trailerTypeDisplayNameFilter,
  //       this.goodsDetailNameFilter,
  //       this.routeDisplayNameFilter
  //     )
  //     .subscribe((result) => {
  //       this._fileDownloadService.downloadTempFile(result);
  //     });
  // }
  GetIncompleteShippingRequests() {
    this.ShowPriceAcceptedNoTrips = true;
    this.getShippingRequests();
  }

  getrowstyle(request: ShippingRequestDto) {
    if (request.hasAccident) return 'table-warning';
    else if (request.status == ShippingRequestStatus.PostPrice) return 'table-success';
    else '';
  }
}
