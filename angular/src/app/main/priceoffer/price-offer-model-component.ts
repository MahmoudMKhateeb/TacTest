import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EnumToArrayPipe } from '../../../shared/common/pipes/enum-to-array.pipe';

import {
  PriceOfferServiceProxy,
  PriceOfferDto,
  PriceOfferItem,
  PriceOfferChannel,
  CreateOrEditPriceOfferInput,
  PriceOfferDetailDto,
  PriceOfferCommissionType,
  PriceOfferTenantCommssionSettings,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './price-offer-model-component.html',
  styleUrls: ['./price-offer-model-component.scss'],
  selector: 'price-offer-model',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class PriceOfferModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<number> = new EventEmitter<number>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  offer: PriceOfferDto = new PriceOfferDto();
  input: CreateOrEditPriceOfferInput = new CreateOrEditPriceOfferInput();
  direction: string;
  Items: PriceOfferItem[] = [];
  priceOfferCommissionType: any;
  commissionTypeTitle: string;
  constructor(injector: Injector, private _CurrentServ: PriceOfferServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }
  ngOnInit(): void {
    this.priceOfferCommissionType = this.enumToArray.transform(PriceOfferCommissionType);
    this.offer.commssionSettings = new PriceOfferTenantCommssionSettings();
  }
  show(id: number): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this._CurrentServ.getPriceOfferForCreateOrEdit(id).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
      this.input.shippingRequestId = id;
      this.input.channel = this.Channel;
      if (!this.feature.isEnabled('App.Carrier')) {
        this.changeItemComissionValue();
        this.changeVasComissionValue();
      }
      console.log(this.offer);
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  changeItemComissionValue() {
    switch (this.offer.commissionType.toString()) {
      case '1':
        this.offer.commissionPercentageOrAddValue = this.offer.commssionSettings.itemCommissionPercentage;
        break;
      case '2':
        this.offer.commissionPercentageOrAddValue = this.offer.commssionSettings.itemCommissionValue;
        break;
      case '3':
        this.offer.commissionPercentageOrAddValue = this.offer.commssionSettings.itemMinValueCommission;
        break;
    }
  }
  changeVasComissionValue() {
    switch (this.offer.vasCommissionType.toString()) {
      case '1':
        this.offer.vasCommissionPercentageOrAddValue = this.offer.commssionSettings.vasCommissionPercentage;
        break;
      case '2':
        this.offer.vasCommissionPercentageOrAddValue = this.offer.commssionSettings.vasCommissionValue;
        break;
      case '3':
        this.offer.vasCommissionPercentageOrAddValue = this.offer.commssionSettings.vasMinValueCommission;
        break;
    }
  }

  sendOffer(): void {
    this.calculatorAll();
    let itemDetails: PriceOfferDetailDto[] = [];
    this.input.itemPrice = this.offer.itemPrice;
    this.Items.forEach((item) => {
      let order = new PriceOfferDetailDto();
      order.itemId = item.sourceId;
      order.price = item.itemPrice;
      itemDetails.push(order);
    });
    this.input.itemDetails = itemDetails;
    this._CurrentServ
      .createOrEdit(this.input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.info(this.l('SendSuccessfully'));
        this.close();
        this.modalSave.emit(result);
      });

    this.saving = true;
  }

  calculator(): void {
    this.offer.itemTotalAmount = this.offer.itemPrice * this.offer.quantity;
    this.offer.itemCommissionAmount = this.calculatorItemCommission(this.offer.itemPrice);
    this.offer.itemSubTotalAmountWithCommission = this.offer.itemCommissionAmount * this.offer.quantity;
    this.calculatorAll();
  }
  calculatorItem(item: PriceOfferItem): void {
    item.itemTotalAmount = item.itemPrice * item.quantity;
    this.calculatorAll();
  }
  calculatorAll() {
    this.offer.subTotalAmount = this.offer.itemTotalAmount + this.getSubtotalVases();
    this.offer.vatAmount = this.calculatorPercentage(this.offer.subTotalAmount, this.offer.taxVat);
    this.offer.totalAmount = this.offer.subTotalAmount + this.offer.vatAmount;
    if (!this.feature.isEnabled('App.Carrier')) this.calculatorCommission();
  }

  calculatorCommission() {
    let ItemsComission = this.calculatorItemCommission(this.offer.itemTotalAmount);
    let VasesComission = this.calculatorVasesCommission(this.getSubtotalVases());
    this.offer.commissionAmount = ItemsComission + VasesComission;
  }
  calculatorItemCommission(amount) {
    if (this.offer.commissionType == PriceOfferCommissionType.CommissionPercentage) {
      return this.calculatorPercentage(amount, this.offer.commissionPercentageOrAddValue);
    } else {
      return amount + this.offer.commissionPercentageOrAddValue;
    }
  }

  calculatorVasesCommission(amount: number) {
    if (this.offer.vasCommissionType == PriceOfferCommissionType.CommissionPercentage) {
      return this.calculatorPercentage(amount, this.offer.vasCommissionPercentageOrAddValue);
    } else {
      return amount + this.offer.vasCommissionPercentageOrAddValue;
    }
  }
  getSubtotalVases() {
    return this.Items.reduce((sum, current) => sum + current.itemTotalAmount, 0);
  }

  calculatorPercentage(amount: number, percentage: number): number {
    return (amount * percentage) / 100;
  }
  toCurrencyString(x): string {
    return (Math.round(x * 100) / 100).toFixed(2);
  }
}
