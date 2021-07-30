import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AbpSessionService } from '@node_modules/abp-ng2-module';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { BalanceRechargeServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-balance-topbar',
  templateUrl: './balance-topbar.component.html',
  styleUrls: ['./balance-topbar.component.css'],
})
export class BalanceTopbarComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private _balanceRechargeServiceProxy: BalanceRechargeServiceProxy) {
    super(injector);
  }
  balance = 0;
  ngOnInit(): void {
    this._balanceRechargeServiceProxy.getTenantBalance(this.appSession.tenantId).subscribe((result) => {
      this.balance = result;
    });
  }
}
