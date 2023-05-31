import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-deleverd-trips',
  templateUrl: './deleverd-trips.component.html',
  styleUrls: ['./delivered-trips.component.scss'],
})
export class DeleverdTripsComponent extends AppComponentBase implements OnInit {
  deliveredTripsCount: number;
  loading = false;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDate();
  }

  getDate() {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getDeliveredTripsInCurrentMonth()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.deliveredTripsCount = result;
        this.loading = false;
      });
  }
}
