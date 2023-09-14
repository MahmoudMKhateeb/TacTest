import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  BrokerDashboardServiceProxy,
  CarrierDashboardServiceProxy,
  FilterDatePeriod,
  GetNeedsActionTripsAndRequestsOutput,
  NeedsActionTripDto,
  ShipperDashboardServiceProxy,
  TMSAndHostDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-needs-action-widget',
  templateUrl: './needs-action-widget.component.html',
  styleUrls: ['./needs-action-widget.component.css'],
})
export class NeedsActionWidgetComponent extends AppComponentBase implements OnInit {
  @Input('isForActors') isForActors = false;
  needsActionTrips: NeedsActionTripDto[] | GetNeedsActionTripsAndRequestsOutput[] = [];
  loading: boolean;
  today = new Date();
  private selectedOption = FilterDatePeriod.Monthly;
  private start: moment.Moment;
  private end: moment.Moment;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy,
    private _brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy,
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
    if (this.isForActors) {
      this._brokerDashboardServiceProxy.getNeedsActionTrips().subscribe((res) => {
        this.needsActionTrips = res;
        this.loading = false;
      });
      return;
    }
    if (this.isShipper) {
      this._shipperDashboardServiceProxy.getNeedsActionTrips(this.start, this.end).subscribe((res) => {
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
    if (this.isTachyonDealerOrHost) {
      this.loading = false;
    }
  }

  goToTrackingPage(trip: NeedsActionTripDto | GetNeedsActionTripsAndRequestsOutput): void {
    const waybillNumber = trip instanceof NeedsActionTripDto ? trip.waybillNumber : trip.waybillOrRequestReference;
    this.router.navigateByUrl(`/app/main/tracking/shipmentTracking?waybillNumber=${waybillNumber}`);
  }

  selectedFilter(filter: { start: moment.Moment; end: moment.Moment }) {
    this.loading = true;
    this.start = filter.start;
    this.end = filter.end;
    if (this.isTachyonDealerOrHost) {
      this.getNeedsActionTripsAndRequests();
    } else {
      this.fetchData();
    }
  }

  private getNeedsActionTripsAndRequests() {
    this._TMSAndHostDashboardServiceProxy.getNeedsActionTripsAndRequests(this.start, this.end).subscribe((res) => {
      this.needsActionTrips = res;
      this.loading = false;
    });
  }
}
