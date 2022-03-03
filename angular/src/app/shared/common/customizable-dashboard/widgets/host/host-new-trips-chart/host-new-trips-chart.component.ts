import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-host-new-trips-chart',
  templateUrl: './host-new-trips-chart.component.html',
  styles: [],
})
export class HostNewTripsChartComponent extends AppComponentBase implements OnInit {
  months: string[];
  trips: number[];
  loading: boolean = false;
  fromDate: moment.Moment = null;
  toDate: moment.Moment = null;
  saving = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }
  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.getTrips();
  }

  getTrips() {
    this.months = [];
    this.trips = [];
    this.loading = true;

    this._hostDashboardServiceProxy
      .getNewTripsCountPerMonth(this.fromDate, this.toDate)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          this.months.push(element.month + '-' + element.year);
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

          fill: {
            opacity: 1,
          },
        };
        this.loading = false;
      });
  }
}
