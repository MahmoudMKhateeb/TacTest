import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { PriceOfferServiceProxy, PriceOfferChannel, GetShippingRequestForPriceOfferListDto } from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: './price-offer-list-model-component.html',
  /*styleUrls: ['/assets/custom/css/model.scss'],*/
  selector: 'price-offer-list-model',
  animations: [appModuleAnimation()],
})
export class PriceOfferListModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  IsStartSearch: boolean = false;
  shippingRequest: GetShippingRequestForPriceOfferListDto = new GetShippingRequestForPriceOfferListDto();

  constructor(injector: Injector, private _CurrentServ: PriceOfferServiceProxy) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    if (!this.active) return;
    this.primengTableHelper.showLoadingIndicator();
    this._CurrentServ
      .getAll(
        this.shippingRequest.id,
        undefined,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  show(shippingRequest: GetShippingRequestForPriceOfferListDto): void {
    this.primengTableHelper.records = [];
    this.active = true;
    this.modal.show();
    this.shippingRequest = shippingRequest;
    this.getAll();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  Reject() {
    this.reloadPage();
  }
}
