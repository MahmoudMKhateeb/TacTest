import { Component, EventEmitter, Injector, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CompayForDropDownDto,
  CreateOrEditInvoiceNoteDto,
  GetAllInvoiceItemDto,
  InvoiceNoteServiceProxy,
  InvoiceRefreanceNumberDto,
  NoteType,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'create-or-edit-note-modal',
  templateUrl: './create-or-edit-note-modal.component.html',
  styleUrls: ['./create-or-edit-note-modal.component.css'],
  providers: [EnumToArrayPipe],
  encapsulation: ViewEncapsulation.None,
})
export class CreateOrEditNoteModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  noteTypeId: number;
  form: CreateOrEditInvoiceNoteDto = new CreateOrEditInvoiceNoteDto();
  noteTypeEnum = NoteType;
  noteTypes = this.enumToArray.transform(this.noteTypeEnum);
  allCompanies: CompayForDropDownDto[];
  allInvoices: InvoiceRefreanceNumberDto[];
  allWaybills: GetAllInvoiceItemDto[] = [];
  selectedWaybills: GetAllInvoiceItemDto[] = [];
  saving: boolean;
  manualInvoiceNoteIsEnabled = true;

  constructor(inject: Injector, private _invoiceNoteServiceProxy: InvoiceNoteServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(inject);
  }

  ngOnInit(): void {
    this.loadDropDowns();
  }

  loadDropDowns() {
    this.getAllCompaniesForDropDown();
  }

  getAllCompaniesForDropDown() {
    this._invoiceNoteServiceProxy.getAllCompanyForDropDown().subscribe((res) => {
      this.allCompanies = res;
    });
  }

  getAllInvoicesByCompanyId() {
    this._invoiceNoteServiceProxy.getAllInvoiceNumberBaseOnCompanyDropDown(this.form.tenantId).subscribe((res) => {
      this.allInvoices = res;
      this.getAllWaybillByInvoiceId();
    });
  }

  getAllWaybillByInvoiceId() {
    if (this.form.id) this.form.invoiceItems = [];
    let id = this.allInvoices.find((x) => x.refreanceNumber == this.form.invoiceNumber).id;
    this._invoiceNoteServiceProxy.getAllInvoicmItemDto(id).subscribe((res) => {
      this.allWaybills = res;
    });
  }

  save() {
    // console.log(this.form);
    this.saving = true;
    this.form.invoiceItems = this.selectedWaybills;
    this._invoiceNoteServiceProxy
      .createOrEdit(this.form)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }
  savePartialVoid() {
    this.saving = true;
    this._invoiceNoteServiceProxy.generatePartialInvoiceNote(this.form).subscribe((res) => {
      this.saving = false;
      this.notify.success('SavedSuccessfully');
      this.modalSave.emit();
    });
  }
  show(id?: number) {
    this.active = true;
    this.form = new CreateOrEditInvoiceNoteDto();
    if (isNotNullOrUndefined(id)) {
      //edit
      this._invoiceNoteServiceProxy.getInvoiceNoteForEdit(id).subscribe((res) => {
        this.form = res;
        this.selectedWaybills = res.invoiceItems;
        this.getAllInvoicesByCompanyId();
        console.log('Edit Fired .........');
        //this.getAllWaybillByInvoiceId();
      });
    }
    this.modal.show();
    console.log(this.form);
  }

  close() {
    this.active = false;
    this.manualInvoiceNoteIsEnabled = true;
    this.modal.hide();
  }

  handleCompanyChange() {
    console.log('handleCompanyChange');
    if (!this.form.isManual) {
      console.log('2handleCompanyChange');
      this.getAllInvoicesByCompanyId();
      this.form.invoiceItems = [];
    }
  }
}
