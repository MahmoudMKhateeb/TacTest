import { Component, Injector, OnInit } from '@angular/core';
import { WidgetComponentBase } from '@app/shared/common/customizable-dashboard/widgets/widget-component-base';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { FilterDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-completed-trips-widget',
  templateUrl: './completed-trips-widget.component.html',
  styleUrls: ['./completed-trips-widget.component.css'],
})
export class CompletedTripsWidgetComponent extends WidgetComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptionsBars>;
  loading = false;
  FilterDatePeriod = FilterDatePeriod;
  selectedDatePeriod: FilterDatePeriod;
  noData = true;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
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
    this._shipperDashboardServiceProxy
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
        (this.chartOptions.chart.locales as any[]) = [
          {
            name: 'en',
            options: {
              toolbar: {
                exportToPNG: this.l('Download') + ' PNG',
                exportToSVG: this.l('Download') + ' SVG',
                exportToCSV: this.l('Download') + ' CSV',
              },
            },
          },
        ];

        this.loading = false;

        if (result?.length > 0) {
          this.noData = false;
        }
      });
  }
}
