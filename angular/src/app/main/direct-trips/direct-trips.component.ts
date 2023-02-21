import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { CreateOrEditShippingRequestTripDto, ShippingRequestDto, ShippingRequestsTripServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { CreateOrEditTripComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/createOrEditTripModal/createOrEditTrip.component';

@Component({
  selector: 'app-direct-trips',
  templateUrl: './direct-trips.component.html',
  styleUrls: ['./direct-trips.component.css'],
})
export class DirectTripsComponent extends AppComponentBase implements OnInit {
  @ViewChild('AddNewTripModal', { static: true }) addNewTripModal: CreateOrEditTripComponent;
  popupPosition: any = { of: window, at: 'top', my: 'top', offset: { y: 10 } };
  dataSource: any = {};
  shippingRequest: ShippingRequestDto = new ShippingRequestDto({
    splitInvoiceFlag: '',
    bidEndDate: undefined,
    bidStartDate: undefined,
    bidStatus: undefined,
    bidStatusTitle: '',
    canAddTrip: false,
    carrierPrice: 0,
    carrierTenantId: 0,
    endTripDate: undefined,
    goodCategoryId: 0,
    hasAccident: false,
    id: 0,
    isBid: false,
    isDirectRequest: false,
    isPriceAccepted: false,
    isRejected: false,
    isSaas: false,
    isTachyonDeal: false,
    numberOfDrops: 0,
    numberOfPacking: 0,
    numberOfTrips: 0,
    otherGoodsCategoryName: '',
    otherTransportTypeName: '',
    otherTrucksTypeName: '',
    packingTypeId: 0,
    price: 0,
    requestType: undefined,
    routeTypeId: undefined,
    shipperInvoiceNo: '',
    shipperReference: '',
    shippingTypeId: 0,
    startTripDate: moment(),
    status: undefined,
    statusTitle: '',
    totalWeight: 0,
    totalsTripsAddByShippier: 0,
  });

  constructor(injector: Injector, private shippingRequestsTripServiceProxy: ShippingRequestsTripServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self.shippingRequestsTripServiceProxy
          .getAllDx(JSON.stringify(loadOptions))
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

  reloadPage() {}

  showEdit(id) {
    let record = new CreateOrEditShippingRequestTripDto();
    record.id = id;
    this.addNewTripModal.show(record);
  }
}
