import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-counters-widget',
  templateUrl: './counters-widget.component.html',
  styleUrls: ['./counters-widget.component.css'],
})
export class CountersWidgetComponent extends AppComponentBase implements OnInit {
  public deliveredTripsCount: number;
  public loading: boolean;
  public inTransitTripsCount: number;
  public numberOfGeneratedInvoices: number;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.fetchData();
  }

  private fetchData() {
    this.getDeliveredTripsCountForCurrentWeek();
    if (this.isShipper) {
      this.getInTransitTripsCount();
    }
    if (this.isCarrier || this.isCarrierSaas) {
      this.getNumberOfGeneratedInvoices();
    }
  }

  getDeliveredTripsCountForCurrentWeek(): void {
    this.loading = true;
    if (this.isShipper) {
      this._shipperDashboardServiceProxy.getDeliveredTripsCountForCurrentWeek().subscribe((res) => {
        this.loading = false;
        this.deliveredTripsCount = res;
      });
    }
    if (this.isCarrier || this.isCarrierSaas) {
      this._carrierDashboardServiceProxy.getDeliveredTripsCountForCurrentWeek().subscribe((res) => {
        this.loading = false;
        this.deliveredTripsCount = res;
      });
    }
  }

  getInTransitTripsCount(): void {
    this.loading = true;
    this._shipperDashboardServiceProxy.getInTransitTripsCount().subscribe((res) => {
      this.loading = false;
      this.inTransitTripsCount = res;
    });
  }

  getNumberOfGeneratedInvoices(): void {
    this.loading = true;
    this._carrierDashboardServiceProxy.getNumberOfGeneratedInvoices().subscribe((res) => {
      this.loading = false;
      this.numberOfGeneratedInvoices = res;
    });
  }
}
