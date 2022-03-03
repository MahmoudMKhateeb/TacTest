import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditInvoiceNoteDto, GetInvoiceNoteForEditOutput, InvoiceNoteServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-create-or-edit-note-modal',
  templateUrl: './create-or-edit-note-modal.component.html',
})
export class CreateOrEditNoteModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;
  companyLoading: boolean;
  invoiceLoading: boolean;
  invoiceItemLoading: boolean;
  company: any;
  Invoice: any;
  allInvoice: any;
  allInvoiceItem: any;
  selectedCompanyId: number;
  invoiceNote: CreateOrEditInvoiceNoteDto = new CreateOrEditInvoiceNoteDto();
  data: GetInvoiceNoteForEditOutput = new GetInvoiceNoteForEditOutput();
  constructor(injector: Injector, private _invoiceNoteServiceProxy: InvoiceNoteServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}
  show(invoiceNoteId?: number): void {
    if (!invoiceNoteId) {
      this.invoiceNote = new CreateOrEditInvoiceNoteDto();
      this.invoiceNote.id = invoiceNoteId;
      this.active = true;
      this.modal.show();
    } else {
      this._invoiceNoteServiceProxy.getInvoiceNoteForEdit(invoiceNoteId).subscribe((result) => {
        this.data = result;
        this.active = true;
        this.modal.show();
      });
    }
  }
  save(): void {
    this.saving = true;
    this._invoiceNoteServiceProxy
      .createOrEdit(this.invoiceNote)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe((id) => {
        console.log(id);
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }
  loadAllCompany() {
    this.companyLoading = true;
    this._invoiceNoteServiceProxy.getAllCompanyForDropDown().subscribe((res) => {
      this.company = res;
      this.companyLoading = false;
    });
  }
  companySelectChange(companyId?: number) {
    if (companyId > 0) {
      this.invoiceLoading = true;
      this._invoiceNoteServiceProxy.getAllInvoiceNumberBaseOnCompanyDropDown(companyId).subscribe((result) => {
        this.allInvoice = result;
        this.invoiceLoading = false;
      });
    } else {
      this.allInvoice = null;
      this.allInvoiceItem = null;
    }
  }
  InvoiceItemSelectChange(invoiceId?: number) {
    if (invoiceId > 0) {
      this.invoiceItemLoading = true;
      this._invoiceNoteServiceProxy.getAllInvoiceItemDto(invoiceId).subscribe((result) => {
        this.allInvoiceItem = result;
        this.invoiceItemLoading = false;
      });
    } else {
      this.allInvoiceItem = null;
    }
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
