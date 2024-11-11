import { Component, Injector, OnInit, Renderer2 } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ContainerReturnTrackerWidgetDataDto, TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'return-container-widget',
  templateUrl: './return-container-widget.component.html',
  styleUrls: ['./return-container-widget.component.scss'],
})
export class ReturnContainerWidgetComponent extends AppComponentBase implements OnInit {
  data: ContainerReturnTrackerWidgetDataDto = new ContainerReturnTrackerWidgetDataDto();

  chartOptions = {
    series: [this.data.lessThatXDaysRemaining, this.data.moreThanXDaysRemaining, this.data.overDue, this.data.withoutReturnDate],
    chart: {
      type: 'pie',
      height: this.isRtl ? '250px' : '350px', // Adjust height for RTL
    },
    labels: [
      this.l('NonReturnedContainersIn3Days'),
      this.l('NonReturnedContainersOver3DaysRemaining'),
      this.l('NonReturnedContainersOverDue'),
      this.l('NonReturnedContainersWithoutReturnDate'),
    ],
    colors: ['#C62C2C', '#333333', '#FF8C01', '#ED2938'],
    dataLabels: {
      enabled: true,
      formatter: (val, opts) => `${opts.w.config.series[opts.seriesIndex]}`,
    },
    plotOptions: {
      pie: {
        expandOnClick: true,
        dataLabels: {
          offset: -10,
          minAngleToShowLabel: 10,
        },
      },
    },
    legend: {
      position: this.isRtl ? 'right' : 'left',
      formatter: (seriesName, opts) => {
        return `${seriesName}: ${opts.w.config.series[opts.seriesIndex]}`;
      },
    },
    tooltip: {
      y: {
        formatter: (val) => val.toFixed(0),
      },
    },
  };

  constructor(private injector: Injector, private myService: TenantDashboardServiceProxy, private renderer: Renderer2) {
    super(injector);
  }

  ngOnInit(): void {
    this.getWidgetData();
  }

  getWidgetData() {
    this.myService.getContainerReturnTrackerWidgetData(3).subscribe((res) => {
      this.data = res;
      this.chartOptions.series = [res.lessThatXDaysRemaining, res.moreThanXDaysRemaining, res.overDue, res.withoutReturnDate];
    });
  }
}
