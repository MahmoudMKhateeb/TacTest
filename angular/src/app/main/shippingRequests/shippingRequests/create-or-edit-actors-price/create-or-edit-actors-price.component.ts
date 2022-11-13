import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ActorsPriceOffersServiceProxy,
  CreateOrEditActorCarrierPrice,
  CreateOrEditActorShipperPriceDto,
  CreateOrEditSrActorCarrierPriceInput,
  CreateOrEditSrActorShipperPriceInput,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-create-or-edit-actors-price',
  templateUrl: './create-or-edit-actors-price.component.html',
  styleUrls: ['./create-or-edit-actors-price.component.css'],
})
export class CreateOrEditActorsPriceComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private actorsPriceOffersServiceProxy: ActorsPriceOffersServiceProxy) {
    super(injector);

    this.shipperPriceInput.actorShipperPriceDto = new CreateOrEditActorShipperPriceDto();
    this.carrierPriceInput.actorCarrierPrice = new CreateOrEditActorCarrierPrice();
    this.shipperSaving = false;
    this.carrierSaving = false;
  }

  @Input() shippingRequestId: any;
  @Input() numberOfCreatedTrips: number;
  @Input() tenantId: number;
  @Input() carrierTenantId: number;

  shipperPriceInput = new CreateOrEditSrActorShipperPriceInput();
  carrierPriceInput = new CreateOrEditSrActorCarrierPriceInput();
  active = true;
  shipperSaving: any;
  carrierSaving: any;

  GetCreateOrEditActorShipperPriceInputForCreateOrEdit() {
    this.actorsPriceOffersServiceProxy.getActorShipperPriceForEdit(this.shippingRequestId).subscribe((result) => {
      this.shipperPriceInput = result;
      if (!isNotNullOrUndefined(this.shipperPriceInput.actorShipperPriceDto)) {
        this.shipperPriceInput.actorShipperPriceDto = new CreateOrEditActorShipperPriceDto();
      }
    });
  }

  GetCreateOrEditActorCarrierPriceInputForCreateOrEdit() {
    this.actorsPriceOffersServiceProxy.getActorCarrierPriceForEdit(this.shippingRequestId).subscribe((result) => {
      this.carrierPriceInput = result;
      if (!isNotNullOrUndefined(this.carrierPriceInput.actorCarrierPrice)) {
        this.carrierPriceInput.actorCarrierPrice = new CreateOrEditActorCarrierPrice();
      }
    });
  }

  save() {
    this.shipperPriceInput.actorShipperPriceDto.shippingRequestId = this.shippingRequestId;
    this.shipperSaving = true;
    this.actorsPriceOffersServiceProxy
      .createOrEditActorShipperPrice(this.shipperPriceInput)
      .pipe(
        finalize(() => {
          this.shipperSaving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
      });
  }

  calculatePrices(dto: CreateOrEditActorShipperPriceDto) {
    dto.vatAmountWithCommission = dto.subTotalAmountWithCommission * 0.15;
    dto.totalAmountWithCommission = dto.vatAmountWithCommission + dto.subTotalAmountWithCommission;
  }

  calculateCarrierPrices(dto: CreateOrEditActorCarrierPrice) {
    dto.vatAmount = dto.subTotalAmount * 0.15;
  }

  saveCarrierPrice() {
    this.carrierPriceInput.actorCarrierPrice.shippingRequestId = this.shippingRequestId;
    this.carrierSaving = true;
    this.actorsPriceOffersServiceProxy
      .createOrEditActorCarrierPrice(this.carrierPriceInput)
      .pipe(
        finalize(() => {
          this.carrierSaving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
      });
  }

  ngOnInit(): void {
    this.GetCreateOrEditActorShipperPriceInputForCreateOrEdit();
    this.GetCreateOrEditActorCarrierPriceInputForCreateOrEdit();
  }

  canAddCarrierActorPrices(): boolean {
    return (
      this.hasCarrierClients &&
      this.tenantId === this.carrierTenantId &&
      this.tenantId === this.appSession.tenantId
    );
  }

  canAddShipperActorPrices(): boolean {
    return (
      this.hasShipperClients &&
      this.tenantId === this.carrierTenantId &&
      this.tenantId === this.appSession.tenantId
    );
  }
}
