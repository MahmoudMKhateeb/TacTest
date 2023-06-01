import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  BrokerDashboardServiceProxy,
  GetTruckTypeUsageOutput,
  ISelectItemDto,
  MostUsedTruckTypeDto,
  ShippingRequestsServiceProxy,
  TMSAndHostDashboardServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ApexPlotOptions, ChartComponent } from '@node_modules/ng-apexcharts';
import { ApexLegend, ApexOptions } from '@node_modules/ng-apexcharts/lib/model/apex-types';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-actor-most-truck-type-used',
  templateUrl: './actor-most-truck-type-used.component.html',
  styleUrls: ['./actor-most-truck-type-used.component.scss'],
})
export class ActorMostTruckTypeUsedComponent extends AppComponentBase implements OnInit {
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ApexOptions>;
  plotOptions: ApexPlotOptions = {
    bar: {
      barHeight: '10px',
      columnWidth: '10px',
      distributed: true,
      horizontal: true,
      dataLabels: {
        position: 'bottom',
      },
    },
  };
  legend: ApexLegend = {};
  colors: string[] = [];
  allTransportTypes: ISelectItemDto[] = [];
  selectedTransportTypeId: string;
  loading: boolean;

  constructor(
    injector: Injector,
    private dashboardCustomizationService: DashboardCustomizationService,
    private brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllTransportTypes();
  }

  private getAllTransportTypes() {
    this.shippingRequestsServiceProxy.getAllTransportTypesForDropdown().subscribe((res) => {
      this.allTransportTypes = res.map((item) => {
        return item;
      });
      this.selectedTransportTypeId = this.allTransportTypes.length > 0 ? this.allTransportTypes[0].id : null;
      this.fetchData();
    });
  }

  fetchData() {
    this.getMostTruckTypeUsedComponent();
  }

  fillChart(items: MostUsedTruckTypeDto[] | GetTruckTypeUsageOutput[]) {
    this.colors = [];
    const numberOfTripsArray = items.map((item) => item.numberOfTrips);
    const categories = items.map(
      (item) => (isNotNullOrUndefined(item.name) ? item.name : item.truckTypeName + '-' + item.capacityName) + ' ' + this.l('Ton')
    );
    this.chartOptions = {
      series: [
        {
          data: numberOfTripsArray,
        },
      ],
      chart: {
        type: 'bar',
        height: 380,
      },
      plotOptions: this.plotOptions,
      colors: numberOfTripsArray.map((item, i) => (i % 2 === 0 ? '#000' : '#707070')),
      dataLabels: {
        enabled: true,
        textAnchor: 'start',
        style: {
          colors: ['#fff'],
        },
        formatter: (val, opt) => {
          return val + ' ' + this.l('Trip');
        },
        offsetX: 0,
        dropShadow: {
          enabled: true,
        },
      },
      stroke: {
        width: 4,
        colors: ['transparent'],
      },
      xaxis: {
        categories,
      },
      yaxis: {
        opposite: this.isRtl,
        labels: {
          show: true,
        },
      },
      tooltip: {
        theme: 'dark',
        x: {
          show: false,
        },
        y: {
          title: {
            formatter: function () {
              return '';
            },
          },
        },
      },
    };
    this.legend = {
      show: false,
      formatter: function (legendName: string, opts?: any) {
        return legendName;
      },
    };
  }

  getMostTruckTypeUsedComponent(): void {
    if (!isNotNullOrUndefined(this.selectedTransportTypeId)) {
      return;
    }
    this.loading = true;
    if (this.isTachyonDealerOrHost) {
      this._TMSAndHostDashboardServiceProxy.getTruckTypeUsage(Number(this.selectedTransportTypeId)).subscribe((res) => {
        this.fillChart(res);
        this.loading = false;
      });
      return;
    }
    this.brokerDashboardServiceProxy.getMostTruckTypesUsed(Number(this.selectedTransportTypeId)).subscribe((res) => {
      this.fillChart(res);
      this.loading = false;
    });
  }

  selectTransportType(transportType: ISelectItemDto) {
    this.selectedTransportTypeId = transportType.id;
    this.fetchData();
  }
}
