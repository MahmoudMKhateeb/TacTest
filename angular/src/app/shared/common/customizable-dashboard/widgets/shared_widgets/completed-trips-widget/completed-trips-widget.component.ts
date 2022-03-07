import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { WidgetComponentBase } from '@app/shared/common/customizable-dashboard/widgets/widget-component-base';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { SalesSummaryDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-completed-trips-widget',
  templateUrl: './completed-trips-widget.component.html',
  styleUrls: ['./completed-trips-widget.component.css'],
})
export class CompletedTripsWidgetComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  months: string[];
  trips: number[];
  toDate: moment.Moment = null;
  fromDate: moment.Moment = null;
  loading: boolean = false;
  saving = false;
  appSalesSummaryDateInterval = SalesSummaryDatePeriod;
  selectedDatePeriod: SalesSummaryDatePeriod;

  constructor(private injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
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
    this.saving = true;
    this._shipperDashboardServiceProxy
      .getCompletedTripsCountPerMonth(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.saving = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          var txt = '';
          if (datePeriod == SalesSummaryDatePeriod.Daily) {
            txt = element.day + '-' + element.month;
          }
          if (datePeriod == SalesSummaryDatePeriod.Weekly) {
            txt = 'wk-' + element.week;
          }
          if (datePeriod == SalesSummaryDatePeriod.Monthly) {
            txt = 'month-' + element.month;
          }
          this.months.push(txt);
          this.trips.push(element.count);
        });

        this.chartOptions = {
          series: [
            {
              name: 'Trips',
              data: this.trips,
              color: '#801e1e',
            },
          ],
          chart: {
            height: 350,
            type: 'line',
            zoom: {
              enabled: false,
            },
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            curve: 'straight',
          },
          grid: {
            row: {
              colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
              opacity: 0.5,
            },
          },
          xaxis: {
            categories: this.months,
          },
        };
        this.loading = false;
        this.saving = false;
      });
  }
}
