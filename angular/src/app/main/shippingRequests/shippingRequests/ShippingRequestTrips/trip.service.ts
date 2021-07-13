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
export class TripService implements OnInit {
  //Shared Facilities to be Used in Multible Components insted Of requesting Them Multible Times
  allFacilities: FacilityForDropdownDto[];
  facilityLoading: boolean;

  //shippingRequest RouteType
  shippingRequest = new BehaviorSubject<GetShippingRequestForViewOutput>(new GetShippingRequestForViewOutput());
  currentShippingRequest = this.shippingRequest.asObservable();
  activeTrip = new BehaviorSubject<CreateOrEditShippingRequestTripDto>(new CreateOrEditShippingRequestTripDto());
  currentActiveTrip = this.activeTrip.asObservable();

  activeTripId = new BehaviorSubject<number>(null);
  currentActiveTripId = this.activeTripId.asObservable();
  constructor(private _routStepsServiceProxy: RoutStepsServiceProxy) {}
  ngOnInit() {
    //let's load facilities
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
      this.facilityLoading = false;
    });
  }

  updateShippingRequest(shippingRequest: GetShippingRequestForViewOutput) {
    this.shippingRequest.next(shippingRequest);
  }
  updateActiveTripId(TripId: number) {
    this.activeTripId.next(TripId);
  }
}
