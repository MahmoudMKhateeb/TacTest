import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { HeatmapLocationsModel } from '@app/shared/common/customizable-dashboard/widgets/host/number-of-requests-for-each-city/heatmap-locations-model';

@Component({
  selector: 'app-number-of-requests-for-each-city',
  templateUrl: './number-of-requests-for-each-city.component.html',
  styleUrls: ['./number-of-requests-for-each-city.component.css'],
})
export class NumberOfRequestsForEachCityComponent extends AppComponentBase implements OnInit {
  loading = false;
  locations: HeatmapLocationsModel[] = [];

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(event: { start: moment.Moment; end: moment.Moment }) {
    this.locations = [];
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getRequestsHeapMap(event.start, event.end)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.loading = false;
        result.map(async (item) => {
          const latlng = await this.getLatLngByCityName(item.cityName);
          this.locations.push({
            latitude: latlng?.lat,
            longitude: latlng?.lng,
            label: item.cityName,
            count: item.numberOfRequests,
            type: item.cityType,
          });
        });
      });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    this.getData(event);
  }

  /**
   * Get City Coordinates By Providing City name
   * @param cityName
   */
  getLatLngByCityName(cityName: string) {
    return new Promise<any>((resolve) => {
      console.log('cityName : ', cityName);
      const geocoder = new google.maps.Geocoder();
      geocoder.geocode(
        {
          address: cityName,
        },
        (results, status) => {
          console.log(results);
          if (status === google.maps.GeocoderStatus.OK) {
            const Lat = results[0].geometry.location.lat();
            const Lng = results[0].geometry.location.lng();
            resolve({ lat: Lat, lng: Lng });
          } else {
            console.log('Something got wrong ' + status);
            resolve(null);
          }
        }
      );
    });
  }
}
