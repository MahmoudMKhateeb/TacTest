import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy, MostTenantWorksListDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-most-worked-with-shippers',
  templateUrl: './most-worked-with-shippers.component.html',
  styleUrls: ['./most-worked-with-shippers.component.css'],
})
export class MostWorkedWithShippersComponent extends AppComponentBase implements OnInit {
  Shippers: MostTenantWorksListDto[];
  loading = false;

  constructor(private injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getShippers();
  }

  getShippers() {
    this.loading = true;
    this._carrierDashboardServiceProxy
      .getMostWorkedWithShippers()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.Shippers = result;
        this.loading = false;
      });
  }
}
