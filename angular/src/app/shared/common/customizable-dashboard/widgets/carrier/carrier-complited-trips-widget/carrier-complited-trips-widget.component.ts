import { Component, Injector, OnInit } from '@angular/core';
import { WidgetComponentBase } from '@app/shared/common/customizable-dashboard/widgets/widget-component-base';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { CarrierDashboardServiceProxy, FilterDatePeriod } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-carrier-complited-trips-widget',
  templateUrl: './carrier-complited-trips-widget.component.html',
  styleUrls: ['./carrier-complited-trips-widget.component.css'],
})
export class CarrierComplitedTripsWidgetComponent extends WidgetComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptionsBars>;
  loading = false;
  FilterDatePeriod = FilterDatePeriod;
  selectedDatePeriod: FilterDatePeriod;
  noData = true;

  constructor(private injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.runDelayed(() => {
      this.getTrips(FilterDatePeriod.Daily);
    });
  }

  getTrips(datePeriod: FilterDatePeriod) {
    this.loading = true;
    this.selectedDatePeriod = datePeriod;
    this._carrierDashboardServiceProxy
      .getCompletedTripsCountPerMonth(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [
            {
              name: 'Trips',
              data: result,
              color: 'rgba(187, 41, 41, 0.99)',
            },
          ],
          chart: {
            type: 'bar',
            width: 400,
            height: 250,
          },
          xaxis: {
            type: 'category',
          },
        };

        this.loading = false;

        if (result?.length > 0) {
          this.noData = false;
        }
      });
  }
}
