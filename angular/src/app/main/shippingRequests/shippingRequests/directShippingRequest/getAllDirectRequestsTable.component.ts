import { Component, ViewChild, Injector, ViewEncapsulation, Input, OnInit, AfterViewInit } from '@angular/core';
import { ShippingRequestsTachyonDealerServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { Router } from '@angular/router';
import { CarrierResponseModalComponent } from '@app/main/shippingRequests/shippingRequests/directShippingRequest/carrierResponseModal/carrierResponseModal.component';
import { TachyonDealerResponceModelComponent } from '@app/main/shippingRequests/shippingRequests/directShippingRequest/tachyonDealerResponseModal/tachyonDealerResponceModel.component';
import { SendDirectRequestModalComponent } from '@app/main/shippingRequests/shippingRequests/directShippingRequest/sendDirectRequestsModal/sendDirectRequestModal.component';

@Component({
  selector: 'GetAllDirectRequestsTable',
  templateUrl: './getAllDirectRequestsTable.component.html',
})
export class GetAllDirectRequestsTableComponent extends AppComponentBase implements AfterViewInit {
  @ViewChild('carrierResponceModal', { static: true }) carrierResponceModal: CarrierResponseModalComponent;
  @ViewChild('tachyonDealerResponceModel', { static: true }) tachyonDealerResponceModel: TachyonDealerResponceModelComponent;
  @ViewChild('sendDirectRequestsModal', { static: false }) sendDirectRequestsModal: SendDirectRequestModalComponent;
  @ViewChild('dataTableForCarrier', { static: true }) dataTableForCarrier: Table;
  @ViewChild('paginatorForCarrier', { static: true }) paginatorForCarrier: Paginator;
  @Input() shippingRequestId: number;
  saving = false;
  loading = true;
  filterText: any;

  constructor(injector: Injector, private _router: Router, private _shippingRequestsTachyonDealer: ShippingRequestsTachyonDealerServiceProxy) {
    super(injector);
  }
  ngAfterViewInit() {
    //takes the shippingRequest Id from the ViewShippingRequest as Input and Passes it to the send Direct Request Modal
    //console.log('my Shipping Request Id From After View Init', this.shippingRequestId);
  }
  reloadPage(): void {
    //console.log('reload page');
    this.paginatorForCarrier.changePage(this.paginatorForCarrier.getPage());
  }
  getDirectRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorForCarrier.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._shippingRequestsTachyonDealer
      .getDriectRequestForAllCarriers(
        this.shippingRequestId,
        this.primengTableHelper.getSorting(this.dataTableForCarrier),
        this.primengTableHelper.getSkipCount(this.paginatorForCarrier, event),
        this.primengTableHelper.getMaxResultCount(this.paginatorForCarrier, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  /**
   * opens View Shipping Request in New Tap
   * @param shippingRequestId
   */
  openViewShippingRequestInNewWindow(shippingRequestId) {
    console.log(shippingRequestId);
    const url = this._router.serializeUrl(
      this._router.createUrlTree([`app/main/shippingRequests/shippingRequests/view`], { queryParams: { id: shippingRequestId } })
    );
    window.open(url, '_blank');
  }

  openCarrierProfile() {
    console.log('prifule opener');
  }
}
