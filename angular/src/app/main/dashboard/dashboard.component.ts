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

    if (isEnabled('App.Shipper')) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultShipperDashboard;
    } else if (isEnabled('App.Carrier')) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultCarrierDashboard;
    } else if (isEnabled('App.TachyonDealer')) {
      this.dashboardName = DashboardCustomizationConst.dashboardNames.defaultTachyonMangedServiceDashboard;
    }
  }
}
