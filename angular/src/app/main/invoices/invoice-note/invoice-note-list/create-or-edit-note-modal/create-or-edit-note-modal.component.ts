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
  AllItemsWithoutInvoice: GetAllInvoiceItemDto[] = [];
  selectedWaybills: any[] = [];
  saving: boolean;
  manualInvoiceNoteIsEnabled = true;
  waybillsLoading = false;
  newAttribute: any = {};

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

  save() {
    // console.log(this.form);
    this.saving = true;
    //this.selectedWaybills=this.allWaybills.filter(item => { return item.checked; })
    // console.log(this.selectedWaybills);
    //JSON.stringify(this.selectedWaybills);
    this.form.invoiceItems = this.selectedWaybills
      .filter((item) => {
        return item.checked;
      })
      .map(
        (it) =>
          new GetAllInvoiceItemDto({
            checked: it.checked,
            id: it.id,
            price: it.price,
            vatAmount: it.vatAmount,
            totalAmount: it.totalAmount,
            taxVat: it.taxVat,
            tripId: it.tripId,
            tripVasId: it.tripVasId,
            waybillNumber: it.wayBillNumber,
          })
      );
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
    if (id) {
      //edit
      this._invoiceNoteServiceProxy.getInvoiceNoteForEdit(id).subscribe((res) => {
        this.form = res;
        this.selectedWaybills = res.invoiceItems;
        if (res.invoiceItems.filter((x) => x.waybillNumber)) {
          this.AllItemsWithoutInvoice = res.invoiceItems;
        }
        this.getAllInvoicesByCompanyId();
      });
    } else {
      this.form.vatAmount = 0;
    }
    this.modal.show();
    //console.log(this.form);
  }

  close() {
    this.active = false;
    this.selectedWaybills = null;
    this.AllItemsWithoutInvoice = null;
    this.manualInvoiceNoteIsEnabled = true;
    this.form = undefined;
    this.allWaybills = undefined;
    this.selectedWaybills = undefined;
    this.modal.hide();
  }

  handleCompanyChange() {
    //if (!this.form.isManual) {
    this.getAllInvoicesByCompanyId();
    this.form.invoiceItems = [];
    //}
  }

  getAllInvoicesByCompanyId() {
    this._invoiceNoteServiceProxy.getAllInvoiceNumberBaseOnCompanyDropDown(this.form.tenantId).subscribe((res) => {
      this.allInvoices = res;
      this.getAllWaybillByInvoiceId();
    });
  }

  getAllWaybillByInvoiceId() {
    let id = this.allInvoices.find((x) => x.refreanceNumber == this.form.invoiceNumber)?.id;
    //var index = this.allInvoices.findIndex((x) => x.refreanceNumber == this.form.invoiceNumber);
    //var id = this.allInvoices[index].id;
    if (!isNotNullOrUndefined(id)) {
      this.allWaybills = [];
      return;
    }

    if (!id) {
      this.AllItemsWithoutInvoice = this.form.invoiceItems;
    }

    //show
    this.waybillsLoading = true;
    if (this.form.id) this.form.invoiceItems = [];

    this._invoiceNoteServiceProxy.getAllInvoicmItemDto(id).subscribe((res) => {
      this.allWaybills = res;
      this.waybillsLoading = false;
      console.log(this.form.invoiceNumber);
      this.allWaybills.forEach((x) => {
        let waybill = this.selectedWaybills.find((y) => x.waybillNumber == y.waybillNumber);

        if (waybill != null) {
          x.id = waybill.id;
          x.checked = waybill.id != null;
          x.price = waybill.price;
          x.vatAmount = waybill.vatAmount;
          x.totalAmount = waybill.totalAmount;
        }
      });
    });
  }
  calculator(price: number, taxVat: number, index: number): void {
    this.allWaybills[index].vatAmount = (price * taxVat) / 100;
    this.allWaybills[index].totalAmount = this.allWaybills[index].vatAmount + price;
    this.selectedWaybills = this.allWaybills.filter((item) => {
      return item.checked;
    });
    //this.allWaybills.push();
    // var sumAllPrice = this.selectedWaybills.reduce((prev, next) => prev + next.price, 0);
    // this.form.price = sumAllPrice;
    // var allVatAmount = this.selectedWaybills.reduce((prev, next) => prev + next.vatAmount, 0);
    // this.form.vatAmount = allVatAmount;
    // this.form.totalValue = sumAllPrice + allVatAmount;
    this.calculateTotalPrice();
  }

  // CalculateTotalPrice(): void {
  //   this.form.totalValue = this.form.price + this.form.vatAmount;
  // }

  addFieldValue() {
    this.newAttribute.checked = true;
    this.AllItemsWithoutInvoice.push(this.newAttribute);
    this.newAttribute = {};
    this.calculateTotalPrice();
  }

  deleteFieldValue(index) {
    this.AllItemsWithoutInvoice.splice(index, 1);
    this.calculateTotalPrice();
  }

  calculatePrice(newPrice: number): void {
    var newVatAmount: number;
    newVatAmount = (newPrice * 15) / 100;
    this.newAttribute.vatAmount = newVatAmount;
    var newTotalAmount = newPrice + newVatAmount;
    this.newAttribute.totalAmount = newTotalAmount;
    this.selectedWaybills = this.AllItemsWithoutInvoice;
  }

  calculateTotalPrice() {
    var sumAllPrice = this.selectedWaybills.reduce((prev, next) => prev + next.price, 0);
    this.form.price = sumAllPrice;
    var allVatAmount = this.selectedWaybills.reduce((prev, next) => prev + next.vatAmount, 0);
    this.form.vatAmount = allVatAmount;
    this.form.totalValue = sumAllPrice + allVatAmount;
  }
}
