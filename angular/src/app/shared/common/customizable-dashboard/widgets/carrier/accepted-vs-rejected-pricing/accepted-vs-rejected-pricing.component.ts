import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarrierDashboardServiceProxy,
  ChartCategoryPairedValuesDto,
  FilterDatePeriod,
  ShipperDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';

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
  options: { key: any; value: any }[] = [];
  selectedOption = FilterDatePeriod.Monthly;

  constructor(
    injector: Injector,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy,
    private _enumService: EnumToArrayPipe,
    private dashboardCustomizationService: DashboardCustomizationService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.dashboardCustomizationService.setColors(this.hasCarrierClients && this.hasShipperClients);
    this.getRequests();
    this.options = this._enumService.transform(FilterDatePeriod);
  }

  getRequests() {
    // this.acceptedReqs = [];
    // this.rejectedReqs = [];
    this.loading = true;

    this._carrierDashboardServiceProxy
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
              color: this.dashboardCustomizationService.acceptedColor,
            },
            {
              name: this.l('Rejected'),
              data: rejectedSeries,
              color: this.dashboardCustomizationService.rejectedColor,
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
