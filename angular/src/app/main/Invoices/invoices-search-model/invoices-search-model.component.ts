import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { InvoiceSearchInputDto } from '../invoices-list/InvoiceSearchInputDto';

@Component({
  selector: 'app-invoices-search-model',
  templateUrl: './invoices-search-model.component.html',
  styleUrls: ['./invoices-search-model.component.css'],
})
export class InvoicesSearchModelComponent extends AppComponentBase implements OnInit {
  @Output() modalsearch: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  isLoad: boolean = false;
  active = false;
  saving = false;
  direction: string;
  input: InvoiceSearchInputDto = new InvoiceSearchInputDto();
  paymentDate: moment.Moment;
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}

  show(Input: InvoiceSearchInputDto): void {
    this.input = Input;
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.active = true;
    this.modal.show();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  searchInvoice(): void {
    if (this.paymentDate != null) {
      this.input.paymentDate = moment(this.paymentDate);
    } else {
      this.input.paymentDate = null;
    }

    this.modalsearch.emit(null);
    this.close();
  }
}
