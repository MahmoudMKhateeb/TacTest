import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ChartOptions } from '@app/shared/common/customizable-dashboard/widgets/ApexInterfaces';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  DedicatedShippingRequestsServiceProxy,
  ShipperDashboardServiceProxy,
  UpdateRequestKPIInput,
  UpdateTruckKPIInput,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DedicatedTruckDto } from '@app/main/shippingRequests/dedicatedShippingRequest/truck-performance/truck-performance-chart/dedicated-truck-dto';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-truck-performance-chart',
  templateUrl: './truck-performance-chart.component.html',
  styleUrls: ['./truck-performance-chart.component.css'],
})
export class TruckPerformanceChartComponent extends AppComponentBase implements OnInit {
  @Input('shippingRequestId') shippingRequestId: number;
  @Input('truckKPI') truckKPI: number;
  @Input('truckNumberOfTrips') truckNumberOfTrips: number;
  @Input('selectedTruckId') selectedTruckId: number;
  @Output() updatedTruckKpi: EventEmitter<boolean> = new EventEmitter<boolean>();
  trucks: DedicatedTruckDto[] = [];
  public chartOptions: Partial<ChartOptions>;
  loading = false;
  showModifyKpi: boolean;

  dataSourceForTrucks: any = {};

  constructor(injector: Injector, private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.subscribeToUpdates();
    if (isNotNullOrUndefined(this.shippingRequestId)) {
      this.getTrucks();
      return;
    }
    this.fillChartData();
  }

  fillChartData() {
    let series = [];
    let categories = [];
    console.log('this.selectedTruckId', this.selectedTruckId);
    const kpiSeriesChartData = this.trucks.map((truck, index) => {
      return truck.kpi;
    });
    const numberOfTripsChartData = this.trucks.map((truck, index) => {
      categories.push(truck.plateNumber);
      return truck.numberOfTrips;
    });
    series = [
      {
        name: this.l('KPI'),
        data: kpiSeriesChartData,
        color: 'rgba(187, 41, 41, 0.847)',
      },
      {
        name: this.l('NumberOfTrips'),
        data: numberOfTripsChartData,
        color: 'rgba(154,154,154,0.84)',
      },
    ];
    this.chartOptions = {
      series: [...series],
      chart: {
        type: 'line',
        width: '100%',
        height: 250,
        zoom: {
          type: 'x',
          enabled: true,
          autoScaleYaxis: true,
        },
      },
      xaxis: {
        categories: categories,
      },
      grid: {
        padding: {
          top: 0,
          right: 50,
          bottom: 0,
          left: 50,
        },
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
    console.log('this.chartOptions', this.chartOptions);
  }

  updateKPI() {
    const input = new UpdateTruckKPIInput({
      dedicatedTruckId: this.selectedTruckId,
      kpi: this.truckKPI,
    });
    this.loading = true;
    this._dedicatedShippingRequestsServiceProxy.updateTruckKPI(input).subscribe((res) => {
      this.notify.success(this.l('UpdatedSuccessfully'));
      this.loading = false;
      abp.event.trigger('TruckPerformanceUpdated', { isChanged: true, value: this.truckKPI });
      this.updatedTruckKpi.emit(true);
    });
  }

  getTrucks() {
    let self = this;
    this.dataSourceForTrucks = {};
    this.dataSourceForTrucks.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._dedicatedShippingRequestsServiceProxy
          .getAllDedicatedTrucks(JSON.stringify(loadOptions), self.shippingRequestId)
          .toPromise()
          .then((response) => {
            self.trucks = response.data;
            if (response.data.length > 0) {
              self.selectedTruckId = self.trucks[0].id;
              self.fillChartData();
            }
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });
    this.dataSourceForTrucks.store.load();
  }

  private subscribeToUpdates() {
    abp.event.on('TruckPerformanceUpdated', (changed) => {
      if (changed?.isChanged && isNotNullOrUndefined(this.shippingRequestId)) {
        this.truckKPI = changed.value;
        this.getTrucks();
      }
    });
  }
}
