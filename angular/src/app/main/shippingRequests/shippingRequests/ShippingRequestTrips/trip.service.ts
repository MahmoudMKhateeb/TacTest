import { Injectable, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {
  CreateOrEditShippingRequestTripDto,
  FacilityForDropdownDto,
  GetShippingRequestForViewOutput,
  RoutStepsServiceProxy,
  ShippingRequestDto,
} from '@shared/service-proxies/service-proxies';

@Injectable({
  providedIn: 'root',
})
export class TripService {
  //shippingRequest RouteType
  private shippingRequest = new BehaviorSubject<GetShippingRequestForViewOutput>(new GetShippingRequestForViewOutput());
  currentShippingRequest = this.shippingRequest.asObservable();
  private activeTrip = new BehaviorSubject<CreateOrEditShippingRequestTripDto>(new CreateOrEditShippingRequestTripDto());
  currentActiveTrip = this.activeTrip.asObservable();
  private activeTripId = new BehaviorSubject<number>(null);
  currentActiveTripId = this.activeTripId.asObservable();
  //Source And Dest Facility
  private sourceFacility = new BehaviorSubject<number>(null);
  currentSourceFacility = this.sourceFacility.asObservable();
  private destFacility = new BehaviorSubject<number>(null);
  currentDestFacility = this.destFacility.asObservable();

  constructor() {}

  updateShippingRequest(shippingRequest: GetShippingRequestForViewOutput) {
    this.shippingRequest.next(shippingRequest);
  }
  updateActiveTripId(TripId: number) {
    this.activeTripId.next(TripId);
  }
  updateActiveTrip(Trip: CreateOrEditShippingRequestTripDto) {
    this.activeTrip.next(Trip);
  }

  //Source And Dest Facility
  updateSourceFacility(id: number) {
    this.sourceFacility.next(id);
  }
  updateDestFacility(id: number) {
    this.destFacility.next(id);
  }
}
