import { Component, Injector, OnInit } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RequestsListPerMonthDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-completed-trip-vs-pod',
  templateUrl: './completed-trip-vs-pod.component.html',
  styleUrls: ['./completed-trip-vs-pod.component.css'],
})
export class CompletedTripVsPodComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  months: string[] = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  completedTrips: number[];
  POD: number[];
  loading: boolean = false;
  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.completedTrips = [];
    this.POD = [];

    this._shipperDashboardServiceProxy.getCompletedTripVsPod().subscribe((result) => {
      this.months.forEach((d) => {
        let i = this.months.indexOf(d) + 1;
        let year = new Date().getFullYear();
        const foundAcceptElement = result.completedTrips.some((el) => el.month === i);
        if (!foundAcceptElement) {
          result.completedTrips.push(
            new RequestsListPerMonthDto({
              count: 0,
              month: i,
              year: year,
            })
          );
        }
        const foundRejectElement = result.podTrips.some((el) => el.month === i);
        if (!foundRejectElement) {
          result.podTrips.push(
            new RequestsListPerMonthDto({
              count: 0,
              month: i,
              year: year,
            })
          );
        }
      });
      result.completedTrips.sort(function (a, b) {
        return a.month - b.month;
      });
      result.completedTrips.forEach((element) => {
        this.completedTrips.push(element.count);
      });
      result.podTrips.sort(function (a, b) {
        return a.month - b.month;
      });
      result.podTrips.forEach((element) => {
        this.POD.push(element.count);
      });

      this.chartOptions = {
        series: [
          {
            name: 'Completed',
            data: this.completedTrips,
          },
          {
            name: 'Pod',
            data: this.POD,
          },
        ],
        chart: {
          height: 350,
          type: 'area',
        },
        dataLabels: {
          enabled: false,
        },
        stroke: {
          curve: 'smooth',
        },
        xaxis: {
          type: 'category',
          categories: this.months,
        },
        tooltip: {
          x: {
            format: 'dd/MM/yy',
          },
        },
      };
      this.loading = true;
    });
  }
}
