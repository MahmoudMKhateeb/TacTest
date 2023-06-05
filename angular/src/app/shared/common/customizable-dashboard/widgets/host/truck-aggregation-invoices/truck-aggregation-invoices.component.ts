import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-truck-aggregation-invoices',
  templateUrl: './truck-aggregation-invoices.component.html',
  styleUrls: ['./truck-aggregation-invoices.component.scss'],
})
export class TruckAggregationInvoicesComponent extends AppComponentBase implements OnInit {
  loading = false;

  selling = 0;
  cost = 0;
  profit = 0;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getTruckAggregationInvoicesCostAndSelling()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.selling = res.selling;
        this.cost = res.cost;
        this.profit = res.profit;
      });
  }
}
