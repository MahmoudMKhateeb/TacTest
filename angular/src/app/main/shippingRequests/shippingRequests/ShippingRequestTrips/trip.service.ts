import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {
  CreateOrEditShippingRequestTripDto,
  FacilityForDropdownDto,
  GetShippingRequestForViewOutput,
  RoutStepsServiceProxy,
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

  private FacilitiesItems: BehaviorSubject<any> = new BehaviorSubject(new Array<DropDownMenu>());
  currentFacilitiesItems = this.FacilitiesItems.asObservable();

  constructor(private _routStepsServiceProxy: RoutStepsServiceProxy) {
    this.GetOrRefreshFacilities();
  }

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
  //Loads All facilities
  GetOrRefreshFacilities() {
    console.log('Facilities Refreshed');
    this.FacilitiesItems.next({ isLoading: true, items: null });
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.FacilitiesItems.next({ isLoading: false, items: result });
    });
  }
}

export interface DropDownMenu {
  isLoading: boolean;
  items: FacilityForDropdownDto[];
}
