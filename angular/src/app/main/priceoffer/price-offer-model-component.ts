import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

import {
  CreateOrEditPriceOfferInput,
  CreateSrUpdateActionInputDto,
  PriceOfferChannel,
  PriceOfferCommissionType,
  PriceOfferDetailDto,
  PriceOfferDto,
  PriceOfferItem,
  PriceOfferServiceProxy,
  PriceOfferTenantCommissionSettings,
  ShippingRequestUpdateServiceProxy,
  ShippingRequestUpdateStatus,
  SelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { NgForm } from '@angular/forms';

@Component({
  templateUrl: './price-offer-model-component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'price-offer-model',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class PriceOfferModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<number> = new EventEmitter<number>();
  @Output() offerRepriced = new EventEmitter();
  @Output() postPriceOfferSubmitted = new EventEmitter<CreateOrEditPriceOfferInput>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('Form', { static: false }) form: NgForm;
  ShippingRequestUpdateStatusEnum = ShippingRequestUpdateStatus;
  SRUpdateId: any;

  active = false;
  saving = false;
  offer: PriceOfferDto = new PriceOfferDto();
  input: CreateOrEditPriceOfferInput = new CreateOrEditPriceOfferInput();
  direction: string;
  Items: PriceOfferItem[] = [];
  priceOfferCommissionType: any;
  commissionTypeTitle: string;
  isPostPriceOffer: boolean;
  type: string;
  priceOfferInput: CreateOrEditPriceOfferInput;
  CreateSrUpdateActionInput: CreateSrUpdateActionInputDto = new CreateSrUpdateActionInputDto();
  AllActorsCarriers: SelectItemDto[];
  isForDedicated: boolean;
  ShipperValueOfGoods: Number;
  CarrierInsuranceCoverage: Number;
  hasMatchesPricePackage: boolean;

  constructor(
    injector: Injector,
    private _srUpdateService: ShippingRequestUpdateServiceProxy,
    private _CurrentServ: PriceOfferServiceProxy,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.priceOfferCommissionType = this.enumToArray.transform(PriceOfferCommissionType);
    this.offer.commissionSettings = new PriceOfferTenantCommissionSettings();
    this.isPostPriceOffer = false;
    this.hasMatchesPricePackage = false;
  }

  show(
    id: number,
    SRUpdateId: number | undefined = undefined,
    type: string | undefined = undefined,
    offerId: number | undefined = undefined,
    isPostPriceOffer: boolean = false,
    isForDedicated = false,
    directRequestId?: number
  ): void {
    this.isForDedicated = isForDedicated;
    this.isPostPriceOffer = isPostPriceOffer;

    this._CurrentServ.getAllCarrierActorsForDropDown().subscribe((result) => {
      this.AllActorsCarriers = result;
      // let defaultItem = new SelectItemDto();
      // defaultItem.id = null;
      // defaultItem.displayName = this.l('Myself');
      // this.AllActorsCarriers.unshift(defaultItem);
    });

    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.input.shippingRequestId = id;
    this.type = type;
    this.input.channel = this.Channel;
    this.SRUpdateId = SRUpdateId;
    this._CurrentServ.getPriceOfferForCreateOrEdit(id, offerId, directRequestId).subscribe((result) => {
      this.Items = result.items;
      this.ShipperValueOfGoods = result.shipperValueOfGoods;
      this.CarrierInsuranceCoverage = result.carrierIsuranceCoverage;
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
      this.input.shippingRequestId = id;
      this.input.channel = this.Channel;
      this.input.carrierActorId = this.offer.carrierActorId;
      this.hasMatchesPricePackage = this.offer.hasMatchedPricePackage;
      if (!this.feature.isEnabled('App.Carrier') && this.offer.id == 0) {
        this.changeItemComissionValue();
        this.changeVasComissionValue();
      }

      if (type == 'Reprice') {
        this.offer = new PriceOfferDto();
        this.offer.quantity = result.quantity;
        this.Items = result.items;
        this.Items.forEach((r) => {
          r.itemPrice = undefined;
          r.itemsTotalPricePreCommissionPreVat = undefined;
        });
      }
      if (this.hasMatchesPricePackage) {
        this.calculator();
      }
      (this.offer.carrierActorId as any) = this.offer.carrierActorId?.toString();
      this.active = true;
      this.modal.show();
    });
  }

  close(): void {
    this.active = false;
    this.hasMatchesPricePackage = false;
    this.modal.hide();
  }

  changeItemComissionValue() {
    switch (this.offer.commissionType.toString()) {
      case '1':
        this.offer.commissionPercentageOrAddValue = this.offer.commissionSettings.itemCommissionPercentage;
        break;
      case '2':
        this.offer.commissionPercentageOrAddValue = this.offer.commissionSettings.itemCommissionValue;
        break;
      case '3':
        this.offer.commissionPercentageOrAddValue = this.offer.commissionSettings.itemMinValueCommission;
        break;
    }
  }

  changeVasComissionValue() {
    switch (this.offer.vasCommissionType.toString()) {
      case '1':
        this.offer.vasCommissionPercentageOrAddValue = this.offer.commissionSettings.vasCommissionPercentage;
        break;
      case '2':
        this.offer.vasCommissionPercentageOrAddValue = this.offer.commissionSettings.vasCommissionValue;
        break;
      case '3':
        this.offer.vasCommissionPercentageOrAddValue = this.offer.commissionSettings.vasMinValueCommission;
        break;
    }
  }

  confirmSendOffer() {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.sendOffer();
      }
    });
  }

  Reprice(id) {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.sendOfferForReprice(id);
      }
    });
  }

  sendOffer(): void {
    // this.calculatorAll();
    let itemDetails: PriceOfferDetailDto[] = [];

    this.Items.forEach((item) => {
      let order = new PriceOfferDetailDto();
      order.itemId = item.sourceId;
      order.price = item.itemPrice;
      itemDetails.push(order);
    });

    this.input.itemPrice = this.offer.itemPrice;
    if (this.offer.parentId) this.input.parentId = this.offer.parentId;
    console.log('this.offer.parentId', this.offer.parentId);
    this.input.itemDetails = itemDetails;
    this.input.commissionPercentageOrAddValue = this.offer.commissionPercentageOrAddValue;
    this.input.commissionType = this.offer.commissionType;
    this.input.vasCommissionPercentageOrAddValue = this.offer.vasCommissionPercentageOrAddValue;
    this.input.vasCommissionType = this.offer.vasCommissionType;
    this.input.carrierActorId = this.offer.carrierActorId;

    if (this.isPostPriceOffer) {
      this.postPriceOfferSubmitted.emit(this.input);
      this.isPostPriceOffer = false;
      this.close();
      return;
    }

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

  sendOfferForReprice(id): void {
    this.CreateSrUpdateActionInput.status = ShippingRequestUpdateStatus.Repriced;

    let itemDetails: PriceOfferDetailDto[] = [];

    this.Items.forEach((item) => {
      let order = new PriceOfferDetailDto();
      order.itemId = item.sourceId;
      order.price = item.itemPrice;
      itemDetails.push(order);
    });

    this.input.itemPrice = this.offer.itemPrice;
    if (this.offer.parentId) this.input.parentId = this.offer.parentId;
    this.input.itemDetails = itemDetails;
    this.input.commissionPercentageOrAddValue = this.offer.commissionPercentageOrAddValue;
    this.input.commissionType = this.offer.commissionType;
    this.input.vasCommissionPercentageOrAddValue = this.offer.vasCommissionPercentageOrAddValue;
    this.input.vasCommissionType = this.offer.vasCommissionType;

    this.CreateSrUpdateActionInput.priceOfferInput = this.input;
    this.CreateSrUpdateActionInput.id = id;
    this._srUpdateService
      .takeAction(this.CreateSrUpdateActionInput)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.offerRepriced.emit();
        this.notify.info(this.l('SendSuccessfully'));
        this.close();
      });

    this.saving = true;
  }

  calculator(): void {
    // this.offer.itemTotalAmount = this.offer.itemPrice * this.offer.quantity;
    // this.offer.itemCommissionAmount = this.calculatorItemCommission(this.offer.itemPrice);
    // this.offer.itemSubTotalAmountWithCommission = this.offer.itemCommissionAmount * this.offer.quantity;
    // this.calculatorAll();
    //if (this.form.form.valid) {
    if (this.offer.itemPrice) {
      this.initPriceOffer();
    }
    //}
  }

  // calculatorItem(item: PriceOfferItem): void {
  //   item.itemTotalAmount = item.itemPrice * item.quantity;
  //   this.calculatorAll();
  // }

  // calculatorAll() {
  //   this.offer.subTotalAmount = this.offer.itemTotalAmount + this.getSubtotalVases();
  //   this.offer.vatAmount = this.calculatorPercentage(this.offer.subTotalAmount, this.offer.taxVat);
  //   this.offer.totalAmount = this.offer.subTotalAmount + this.offer.vatAmount;
  //   if (!this.feature.isEnabled('App.Carrier')) this.calculatorCommission();
  // }

  // calculatorCommission() {
  //   let ItemsComission = this.calculatorItemCommission(this.offer.itemTotalAmount);
  //   let VasesComission = this.calculatorVasesCommission(this.getSubtotalVases());
  //   this.offer.commissionAmount = this.offer.itemSubTotalAmountWithCommission + VasesComission;
  // }

  // calculatorItemCommission(amount) {
  //   if (this.offer.commissionType == PriceOfferCommissionType.CommissionPercentage) {
  //     return this.calculatorPercentage(amount, this.offer.commissionPercentageOrAddValue);
  //   } else {
  //     return this.offer.commissionPercentageOrAddValue;
  //   }
  // }

  // calculatorVasesCommission(amount: number) {
  //   if (this.offer.vasCommissionType == PriceOfferCommissionType.CommissionPercentage) {
  //     return this.calculatorPercentage(amount, this.offer.vasCommissionPercentageOrAddValue);
  //   } else {
  //     return this.gettotalQytVases() * this.offer.vasCommissionPercentageOrAddValue;
  //   }
  // }
  // getSubtotalVases() {
  //   return this.Items.reduce((sum, current) => sum + current.itemTotalAmount, 0);
  // }
  // gettotalQytVases() {
  //   let total = this.Items.reduce((sum, current) => sum + current.quantity, 0);
  //   return this.Items.reduce((sum, current) => sum + current.quantity, 0);
  // }
  // calculatorPercentage(amount: number, percentage: number): number {
  //   return (amount * percentage) / 100;
  // }
  // toCurrencyString(x): string {
  //   return (Math.round(x * 100) / 100).toFixed(2);
  // }

  initPriceOffer() {
    let itemDetails: PriceOfferDetailDto[] = [];

    this.Items.forEach((item) => {
      let order = new PriceOfferDetailDto();
      order.itemId = item.sourceId;
      order.price = item.itemPrice;
      itemDetails.push(order);
    });

    this.input.itemPrice = this.offer.itemPrice;
    this.input.itemDetails = itemDetails;
    this.input.commissionPercentageOrAddValue = this.offer.commissionPercentageOrAddValue;
    this.input.commissionType = this.offer.commissionType;
    this.input.vasCommissionPercentageOrAddValue = this.offer.vasCommissionPercentageOrAddValue;
    this.input.vasCommissionType = this.offer.vasCommissionType;
    this.input.isPostPrice = this.isPostPriceOffer;
    const parentId = this.offer.parentId;
    this._CurrentServ.initPriceOffer(this.input).subscribe((result) => {
      this.offer = result;
      this.offer.parentId = parentId;
      this.Items = this.offer.items;
      console.log(this.offer);
    });
  }
}
