import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CreateOrEditRoutPointDto, GetShippingRequestForViewOutput, RoutPointDto } from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';

@Injectable({
  providedIn: 'root',
})
export class PointsService {
  currentShippingRequest: GetShippingRequestForViewOutput;
  currentPointIndex: number;
  constructor(private _TripService: TripService) {}

  private singleWayPoint = new BehaviorSubject<CreateOrEditRoutPointDto>(new CreateOrEditRoutPointDto());
  currentSingleWayPoint = this.singleWayPoint.asObservable();
  private wayPointsList = new BehaviorSubject<any[]>([]);
  currentWayPointsList = this.wayPointsList.asObservable();

  private usedIn = new BehaviorSubject<any>('view');
  currentUsedIn = this.usedIn.asObservable();

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
  updateWayPoints(incomingWayPointsList: CreateOrEditRoutPointDto[] | RoutPointDto[]) {
    this.wayPointsList.next(incomingWayPointsList);
  }

  /**
   * Detects witch Component is using Points Service/Component
   * @param usedIn
   */
  updateCurrentUsedIn(usedIn: 'view' | 'createOrEdit') {
    console.log('mode is : ', usedIn);
    this.usedIn.next(usedIn);
  }
}
