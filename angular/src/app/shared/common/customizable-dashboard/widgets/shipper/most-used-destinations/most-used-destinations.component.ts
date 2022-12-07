import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MostUsedOriginsDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-most-used-destinations',
  templateUrl: './most-used-destinations.component.html',
  styleUrls: ['./most-used-destinations.component.css'],
})
export class MostUsedDestinationsComponent extends AppComponentBase implements OnInit {
  data: MostUsedOriginsDto[] = [];
  loading: boolean = false;
  total = 0;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDestinations();
  }

  getDestinations() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getMostUsedDestinatiions()
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
}
