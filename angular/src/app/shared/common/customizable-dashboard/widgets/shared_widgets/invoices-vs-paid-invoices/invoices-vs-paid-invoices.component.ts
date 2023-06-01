import { Component, Injector, Input, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ActorsServiceProxy,
  ActorTypesEnum,
  BrokerDashboardServiceProxy,
  ChartCategoryPairedValuesDto,
  InvoicesVsPaidInvoicesDto,
  SelectItemDto,
  ShipperDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';

@Component({
  selector: 'app-invoices-vs-paid-invoices',
  templateUrl: './invoices-vs-paid-invoices.component.html',
  styleUrls: ['./invoices-vs-paid-invoices.component.css'],
})
export class InvoicesVsPaidInvoicesComponent extends AppComponentBase implements OnInit {
  @Input('isForActor') isForActor = false;
  public chartOptions: Partial<ChartOptions>;

  loading = false;
  acceptedVsRejected: { total: number; paid: number; unpaid: number };
  yaxis = [
    {
      opposite: this.isRtl,
      labels: {
        formatter: function (val) {
          console.log('InvoicesVsPaidInvoicesComponent val', val);
          return isNaN(val) ? val.toFixed(0) : val;
        },
      },
    },
  ];
  shipperActors: SelectItemDto[];
  selectedShipperActor: any;

  constructor(
    injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private _actorsServiceProxy: ActorsServiceProxy,
    private dashboardCustomizationService: DashboardCustomizationService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.dashboardCustomizationService.setColors(this.hasCarrierClients && this.hasShipperClients);
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
    this._shipperDashboardServiceProxy
      .getInvoicesVSPaidInvoices()
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
    if (!isNotNullOrUndefined(this.selectedShipperActor)) {
      return;
    }
    this.loading = true;
    this._brokerDashboardServiceProxy
      .getInvoicesVsPaidInvoices(this.selectedShipperActor)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.fillChart(result);
      });
  }

  private fillChart(result: InvoicesVsPaidInvoicesDto) {
    const paid = result.paidInvoices.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
    const unpaid = result.shipperInvoices.reduce((accumulator, currentValue) => accumulator + currentValue.y, 0);
    this.acceptedVsRejected = {
      paid,
      unpaid,
      total: paid + unpaid,
    };
    // const categories = [
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
    const categories = result.paidInvoices.map((item) => item.x.slice(0, 3));
    const paidSeries = categories.map((item) => {
      const foundFromResponse = result.paidInvoices.find((accepted) => {
        accepted.x = accepted?.x?.slice(0, 3);
        return accepted.x.toLocaleLowerCase() === item.toLocaleLowerCase();
      });
      return ChartCategoryPairedValuesDto.fromJS({
        x: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.x : item,
        y: isNotNullOrUndefined(foundFromResponse) ? foundFromResponse.y : 0,
      });
    });
    const unpaidSeries = categories.map((item) => {
      const foundFromResponse = result.shipperInvoices.find((rejected) => {
        rejected.x = rejected?.x?.slice(0, 3);
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
          name: this.isShipper ? this.l('UnPaidInvoice') : this.l('Claimed'),
          data: unpaidSeries,
          color: this.dashboardCustomizationService.unpaidColor,
        },
        {
          name: this.isShipper ? this.l('PaidInvoice') : this.l('Paid'),
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
      grid: {
        row: {
          opacity: 0.5,
        },
      },
      xaxis: {
        type: 'category',
        categories,
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
    this._actorsServiceProxy.getAllActorsForDropDown(ActorTypesEnum.Shipper).subscribe((shipperActors) => {
      this.shipperActors = shipperActors;
      this.selectedShipperActor = this.shipperActors.length > 0 ? this.shipperActors[0].id : null;
      this.fetchData();
    });
  }
}
