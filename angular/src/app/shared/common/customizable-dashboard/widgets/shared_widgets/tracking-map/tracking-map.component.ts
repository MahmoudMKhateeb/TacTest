import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-tracking-map',
  templateUrl: './tracking-map.component.html',
  styleUrls: ['./tracking-map.component.scss'],
})
export class TrackingMapComponent extends AppComponentBase implements OnInit {
  route1: any;
  waypoints1: any;
  source1: any;
  dest1: any;
  trips: any;
  renderOptions1: any;
  option: any;
  items: any[];
  loading: boolean;
  zoom: 12;
  colors: any = ['#9604f3', '#0f0', '#0ff', '#d35', '#f0f', '#ff0', '#f29', '#000', '#233', '#3ff', '747', '944', '833'];
  colors2: any[];
  public markerOptions: any;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getdata();
  }

  getdata() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getTrackingMap()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
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
        this.loading = false;
      });
  }
}
