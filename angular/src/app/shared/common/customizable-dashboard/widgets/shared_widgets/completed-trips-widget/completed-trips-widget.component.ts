import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { WidgetComponentBase } from '@app/shared/common/customizable-dashboard/widgets/widget-component-base';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-completed-trips-widget',
  templateUrl: './completed-trips-widget.component.html',
  styleUrls: ['./completed-trips-widget.component.css'],
})
export class CompletedTripsWidgetComponent extends AppComponentBase implements OnInit {
  public chartOptions: Partial<ChartOptions>;
  months: string[];
  trips: number[];
  toDate: moment.Moment = null;
  fromDate: moment.Moment = null;
  loading: boolean = false;
  saving = false;

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getTrips();
  }

  getTrips() {
    this.months = [];
    this.trips = [];
    this.loading = true;
    this.saving = true;
    // this._shipperDashboardServiceProxy
    //   .getCompletedTripsCountPerMonth(this.fromDate, this.toDate)
    //   .pipe(
    //     finalize(() => {
    //       this.loading = false;
    //       this.saving = false;
    //     })
    //   )
    //   .subscribe((result) => {
    //     result.forEach((element) => {
    //       this.months.push(element.month + '_' + element.year);
    //       this.trips.push(element.count);
    //     });

    //     this.chartOptions = {
    //       series: [
    //         {
    //           name: 'Trips',
    //           data: this.trips,
    //           color: '#801e1e',
    //         },
    //       ],
    //       chart: {
    //         height: 350,
    //         type: 'line',
    //         zoom: {
    //           enabled: false,
    //         },
    //       },
    //       dataLabels: {
    //         enabled: false,
    //       },
    //       stroke: {
    //         curve: 'straight',
    //       },
    //       grid: {
    //         row: {
    //           colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
    //           opacity: 0.5,
    //         },
    //       },
    //       xaxis: {
    //         categories: this.months,
    //       },
    //     };
    //     this.loading = false;
    //     this.saving = false;
    //   });
  }
}
