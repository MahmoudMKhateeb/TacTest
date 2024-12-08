import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-saas-trips',
  templateUrl: './number-of-saas-trips.component.html',
  styleUrls: ['./number-of-saas-trips.component.scss'],
})
export class NumberOfSaasTripsComponent extends AppComponentBase implements OnInit {
  tripsCount: number;
  loading: boolean = false;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getSaasTripsNumber().subscribe((result) => {
      this.tripsCount = result;
      this.loading = false;
    });
  }
}
