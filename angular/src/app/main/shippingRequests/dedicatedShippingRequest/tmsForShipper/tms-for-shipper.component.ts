import { AfterViewInit, Component, ElementRef, Injector, Input, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DedicatedShippingRequestsServiceProxy, LoadResult, NormalPricePackagesServiceProxy } from '@shared/service-proxies/service-proxies';
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
import { DedicatedShippingRequestAttendanceSheetModalComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-shipping-request-attendance-sheet-modal.component';
import { DedicatedTruckModel } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-truck-model';
import * as moment from '@node_modules/moment';
import { TruckPerformanceComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/truck-performance/truck-performance.component';
import { TruckAndDriverReplacementComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/truck-and-driver-replacement/truck-and-driver-replacement.component';

@Component({
  selector: 'tms-for-shipper',
  templateUrl: './tms-for-shipper.component.html',
  animations: [appModuleAnimation()],
})
export class TmsForShipperComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('attendanceModal', { static: true }) attendanceModal: DedicatedShippingRequestAttendanceSheetModalComponent;
  @ViewChild('truckPerformance') truckPerformance: TruckPerformanceComponent;
  @ViewChild('appTruckAndDriverReplacement') appTruckAndDriverReplacement: TruckAndDriverReplacementComponent;
  @ViewChild('card', { static: true }) cardEl: ElementRef;
  @Input('shippingRequestId') shippingRequestId: number;
  @Input('rentalRange') rentalRange: { rentalStartDate: moment.Moment; rentalEndDate: moment.Moment } = {
    rentalStartDate: null,
    rentalEndDate: null,
  };
  card: any;
  dataSourceForDrivers: any = {};
  dataSourceForTrucks: any = {};
  activeTab = 1;
  trucks: DedicatedTruckModel[] = [];

  constructor(injector: Injector, private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  ngAfterViewInit(): void {
    this.card = new KTCard(this.cardEl.nativeElement, {});
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
        return self._dedicatedShippingRequestsServiceProxy
          .getAllDedicatedDrivers(JSON.stringify(loadOptions), self.shippingRequestId)
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });

    this.dataSourceForTrucks.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._dedicatedShippingRequestsServiceProxy
          .getAllDedicatedTrucks(JSON.stringify(loadOptions), self.shippingRequestId)
          .toPromise()
          .then((response) => {
            self.trucks = response.data;
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  openFillAttendanceModal(truckId?: number) {
    this.attendanceModal.show(truckId);
  }

  openTruckPerformanceModal(truck: any) {
    this.truckPerformance.show(truck.id, truck.kpi, truck.numberOfTrips);
  }

  openTruckAndDriverReplacement(isForTruck: boolean, data) {
    console.log('data', data);
    this.appTruckAndDriverReplacement.show(isForTruck, data.id);
  }
}
