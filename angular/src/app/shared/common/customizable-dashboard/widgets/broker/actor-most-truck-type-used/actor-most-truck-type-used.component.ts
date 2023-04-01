import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ActiveActorDto,
  BrokerDashboardServiceProxy,
  ISelectItemDto,
  MostUsedTruckTypeDto,
  ShippingRequestsServiceProxy,
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

  constructor(
    injector: Injector,
    private dashboardCustomizationService: DashboardCustomizationService,
    private brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private shippingRequestsServiceProxy: ShippingRequestsServiceProxy
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

  fillChart(items: MostUsedTruckTypeDto[]) {
    this.colors = [];
    // this.chartOptions = {
    //     series: items.map((item, index) => {
    //         this.colors.push(index % 2 == 0 ? this.dashboardCustomizationService.acceptedColor : this.dashboardCustomizationService.rejectedColor);
    //         return {
    //             name: item.actorName,
    //             data: [
    //                 {
    //                     x: item.actorName,
    //                     y: item.numberOfTrips,
    //                     color: index % 2 == 0 ? this.dashboardCustomizationService.acceptedColor : this.dashboardCustomizationService.rejectedColor
    //                 }
    //             ],
    //             color: index % 2 == 0 ? this.dashboardCustomizationService.acceptedColor : this.dashboardCustomizationService.rejectedColor,
    //         };
    //     }),
    //     chart: {
    //         type: 'bar',
    //         width: '100%',
    //         height: 250,
    //     },
    //     xaxis: {
    //         type: 'category',
    //         categories: items.map(item => item.actorName),
    //         title: {
    //             text: this.l('Actors')
    //         }
    //     },
    //     yaxis: {
    //         min: 0,
    //         tickAmount: 1,
    //         floating: false,
    //         decimalsInFloat: 0,
    //         title: {
    //             text: this.l('Trips')
    //         }
    //     },
    //     dataLabels: {
    //         enabled: true,
    //         textAnchor: 'start',
    //     },
    //     plotOptions: {
    //         bar: {
    //             horizontal: true,
    //             // dataLabels: {
    //             //     position: 'top' // top, center, bottom
    //             // },
    //             columnWidth: '10px'
    //         }
    //     },
    // };
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
      //     [
      // '#33b2df',
      // '#546E7A',
      // '#d4526e',
      // '#13d8aa',
      // '#A5978B',
      // '#2b908f',
      // '#f9a3a4',
      // '#90ee7e',
      // '#f48024',
      // '#69d2e7'
      // ]
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
        categories /*: [
                    'South Korea',
                    'Canada',
                    'United Kingdom',
                    'Netherlands',
                    'Italy',
                    'France',
                    'Japan',
                    'United States',
                    'China',
                    'India'
                ]*/,
      },
      yaxis: {
        opposite: this.isRtl,
        labels: {
          show: true,
        },
      },
      // title: {
      //     text: 'Custom DataLabels',
      //     align: 'center',
      //     floating: true
      // },
      // subtitle: {
      //     text: 'Category Names as DataLabels inside bars',
      //     align: 'center'
      // },
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
        // return result[opts.seriesIndex].numberOfTrips + ' ' + legendName;
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

  selectTransportType(transportType: ISelectItemDto) {
    this.selectedTransportTypeId = transportType.id;
    this.fetchData();
  }
}
