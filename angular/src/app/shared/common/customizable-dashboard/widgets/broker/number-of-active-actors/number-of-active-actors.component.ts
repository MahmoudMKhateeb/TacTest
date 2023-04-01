import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BrokerDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-active-actors',
  templateUrl: './number-of-active-actors.component.html',
  styleUrls: ['./number-of-active-actors.component.scss'],
})
export class NumberOfActiveActorsComponent extends AppComponentBase implements OnInit {
  activeShipperActorsCount: number;
  activeCarrierActorsCount: number;
  nonActiveShipperActorsCount: number;
  nonActiveCarrierActorsCount: number;

  constructor(injector: Injector, private brokerDashboardServiceProxy: BrokerDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getNumberOfActiveAndNoneActiveActors();
  }

  private getNumberOfActiveAndNoneActiveActors() {
    this.brokerDashboardServiceProxy.getNumberOfActiveAndNonActiveActors().subscribe((res) => {
      this.activeShipperActorsCount = res.activeShipperActorsCount;
      this.activeCarrierActorsCount = res.activeCarrierActorsCount;
      this.nonActiveShipperActorsCount = res.nonActiveShipperActorsCount;
      this.nonActiveCarrierActorsCount = res.nonActiveCarrierActorsCount;
    });
  }
}
