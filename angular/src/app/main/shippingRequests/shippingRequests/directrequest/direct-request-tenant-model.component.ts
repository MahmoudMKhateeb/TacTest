import { Component, ViewChild, Injector, Input, EventEmitter, Output } from '@angular/core';
import {
  ShippingRequestDirectRequestServiceProxy,
  CreateShippingRequestDirectRequestInput,
  ShippingRequestDirectRequestGetCarrirerListDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from '@node_modules/primeng/public_api';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'direct-request-tenant-model',
  templateUrl: './direct-request-tenant-model.component.html',
  styleUrls: ['./direct-request-tenant-model.component.scss'],
})
export class DirectRequestTenantModelComponent extends AppComponentBase {
  @ViewChild('dataTableForCarrier', { static: true }) dataTableForCarrier: Table;
  @ViewChild('paginatorForCarrier', { static: true }) paginatorForCarrier: Paginator;
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() directRequestSent: EventEmitter<any> = new EventEmitter<any>();
  @Input() shippingRequestId: number;
  saving = false;
  loading = true;
  filterText: any;
  active = false;
  constructor(injector: Injector, private _router: Router, private _currentServ: ShippingRequestDirectRequestServiceProxy) {
    super(injector);
  }

  reloadPage(): void {
    this.paginatorForCarrier.changePage(this.paginatorForCarrier.getPage());
  }
  getDirectRequests(event?: LazyLoadEvent) {
    if (!this.active) return;
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginatorForCarrier.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();
    this._currentServ
      .getAllCarriers(
        undefined,
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

  sendDirectRequestToCarrier(directRequest: ShippingRequestDirectRequestGetCarrirerListDto) {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        let input: CreateShippingRequestDirectRequestInput = new CreateShippingRequestDirectRequestInput();
        this.saving = true;
        input.shippingRequestId = this.shippingRequestId;
        input.carrierTenantId = directRequest.id;
        this._currentServ
          .create(input)
          .pipe(
            finalize(() => {
              this.saving = false;
              directRequest.isRequestSent = true;
            })
          )
          .subscribe(() => {
            this.notify.success('SendSuccessfully');
            this.directRequestSent.emit('');
          });
      }
    });
  }

  show(): void {
    this.active = true;
    this.getDirectRequests();
    this.modal.show();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
