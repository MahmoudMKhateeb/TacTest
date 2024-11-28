import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ShippingRequestsTripServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  selector: 'tripDrivers-log-table',
  templateUrl: './tripDriversLog.component.html',
})
export class TripDriversLogComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector, private _shippingRequestTripsService: ShippingRequestsTripServiceProxy) {
    super(injector);
  }

  filterText = '';
  dataSource: any = {};
  @Input() isEditOrView: boolean;
  @Input() tripId: number;
  @Input() roundTripType: number;

  ngOnInit(): void {
    this.getAll();
  }

  // getDriversLogForTable() {
  //   if (!this.isEditOrView || !this.tripId) return;
  //   this._shippingRequestTripsService.getTripDriversByTripId(this.tripId).subscribe((result) => {
  //     this.driversLogData = result;
  //   });
  // }

  getAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._shippingRequestTripsService
          .getTripDriversByTripIdDX(self.tripId, JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
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

      update: (key, values) => {
        return self._shippingRequestTripsService.updateTripDriver(values).toPromise();
      },
    });
  }

  // addOrUpdateDriverCommission(driverId: number, commission: number) {
  //   this._shippingRequestTripsService.addOrUpdateDriverCommission(this.tripId, driverId, commission).subscribe((res) => {
  //     this.notify.success('Driver Commission Updated Successfully');
  //   });
  // }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
