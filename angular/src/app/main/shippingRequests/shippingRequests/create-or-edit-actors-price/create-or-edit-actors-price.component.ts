import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ActorCarrierPriceDto,
  ActorShipperPriceDto,
  ActorsPriceOffersServiceProxy,
  CreateOrEditActorCarrierPriceInput,
  CreateOrEditActorShipperPriceInput,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-create-or-edit-actors-price',
  templateUrl: './create-or-edit-actors-price.component.html',
  styleUrls: ['./create-or-edit-actors-price.component.css'],
})
export class CreateOrEditActorsPriceComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private actorsPriceOffersServiceProxy: ActorsPriceOffersServiceProxy) {
    super(injector);

    this.createOrEditActorShipperPriceInput.actorShipperPriceDto = new ActorShipperPriceDto();
    this.createOrEditActorCarrierPriceInput.actorCarrierPriceDto = new ActorCarrierPriceDto();
  }

  @Input() shippingRequestId: any;
  createOrEditActorShipperPriceInput: CreateOrEditActorShipperPriceInput = new CreateOrEditActorShipperPriceInput();
  createOrEditActorCarrierPriceInput: CreateOrEditActorCarrierPriceInput = new CreateOrEditActorCarrierPriceInput();
  active = true;
  saving: any;

  GetCreateOrEditActorShipperPriceInputForCreateOrEdit() {
    this.actorsPriceOffersServiceProxy.getCreateOrEditActorShipperPriceInputForCreateOrEdit(this.shippingRequestId).subscribe((result) => {
      console.log(result);
      this.createOrEditActorShipperPriceInput = result;
      this.createOrEditActorShipperPriceInput.actorShipperPriceDto = new ActorShipperPriceDto();
    });
  }

  GetCreateOrEditActorCarrierPriceInputForCreateOrEdit() {
    this.actorsPriceOffersServiceProxy.getCreateOrEditActorCarrierPriceInputForCreateOrEdit(this.shippingRequestId).subscribe((result) => {
      console.log(result);
      this.createOrEditActorCarrierPriceInput = result;
      this.createOrEditActorCarrierPriceInput.actorCarrierPriceDto = new ActorCarrierPriceDto();
    });
  }

  save() {
    this.createOrEditActorShipperPriceInput.shippingRequestId = this.shippingRequestId;
    this.saving = true;
    this.actorsPriceOffersServiceProxy
      .createOrEditActorShipperPrice(this.createOrEditActorShipperPriceInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
      });
  }

  calculatePrices(dto: ActorShipperPriceDto) {
    dto.vatAmountWithCommission = dto.subTotalAmountWithCommission * 0.15;
    dto.totalAmountWithCommission = dto.vatAmountWithCommission + dto.subTotalAmountWithCommission;
  }

  calculateCarrierPrices(dto: ActorCarrierPriceDto) {
    dto.vatAmount = dto.subTotalAmount * 0.15;
  }

  saveCarrierPrice() {
    this.createOrEditActorCarrierPriceInput.shippingRequestId = this.shippingRequestId;
    this.saving = true;
    this.actorsPriceOffersServiceProxy
      .createOrEditActorCarrierPrice(this.createOrEditActorCarrierPriceInput)
      .pipe(
        finalize(() => {
          this.saving = false;
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
}
