import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { LoadOptionsInput, ShippingRequestsServiceProxy, ShippingRequestStatus, ShippingRequestType } from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
@Component({
  selector: 'app-shipment-history',
  templateUrl: './shipment-history.component.html',
  styleUrls: ['./shipment-history.component.css'],
  providers: [EnumToArrayPipe],
})
export class ShipmentHistoryComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};
  requestTypeData: object[] = [];
  statusData: object[] = [];
  constructor(injector: Injector, private _shippingRequestService: ShippingRequestsServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAll();
    this.requestTypeData = this.enumToArray.transform(ShippingRequestType);
    this.statusData = this.enumToArray.transform(ShippingRequestStatus);
  }

  getAll() {
    let self = this;
    this.dataSource = {};

    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._shippingRequestService
          .getAllShippingRequstHistory(JSON.stringify(loadOptions))
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
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
