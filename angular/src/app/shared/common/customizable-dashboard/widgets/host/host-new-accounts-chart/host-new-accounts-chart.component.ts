import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { FilterDatePeriod, HostDashboardServiceProxy, SalesSummaryDatePeriod } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { WidgetComponentBase } from '../../widget-component-base';

@Component({
  selector: 'app-host-new-accounts-chart',
  templateUrl: './host-new-accounts-chart.component.html',
  styleUrls: ['./host-new-accounts-chart.component.css'],
})
export class HostNewAccountsChartComponent extends WidgetComponentBase implements OnInit {
  months: string[];
  counts: number[];
  loading: boolean = false;
  saving = false;
  filterDatePeriodInterval = FilterDatePeriod;
  selectedDatePeriod: FilterDatePeriod;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  public chartOptions: Partial<ChartOptionsBars>;

  ngOnInit(): void {
    this.runDelayed(() => {
      this.getAccounts(this.filterDatePeriodInterval.Daily);
    });
  }

  reload(datePeriod) {
    this.loading = true;

    if (this.selectedDatePeriod === datePeriod) {
      this.loading = false;
      return;
    }

    this.selectedDatePeriod = datePeriod;

    this.getAccounts(this.selectedDatePeriod);
  }

  getAccounts(datePeriod: FilterDatePeriod) {
    this.counts = [];
    this.months = [];
    this.loading = true;

    this._hostDashboardServiceProxy
      .getAccountsStatistics(datePeriod)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        result.forEach((element) => {
          var txt = '';
          if (datePeriod == FilterDatePeriod.Daily) {
            txt = element.day + '-' + element.month;
          }
          if (datePeriod == FilterDatePeriod.Weekly) {
            txt = 'week-' + element.week;
          }
          if (datePeriod == FilterDatePeriod.Monthly) {
            txt = 'month-' + element.month;
          }
          this.months.push(txt);
          this.counts.push(parseInt(element.count.toString()));
        });
        this.chartOptions = {
          series: [
            {
              name: 'Accounts',
              data: this.counts,
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
      });
  }
}
