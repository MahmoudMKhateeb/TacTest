import { AfterViewInit, Component, ElementRef, Injector, Input, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DedicatedShippingRequestsServiceProxy, NormalPricePackagesServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';

import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import * as _ from 'lodash';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import KTCard from '@metronic/common/js/components/card';
import { TruckPerformanceComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/truck-performance/truck-performance.component';

@Component({
  selector: 'tms-for-shipper',
  templateUrl: './tms-for-shipper.component.html',
  animations: [appModuleAnimation()],
})
export class TmsForShipperComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('truckPerformance') truckPerformance: TruckPerformanceComponent;
  @ViewChild('card', { static: true }) cardEl: ElementRef;
  @Input('shippingRequestId') shippingRequestId: number;
  card: any;
  dataSourceForDrivers: any = {};
  dataSourceForTrucks: any = {};
  activeTab = 1;

  constructor(injector: Injector, private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  ngAfterViewInit(): void {
    this.card = new KTCard(this.cardEl.nativeElement, {});
    // this.card.on('');
    console.log('this.card', this.card);
  }

  ngOnInit() {
    this.getAllTrucksAndDrivers();
  }

  getAllTrucksAndDrivers() {
    let self = this;

    this.dataSourceForDrivers = {};
    this.dataSourceForTrucks = {};
    this.dataSourceForDrivers.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._dedicatedShippingRequestsServiceProxy
          .getAllDedicatedDrivers(JSON.stringify(loadOptions), self.shippingRequestId)
          .toPromise()
          .then((response) => {
            console.log('response', response);
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });

    this.dataSourceForTrucks.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._dedicatedShippingRequestsServiceProxy
          .getAllDedicatedTrucks(JSON.stringify(loadOptions), self.shippingRequestId)
          .toPromise()
          .then((response) => {
            console.log('response', response);
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  openTruckPerformanceModal(truck: any) {
    this.truckPerformance.show(truck.id, truck.kpi, truck.numberOfTrips);
  }
}
