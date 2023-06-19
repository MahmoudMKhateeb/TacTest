import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  BrokerDashboardServiceProxy,
  MostUsedCityDto,
  MostUsedOriginsDto,
  ShipperDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-most-used-origin',
  templateUrl: './most-used-origin.component.html',
  styleUrls: ['./most-used-origin.component.css'],
})
export class MostUsedOriginComponent extends AppComponentBase implements OnInit {
  @Input('isForActors') isForActors = false;
  data: MostUsedOriginsDto[] | MostUsedCityDto[] = [];
  loading = false;
  total = 0;

  constructor(
    private injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _brokerDashboardServiceProxy: BrokerDashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (!this.isForActors) {
      this.getDestinations();
    } else {
      this.getDestinationsForActors();
    }
  }

  getDestinations() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getMostUsedOrigins()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.data = result;
        this.total = result.reduce((accumulator, currentValue) => accumulator + currentValue.numberOfRequests, 0);
        this.loading = false;
      });
  }

  getDestinationsForActors() {
    this.loading = true;
    this._brokerDashboardServiceProxy
      .getMostUsedOrigins()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.data = result;
        this.total = result.reduce((accumulator, currentValue) => accumulator + currentValue.numberOfTrips, 0);
        this.loading = false;
      });
  }
}
