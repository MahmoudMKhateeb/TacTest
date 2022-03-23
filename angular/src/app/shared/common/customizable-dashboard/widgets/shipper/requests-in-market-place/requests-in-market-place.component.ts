import { Component, Injector, OnInit } from '@angular/core';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { WidgetComponentBase } from '../../widget-component-base';

@Component({
  selector: 'app-requests-in-market-place',
  templateUrl: './requests-in-market-place.component.html',
  styleUrls: ['./requests-in-market-place.component.css'],
})
export class RequestsInMarketPlaceComponent extends WidgetComponentBase implements OnInit {
  Requests: any;
  loading = false;
  saving = false;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.runDelayed(() => {
      this.getRequests();
    });
  }

  getRequests() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getRequestsInMarketpalce()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.Requests = result;
        this.loading = false;
      });
  }
}
