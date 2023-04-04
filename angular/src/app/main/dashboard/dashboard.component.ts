import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DashboardCustomizationConst } from '@app/shared/common/customizable-dashboard/DashboardCustomizationConsts';
import isEnabled = abp.features.isEnabled;

@Component({
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less'],
  encapsulation: ViewEncapsulation.None,
})
export class DashboardComponent extends AppComponentBase {
  dashboardName = DashboardCustomizationConst.dashboardNames.defaultTenantDashboard;

  constructor(injector: Injector) {
    super(injector);

    const isBroker = this.hasCarrierClients && this.hasShipperClients;
    if (isBroker) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultBrokerDashboard;
    } else if (this.isShipper) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultShipperDashboard;
    } else if (this.isCarrier) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultCarrierDashboard;
    } else if (this.isTachyonDealer) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultTachyonMangedServiceDashboard;
    }
  }
}
