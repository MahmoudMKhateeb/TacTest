import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { InvoiceNoteListComponent } from '../invoice-note-list.component';

@Component({
  selector: 'app-inovice-note-modal',
  templateUrl: './inovice-note-modal.component.html',
})
export class InoviceNoteModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  // @ViewChild('invoiceNoteListComponent',{ static: true }) invoiceNoteListComponent: InvoiceNoteListComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  item: any;
  constructor(injector: Injector) {
    super(injector);
    this.item = null;
  }
  ngOnInit(): void {}
  show(item: any): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
