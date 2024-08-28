import { Injectable } from '@angular/core';
import { BehaviorSubject, of } from 'rxjs';
import { CreateOrEditRoutPointDto, GetShippingRequestForViewOutput, RoutPointDto } from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { HttpHeaders, HttpClientModule, HttpClient } from '@angular/common/http';
import { map } from '@node_modules/rxjs/internal/operators';
import { catchError } from 'rxjs/operators';
import { Observable } from '@node_modules/rxjs';

@Injectable({
  providedIn: 'root',
})
export class PointsService {
  currentShippingRequest: GetShippingRequestForViewOutput;
  currentPointIndex: number;
  constructor(private _TripService: TripService, private http: HttpClient) {}

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

  /**
   * this method is for Saab-->ReachWere integration for checking the water Qnty
   */
  checkAvailability(Quantity: number): Observable<boolean> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    console.log(Quantity, 'QuantityQuantityQuantityQuantityQuantityQuantity');
    const body = {
      subGoodsCategoryName: 'Water',
      Quantity: Quantity,
      isMarkedAsSaleOrder: true,
    };

    const url = 'https://api.reachware.com/tachyon/CheckGoodsAvailability';

    return this.http.post<any>(url, body, { headers: headers, withCredentials: false }).pipe(
      map((response) => {
        console.log('Goods availability response:', response);
        return response.isAvailable === true;
      }),
      catchError((error) => {
        console.error('Error checking goods availability:', error);
        return of(false); // return false in case of an error
      })
    );
  }
}
