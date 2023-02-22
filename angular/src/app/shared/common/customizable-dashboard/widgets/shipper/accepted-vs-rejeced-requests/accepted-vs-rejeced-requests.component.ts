import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartCategoryPairedValuesDto, FilterDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-accepted-vs-rejeced-requests',
  templateUrl: './accepted-vs-rejeced-requests.component.html',
  styleUrls: ['./accepted-vs-rejeced-requests.component.css'],
})
export class AcceptedVsRejecedRequestsComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  loading = false;
  legend: ApexLegend = {
    show: false,
    // position: 'right',
    // offsetY: 40,
    // fontWeight: 500,
  };
  public acceptedVsRejected: any;
  // this.l('Daily'), this.l('Weekly'), this.l('Monthly')
  options: { key: any; value: any }[] = [];
  yaxis = [
    // {
    //     labels: {
    //         formatter: function(val) {
    //             console.log('AcceptedVsRejecedRequestsComponent val', val);
    //             return isNaN(val) ? val.toFixed(0) : 0;
    //         }
    //     }
    // }
  ];
  selectedOption = FilterDatePeriod.Monthly;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy, private _enumService: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit() {
    this.getRequests();
    this.options = this._enumService.transform(FilterDatePeriod).map((item) => {
      item.key = Number(item.key);
      return item;
    });
  }

  getRequests() {
    console.log('this.selectedOption', this.selectedOption);
    this.loading = true;
    this._shipperDashboardServiceProxy
      .getAcceptedAndRejectedRequests(this.selectedOption)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        const accepted = result.acceptedOffers.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
        const rejected = result.rejectedOffers.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
        this.acceptedVsRejected = {
          accepted,
          rejected,
          total: accepted + rejected,
        };
        let categories = [];
        if (this.selectedOption == FilterDatePeriod.Monthly) {
          categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        }
        if (this.selectedOption == FilterDatePeriod.Weekly) {
          categories = Array.from(
            new Set<string>(result.acceptedOffers.map((item) => item.x).concat(result.rejectedOffers.map((rej) => rej.x))).values()
          );
        }
        if (this.selectedOption == FilterDatePeriod.Daily) {
          categories = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        }
        const acceptedSeries = categories.map((item) => {
          const foundFromResponse = result.acceptedOffers.find((accepted) => {
            accepted.x = this.selectedOption != FilterDatePeriod.Weekly ? accepted?.x?.slice(0, 3) : accepted?.x;
            return accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
          console.log('acceptedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        const rejectedSeries = categories.map((item) => {
          const foundFromResponse = result.rejectedOffers.find((rejected) => {
            rejected.x = this.selectedOption != FilterDatePeriod.Weekly ? rejected?.x?.slice(0, 3) : rejected?.x;
            return rejected.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
          console.log('rejectedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        console.log('acceptedSeries', acceptedSeries);
        console.log('rejectedSeries', rejectedSeries);
        this.chartOptions = {
          series: [
            {
              name: this.l('Accepted'),
              data: acceptedSeries,
              color: 'rgba(105, 228, 94, 0.89)',
            },
            {
              name: this.l('Rejected'),
              data: rejectedSeries,
              color: '#d82631',
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
        console.log('this.chartOptions', this.chartOptions);
        this.loading = false;
      });
  }
}
