import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'app-upcoming-trips-widgets',
  templateUrl: './upcoming-trips-widgets.component.html',
  styleUrls: ['./upcoming-trips-widgets.component.css'],
})
export class UpcomingTripsWidgetsComponent extends AppComponentBase implements OnInit {
  today = new Date();
  weekDates: Date[] = [];

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    for (let i = 0; i < 7; i++) {
      this.weekDates.push(moment(this.today).add('d', i).toDate());
    }
  }
}
