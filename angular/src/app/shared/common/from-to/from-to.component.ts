import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'app-from-to',
  templateUrl: './from-to.component.html',
  styleUrls: ['./from-to.component.scss'],
})
export class FromToComponent extends AppComponentBase implements OnInit {
  @Input('from') from: string;
  @Input('to') to: string | string[];
  dest: string;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.dest = this.to instanceof Array ? this.to.join(', ') : this.to;
  }
}
