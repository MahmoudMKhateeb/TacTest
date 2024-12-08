import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ActorsPriceOffersServiceProxy,
  CreateOrEditActorCarrierPrice,
  CreateOrEditActorShipperPriceDto,
  CreateOrEditSrActorCarrierPriceInput,
  CreateOrEditSrActorShipperPriceInput,
  SaasPricePackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { NgForm } from '@angular/forms';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';

@Component({
  selector: 'app-create-or-edit-actors-price',
  templateUrl: './create-or-edit-actors-price.component.html',
  styleUrls: ['./create-or-edit-actors-price.component.css'],
})
export class CreateOrEditActorsPriceComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditActorsPricesModal', { static: true }) modal: ModalDirective;
  @ViewChild('createOrEditShipperPriceForm', { static: false }) createOrEditShipperPriceForm: NgForm;
  @ViewChild('createOrEditCarrierPriceForm', { static: false }) createOrEditCarrierPriceForm: NgForm;
  @Input() shippingRequestId: any;
  @Input() numberOfCreatedTrips: number;
  @Input() tenantId: number;
  @Input() carrierTenantId: number;
  @Input() actorCarrierId: number;
  @Input() actorShipperId: number;

  shipperPriceInput = new CreateOrEditSrActorShipperPriceInput();
  carrierPriceInput = new CreateOrEditSrActorCarrierPriceInput();
  active = true;
  shipperSaving: any;
  carrierSaving: any;

  constructor(
    injector: Injector,
    private actorsPriceOffersServiceProxy: ActorsPriceOffersServiceProxy,
    private _TripService: TripService,
    private _saasPricePackageServiceProxy: SaasPricePackageServiceProxy
  ) {
    super(injector);

    this.shipperPriceInput.actorShipperPriceDto = new CreateOrEditActorShipperPriceDto();
    this.carrierPriceInput.actorCarrierPrice = new CreateOrEditActorCarrierPrice();
    this.shipperSaving = false;
    this.carrierSaving = false;
  }

  ngOnInit(): void {
    this.GetCreateOrEditActorShipperPriceInputForCreateOrEdit();
    this.GetCreateOrEditActorCarrierPriceInputForCreateOrEdit();
  }

  show(): void {
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

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

  canAddCarrierActorPrices(): boolean {
    return (
      this.hasCarrierClients &&
      (this.carrierTenantId === this.appSession.tenantId || this.tenantId === this.appSession.tenantId) &&
      isNotNullOrUndefined(this.actorCarrierId) &&
      !this.carrierPriceInput.isMyselfActor
    );
  }

  canAddShipperActorPrices(): boolean {
    return (
      this.hasShipperClients &&
      (this.carrierTenantId === this.appSession.tenantId || this.tenantId === this.appSession.tenantId) &&
      isNotNullOrUndefined(this.actorShipperId) &&
      !this.shipperPriceInput.isMyselfActor
    );
  }

  saveAllPrices() {
    if (isNotNullOrUndefined(this.createOrEditShipperPriceForm) && this.createOrEditShipperPriceForm.form.valid) {
      this.save();
    }
    if (isNotNullOrUndefined(this.createOrEditCarrierPriceForm) && this.createOrEditCarrierPriceForm.form.valid) {
      this.saveCarrierPrice();
    }
    this.close();
  }

  getPriceFromPricePackage() {
    let actorShipperId = this._TripService?.GetShippingRequestForViewOutput?.shipperActorId;
    let originCityId = this._TripService?.GetShippingRequestForViewOutput?.originalCityId;
    let destinationCity = null;
    let truckType = this._TripService?.GetShippingRequestForViewOutput?.truckTypeId;
    let ShippingType = this._TripService?.GetShippingRequestForViewOutput?.shippingRequest.shippingTypeId || undefined;
    let GoodCat = this._TripService?.GetShippingRequestForViewOutput?.goodCategoryId;
    //let loadingType = this._TripService?.GetShippingRequestForViewOutput?.lo;
    let RoundTripType = this._TripService?.GetShippingRequestForViewOutput?.roundTripType || undefined;

    if (ShippingType == 1) {
      destinationCity = originCityId;
    } else {
      if (this._TripService?.GetShippingRequestForViewOutput?.destinationCitiesDtos?.length > 0) {
        destinationCity = this._TripService?.GetShippingRequestForViewOutput?.destinationCitiesDtos[0]?.cityId || null;
      }
    }
    let validation = true;

    if (validation) {
      this._saasPricePackageServiceProxy
        .getForPricing(actorShipperId, originCityId, destinationCity, undefined, truckType, ShippingType, GoodCat, undefined, RoundTripType)
        .subscribe((result) => {
          if (!result) {
            this.notify.info(this.l('cantFindMatchingPricePackages'));
          }
          this.shipperPriceInput.actorShipperPriceDto.subTotalAmountWithCommission = result;
          this.calculatePrices(this.shipperPriceInput.actorShipperPriceDto);
        });
    }
  }
}
