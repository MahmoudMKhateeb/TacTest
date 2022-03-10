import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy, SalesSummaryDatePeriod } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-host-new-trips-chart',
  templateUrl: './host-new-trips-chart.component.html',
  styleUrls: ['./host-new-trips-chart.component.css'],
})
export class HostNewTripsChartComponent extends AppComponentBase implements OnInit {
  months: string[];
  trips: number[];
  loading: boolean = false;
  fromDate: moment.Moment = null;
  toDate: moment.Moment = null;
  saving = false;
  appSalesSummaryDateInterval = SalesSummaryDatePeriod;
  selectedDatePeriod: SalesSummaryDatePeriod;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }
  public chartOptions: Partial<ChartOptionsBars>;

  ngOnInit(): void {
    this.getTrips(this.appSalesSummaryDateInterval.Daily);
  }

  reload(datePeriod) {
    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getTrips(this.selectedDatePeriod);
  }

  getTrips(datePeriod: SalesSummaryDatePeriod) {
    this.months = [];
    this.trips = [];
    this.loading = true;

    this._hostDashboardServiceProxy
      .getNewTripsStatistics(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          var txt = element.day + '-' + element.month;
          if (datePeriod == SalesSummaryDatePeriod.Weekly) {
            txt = 'week-' + element.week;
          }
          if (datePeriod == SalesSummaryDatePeriod.Monthly) {
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
              color: '#b10303',
            },
          ],
          chart: {
            type: 'bar',
            height: 350,
          },
          plotOptions: {
            bar: {
              horizontal: false,
              columnWidth: '55%',
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
