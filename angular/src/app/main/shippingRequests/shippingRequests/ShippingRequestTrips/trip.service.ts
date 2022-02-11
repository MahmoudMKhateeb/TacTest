import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {
  CreateOrEditShippingRequestTripDto,
  FacilityForDropdownDto,
  GetShippingRequestForViewOutput,
  RoutStepsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';

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

  //private FacilitiesItems: BehaviorSubject<any> = new BehaviorSubject(new Array<DropDownMenu>());
  //currentFacilitiesItems = this.FacilitiesItems.asObservable();

  public currentSourceFacilitiesItems: FacilityForDropdownDto[];
  public currentDestinationFacilitiesItems: FacilityForDropdownDto[];

  facilitiesLodaing: boolean;
  citySourceId: number;
  cityDestenationId: number;
  shippingTypeId: number;
  routeTypeId: number;

  constructor(private _routStepsServiceProxy: RoutStepsServiceProxy, private feature: FeatureCheckerService) {
    //this.GetOrRefreshFacilities();
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
  GetOrRefreshFacilities(shippingRequestId: number) {
    this.facilitiesLodaing = true;
    if (this.feature.isEnabled('App.Shipper') || this.feature.isEnabled('App.TachyonDealer')) {
      if (shippingRequestId != null && shippingRequestId != undefined) {
        this.currentShippingRequest.subscribe((res) => {
          this.citySourceId = res.originalCityId;
          this.cityDestenationId = res.destinationCityId;
          this.shippingTypeId = res.shippingRequest.shippingTypeId;
          this.routeTypeId = res.shippingRequest.routeTypeId;
        });
        this._routStepsServiceProxy.getAllFacilitiesByCityAndTenantForDropdown(shippingRequestId).subscribe((result) => {
          if (this.shippingTypeId == 1) {
            //inside city
            this.currentSourceFacilitiesItems = this.currentDestinationFacilitiesItems = result;
          } else {
            //outside side
            this.currentSourceFacilitiesItems = result.filter((r) => r.cityId == this.citySourceId);
            this.currentDestinationFacilitiesItems = result.filter((r) => r.cityId == this.cityDestenationId);
          }
          this.facilitiesLodaing = false;
        });
      } else {
        this._routStepsServiceProxy.getAllFacilitiesForDropdown(shippingRequestId).subscribe((result) => {
          this.currentSourceFacilitiesItems = this.currentDestinationFacilitiesItems = result;
          this.facilitiesLodaing = false;
        });
      }
    }
  }
}

export interface DropDownMenu {
  isLoading: boolean;
  items: FacilityForDropdownDto[];
}
