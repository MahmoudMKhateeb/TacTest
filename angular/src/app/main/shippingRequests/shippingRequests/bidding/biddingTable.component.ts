import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import Swal from 'sweetalert2';

import {
  ShippingRequestBidsServiceProxy,
  ShippingRequestsServiceProxy,
  CancelBidShippingRequestInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, NavigationEnd, Router, RouterEvent } from '@angular/router';
import { Paginator } from 'primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';

@Component({
  selector: 'biddingTableComponent',
  templateUrl: './biddingTable.component.html',
})
export class BiddingTableComponent extends AppComponentBase {
  @Input() shippingRequestId: number;
  @Output() onSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('dataTableChild', { static: false }) dataTable: Table;
  @ViewChild('paginatorChild', { static: false }) paginator: Paginator;
  active = false;
  saving = false;
  loading = true;
  CancelBidShippingRequest: CancelBidShippingRequestInput = new CancelBidShippingRequestInput();
  bidsloading = false;
  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(injector);
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  getShippingRequestsBids(event?: LazyLoadEvent) {
    this.bidsloading = true;
    this.changeDetectorRef.detectChanges();

    this.primengTableHelper.showLoadingIndicator();

    this._shippingRequestBidsServiceProxy
      .getAllShippingRequestBids(
        undefined,
        undefined,
        undefined,
        this.shippingRequestId,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.bidsloading = false;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
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
        this._shippingRequestBidsServiceProxy.acceptShippingRequestBid(id).subscribe(() => {
          Swal.fire('Accepted!', 'YouSuccessfullyAcceptedthebidrequest.', 'success');
          this.onSave.emit('');
          //emit Goes here
          //this.reloadCurrentPage();
        });
      }
    });
  } //end of Accept

  /**
   * this method is for stop the bidding on the shipping request
   * @param shippingRequestId
   */
  stopBiddingOnShippingRequest(shippingRequestId: number) {
    this.CancelBidShippingRequest.shippingRequestId = shippingRequestId;
    Swal.fire({
      title: 'Are you sure You Want to Cancel Bidding on this Shipping Request?',
      icon: 'warning',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
    }).then((result) => {
      if (result.value) {
        this._shippingRequestBidsServiceProxy.cancelBidShippingRequest(this.CancelBidShippingRequest).subscribe(() => {
          Swal.fire('Success!', 'You Successfully Stopped the Bidding on this request.', 'success');
          this.reloadPage();
        });
      }
    });
  }
}
