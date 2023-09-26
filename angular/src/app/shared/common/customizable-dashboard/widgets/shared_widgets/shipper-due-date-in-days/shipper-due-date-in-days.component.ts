import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-shipper-due-date-in-days',
  templateUrl: './shipper-due-date-in-days.component.html',
  styleUrls: ['./shipper-due-date-in-days.component.css'],
})
export class ShipperDueDateInDaysComponent extends AppComponentBase implements OnInit {
  DocumentsCount: number;
  TimeUnit: string;
  loading = false;
  saving = false;

  constructor(private injector: Injector, private router: Router, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.loading = true;
    this.saving = true;
    this._shipperDashboardServiceProxy
      .getDocumentsDueDateInDays()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.saving = false;
        })
      )
      .subscribe((result) => {
        if (isNotNullOrUndefined(result) && result.length > 0) {
          this.DocumentsCount = result[0].count;
          this.TimeUnit = result[0].timeUnit;
          this.loading = false;
          this.saving = false;
        }
      });
  }
}
