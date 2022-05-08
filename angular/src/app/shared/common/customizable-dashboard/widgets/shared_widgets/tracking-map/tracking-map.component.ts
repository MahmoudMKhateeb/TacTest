import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy, TrackingMapDto, WaybillsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AngularFireDatabase, AngularFireList } from '@angular/fire/compat/database';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FirebaseHelperClass, trackingIconsList } from '@app/main/shippingRequests/shippingRequests/tracking/firebaseHelper.class';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-tracking-map',
  templateUrl: './tracking-map.component.html',
  styleUrls: ['./tracking-map.component.scss'],
})
export class TrackingMapComponent extends AppComponentBase implements OnInit {
  route1: any;
  source1 = {
    lng: 21,
    lat: 22,
  };
  dest1: any;
  directions: any[];
  loading: boolean = false;
  zoom: 12;
  private fireDB: AngularFireList<unknown>;
  allDrivers = [];
  map: any;
  tripsToggle = true;
  driversToggle = true;
  waybillDownloading = false;
  trackingIconsList = trackingIconsList;
  constructor(
    private injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _db: AngularFireDatabase
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getTableRecords();
    this.getDriverLiveLocation();
  }

  getTableRecords() {
    this.loading = true;

    this._shipperDashboardServiceProxy
      .getTrackingMap()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.directions = [];
        result.forEach((r) => {
          let renderOptions: google.maps.DirectionsRendererOptions = { polylineOptions: { strokeColor: '#344440' } };
          let color = this.getRandomColor();
          let direction: Direction = {
            origin: undefined,
            destination: undefined,
            waypoints: [],
            renderOptions: {
              polylineOptions: {
                strokeWeight: 6,
                strokeOpacity: 0.55,
                strokeColor: color,
              },
            },
            trackingMapDto: r,
            show: true,
            color: color,
          };

          r.routPoints.forEach((x) => {
            if (r.routPoints.indexOf(x) === 0) {
              direction.origin = new google.maps.LatLng(x.latitude, x.longitude);
            } else if (r.routPoints.indexOf(x) === r.routPoints.length - 1) {
              direction.destination = new google.maps.LatLng(x.latitude, x.longitude);
            } else {
              direction.waypoints.push({
                location: new google.maps.LatLng(x.latitude, x.longitude),
              });
            }
          });

          this.directions.push(direction);
        });
        this.loading = true;
      });
  }

  /**
   * get live Tracking Of Drivers For Tenant and Host
   */
  getDriverLiveLocation() {
    this.loading = true;
    let helper = new FirebaseHelperClass(this._db);
    if (this.isCarrier) {
      helper
        .getAllActiveDriversLiveLocationByTenantId(this.appSession.tenantId)
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe((res) => {
          this.allDrivers = res;
          console.log('allDrivers', this.allDrivers);
        });
    } else if (!this.appSession.tenant.id || this.isTachyonDealer) {
      helper.getAllActiveDriversLocationsInTheSystem().subscribe((res) => {
        this.allDrivers = res;
        console.log('allDrivers', this.allDrivers);
      });
    }
  }
  mapReady(event: any) {
    this.map = event;
    this.map.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById('Settings'));
  }

  /**
   * Downloads A single Drop Wayboll
   * @param id
   * @constructor
   */
  DownloadSingleDropWaybillPdf(id: number): void {
    this.waybillDownloading = true;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.waybillDownloading = false;
    });
  }

  /**
   * show or hide drivers  from the map
   */
  driverToggle() {
    this.driversToggle = !this.driversToggle;
  }

  /**
   * show or hide trips from the map
   */
  tripToggle() {
    this.tripsToggle = !this.tripsToggle;
  }

  getRandomColor(): string {
    let color = '#'; // <-----------
    let letters = '0123456789ABCDEF';

    for (var i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }
}
export interface Direction {
  origin: google.maps.LatLng;
  destination: google.maps.LatLng;
  waypoints: google.maps.DirectionsWaypoint[];
  renderOptions?: google.maps.DirectionsRendererOptions;
  trackingMapDto: TrackingMapDto;
  show: boolean;
  color: string;
}
