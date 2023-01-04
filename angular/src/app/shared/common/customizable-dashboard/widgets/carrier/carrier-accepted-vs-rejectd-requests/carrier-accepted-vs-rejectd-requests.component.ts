import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { CarrierDashboardServiceProxy, ChartCategoryPairedValuesDto } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-carrier-accepted-vs-rejectd-requests',
  templateUrl: './carrier-accepted-vs-rejectd-requests.component.html',
  styleUrls: ['./carrier-accepted-vs-rejectd-requests.component.css'],
})
export class CarrierAcceptedVsRejectdRequestsComponent extends AppComponentBase implements OnInit {
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

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
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
  }
}
