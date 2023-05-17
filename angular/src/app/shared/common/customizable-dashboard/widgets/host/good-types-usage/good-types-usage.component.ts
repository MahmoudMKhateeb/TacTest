import { Component, HostListener, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ActiveActorDto,
  BrokerDashboardServiceProxy,
  GetAllGoodsCategoriesForDropDownOutput,
  ISelectItemDto,
  MostUsedTruckTypeDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ApexPlotOptions, ChartComponent } from '@node_modules/ng-apexcharts';
import { ApexLegend, ApexOptions } from '@node_modules/ng-apexcharts/lib/model/apex-types';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-good-types-usage',
  templateUrl: './good-types-usage.component.html',
  styleUrls: ['./good-types-usage.component.scss'],
})
export class GoodTypesUsageComponent extends AppComponentBase implements OnInit {
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
  allGoodTypes: GetAllGoodsCategoriesForDropDownOutput[] = [];
  selectedTransportTypeId: number;

  @HostListener('wheel', ['$event']) onScroll(event: WheelEvent): void {
    const targetElement = event.target as HTMLElement;
    const scrollableColElement = targetElement.closest('.scrollable-col');
    if (scrollableColElement) {
      event.stopPropagation();
    }
  }

  constructor(
    injector: Injector,
    private dashboardCustomizationService: DashboardCustomizationService,
    private brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private shippingRequestsServiceProxy: ShippingRequestsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllGoodTypes();
  }

  private getAllGoodTypes() {
    this.shippingRequestsServiceProxy.getAllGoodCategoriesForTableDropdown().subscribe((res) => {
      this.allGoodTypes = res.map((item) => {
        return item;
      });
      this.selectedTransportTypeId = this.allGoodTypes.length > 0 ? this.allGoodTypes[0].id : null;
      this.fetchData();
    });
  }

  fetchData() {
    this.getMostTruckTypeUsedComponent();
  }

  fillChart(items: MostUsedTruckTypeDto[]) {
    this.colors = [];
    const numberOfTripsArray = items.map((item) => item.numberOfTrips);
    const categories = items.map((item) => item.truckTypeName + '-' + item.capacityName + ' ' + this.l('Ton'));
    this.chartOptions = {
      series: [
        {
          data: numberOfTripsArray /*[400, 430, 448, 470, 540, 580, 690, 1100, 1200, 1380]*/,
        },
      ],
      chart: {
        type: 'bar',
        height: 380,
      },
      plotOptions: this.plotOptions,
      colors: numberOfTripsArray.map((item, i) => (i % 2 == 0 ? '#000' : '#707070')),

      dataLabels: {
        enabled: true,
        textAnchor: 'start',
        style: {
          colors: ['#fff'],
        },
        formatter: (val, opt) => {
          return val + ' ' + this.l('Trip') /*opt.w.globals.labels[opt.dataPointIndex] + ':  ' + val*/;
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
        console.log('legendName', legendName);
        console.log('opts', opts);
        return legendName;
      },
    };
  }

  getMostTruckTypeUsedComponent(): void {
    if (!isNotNullOrUndefined(this.selectedTransportTypeId)) {
      return;
    }
    this.brokerDashboardServiceProxy.getMostTruckTypesUsed(Number(this.selectedTransportTypeId)).subscribe((res) => {
      this.fillChart(res);
    });
  }

  selectTransportType(transportType: GetAllGoodsCategoriesForDropDownOutput) {
    this.selectedTransportTypeId = transportType.id;
    this.fetchData();
  }
}
