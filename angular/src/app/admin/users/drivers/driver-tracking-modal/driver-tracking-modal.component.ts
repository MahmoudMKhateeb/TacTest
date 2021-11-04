import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ShippingRequestDriverServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'app-driver-tracking-modal',
  templateUrl: './driver-tracking-modal.component.html',
  styleUrls: ['./driver-tracking-modal.component.css'],
})
export class DriverTrackingModalComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  @ViewChild('DriverTrackingModal', { static: true }) modal: ModalDirective;
  driverId: number;
  trackDriver = { longitude: 1, latitude: 1 };
  tripId: number;
  constructor(injector: Injector, private _ShippingRequestDriver: ShippingRequestDriverServiceProxy) {
    super(injector);
  }
  zoom = 3;
  saving = false;
  filter: any;
  dateFilter: any;
  driverLocations: any;
  active = false;
  ngOnInit(): void {
    // this.GetAllDriverLocationLogs(this.dateFilter, this.driverId, this.filter, this.tripId);
    // this.GetAllDriverLocationLogs(this.dateFilter, 130, [], null);
  }
  GetAllDriverLocationLogs(driverId: number) {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',

      load(loadOptions: LoadOptions) {
        return self._ShippingRequestDriver
          .getAllDriverLocationLogs(driverId, JSON.stringify(loadOptions), undefined)
          .toPromise()
          .then((response) => {
            self.driverLocations = response.data;
            return {
              data: response.data,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
  show(driverId: number, tripId: number) {
    console.log(driverId, 'driverId', tripId, 'tripId');
    this.active = true;
    this.driverId = driverId;
    this.tripId = tripId;
    this.modal.show();
    this.GetAllDriverLocationLogs(driverId);
  }

  close(): void {
    this.active = false;
    this.dataSource = {};
    this.modal.hide();
  }
}
