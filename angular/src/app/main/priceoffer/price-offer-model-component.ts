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
        this.changeTripComissionValue();
        this.changeVasComissionValue();
      }
      console.log(this.offer);
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  updateCommissionTypeTitle(): void {
    this.commissionTypeTitle = PriceOfferCommissionType[this.offer.commssionSettings.tripCommissionType];
  }

  changeTripComissionValue() {
    switch (this.offer.commssionSettings.tripCommissionType.toString()) {
      case '1':
        this.offer.commissionPercentageOrAddValue = this.offer.commssionSettings.tripCommissionPercentage;
        break;
      case '2':
        this.offer.commissionPercentageOrAddValue = this.offer.commssionSettings.tripCommissionValue;
        break;
      case '3':
        this.offer.commissionPercentageOrAddValue = this.offer.commssionSettings.tripMinValueCommission;
        break;
    }
    this.updateCommissionTypeTitle();
  }
  changeVasComissionValue() {
    switch (this.offer.commssionSettings.vasCommissionType.toString()) {
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
    this.calculatorAll();
  }
  calculatorItem(item: PriceOfferItem): void {
    item.itemTotalAmount = item.itemPrice * item.quantity;
    this.calculatorAll();
  }
  calculatorAll() {
    this.offer.subTotalAmount = this.offer.itemTotalAmount + this.Items.reduce((sum, current) => sum + current.itemTotalAmount, 0);
    this.offer.vatAmount = (this.offer.subTotalAmount * this.offer.taxVat) / 100;
    this.offer.totalAmount = this.offer.subTotalAmount + this.offer.vatAmount;
    if (!this.feature.isEnabled('App.Carrier')) this.calculatorCommission();
  }
  calculatorCommission() {}
  toCurrencyString(x): string {
    return (Math.round(x * 100) / 100).toFixed(2);
  }
}
