import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-deleverd-trips',
  templateUrl: './deleverd-trips.component.html',
  styles: [],
})
export class DeleverdTripsComponent extends AppComponentBase implements OnInit {
  deliveredTripsCount: number;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDate();
  }

  getDate() {
    this.loading = true;
    this._hostDashboardServiceProxy
      .getDeliveredTripsCount()
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
