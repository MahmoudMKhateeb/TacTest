import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy, WaybillsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AngularFireDatabase, AngularFireList } from '@angular/fire/compat/database';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FirebaseHelperClass, trackingIconsList } from '@app/main/shippingRequests/shippingRequests/tracking/firebaseHelper.class';

@Component({
  selector: 'app-tracking-map',
  templateUrl: './tracking-map.component.html',
  styleUrls: ['./tracking-map.component.scss'],
})
export class TrackingMapComponent extends AppComponentBase implements OnInit {
  route1: any;
  waypoints1: any;
  source1 = {
    lng: 21,
    lat: 22,
  };
  dest1: any;
  trips: any;
  renderOptions1: any;
  option: any;
  items: any[];
  loading: boolean = false;
  zoom: 12;
  colors: any = ['#9604f3', '#0f0', '#0ff', '#dec', '#f0f', '#ff0', '#f99', '#000', '#233', '#dff', '747', '944', '833'];
  colors2: any[];
  public markerOptions: any;
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
    this._shipperDashboardServiceProxy.getTrackingMap().subscribe((result) => {
      this.items = [];
      this.colors2 = this.colors;
      result.forEach((r) => {
        this.waypoints1 = [];
        this.source1 = {
          lat: r.originLatitude || undefined,
          lng: r.originLongitude || undefined,
        };

        this.dest1 = {
          lat: r.destinationLatitude || undefined,
          lng: r.destinationLongitude || undefined,
        };
        var t = r.routPoints;
        for (let i = 0; i < t.length; i++) {
          this.waypoints1.push({
            location: {
              lat: r.routPoints[i].latitude,
              lng: r.routPoints[i].longitude,
            },
          });
        }

        var item = this.colors2[Math.floor(Math.random() * this.colors2.length)];
        var index: number = this.colors2.indexOf(item);

        if (index !== -1) {
          this.colors2.splice(index, 1);
        }

        this.renderOptions1 = { polylineOptions: { strokeColor: item || '#344440' } };

        this.markerOptions = {
          origin: {
            icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|' + item + '|2',
          },
          destination: {
            icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|9604f3|2',
          },
          waypoints: { icon: 'https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=•|f75|2' },
        };

        this.option = {
          source: this.source1,
          destination: this.dest1,
          waybill: this.waypoints1,
          renderOption: this.renderOptions1,
          markerOptions: this.markerOptions,
        };
        this.trips = result;
        this.items.push(this.option);
      });
      this.loading = true;
    });
  }

  /**
   * get live Tracking Of Drivers For Tenant and Host
   */
  getDriverLiveLocation() {
    let helper = new FirebaseHelperClass(this._db);
    if (this.isCarrier) {
      helper.getAllActiveDriversLiveLocationByTenantId(this.appSession.tenantId).subscribe((res) => {
        this.allDrivers = res;
      });
    } else if (!this.appSession.tenant.id || this.isTachyonDealer) {
      helper.getAllActiveDriversLocationsInTheSystem().subscribe((res) => {
        this.allDrivers = res;
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
}
