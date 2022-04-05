import { AngularFireDatabase, AngularFireList } from '@angular/fire/compat/database';
import { Observable } from 'rxjs';
import { map } from '@node_modules/rxjs/internal/operators';
import { TrackingListDto, TrackingRoutePointDto } from '@shared/service-proxies/service-proxies';

export const trackingIconsList = {
  driverIcon: {
    url: 'https://img.icons8.com/external-victoruler-flat-gradient-victoruler/64/000000/external-delivery-courier-food-and-delivery-victoruler-flat-gradient-victoruler.png',
    scaledSize: {
      width: 40,
      height: 40,
    },
  },
  truckIcon: {
    url: 'https://img.icons8.com/external-justicon-flat-justicon/64/000000/external-truck-transportation-justicon-flat-justicon.png',
    scaledSize: {
      width: 40,
      height: 40,
    },
  },
  offlineDriverIcon: {
    url: 'https://img.icons8.com/dusk/344/wifi-off.png',
    scaledSize: {
      width: 40,
      height: 40,
    },
  },
};

export interface DriverLocation {
  lng: number;
  lat: number;
}

export class FirebaseHelperClass {
  private fireDB: AngularFireList<unknown>;
  private database: { driverDatabase: string; liveTripDatabase: string } = { driverDatabase: 'maps', liveTripDatabase: 'livetrip' };

  constructor(private _db: AngularFireDatabase) {
    this.envDetector();
  }

  /**
   * get live Driver Location Base On Trip Id
   */
  getDriverLocationLiveByTripId(tripId: number): Observable<any> {
    this.fireDB = this._db.list(this.database.driverDatabase, (ref) => ref.orderByChild('tripId').equalTo(tripId));
    return this.fireDB.valueChanges();
  }

  /**
   * get All Drivers Location in the system based on tenant Id
   */
  getAllActiveDriversLiveLocationByTenantId(tenantId: number): Observable<any> {
    console.log(this.database);
    this.fireDB = this._db.list(this.database.driverDatabase, (ref) => {
      return ref.orderByChild('tenantId').equalTo(tenantId);
    });
    return this.fireDB.valueChanges().pipe(map((res) => res.filter((x: any) => x.onlineStatus == true)));
  }

  /**
   * get all drivers locations in the system
   */
  getAllActiveDriversLocationsInTheSystem(): Observable<any> {
    this.fireDB = this._db.list(this.database.driverDatabase, (ref) => ref.orderByChild('onlineStatus').equalTo(true));
    return this.fireDB.valueChanges();
  }

  /**
   * Emit Driver Starting Trip From Frontend
   */
  assignDriverToTrip(trip: TrackingListDto, point: TrackingRoutePointDto, tenantId: number, transaction: string) {
    this.fireDB = this._db.list(this.database.liveTripDatabase);
    let data = {
      driverId: trip.assignedDriverUserId,
      tripId: trip.id,
      transaction: transaction,
      tenantId: tenantId,
      activePointId: point.id,
      activePointStatus: point.status,
      driverName: trip.driver,
      hasAccident: trip.hasAccident,
      shippingRequestId: trip.requestId,
      tripStatus: trip.status,
      waybillNumber: trip.waybillNumber,
    };
    this.fireDB.set(trip.assignedDriverUserId.toString(), data);
  }

  /**
   * remove Driver From FireBase
   */

  unAssignDriver(trip: TrackingListDto) {
    this.fireDB = this._db.list(this.database.liveTripDatabase);
    this.fireDB.remove(trip.assignedDriverUserId.toString());
  }

  /**
   * detects the Current Enviroment Based on user App Url
   * to Determine Witch Firebase database should the app connect to
   * @private
   */
  private envDetector() {
    switch (location.host) {
      case 'internaltest.tachyonhub.com:4443': {
        this.database.driverDatabase = 'mapinternaltest';
        this.database.liveTripDatabase = 'livetripinternaltest';
        break;
      }
      case 'dev.tachyonhub.com': {
        this.database.driverDatabase = 'mapdev';
        this.database.liveTripDatabase = 'livetripdev';
        break;
      }
      case 'test.tachyonhub.com': {
        this.database.driverDatabase = 'maptest';
        this.database.liveTripDatabase = 'livetriptest';
        break;
      }
      case 'app.tachyonhub.com': {
        this.database.driverDatabase = 'mapproduction';
        this.database.liveTripDatabase = 'livetripproduction';
        break;
      }
      case 'uat.tachyonhub.com:4441': {
        this.database.driverDatabase = 'mapQA';
        this.database.liveTripDatabase = 'livetripQA';
        break;
      }
      default: {
        this.database.driverDatabase = 'map'; //toBeChanged
        this.database.liveTripDatabase = 'livetrip'; //toBeChanged
        break;
      }
    }
  }
}
