import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions, MeterCharts } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-host-rquest-pricing-meter',
  templateUrl: './host-rquest-pricing-meter.component.html',
  styles: [],
})
export class HostRquestPricingMeterComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<MeterCharts>;
  loading: boolean = false;

  constructor(private injector: Injector, private _hostDashboardServiceProxy: HostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getRequests();
  }

  getRequests() {
    this.loading = true;

    this._hostDashboardServiceProxy
      .getRequestBeingPricedBeforeBidEndDateCount()
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((result) => {
        this.chartOptions = {
          series: [result],

          chart: {
            height: 350,
            type: 'radialBar',
            offsetY: -10,
          },

          plotOptions: {
            radialBar: {
              startAngle: -135,
              endAngle: 135,
              dataLabels: {
                name: {
                  fontSize: '16px',
                  color: undefined,
                  offsetY: 120,
                },
                value: {
                  offsetY: 76,
                  fontSize: '22px',
                  color: undefined,
                },
              },
            },
          },
          fill: {
            type: 'gradient',
            gradient: {
              shade: 'dark',
              shadeIntensity: 0.15,
              inverseColors: false,
              opacityFrom: 1,
              opacityTo: 1,
              stops: [0, 50, 65, 91],
            },
          },
          stroke: {
            dashArray: 4,
          },
          labels: ['RequestBeingPricedBeforeBidEndDate'],
        };

        this.loading = false;
      });
  }
}
