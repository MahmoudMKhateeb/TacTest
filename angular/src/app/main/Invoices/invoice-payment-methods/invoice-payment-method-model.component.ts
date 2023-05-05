import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  InvoicePaymentMethodServiceProxy,
  InvoicePaymentMethodListDto,
  InvoicePaymentType,
  CreateOrEditInvoicePaymentMethod,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'invoice-payment-method-model',
  templateUrl: './invoice-payment-method-model.component.html',
  providers: [EnumToArrayPipe],
})
export class InvoicePaymentMethodModelComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('nameInput', { static: false }) nameInput: ElementRef;

  paymentMethod: InvoicePaymentMethodListDto;
  PaymentType: any;
  active: boolean = false;
  saving: boolean = false;

  constructor(injector: Injector, private enumToArray: EnumToArrayPipe, private _currentSrv: InvoicePaymentMethodServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.PaymentType = this.enumToArray.transform(InvoicePaymentType);
  }

  public show(paymentMethod: InvoicePaymentMethodListDto | null): void {
    this.paymentMethod = new InvoicePaymentMethodListDto();
    if (paymentMethod) this.paymentMethod = paymentMethod;
    this.active = true;
    this.modal.show();
  }

  onShown(): void {
    this.nameInput.nativeElement.focus();
  }

  save(): void {
    let input: CreateOrEditInvoicePaymentMethod = <CreateOrEditInvoicePaymentMethod>this.paymentMethod;
    console.log(this.paymentMethod);
    console.log(input);
    this.saving = true;
    this._currentSrv
      .createOrEdit(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        if (!this.paymentMethod.id) {
          this.paymentMethod.id = result.id;
          this.modalSave.emit(this.paymentMethod);
        }
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
