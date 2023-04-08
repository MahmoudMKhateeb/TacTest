import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarrierDashboardServiceProxy,
  ChartCategoryPairedValuesDto,
  CompletedTripVsPodListDto,
  FilterDatePeriod,
  ShipperDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ApexLegend } from '@node_modules/ng-apexcharts';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';
import { Observable } from '@node_modules/rxjs';

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
    { opposite: this.isRtl },
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
  options: { key: any; value: any }[] = [];
  selectedOption = FilterDatePeriod.Monthly;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy,
    private _enumService: EnumToArrayPipe,
    private dashboardCustomizationService: DashboardCustomizationService
  ) {
    super(injector);
  }

  ngOnInit() {
    this.dashboardCustomizationService.setColors(this.hasShipperClients && this.hasCarrierClients);
    this.getTrips();
    this.options = this._enumService.transform(FilterDatePeriod).map((item) => {
      item.key = Number(item.key);
      return item;
    });
  }

  getTrips() {
    let request: Observable<CompletedTripVsPodListDto> = this.isShipper
      ? this._shipperDashboardServiceProxy.getCompletedTripVsPod(this.selectedOption)
      : this._carrierDashboardServiceProxy.getCompletedTripVsPod(this.selectedOption);

    this.loading = true;
    request
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
        let categories = [];
        if (this.selectedOption == FilterDatePeriod.Monthly) {
          categories = [
            this.l('Jan'),
            this.l('Feb'),
            this.l('Mar'),
            this.l('Apr'),
            this.l('May'),
            this.l('Jun'),
            this.l('Jul'),
            this.l('Aug'),
            this.l('Sep'),
            this.l('Oct'),
            this.l('Nov'),
            this.l('Dec'),
          ];
        }
        if (this.selectedOption == FilterDatePeriod.Weekly) {
          categories = Array.from(new Set<string>(result.completedTrips.map((item) => item.x).concat(result.podTrips.map((rej) => rej.x))).values());
        }
        if (this.selectedOption == FilterDatePeriod.Daily) {
          categories = [this.l('Sun'), this.l('Mon'), this.l('Tue'), this.l('Wed'), this.l('Thu'), this.l('Fri'), this.l('Sat')];
        }
        const completedSeries = categories.map((item) => {
          const foundFromResponse = result.completedTrips.find((completed) => {
            completed.x = this.selectedOption != FilterDatePeriod.Weekly ? completed?.x?.slice(0, 3) : completed?.x;
            return completed.x.toLocaleLowerCase() === item.toLocaleLowerCase();
          });
          console.log('acceptedSeries foundFromResponse', foundFromResponse);
          return ChartCategoryPairedValuesDto.fromJS({
            x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
            y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
          });
        });
        const podSeries = categories.map((item) => {
          const foundFromResponse = result.podTrips.find((pod) => {
            pod.x = this.selectedOption != FilterDatePeriod.Weekly ? pod?.x?.slice(0, 3) : pod?.x;
            return pod.x.toLocaleLowerCase() === item.toLocaleLowerCase();
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
              color: this.dashboardCustomizationService.acceptedColor,
            },
            {
              name: this.l('POD'),
              data: podSeries,
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
