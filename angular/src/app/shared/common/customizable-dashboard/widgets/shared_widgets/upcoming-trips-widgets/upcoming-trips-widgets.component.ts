import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  BrokerDashboardServiceProxy,
  CarrierDashboardServiceProxy,
  NeedsActionTripDto,
  ShipperDashboardServiceProxy,
  UpcomingTripItemDto,
  UpcomingTripsOutput,
} from '@shared/service-proxies/service-proxies';
import * as moment from '@node_modules/moment';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { Router } from '@angular/router';

@Component({
  selector: 'app-upcoming-trips-widgets',
  templateUrl: './upcoming-trips-widgets.component.html',
  styleUrls: ['./upcoming-trips-widgets.component.css'],
})
export class UpcomingTripsWidgetsComponent extends AppComponentBase implements OnInit {
  @Input('isForActors') isForActors = false;
  today = new Date();
  weekDates: Date[] = [];
  upcomingTrips: UpcomingTripsOutput[];
  loading: boolean;
  upcomingTripsForSelectedDay: UpcomingTripItemDto[] = [];
  selectedDay: Date;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy,
    private _brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private router: Router
  ) {
    super(injector);
  }

  ngOnInit(): void {
    for (let i = 0; i < 7; i++) {
      this.weekDates.push(moment(this.today).add('d', i).toDate());
    }
    this.getUpcomingTripsForShipper();
  }

  getUpcomingTripsForShipper(): void {
    this.loading = true;
    if (this.isForActors) {
      this._brokerDashboardServiceProxy.getUpcomingTrips().subscribe((res) => {
        this.upcomingTrips = res;
        this.selectDay(this.weekDates[0]);
        this.loading = false;
      });
      return;
    }
    if (this.isShipper) {
      this._shipperDashboardServiceProxy.getUpcomingTrips().subscribe((res) => {
        this.upcomingTrips = res;
        this.selectDay(this.weekDates[0]);
        this.loading = false;
      });
    }
    if (this.isCarrier || this.isCarrierSaas) {
      this._carrierDashboardServiceProxy.getUpcomingTrips().subscribe((res) => {
        this.upcomingTrips = res;
        this.selectDay(this.weekDates[0]);
        this.loading = false;
      });
    }
    if (this.isTachyonDealerOrHost) {
      this.loading = false;
    }
  }

  selectDay(day: Date) {
    this.selectedDay = day;
    const foundItem = this.upcomingTrips.find((item) => moment(item.date).isSame(day, 'day'));
    if (isNotNullOrUndefined(foundItem)) {
      this.upcomingTripsForSelectedDay = foundItem?.trips;
    } else {
      this.upcomingTripsForSelectedDay = [];
    }
  }

  goToTrackingPage(trip: NeedsActionTripDto): void {
    this.router.navigateByUrl(`/app/main/tracking?waybillNumber=${trip.waybillNumber}`);
  }
}
