import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ChartOptionsBars } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import {
  ActorsServiceProxy,
  ActorTypesEnum,
  BrokerDashboardServiceProxy,
  CarrierDashboardServiceProxy,
  ChartCategoryPairedValuesDto,
  FilterDatePeriod,
  GetCarrierInvoicesDetailsOutput,
  SelectItemDto,
  TMSAndHostDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-carrier-invoices-details-widget',
  templateUrl: './carrier-invoices-details-widget.component.html',
  styleUrls: ['./carrier-invoices-details-widget.component.css'],
})
export class CarrierInvoicesDetailsWidgetComponent extends AppComponentBase implements OnInit {
  @Input('isForActor') isForActor = false;
  public chartOptions: Partial<ChartOptionsBars>;

  loading = false;
  acceptedVsRejected: { total: number; paid: number; unpaid: number };
  yaxis = [
    {
      labels: {
        formatter: function (val) {
          return isNaN(val) ? val.toFixed(0) : val;
        },
      },
    },
  ];

  carrierActors: SelectItemDto[];
  selectedCarrierActor: any;
  private selectedOption = FilterDatePeriod.Monthly;

  constructor(
    injector: Injector,
    private _carrierDashboardServiceProxy: CarrierDashboardServiceProxy,
    private _brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private _actorsServiceProxy: ActorsServiceProxy,
    private dashboardCustomizationService: DashboardCustomizationService,
    private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.dashboardCustomizationService.setColors(this.hasShipperClients && this.hasCarrierClients);

    if (this.isForActor) {
      this.getShipperActors();
    }
    this.fetchData();
  }

  fetchData() {
    if (this.isTachyonDealerOrHost) {
      return;
    }
    if (!this.isForActor) {
      this.getInvoices();
      return;
    }
    this.getInvoicesForActors();
  }

  getInvoices() {
    this.loading = true;

    this._carrierDashboardServiceProxy
      .getCarrierInvoicesDetails(this.selectedOption)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.fillChart(result);
      });
  }

  getInvoicesForActors() {
    if (!isNotNullOrUndefined(this.selectedCarrierActor)) {
      return;
    }
    this.loading = true;

    this._brokerDashboardServiceProxy
      .getPaidVsClaimedInvoices(this.selectedCarrierActor)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.fillChart(result);
      });
  }

  private fillChart(result: GetCarrierInvoicesDetailsOutput) {
    const paid = result.paidInvoices.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
    const unpaidResult = isNotNullOrUndefined(result.newInvoices) ? result.claimed.concat(result.newInvoices) : result.claimed;
    const unpaid = unpaidResult.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
    this.acceptedVsRejected = {
      paid,
      unpaid,
      total: paid + unpaid,
    };
    const categories = [
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
    let paidSeries = result.paidInvoices;
    let unpaidSeries = result.claimed;
    // if (!this.isTachyonDealerOrHost) {
    //   paidSeries = categories.map((item) => {
    //     const foundFromResponse = result.paidInvoices.find((accepted) => {
    //       accepted.x = accepted?.x.slice(0, 3);
    //       return accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase();
    //     });
    //     return ChartCategoryPairedValuesDto.fromJS({
    //       x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
    //       y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
    //     });
    //   });
    //   unpaidSeries = categories.map((item) => {
    //     const foundFromResponse = unpaidResult.find((rejected) => {
    //       rejected.x = rejected?.x.slice(0, 3);
    //       return rejected.x.toLocaleLowerCase() === item.toLocaleLowerCase();
    //     });
    //     return ChartCategoryPairedValuesDto.fromJS({
    //       x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
    //       y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
    //     });
    //   });
    // }
    this.chartOptions = {
      series: [
        {
          name: this.l('Claimed'),
          data: unpaidSeries,
          color: this.dashboardCustomizationService.unpaidColor,
        },
        {
          name: this.l('Paid'),
          color: this.dashboardCustomizationService.paidColor,
          data: paidSeries,
        },
      ],
      chart: {
        type: 'line',
        width: '100%',
        height: 250,
        zoom: {
          enabled: false,
        },
      },
      dataLabels: {
        enabled: false,
      },
      stroke: {
        curve: 'smooth',
      },
      xaxis: {
        type: 'category',
        categories: /* !this.isTachyonDealerOrHost ? categories : */ result.paidInvoices.map((item) => item.x),
      },
      yaxis: {
        opposite: this.isRtl,
        min: 0,
        tickAmount: 1,
        floating: false,
        decimalsInFloat: 0,
      },
    };
    (this.chartOptions as any).grid = {
      row: {
        opacity: 0.5,
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
  }

  private getShipperActors() {
    this._actorsServiceProxy.getAllActorsForDropDown(ActorTypesEnum.Carrier).subscribe((shipperActors) => {
      this.carrierActors = shipperActors;
      this.selectedCarrierActor = this.carrierActors.length > 0 ? this.carrierActors[0].id : null;
      this.fetchData();
    });
  }

  selectedFilter(event: { start: moment.Moment; end: moment.Moment }) {
    this.getInvoicesForHostOrTMS(event);
  }

  private getInvoicesForHostOrTMS(event: { start: moment.Moment; end: moment.Moment }) {
    this.loading = true;
    this._TMSAndHostDashboardServiceProxy
      .getClaimedVsPaidDetails(event.start, event.end)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.fillChart(result);
      });
  }

  filterOptionSelected($event: FilterDatePeriod) {
    this.selectedOption = $event;
    this.fetchData();
  }
}
