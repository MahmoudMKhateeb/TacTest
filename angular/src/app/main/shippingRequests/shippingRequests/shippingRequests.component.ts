import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ShippingRequestDto, ShippingRequestsServiceProxy, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
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
  isTachyonDeal: boolean;

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

  createShippingRequest(): void {
    this._router.navigate(['/app/main/shippingRequests/shippingRequests/createOrEdit']);
  }

  deleteShippingRequest(shippingRequest: ShippingRequestDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._shippingRequestsServiceProxy.delete(shippingRequest.id).subscribe(() => {
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
}
