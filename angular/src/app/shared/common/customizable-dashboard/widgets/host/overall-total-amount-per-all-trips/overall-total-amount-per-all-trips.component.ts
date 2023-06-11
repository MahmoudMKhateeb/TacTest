import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-overall-total-amount-per-all-trips',
  templateUrl: './overall-total-amount-per-all-trips.component.html',
  styleUrls: ['./overall-total-amount-per-all-trips.component.scss'],
})
export class OverallTotalAmountPerAllTripsComponent extends AppComponentBase implements OnInit {
  loading = false;
  selling = 0;
  cost = 0;
  profit = 0;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(filter: number) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getOverallAmountForAlltrips(filter)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.selling = res.selling;
        this.cost = res.cost;
        this.profit = res.profit;
      });
  }

  selectedFilter(filter: number) {
    this.getData(filter);
  }
}
