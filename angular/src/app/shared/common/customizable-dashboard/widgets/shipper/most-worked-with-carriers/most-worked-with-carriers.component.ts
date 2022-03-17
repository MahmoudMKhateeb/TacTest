import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-most-worked-with-carriers',
  templateUrl: './most-worked-with-carriers.component.html',
  styleUrls: ['./most-worked-with-carriers.component.css'],
})
export class MostWorkedWithCarriersComponent extends AppComponentBase implements OnInit {
  Carriers: any;
  loading: boolean = false;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getCarriers();
  }

  getCarriers() {
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getMostWorkedWithCarriers()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.Carriers = result;
        this.loading = false;
      });
  }
}
