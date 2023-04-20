import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { ChartComponent } from '@node_modules/ng-apexcharts';

import { ApexLegend, ApexOptions } from '@node_modules/ng-apexcharts/lib/model/apex-types';
import { DashboardCustomizationService } from '@app/shared/common/customizable-dashboard/dashboard-customization.service';
import { ActiveActorDto, ActorTypesEnum, BrokerDashboardServiceProxy, DateRangeType } from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'app-most-active-actor',
  templateUrl: './most-active-actor.component.html',
  styleUrls: ['./most-active-actor.component.scss'],
})
export class MostActiveActorComponent extends AppComponentBase implements OnInit {
  @Input('isForShipperActor') isForShipperActor = false;
  @ViewChild('chart') chart: ChartComponent;
  public chartOptions: Partial<ApexOptions>;
  legend: ApexLegend = {};
  DateRangeType = DateRangeType;
  selectionList: any[];
  selectedDateRangeType: DateRangeType;
  from: string;
  to: string;
  today: Date = new Date();
  colors: string[] = [];

  constructor(
    injector: Injector,
    private dashboardCustomizationService: DashboardCustomizationService,
    private brokerDashboardServiceProxy: BrokerDashboardServiceProxy,
    private enumService: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.selectionList = this.enumService.transform(DateRangeType).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.selectedDateRangeType = this.selectionList[0].key;
    this.fetchData();
  }

  fetchData(shouldFetch = true) {
    if (this.selectedDateRangeType === DateRangeType.CustomRange && !shouldFetch) {
      return;
    }
    if (this.selectedDateRangeType === DateRangeType.CustomRange && shouldFetch && (!this.from || !this.to)) {
      return;
    }
    console.log('isForShipperActor', this.isForShipperActor);
    const from = this.selectedDateRangeType == DateRangeType.CustomRange ? moment(this.from) : null;
    const to = this.selectedDateRangeType == DateRangeType.CustomRange ? moment(this.to) : null;
    if (this.isForShipperActor) {
      this.getMostActiveActorsShipper(from, to);
    } else {
      this.getMostActiveActorsCarrier(from, to);
    }
  }

  private getMostActiveActorsCarrier(from, to) {
    this.brokerDashboardServiceProxy.getMostActiveActors(ActorTypesEnum.Carrier, this.selectedDateRangeType, from, to).subscribe((res) => {
      console.log('res', res);
      this.chartOptions = undefined;
      this.fillChart(res.items);
    });
  }

  private getMostActiveActorsShipper(from, to) {
    this.brokerDashboardServiceProxy.getMostActiveActors(ActorTypesEnum.Shipper, this.selectedDateRangeType, from, to).subscribe((res) => {
      console.log('res', res);
      this.chartOptions = undefined;
      this.fillChart(res.items);
    });
  }

  fillChart(items: ActiveActorDto[]) {
    this.colors = [];
    this.chartOptions = {
      series: [
        {
          data: items.map((item, index) => {
            this.colors.push(index % 2 == 0 ? this.dashboardCustomizationService.acceptedColor : this.dashboardCustomizationService.rejectedColor);
            return {
              name: item.actorName,
              x: item.actorName,
              y: item.numberOfTrips,
              color: index % 2 == 0 ? this.dashboardCustomizationService.acceptedColor : this.dashboardCustomizationService.rejectedColor,
              fillColor: index % 2 == 0 ? this.dashboardCustomizationService.acceptedColor : this.dashboardCustomizationService.rejectedColor,
            };
          }),
        },
      ],
      chart: {
        type: 'bar',
        width: '100%',
        height: 250,
      },
      xaxis: {
        type: 'category',
        categories: items.map((item) => item.actorName),
        title: {
          text: this.l('Actors'),
        },
      },
      yaxis: {
        opposite: this.isRtl,
        min: 0,
        tickAmount: 1,
        floating: false,
        decimalsInFloat: 0,
        title: {
          text: this.l('Trips'),
        },
      },
      dataLabels: {
        enabled: false,
      },
      plotOptions: {
        bar: {
          horizontal: false,
          dataLabels: {
            position: 'top', // top, center, bottom
          },
          columnWidth: '10px',
        },
      },
    };
    // this.chartOptions = {
    //     series: [
    //         {
    //             name: 'basic',
    //             data: items.map((item, index) => item.numberOfTrips) /*[400, 430, 448, 470, 540, 580, 690, 1100, 1200, 1380]*/
    //         }
    //     ],
    //     chart: {
    //         type: 'bar',
    //         height: 250
    //     },
    //     plotOptions: {
    //         bar: {
    //             horizontal: false,
    //             dataLabels: {
    //                 position: 'top' // top, center, bottom
    //             }
    //         }
    //     },
    //     dataLabels: {
    //         enabled: false
    //     },
    //     xaxis: {
    //         categories: items.map((item, index) => item.actorName)/*[
    //             "South Korea",
    //             "Canada",
    //             "United Kingdom",
    //             "Netherlands",
    //             "Italy",
    //             "France",
    //             "Japan",
    //             "United States",
    //             "China",
    //             "Germany"
    //         ]*/
    //     }
    // };
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
}
