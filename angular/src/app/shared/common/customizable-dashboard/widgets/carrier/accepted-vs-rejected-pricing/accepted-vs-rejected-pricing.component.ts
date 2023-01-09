import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CarrierDashboardServiceProxy, ChartCategoryPairedValuesDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-accepted-vs-rejected-pricing',
  templateUrl: './accepted-vs-rejected-pricing.component.html',
  styleUrls: ['./accepted-vs-rejected-pricing.component.css'],
})
export class AcceptedVsRejectedPricingComponent extends AppComponentBase implements OnInit {
  // public chartOptions: Partial<ChartOptions>;
  months: string[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  // acceptedReqs: number[];
  // rejectedReqs: number[];
  // loading = false;

  public chartOptions: Partial<ChartOptionsBars>;
  loading = false;

  legend: ApexLegend = {
    show: false,
    // position: 'right',
    // offsetY: 40,
    // fontWeight: 500,
  };
  public acceptedVsRejected: any;
  options: string[] = [this.l('Daily'), this.l('Weekly'), this.l('Monthly')];

  constructor(injector: Injector, private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getRequests();
  }

  getRequests() {
    // this.acceptedReqs = [];
    // this.rejectedReqs = [];
    this.loading = true;

    this._carrierDashboardServiceProxy
      .getAcceptedAndRejectedRequests()
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
        const categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        const acceptedSeries = categories.map((item) => {
          const foundFromResponse = result.acceptedOffers.find((accepted) => {
            accepted.x = accepted?.x?.slice(0, 3);
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
            rejected.x = rejected?.x?.slice(0, 3);
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
          yaxis: {
            min: 0,
            tickAmount: 1,
            floating: false,
            decimalsInFloat: 0,
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
    // this.months.forEach((d) => {
    //   let i = this.months.indexOf(d) + 1;
    //   let year = new Date().getFullYear();
    //   const foundAcceptElement = result.acceptedRequests.filter((el) => el.month === i);
    //   if (!foundAcceptElement) {
    //     result.acceptedRequests.push(
    //       new RequestsListPerMonthDto({
    //         count: 0,
    //         month: i,
    //         year: year,
    //       })
    //     );
    //   }
    //   const foundRejectElement = result.rejectedRequests.filter((el) => el.month === i);
    //   if (!foundRejectElement) {
    //     result.rejectedRequests.push(
    //       new RequestsListPerMonthDto({
    //         count: 0,
    //         month: i,
    //         year: year,
    //       })
    //     );
    //   }
    // });
    // result.acceptedRequests.sort(function (a, b) {
    //   return a.month - b.month;
    // });
    // result.acceptedRequests.forEach((element) => {
    //   this.acceptedReqs.push(element.count);
    // });
    // result.rejectedRequests.sort(function (a, b) {
    //   return a.month - b.month;
    // });
    // result.rejectedRequests.forEach((element) => {
    //   this.rejectedReqs.push(element.count);
    // });
    //
    // this.chartOptions = {
    //   series: [
    //     {
    //       name: 'Accepted',
    //       data: this.acceptedReqs,
    //       color: 'rgba(187, 41, 41, 0.847)',
    //     },
    //     {
    //       name: 'Rejected',
    //       data: this.rejectedReqs,
    //       color: '#b10303',
    //     },
    //   ],
    //   chart: {
    //     height: 350,
    //     type: 'area',
    //   },
    //   dataLabels: {
    //     enabled: false,
    //   },
    //   stroke: {
    //     curve: 'smooth',
    //   },
    //   xaxis: {
    //     type: 'category',
    //     categories: this.months,
    //   },
    //   tooltip: {
    //     x: {
    //       format: 'dd/MM/yy',
    //     },
    //     y: {
    //       formatter: function (val) {
    //         return val.toFixed(0);
    //       },
    //     },
    //   },
    // };
    // this.loading = false;
    // });
  }
}
