import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarriersForDropDownDto,
  DedicatedShippingRequestsServiceProxy,
  ISelectItemDto,
  ShippingRequestsServiceProxy,
  TrucksServiceProxy,
  TrucksTypeSelectItemDto,
  TruckTruckStatusLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-truck-filter-modal',
  templateUrl: './truck-filter-modal.component.html',
  styleUrls: ['./truck-filter-modal.component.css'],
})
export class TruckFilterModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  loading = false;
  truckPattern: any = /^\d{4}\s[a-zA-Z\u0600-\u06FF]{1}\s[a-zA-Z\u0600-\u06FF]{1}\s[a-zA-Z\u0600-\u06FF]{1}$/;
  maxDate: Date = new Date();
  driverStatusesList: ISelectItemDto[] = [];
  carriersList: CarriersForDropDownDto[] = [];
  selectedCarrier: any;
  transportTypeList: ISelectItemDto[] = [];
  truckTypesList: TrucksTypeSelectItemDto[] = [];
  selectedTransportTypes: any;
  selectedCapacity: any;
  capacityList: ISelectItemDto[] = [];
  truckStatusList: TruckTruckStatusLookupTableDto[] = [];

  constructor(injector: Injector, private _shippingRequestService: ShippingRequestsServiceProxy, private _trucksService: TrucksServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getAllCarriersForDropDown();
    this.getAllTrucksTypeForTableDropdown();
    this.getAllTransportTypesForDropdown();
    this.getAllCapacitiesForDropdown();
    this.getAllTruckStatusForTableDropdown();
    this.driverStatusesList = [
      ISelectItemDto.fromJS({ id: 'true', displayName: this.l('Active') }),
      ISelectItemDto.fromJS({ id: 'false', displayName: this.l('InActive') }),
    ];
  }

  show(): void {
    this.modal.show();
  }

  close(): void {
    this.modal.hide();
  }

  search() {}

  private getAllCarriersForDropDown() {
    this._shippingRequestService.getAllCarriersForDropDown().subscribe((res) => {
      this.carriersList = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  private getAllTransportTypesForDropdown() {
    this._shippingRequestService.getAllTransportTypesForDropdown().subscribe((res) => {
      this.transportTypeList = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  private getAllTrucksTypeForTableDropdown() {
    this._shippingRequestService.getAllTrucksTypeForTableDropdown().subscribe((res) => {
      this.truckTypesList = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
  private getAllCapacitiesForDropdown() {
    this._shippingRequestService.getAllCapacitiesForDropdown().subscribe((res) => {
      this.capacityList = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }
  private getAllTruckStatusForTableDropdown() {
    this._trucksService.getAllTruckStatusForTableDropdown().subscribe((res) => {
      this.truckStatusList = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  logEvent(event) {
    console.log('event', event);
    console.log('carriersList', this.carriersList);
    console.log('selectedCarrier', this.selectedCarrier);
  }

  onSelectionChanged(event) {
    console.log('onSelectionChanged event', event);
    console.log('carriersList', this.carriersList);
    console.log('selectedCarrier', this.selectedCarrier);
  }
}
