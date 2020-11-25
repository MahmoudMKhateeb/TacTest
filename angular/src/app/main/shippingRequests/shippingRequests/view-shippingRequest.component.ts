import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import Swal from 'sweetalert2';

import {
  GetShippingRequestForViewDto,
  PagedResultDtoOfGetShippingRequestBidsForViewDto,
  ShippingRequestBidsServiceProxy,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  StopShippingRequestBidInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, NavigationEnd, Router, RouterEvent } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { Paginator } from 'primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { filter } from '@node_modules/rxjs/internal/operators';
@Component({
  templateUrl: './view-shippingRequest.component.html',
  animations: [appModuleAnimation()],
})
export class ViewShippingRequestComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTablechild', { static: true }) dataTable: Table;
  @ViewChild('paginatorchild', { static: true }) paginator: Paginator;
  active = false;
  saving = false;
  stopShippingRequestBody: StopShippingRequestBidInput = new StopShippingRequestBidInput();
  item: GetShippingRequestForViewDto;
  private AllBids: PagedResultDtoOfGetShippingRequestBidsForViewDto;
  private activeShippingRequestId: number;

  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests'),
    new BreadcrumbItem(this.l('ShippingRequests') + '' + this.l('Details')),
  ];

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,

    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy
  ) {
    super(injector);
    this.item = new GetShippingRequestForViewDto();
    this.item.shippingRequest = new ShippingRequestDto();
  }

  ngOnInit(): void {
    this.show(this._activatedRoute.snapshot.queryParams['id']);
    this._router.events.pipe(filter((event: RouterEvent) => event instanceof NavigationEnd)).subscribe(() => {
      this.show(this._activatedRoute.snapshot.queryParams['id']);
      this.reloadPage();
    });
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy.getShippingRequestForView(shippingRequestId).subscribe((result) => {
      this.item = result;
      this.activeShippingRequestId = this.item.shippingRequest.id;
      this.active = true;
    });
  }

  reloadPage(): void {
    console.log('reload page');
    this.paginator.changePage(this.paginator.getPage());
  }
  getShippingRequestsBids(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    console.log('bids Gotten');
    this._shippingRequestBidsServiceProxy
      .getAllBidsByShippingRequestIdPaging(
        null,
        0,
        10000,
        this.activeShippingRequestId,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        //console.log(result);
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        //console.log(result.items);
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  AcceptBidMeth(id: number) {
    Swal.fire({
      title: 'Are you sure you want accept this bid ?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No',
    }).then((result) => {
      if (result.value) {
        this._shippingRequestBidsServiceProxy.acceptBid(id).subscribe(() => {
          Swal.fire('Accepted!', 'You Successfully Accepted the bid request.', 'success');
          this.reloadPage();
          console.log('bid Accepted', id);
        });
      }
    });
  } //end of Accept

  //this method is for CancelShippingRequestBid: which is for canceling bidding on the shipping request
  CancelShippingRequestBid(id: number) {
    this.stopShippingRequestBody.shippingRequestId = id;
    Swal.fire({
      title: 'Are you sure You Want to Cancel Bidding on this Shipping Request?',
      icon: 'warning',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
    }).then((result) => {
      if (result.value) {
        this._shippingRequestBidsServiceProxy.cancelShippingRequestBid(this.stopShippingRequestBody).subscribe(() => {
          Swal.fire('Success!', 'You Successfully Stopped the Bidding on this request.', 'success');
          this.reloadPage();
          console.log('Shipping Request bid Canceld', id);
        });
      }
    });
  }
}
