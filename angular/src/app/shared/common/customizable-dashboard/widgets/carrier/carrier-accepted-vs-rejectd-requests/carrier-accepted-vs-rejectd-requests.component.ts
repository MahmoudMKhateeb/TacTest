import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartOptions, ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { CarrierDashboardServiceProxy, ChartCategoryPairedValuesDto, FilterDatePeriod } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';

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

  ngOnInit() {
    this.getRequests();
    this.options = this._enumService.transform(FilterDatePeriod);
    this.dashboardCustomizationService.setColors(this.hasShipperClients && this.hasCarrierClients);
  }

  getRequests() {
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
          yaxis: {
            opposite: this.isRtl,
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
        this.loading = false;
      });
  }
}
