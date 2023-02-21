import { Injectable } from '@angular/core';
import {
  CreateOrEditActorCarrierPrice,
  CreateOrEditActorShipperPriceDto,
  CreateOrEditShippingRequestTripDto,
  GetShippingRequestForViewOutput,
  RoutStepsServiceProxy,
} from '@shared/service-proxies/service-proxies';

@Injectable({
  providedIn: 'root',
})
export class TripService {
  public GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();
  public CreateOrEditShippingRequestTripDto = new CreateOrEditShippingRequestTripDto();

  public activeTripId: number = null;

  public currentSourceFacility: number = null;
  public destFacility: number = null;

  constructor(private _routStepsServiceProxy: RoutStepsServiceProxy) {
    this.CreateOrEditShippingRequestTripDto.actorShipperPrice = new CreateOrEditActorShipperPriceDto();
    this.CreateOrEditShippingRequestTripDto.actorCarrierPrice = new CreateOrEditActorCarrierPrice();
  }
}
