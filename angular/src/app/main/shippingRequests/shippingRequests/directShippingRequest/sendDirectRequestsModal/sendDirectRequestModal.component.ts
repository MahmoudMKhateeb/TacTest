import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CarrirSetPriceForDirectRequestDto,
  CreateOrEditTachyonPriceOfferDto,
  ShippingRequestsTachyonDealerServiceProxy,
  TachyonDealerCreateDirectOfferToCarrirerInuptDto,
  TachyonPriceOffersServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';

@Component({
  selector: 'sendDirectRequestsModal',
  templateUrl: './sendDirectRequestModal.component.html',
})
export class SendDirectRequestModalComponent extends AppComponentBase {
  @ViewChild('dataTableForCarrierSendRequest', { static: true }) dataTableForCarrier: Table;
  @ViewChild('paginatorForCarrierSendRequest', { static: true }) paginatorForCarrier: Paginator;
  @ViewChild('sendDirectRequestsModal', { static: false }) modal: ModalDirective;
  @Input() shippingRequestId: number;
  @Output() directRequestSent: EventEmitter<any> = new EventEmitter<any>();
  sendDirectRequestToCarrierInput: TachyonDealerCreateDirectOfferToCarrirerInuptDto = new TachyonDealerCreateDirectOfferToCarrirerInuptDto();
  element: HTMLElement;
  saving = false;
  loading = true;
  fillterText = '';

  constructor(injector: Injector, private _shippingRequestsTachyonDealer: ShippingRequestsTachyonDealerServiceProxy) {
    super(injector);
  }

  show(): void {
    this.modal.show();
  }
  close(): void {
    this.sendDirectRequestToCarrierInput = new TachyonDealerCreateDirectOfferToCarrirerInuptDto();
    this.modal.hide();
  }

  getAllCarriersForSendDirectRequests(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorForCarrier.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._shippingRequestsTachyonDealer
      .getAllCarriers(
        this.fillterText,
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
  reloadPage(): void {
    //console.log('reload page');
    this.paginatorForCarrier.changePage(this.paginatorForCarrier.getPage());
  }

  disableButton(id) {
    this.element = document.getElementById(id) as HTMLElement;
    console.log(this.element);
    this.element.setAttribute('disabled', '');
  }

  sendDirectRequestToCarrier(carrierId: number) {
    this.saving = true;
    this.sendDirectRequestToCarrierInput.id = this.shippingRequestId;
    this.sendDirectRequestToCarrierInput.tenantId = carrierId;
    this._shippingRequestsTachyonDealer
      .sendDriectRequestForCarrier(this.sendDirectRequestToCarrierInput)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.disableButton(carrierId);
        })
      )
      .subscribe(() => {
        this.notify.success('directRequestSendSuccessfully');
        this.directRequestSent.emit('');
      });
    //this.disableButton(carrierId);
  }
}
