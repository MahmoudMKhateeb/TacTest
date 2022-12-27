import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
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

@Component({
  selector: 'app-truck-performance',
  templateUrl: './truck-performance.component.html',
  styleUrls: ['./truck-performance.component.css'],
})
export class TruckPerformanceComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  loading = false;
  truckKPI: number;
  truckNumberOfTrips: number;
  selectedTruckId: number;

  constructor(injector: Injector, private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(truckId?: number, kpi?: number, numberOfTrips?: number): void {
    if (isNotNullOrUndefined(kpi)) {
      this.truckKPI = kpi;
    }
    if (isNotNullOrUndefined(numberOfTrips)) {
      this.truckNumberOfTrips = numberOfTrips;
    }
    if (isNotNullOrUndefined(truckId)) {
      this.selectedTruckId = truckId;
    }
    this.modal.show();
  }

  close(): void {
    this.truckKPI = null;
    this.truckNumberOfTrips = null;
    this.selectedTruckId = null;
    this.modal.hide();
  }

  updatedTruckKpi(isUpdated: boolean) {
    if (isUpdated) {
      this.close();
    }
  }
}
