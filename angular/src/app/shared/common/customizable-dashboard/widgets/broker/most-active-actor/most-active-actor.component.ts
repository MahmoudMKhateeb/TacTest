import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { ApexTooltip, ChartComponent } from '@node_modules/ng-apexcharts';

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
  tooltip: ApexTooltip = {};

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
      this.chartOptions = undefined;
      this.fillChart(res.items);
    });
  }

  private getMostActiveActorsShipper(from, to) {
    this.brokerDashboardServiceProxy.getMostActiveActors(ActorTypesEnum.Shipper, this.selectedDateRangeType, from, to).subscribe((res) => {
      this.chartOptions = undefined;
      this.fillChart(res.items);
    });
  }

  fillChart(items: ActiveActorDto[]) {
    const that = this;
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
        floating: false,
        decimalsInFloat: 0,
        labels: {
          formatter(val) {
            return val.toFixed(0);
          },
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
    this.legend = {
      show: false,
      formatter: function (legendName: string, opts?: any) {
        return legendName;
      },
    };
    this.tooltip = {
      x: {
        show: false,
      },
      y: {
        title: {
          formatter(seriesName: string) {
            return '';
          },
        },
        formatter(val: number, opts?: any) {
          return `${that.l('Trips')}: ${val.toString()}`;
        },
      },
    };
  }
}
