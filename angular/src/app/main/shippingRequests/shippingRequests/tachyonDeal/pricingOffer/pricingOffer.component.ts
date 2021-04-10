import { Component, ViewChild, Injector, OnInit, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AcceptOrRejectOfferByShipperInput, FacilityForDropdownDto, TachyonPriceOffersServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import Swal from 'sweetalert2';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { SendPricingOfferModalComponent } from '@app/main/shippingRequests/shippingRequests/tachyonDeal/pricingOffer/sendPricingOfferModal.component';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'pricingOffer',
  templateUrl: './pricingOffer.component.html',
})
export class PricingOfferComponent extends AppComponentBase {
  @Input() shippingRequestId: number;
  @ViewChild('dataTableForOfferaing', { static: true }) dataTableForOfferaing: Table;
  @ViewChild('paginatorForOfferaing', { static: true }) paginatorForOfferaing: Paginator;
  @ViewChild('sendPricingOfferModal', { static: true }) sendPricingOfferModal: SendPricingOfferModalComponent;

  acceptOrRejectInputs: AcceptOrRejectOfferByShipperInput = new AcceptOrRejectOfferByShipperInput();

  saving = false;
  loading = true;

  constructor(injector: Injector, private _tachyonPriceOffersServiceProxy: TachyonPriceOffersServiceProxy) {
    super(injector);
  }

  getallPricingOffers(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorForOfferaing.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._tachyonPriceOffersServiceProxy
      .getAllTachyonPriceOffers(
        this.shippingRequestId,
        this.primengTableHelper.getSorting(this.dataTableForOfferaing),
        this.primengTableHelper.getSkipCount(this.paginatorForOfferaing, event),
        this.primengTableHelper.getMaxResultCount(this.paginatorForOfferaing, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadpage() {
    this.paginatorForOfferaing.changePage(this.paginatorForOfferaing.getPage());
  }

  acceptOffer(id: number) {
    this.saving = true;
    this.acceptOrRejectInputs.id = id;
    this.acceptOrRejectInputs.isAccepted = true;
    this._tachyonPriceOffersServiceProxy
      .acceptOrRejectOfferByShipper(this.acceptOrRejectInputs)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.reloadpage();
        this.notify.success('offerAcceptedSuccessfully');
      });
  }

  deleteOffer(id: number) {
    this._tachyonPriceOffersServiceProxy.delete(id).subscribe(() => {
      this.reloadpage();
      this.notify.success('deletedSuccessfully');
    });
  }
}
