import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CarrirSetPriceForDirectRequestDto,
  CreateOrEditTachyonPriceOfferDto,
  ShippingRequestAmountDto,
  ShippingRequestsTachyonDealerServiceProxy,
  TachyonDealerCreateDirectOfferToCarrirerInuptDto,
  TachyonPriceOffersServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';
import Swal from 'sweetalert2';

@Component({
  selector: 'tachyonDealerResponceModel',
  templateUrl: './tachyonDealerResponceModel.component.html',
})
export class TachyonDealerResponceModelComponent extends AppComponentBase implements OnInit {
  @ViewChild('tachyonDealerResponceModal', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();
  directRequestId: number;
  bidId: number;

  carrierPriceResponse: CarrirSetPriceForDirectRequestDto = new CarrirSetPriceForDirectRequestDto();
  saving = false;
  loading = false;
  disAllowEdits: boolean;
  commetionCalculations: ShippingRequestAmountDto = new ShippingRequestAmountDto();
  createOfferInput: CreateOrEditTachyonPriceOfferDto = new CreateOrEditTachyonPriceOfferDto();
  shippingRequestId: any;
  constructor(
    injector: Injector,
    private _shippingRequestsTachyonDealer: ShippingRequestsTachyonDealerServiceProxy,
    private _tachyonPriceOffers: TachyonPriceOffersServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {}

  show(directRequestId, shippingRequestId, bidId?): void {
    console.log(`direntRequestId ${directRequestId} shippingRequestId`);
    this.disAllowEdits = true;
    this.directRequestId = directRequestId;
    this.bidId = bidId;
    this.shippingRequestId = shippingRequestId;
    this.getCommetion(directRequestId, bidId);
    this.modal.show();
  }
  close(): void {
    this.commetionCalculations = new ShippingRequestAmountDto();
    this.modal.hide();
  }

  getCommetion(directRequestid: number, bidId?: number): void {
    this.loading = true;
    this._shippingRequestsTachyonDealer.getCarrierPricing(directRequestid, bidId).subscribe((result) => {
      this.commetionCalculations = result;
      this.loading = false;
    });
  }
  CalculateVat(amount, vat) {
    return (amount / 100) * vat;
  }

  calculateTotalComm() {
    this.commetionCalculations.totalCommission =
      (this.commetionCalculations.carrierPrice * this.commetionCalculations.actualPercentCommission) / 100 +
      this.commetionCalculations.actualCommissionValue;
    this.commetionCalculations.subTotalAmount = this.commetionCalculations.carrierPrice + this.commetionCalculations.totalCommission;
    this.commetionCalculations.vatAmount = this.CalculateVat(this.commetionCalculations.subTotalAmount, this.commetionCalculations.vatSetting);
    this.commetionCalculations.totalAmount = this.commetionCalculations.subTotalAmount + this.commetionCalculations.vatAmount;
    if (this.commetionCalculations.totalCommission < this.commetionCalculations.minCommissionValueSetting) {
      Swal.fire({
        title: 'yourTotalCommissionPriceIsLessThanminCommissionValue',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: this.l('confirm'),
      });
      // this.notify.error('PleaseNotThatYourCommestionValueisLess');
    }
  }

  allowEdits() {
    return (this.disAllowEdits = !this.disAllowEdits);
  }

  acceptCarrierRequest() {
    this.saving = true;
    this.createOfferInput.shippingRequestId = this.shippingRequestId;
    this.createOfferInput.shippingRequestBidId = this.bidId;
    this.createOfferInput.driectRequestForCarrierId = this.directRequestId;
    // this.createOfferInput.carrirerTenantId = this.commetionCalculations.carrirerTenantId;
    // this.createOfferInput.carrierPrice = this.commetionCalculations.carrierPrice;
    this.createOfferInput.totalAmount = this.commetionCalculations.totalAmount;
    this.createOfferInput.actualCommissionValue = this.commetionCalculations.actualCommissionValue;
    this.createOfferInput.actualPercentCommission = this.commetionCalculations.actualPercentCommission;
    this._tachyonPriceOffers
      .createOrEditTachyonPriceOffer(this.createOfferInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.success(this.l('success'));
        this.modalsave.emit('');
        this.close();
      });
  }

  validateCommision() {
    Swal.fire({
      title: this.l('pleaseConfirmthisAction'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('confirm'),
      cancelButtonText: this.l('close'),
    }).then((result) => {
      if (result.value) {
        this.acceptCarrierRequest();
      }
    });
  }
}
