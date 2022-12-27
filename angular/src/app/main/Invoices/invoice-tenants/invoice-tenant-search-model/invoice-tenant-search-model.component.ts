import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { TrackingSearchInput } from '@app/shared/common/search/TrackingSearchInput';
import { SubmitInvoicesSearchInput } from '@app/shared/common/search/SubmitInvoiceSearchInput';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { waybillsComponent } from '@app/admin/waybills/waybills';

@Component({
  selector: 'app-invoice-tenant-search-model',
  templateUrl: './invoice-tenant-search-model.component.html',
  styleUrls: ['./invoice-tenant-search-model.component.css'],
})
export class InvoiceTenantSearchModelComponent extends AppComponentBase {
  @Output() modalsearch: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  active = false;
  saving = false;
  direction: string;
  input: SubmitInvoicesSearchInput = new SubmitInvoicesSearchInput();

  constructor(injector: Injector) {
    super(injector);
  }

  show(Input: SubmitInvoicesSearchInput): void {
    this.input = Input;
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  search(): void {
    this.modalsearch.emit(null);
    this.close();
  }
}
