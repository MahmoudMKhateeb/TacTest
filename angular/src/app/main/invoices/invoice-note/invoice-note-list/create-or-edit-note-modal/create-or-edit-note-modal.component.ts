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
import { timeStamp } from 'console';

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
  waybillsLoading = false;

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
      //this.getAllWaybillByInvoiceId();
    });
  }

  getAllWaybillByInvoiceId() {
    this.waybillsLoading = true;
    if (this.form.id) this.form.invoiceItems = [];
    let id = this.allInvoices.find((x) => x.refreanceNumber == this.form.invoiceNumber).id;
    console.log(id);
    this._invoiceNoteServiceProxy
      .getAllInvoicmItemDto(id)
      .pipe(
        finalize(() => {
          this.waybillsLoading = false;
        })
      )
      .subscribe((res) => {
        this.allWaybills = res;
      });
  }

  save() {
    // console.log(this.form);
    this.saving = true;
    //this.selectedWaybills=this.allWaybills.filter(item => { return item.checked; })
    this.form.invoiceItems = this.selectedWaybills.filter((item) => {
      return item.checked;
    });
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
      this._invoiceNoteServiceProxy
        .getInvoiceNoteForEdit(id)
        .pipe(
          finalize(() => {
            this.getAllWaybillByInvoiceId();
          })
        )
        .subscribe((res) => {
          this.form = res;
          this.selectedWaybills = res.invoiceItems;
          this.getAllInvoicesByCompanyId();
          console.log('Edit Fired .........');
        });
    } else {
      this.form.vatAmount = 0;
    }
    this.modal.show();
    console.log(this.form);
  }

  close() {
    this.active = false;
    this.selectedWaybills = null;
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

  calculator(price: number, taxVat: number, index: number): void {
    this.allWaybills[index].vatAmount = (price * taxVat) / 100;
    this.allWaybills[index].totalAmount = this.allWaybills[index].vatAmount + price;
    this.selectedWaybills = this.allWaybills.filter((item) => {
      return item.checked;
    });
    //this.allWaybills.push();
    var sumAllPrice = this.selectedWaybills.reduce((prev, next) => prev + next.price, 0);
    this.form.price = sumAllPrice;
    var allVatAmount = this.selectedWaybills.reduce((prev, next) => prev + next.vatAmount, 0);
    this.form.vatAmount = allVatAmount;
    this.form.totalValue = sumAllPrice + allVatAmount;
    // console.log(this.form.price);
    // console.log(this.form);
  }

  CalculateTotalPrice(): void {
    this.form.totalValue = this.form.price + this.form.vatAmount;
    console.log(this.form);
  }
}
