import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy, NeedsActionTripDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-needs-action-widget',
  templateUrl: './needs-action-widget.component.html',
  styleUrls: ['./needs-action-widget.component.css'],
})
export class NeedsActionWidgetComponent extends AppComponentBase implements OnInit {
  needsActionTrips: NeedsActionTripDto[] = [];
  loading: boolean;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy,
    private router: Router
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.fetchData();
  }

  private fetchData() {
    this.getNeedsActionTrips();
  }

  getNeedsActionTrips(): void {
    this.loading = true;
    if (this.isShipper) {
      this._shipperDashboardServiceProxy.getNeedsActionTrips().subscribe((res) => {
        this.needsActionTrips = res;
        this.loading = false;
      });
    }
    if (this.isCarrier || this.isCarrierSaas) {
      this._carrierDashboardServiceProxy.getNeedsActionTrips().subscribe((res) => {
        this.needsActionTrips = res;
        this.loading = false;
      });
    }
  }

  goToTrackingPage(): void {
    this.router.navigate(['/app/main/tracking']);
  }
}
