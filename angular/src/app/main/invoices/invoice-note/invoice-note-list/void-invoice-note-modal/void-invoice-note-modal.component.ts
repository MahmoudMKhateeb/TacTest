import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { InvoiceNoteServiceProxy, PartialVoidInvoiceDto } from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { CreateOrEditNoteModalComponent } from '../create-or-edit-note-modal/create-or-edit-note-modal.component';

@Component({
  selector: 'void-invoice-note-modal',
  templateUrl: './void-invoice-note-modal.component.html',
  styleUrls: ['./void-invoice-note-modal.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class VoidInvoiceNoteModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('voidInvoiceModal', { static: true }) modal: ModalDirective;
  @ViewChild('CreateOrEditNoteModalComponent', { static: true }) CreateOrEditNoteModalComponent: CreateOrEditNoteModalComponent;
  active: boolean;
  saving: boolean;

  private invoiceId: number;
  sendFullVoidLoading = false;
  invoiceData: PartialVoidInvoiceDto;
  partialLoading = false;
  invoiceType: number;

  constructor(inject: Injector, private _service: InvoiceNoteServiceProxy) {
    super(inject);
  }

  ngOnInit(): void {}

  show(id?: number, invoiceType?: number) {
    this.invoiceType = invoiceType;
    if (isNotNullOrUndefined(id)) {
      this.invoiceId = id;
    }
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.modal.hide();
  }

  sendFullVoid() {
    this.sendFullVoidLoading = true;
    if (this.invoiceType == 1) {
      this._service.genrateFullVoidInvoiceNote(this.invoiceId).subscribe((res) => {
        this.notify.success(this.l('Success'));
        this.sendFullVoidLoading = false;
        this.close();
      });
    } else {
      this._service.genrateFullVoidSubmitInvoiceNote(this.invoiceId).subscribe((res) => {
        this.notify.success(this.l('Success'));
        this.sendFullVoidLoading = false;
        this.close();
      });
    }
  }

  getInvoiceForPartialVoid() {
    if (this.invoiceType == 1) {
      // invoice
      this._service.getInvoiceForPartialVoid(this.invoiceId).subscribe((res) => {
        // this.notify.success(this.l(""))
        this.invoiceData = res;
        this.CreateOrEditNoteModalComponent.manualInvoiceNoteIsEnabled = false;
        this.CreateOrEditNoteModalComponent.form.invoiceNumber = this.invoiceData.invoiceNumber;
        this.CreateOrEditNoteModalComponent.form.tenantId = this.invoiceData.tenantId;
        this.CreateOrEditNoteModalComponent.form.invoiceItems = this.invoiceData.invoiceItems;
        this.CreateOrEditNoteModalComponent.handleCompanyChange();
        this.CreateOrEditNoteModalComponent.getAllWaybillByInvoiceId();
        //ls
      });
    } else {
      //submit invoice
      this._service.getSubmitInvoiceForPartialVoid(this.invoiceId).subscribe((res) => {
        this.invoiceData = res;
        this.CreateOrEditNoteModalComponent.manualInvoiceNoteIsEnabled = false;
        this.CreateOrEditNoteModalComponent.form.submitInvoiceNumber = this.invoiceData.invoiceNumber;
        this.CreateOrEditNoteModalComponent.form.tenantId = this.invoiceData.tenantId;
        this.CreateOrEditNoteModalComponent.InvoiceType = 2;
        this.CreateOrEditNoteModalComponent.form.invoiceItems = this.invoiceData.invoiceItems;
        this.CreateOrEditNoteModalComponent.handleCompanyChange();
        this.CreateOrEditNoteModalComponent.getAllWaybillByInvoiceId();
      });
    }
  }

  SendPartiallyVoid() {
    this.partialLoading = true;
    this.getInvoiceForPartialVoid();
    this.CreateOrEditNoteModalComponent.show();
  }
}
