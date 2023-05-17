import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-top-worst-rated-per-trip',
  templateUrl: './top-worst-rated-per-trip.component.html',
  styleUrls: ['./top-worst-rated-per-trip.component.css'],
})
export class TopWorstRatedPerTripComponent extends AppComponentBase implements OnInit {
  topWorstRated: any[] = [
    {
      id: 164,
      name: 'shipperPlus',
      numberOfRequests: 10,
      rating: 3.3,
    },
    {
      id: 165,
      name: 'shipperProPlus',
      numberOfRequests: 15,
      rating: 4.3,
    },
    {
      id: 166,
      name: 'shipperTest',
      numberOfRequests: 8,
      rating: 4.0,
    },
    {
      id: 167,
      name: 'shipperTestPlus',
      numberOfRequests: 20,
      rating: 5.0,
    },
  ];
  loading: boolean = false;
  rateTypes: string[] = ['Top', 'Worst'];
  rateType: string;
  editionType: string;
  editionTypes: string[] = [];

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    // this.loading = true;
    // this._hostDashboardServiceProxy.getTopRatedShippers().subscribe((result) => {
    //   this.topWorstRated = result;
    //   this.loading = false;
    // });
  }
}
