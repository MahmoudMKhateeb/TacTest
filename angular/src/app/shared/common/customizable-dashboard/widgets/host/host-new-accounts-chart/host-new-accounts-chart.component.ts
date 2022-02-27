import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-host-new-accounts-chart',
  templateUrl: './host-new-accounts-chart.component.html',
  styles: [],
})
export class HostNewAccountsChartComponent extends AppComponentBase implements OnInit {
  x: string[];
  y: number[];
  loading: boolean = false;
  fromDate: moment.Moment = null;
  toDate: moment.Moment = null;
  saving = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }
  public chartOptions: Partial<ChartOptionsBars>;
  ngOnInit() {
    this.getAccounts();
  }

  getAccounts() {
    this.x = [];
    this.y = [];
    this.loading = true;

    this._hostDashboardServiceProxy
      .getAccountsCountsPerMonth(this.fromDate, this.toDate)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          element.year;
          this.x.push(element.month + '-' + element.year);
          this.y.push(element.count);
        });
        this.chartOptions = {
          series: [
            {
              name: 'Accounts',
              data: this.y,
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
            categories: this.x,
          },
          fill: {
            opacity: 1,
          },
        };

        this.loading = false;
      });
  }
}
