import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { BrokerDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-new-actors-this-month',
  templateUrl: './new-actors-this-month.component.html',
  styleUrls: ['./new-actors-this-month.component.scss'],
})
export class NewActorsThisMonthComponent extends AppComponentBase implements OnInit {
  newActorsCount: number;
  percentage: number;
  shipperCount: number;
  carrierCount: number;

  constructor(injector: Injector, private brokerDashboardServiceProxy: BrokerDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.fetchData();
  }

  private fetchData() {
    this.brokerDashboardServiceProxy.getNewActorsStatistics().subscribe((res) => {
      this.newActorsCount = res.totalActorsForCurrentMonth;
      this.percentage = res.growthChangePercentage;
      this.carrierCount = res.carrierActorsPercentage;
      this.shipperCount = res.shipperActorsPercentage;
    });
  }
}
