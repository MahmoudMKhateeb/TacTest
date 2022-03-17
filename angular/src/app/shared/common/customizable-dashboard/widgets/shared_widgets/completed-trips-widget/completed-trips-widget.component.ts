import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { WidgetComponentBase } from '@app/shared/common/customizable-dashboard/widgets/widget-component-base';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { FilterDatePeriod, SalesSummaryDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';
import { format } from 'path/posix';
import { formatNumber } from '@angular/common';
import { count } from 'console';

@Component({
  selector: 'app-completed-trips-widget',
  templateUrl: './completed-trips-widget.component.html',
  styleUrls: ['./completed-trips-widget.component.css'],
})
export class CompletedTripsWidgetComponent extends WidgetComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptionsBars>;
  months: string[];
  trips: number[];
  loading: boolean = false;
  filterDatePeriodInterval = FilterDatePeriod;
  selectedDatePeriod: FilterDatePeriod;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.runDelayed(() => {
      this.getTrips(this.filterDatePeriodInterval.Daily);
    });
  }

  reload(datePeriod) {
    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getTrips(this.selectedDatePeriod);
  }

  getTrips(datePeriod: FilterDatePeriod) {
    this.months = [];
    this.trips = [];
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getCompletedTripsCountPerMonth(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          var txt = '';
          if (datePeriod == FilterDatePeriod.Daily) {
            txt = element.day + '-' + element.month + '-' + element.year;
          }
          if (datePeriod == FilterDatePeriod.Weekly) {
            txt = 'week-' + element.week;
          }
          if (datePeriod == FilterDatePeriod.Monthly) {
            txt = element.month;
          }
          this.months.push(txt);
          this.trips.push(element.count);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Trips',
              data: this.trips,
              color: 'rgba(187, 41, 41, 0.99)',
            },
          ],
          chart: {
            type: 'bar',
            height: 350,
          },
          plotOptions: {
            area: {
              fillTo: 'origin',
            },
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            show: true,
            width: 2,
            colors: ['transparent'],
          },
          xaxis: {
            categories: this.months,
          },
          tooltip: {
            y: {
              formatter: function (val) {
                return val.toFixed(0);
              },
            },
          },
          fill: {
            opacity: 1,
          },
        };
        this.loading = false;
      });
  }
}
