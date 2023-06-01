import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-truck-aggregation-invoices',
  templateUrl: './truck-aggregation-invoices.component.html',
  styleUrls: ['./truck-aggregation-invoices.component.scss'],
})
export class TruckAggregationInvoicesComponent extends AppComponentBase implements OnInit {
  loading = false;

  selling = 20000;
  cost = 15000;
  profit = 5000;

  constructor(private injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    // this.loading = true;
  }
}
