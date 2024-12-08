/* tslint:disable:triple-equals */
import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  CreateOrEditPriceOfferInput,
  GetOfferForViewOutput,
  PriceOfferChannel,
  PriceOfferItem,
  PriceOfferServiceProxy,
  PriceOfferStatus,
  PriceOfferViewDto,
  ShippingRequestStatus,
} from '@shared/service-proxies/service-proxies';
import { OfferPostPriceResponse } from '../shippingRequests/shippingRequests/srpost-price-update/offer-post-price-response';

@Component({
  templateUrl: './price-offer-view-model-component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'price-offer-view-model',
  animations: [appModuleAnimation()],
  styles: [
    `
      .custom-primary-outline {
        color: var(--primary, #da1a32) !important;
        background: var(--white, #fff) !important;
      }
    `,
  ],
})
export class PriceOfferViewModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Output() modalDelete: EventEmitter<any> = new EventEmitter<any>();
  @Output() modalRefresh: EventEmitter<any> = new EventEmitter<any>();
  @Output() modalPostPriceResoponse = new EventEmitter<OfferPostPriceResponse>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  offerForEditOutput: GetOfferForViewOutput = new GetOfferForViewOutput();
  input: CreateOrEditPriceOfferInput = new CreateOrEditPriceOfferInput();
  Items: PriceOfferItem[] = [];
  isPostPriceOffer: boolean;
  isForDedicated: boolean;
  ShipperValueOfGoods: Number;
  CarrierInsuranceCoverage: Number;
  constructor(injector: Injector, private _CurrentServ: PriceOfferServiceProxy) {
    super(injector);
    this.offerForEditOutput.priceOfferViewDto = new PriceOfferViewDto();
  }

  show(shippingRequestId: number, offerId: number, isPostPriceOffer: boolean = false, isForDedicated = false): void {
    this.isForDedicated = isForDedicated;
    this.isPostPriceOffer = isPostPriceOffer;
    this._CurrentServ.getPriceOfferForView(offerId).subscribe((result) => {
      this.offerForEditOutput = result;
      this.Items = this.offerForEditOutput.priceOfferViewDto.items;
      this.active = true;
      this.modal.show();
      this.input.shippingRequestId = shippingRequestId;
      this.input.channel = this.Channel;
      this.ShipperValueOfGoods = this.offerForEditOutput.priceOfferViewDto.shipperValueOfGoods;
      this.CarrierInsuranceCoverage = this.offerForEditOutput.priceOfferViewDto.carrierInsuranceCoverage;
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
    this.isPostPriceOffer = false;
  }

  delete(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.delete(this.offerForEditOutput.priceOfferViewDto.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.modalDelete.emit(null);
          this.close();
        });
      }
    });
  }

  acceptoffer(): void {
    if (this.isPostPriceOffer) {
      this.modalPostPriceResoponse.emit(new OfferPostPriceResponse(false, undefined));
      this.close();
      return;
    }

    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.accept(this.offerForEditOutput.priceOfferViewDto.id).subscribe((result) => {
          this.notify.success(this.l('SuccessfullyAccepted'));
          this.offerForEditOutput.priceOfferViewDto.status = result;
          this.modalRefresh.emit(true);
          //this.modalDelete.emit(null);
          this.close();
        });
      }
    });
  }
  accepTMStoffer(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.acceptOfferOnBehalfShipper(this.offerForEditOutput.priceOfferViewDto.id).subscribe((result) => {
          this.notify.success(this.l('SuccessfullyAccepted'));
          this.offerForEditOutput.priceOfferViewDto.status = result;
          this.modalRefresh.emit(null);
          this.close();
        });
      }
    });
  }
  SendOffer(): void {}
  CancelAccepted(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.cancel(this.offerForEditOutput.priceOfferViewDto.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyCanceled'));
          this.offerForEditOutput.priceOfferViewDto.status = PriceOfferStatus.New;
          //this.modalDelete.emit(null);
          this.close();
        });
      }
    });
  }
  canSendOfferOrCancel() {
    if (
      this.offerForEditOutput.priceOfferViewDto.shippingRequestStatus == ShippingRequestStatus.NeedsAction &&
      this.offerForEditOutput.priceOfferViewDto.status == PriceOfferStatus.Pending
    ) {
      if (this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) {
        return true;
      }
    }
    return false;
  }

  canDeleteOffer() {
    if (
      this.offerForEditOutput.priceOfferViewDto.shippingRequestStatus == ShippingRequestStatus.NeedsAction &&
      this.offerForEditOutput.priceOfferViewDto.status == PriceOfferStatus.New
    ) {
      if ((this.feature.isEnabled('App.TachyonDealer') || !this.appSession.tenantId) && this.offerForEditOutput.priceOfferViewDto.editionId == 4) {
        return true;
      } else if (this.feature.isEnabled('App.Carrier')) {
        return !this.offerForEditOutput.hasMatchedPricePackage;
      }
    }
    return false;
  }
  reject(reason: string) {
    this.offerForEditOutput.priceOfferViewDto.status = PriceOfferStatus.Rejected;
    this.offerForEditOutput.priceOfferViewDto.rejectedReason = reason;
    this.modalRefresh.emit(null);
    this.close();
  }

  postPriceOfferReject(reason: string) {
    this.modalPostPriceResoponse.emit(new OfferPostPriceResponse(true, reason));
    this.close();
  }
}
