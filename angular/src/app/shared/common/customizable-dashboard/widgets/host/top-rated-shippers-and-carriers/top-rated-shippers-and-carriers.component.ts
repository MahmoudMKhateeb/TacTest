import { Component, HostListener, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetTopTenantsCreatedTripsOutput, TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'app-top-rated-shippers-and-carriers',
  templateUrl: './top-rated-shippers-and-carriers.component.html',
  styleUrls: ['./top-rated-shippers-and-carriers.component.scss'],
})
export class TopRatedShippersAndCarriersComponent extends AppComponentBase implements OnInit {
  topTenantsCreatedTrips: GetTopTenantsCreatedTripsOutput;
  loading = false;

  @HostListener('wheel', ['$event']) onScroll(event: WheelEvent): void {
    const targetElement = event.target as HTMLElement;
    const scrollableColElement = targetElement.closest('.scrollable-col');
    if (scrollableColElement) {
      event.stopPropagation();
    }
  }

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}

  getData(event: { start: moment.Moment; end: moment.Moment }) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy.getTopTenantsCreatedTrips(event.start, event.end).subscribe((res) => {
      this.topTenantsCreatedTrips = res;
      this.loading = false;
    });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    this.getData(event);
  }
}
