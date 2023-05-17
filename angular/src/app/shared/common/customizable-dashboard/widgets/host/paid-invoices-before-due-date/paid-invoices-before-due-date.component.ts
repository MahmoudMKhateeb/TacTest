import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-paid-invoices-before-due-date',
  templateUrl: './paid-invoices-before-due-date.component.html',
  styleUrls: ['./paid-invoices-before-due-date.component.scss'],
})
export class PaidInvoicesBeforeDueDateComponent extends AppComponentBase implements OnInit {
  loading = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    // this.loading = true;
    // this._hostDashboardServiceProxy.getTopRatedCarriers().subscribe((result) => {
    //     this.topCarriers = result;
    //     this.loading = false;
    // });
  }
}
