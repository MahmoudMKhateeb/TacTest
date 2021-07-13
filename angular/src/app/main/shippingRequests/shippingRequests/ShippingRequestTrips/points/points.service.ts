import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CreateOrEditRoutPointDto, GetAllGoodsCategoriesForDropDownOutput, GoodsDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';

@Injectable({
  providedIn: 'root',
})
export class PointsService {
  constructor(private _TripService: TripService) {}

  private singleWayPoint = new BehaviorSubject<CreateOrEditRoutPointDto>(new CreateOrEditRoutPointDto());
  currentSingleWayPoint = this.singleWayPoint.asObservable();
  private wayPointsList = new BehaviorSubject<CreateOrEditRoutPointDto[]>([]);
  currentWayPointsList = this.wayPointsList.asObservable();

  /**
   * Takes the single point as an input and updates it
   * @param inComingSinglePointUpdate
   */
  updateSinglePoint(inComingSinglePointUpdate: CreateOrEditRoutPointDto) {
    this.singleWayPoint.next(inComingSinglePointUpdate);
    console.log('Point Updated From Service : ', inComingSinglePointUpdate);
  }

  /**
   * updates Service WayPoints List
   * @param incomingWayPointsList
   */
  updateWayPoints(incomingWayPointsList: CreateOrEditRoutPointDto[]) {
    this.wayPointsList.next(incomingWayPointsList);
  }
}
