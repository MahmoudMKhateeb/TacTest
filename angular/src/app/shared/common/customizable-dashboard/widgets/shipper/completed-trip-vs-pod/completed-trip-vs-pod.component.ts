import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartCategoryPairedValuesDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-completed-trip-vs-pod',
  templateUrl: './completed-trip-vs-pod.component.html',
  styleUrls: ['./completed-trip-vs-pod.component.css'],
})
export class CompletedTripVsPodComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  loading = false;
  legend: ApexLegend = {
    show: false,
    // position: 'right',
    // offsetY: 40,
    // fontWeight: 500,
  };
  yaxis = [
    // {
    //     labels: {
    //         formatter: function(val) {
    //             console.log('InvoicesVsPaidInvoicesComponent val', val);
    //             return isNaN(val) ? val.toFixed(0) : val;
    //         }
    //     }
    // }
  ];
  public completedTripVsPod: any;
  options: string[] = [this.l('Daily'), this.l('Weekly'), this.l('Monthly')];

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getTrips();
  }

  getTrips() {
    this.loading = true;

    this._shipperDashboardServiceProxy
      .getCompletedTripVsPod()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        const completed =
          result.completedTrips.length > 0 ? result.completedTrips.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0) : 0;
        const pod = result.podTrips.length > 0 ? result.podTrips.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0) : 0;
        this.completedTripVsPod = {
          completed,
          pod,
          total: completed + pod,
        };
        const categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        const completedSeries = categories.map((item) => {
          const foundFromResponse = result.completedTrips.find((accepted) => {
            accepted.x = accepted?.x.slice(0, 3);
            return accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
          console.log('acceptedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        const podSeries = categories.map((item) => {
          const foundFromResponse = result.podTrips.find((rejected) => {
            rejected.x = rejected?.x.slice(0, 3);
            return rejected.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
          console.log('rejectedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        console.log('completedSeries', completedSeries);
        console.log('podSeries', podSeries);
        this.chartOptions = {
          series: [
            {
              name: this.l('Completed'),
              data: completedSeries,
              color: 'rgba(187, 41, 41, 0.847)',
            },
            {
              name: this.l('POD'),
              data: podSeries,
            },
          ],
          chart: {
            type: 'bar',
            width: '100%',
            height: 250,
            stacked: true,
            stackType: '100%',
          },
          xaxis: {
            type: 'category',
            categories,
          },
          dataLabels: {
            enabled: false,
          },
        };
        (this.chartOptions as any).legend = {
          position: 'right',
          offsetY: 40,
        };
        (this.chartOptions as any).plotOptions = {
          bar: {
            columnWidth: '45%',
            // distributed: true
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
