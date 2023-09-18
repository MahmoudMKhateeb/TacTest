import { Injectable } from '@angular/core';
import * as moment from '@node_modules/moment';

@Injectable({
  providedIn: 'root',
})
export class EtaCalculatorService {
  constructor() {}

  /**
   * calculates Distance and time between 2 points
   * @param point1
   * @param point2
   */
  calculateDistance(point1, point2): Promise<google.maps.DistanceMatrixResponseElement> {
    return new Promise<google.maps.DistanceMatrixResponseElement>((resolve, reject) => {
      const service = new google.maps.DistanceMatrixService();
      // build request
      const origin = { lat: point1.lat || point1.latitude, lng: point1.lng || point1.longitude };
      const destination = { lat: point2.lat || point2.latitude, lng: point2.lng || point2.longitude };

      const request = {
        origins: [origin],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false,
      };

      service.getDistanceMatrix(request, (response) => {
        const googleResponse = response.rows[0].elements[0];
        resolve(googleResponse);
      });
    });
  }

  /**
   * check if the trip is Delayed or not
   * @param tripExpectedArrivalDate
   */
  isTripDelayed(tripExpectedArrivalDate): string {
    if (tripExpectedArrivalDate === '') {
      return 'Unknown';
    }
    const todayMoment = moment();
    const momentExpected = moment(tripExpectedArrivalDate);
    if (momentExpected.isSameOrAfter(todayMoment)) {
      return 'OnTime';
    } else if (momentExpected.isBefore(todayMoment)) {
      return 'Delayed';
    }
  }
}
