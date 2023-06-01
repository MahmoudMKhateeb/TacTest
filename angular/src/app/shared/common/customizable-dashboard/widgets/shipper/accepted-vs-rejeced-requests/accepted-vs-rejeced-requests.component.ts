import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartCategoryPairedValuesDto, FilterDatePeriod, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';

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
  };
  public acceptedVsRejected: any;
  options: { key: any; value: any }[] = [];
  yaxis = [{ opposite: this.isRtl }];
  selectedOption = FilterDatePeriod.Monthly;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _enumService: EnumToArrayPipe,
    private dashboardCustomizationService: DashboardCustomizationService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.dashboardCustomizationService.setColors(this.hasShipperClients && this.hasCarrierClients);
    this.getRequests();
    this.options = this._enumService.transform(FilterDatePeriod).map((item) => {
      item.key = Number(item.key);
      return item;
    });
  }

  getRequests() {
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
          // categories = [
          //   this.l('Jan'),
          //   this.l('Feb'),
          //   this.l('Mar'),
          //   this.l('Apr'),
          //   this.l('May'),
          //   this.l('Jun'),
          //   this.l('Jul'),
          //   this.l('Aug'),
          //   this.l('Sep'),
          //   this.l('Oct'),
          //   this.l('Nov'),
          //   this.l('Dec'),
          // ];
          categories = result.acceptedOffers.map((item) => item.x.slice(0, 3));
        }
        if (this.selectedOption == FilterDatePeriod.Weekly) {
          categories = Array.from(
            new Set<string>(result.acceptedOffers.map((item) => item.x).concat(result.rejectedOffers.map((rej) => rej.x))).values()
          );
        }
        if (this.selectedOption == FilterDatePeriod.Daily) {
          categories = [this.l('Sun'), this.l('Mon'), this.l('Tue'), this.l('Wed'), this.l('Thu'), this.l('Fri'), this.l('Sat')];
        }
        const acceptedSeries = categories.map((item) => {
          const foundFromResponse = result.acceptedOffers.find((accepted) => {
            accepted.x = this.selectedOption != FilterDatePeriod.Weekly ? accepted?.x?.slice(0, 3) : accepted?.x;
            return accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
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
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
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
