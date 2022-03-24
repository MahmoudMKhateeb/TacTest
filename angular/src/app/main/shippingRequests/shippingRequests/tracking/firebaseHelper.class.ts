import { AngularFireDatabase, AngularFireList } from '@angular/fire/compat/database';
import { Observable } from 'rxjs';
import { map } from '@node_modules/rxjs/internal/operators';

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
};

export interface DriverLocation {
  lng: number;
  lat: number;
}

export class FirebaseHelperClass {
  private fireDB: AngularFireList<unknown>;
  private database: string;

  constructor(private _db: AngularFireDatabase) {
    this.envDetector();
  }

  /**
   * get live Driver Location Base On Trip Id
   */
  getDriverLocationLiveByTripId(tripId: number): Observable<any> {
    this.fireDB = this._db.list(this.database, (ref) => ref.orderByChild('tripId').equalTo(tripId));
    return this.fireDB.valueChanges();
  }

  /**
   * get All Drivers Location in the system based on tenant Id
   */
  getAllActiveDriversLiveLocationByTenantId(tenantId: number): Observable<any> {
    console.log(this.database);
    this.fireDB = this._db.list(this.database, (ref) => {
      return ref.orderByChild('tenantId').equalTo(tenantId);
    });
    return this.fireDB.valueChanges().pipe(map((res) => res.filter((x: any) => x.activePointId > 0)));
  }

  /**
   * get all drivers locations in the system
   */
  getAllActiveDriversLocationsInTheSystem(): Observable<any> {
    this.fireDB = this._db.list(this.database, (ref) => ref.orderByChild('activePointId').startAfter(0));
    return this.fireDB.valueChanges();
  }

  /**
   * detects the Current Enviroment Based on user App Url
   * to Determine Witch Firebase database should the app connect to
   * @private
   */
  private envDetector() {
    switch (location.host) {
      case 'internaltest.tachyonhub.com:4443': {
        this.database = 'mapinternaltest';
        break;
      }
      case 'dev.tachyonhub.com': {
        this.database = 'mapinternaltest';
        break;
      }
      case 'test.tachyonhub.com': {
        this.database = 'maptest';
        break;
      }
      case 'app.tachyonhub.com': {
        this.database = 'mapproduction';
        break;
      }
      default: {
        this.database = 'maps';
        break;
      }
    }
  }
}
