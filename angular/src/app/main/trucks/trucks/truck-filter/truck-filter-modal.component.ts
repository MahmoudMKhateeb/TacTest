import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
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
import { DriverFilter } from '@app/admin/users/drivers/driver-filter/driver-filter-model';
import { TruckFilter } from '@app/main/trucks/trucks/truck-filter/truck-filter-model';

@Component({
  selector: 'app-truck-filter-modal',
  templateUrl: './truck-filter-modal.component.html',
  styleUrls: ['./truck-filter-modal.component.css'],
})
export class TruckFilterModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() searchClicked: EventEmitter<TruckFilter> = new EventEmitter<TruckFilter>();
  loading = false;
  truckPattern: any = /^\d{4}\s[a-zA-Z\u0600-\u06FF]{1}\s[a-zA-Z\u0600-\u06FF]{1}\s[a-zA-Z\u0600-\u06FF]{1}$/;
  maxDate: Date = new Date();
  driverStatusesList: ISelectItemDto[] = [];
  carriersList: CarriersForDropDownDto[] = [];
  selectedCarrier: number[] = [];
  transportTypeList: ISelectItemDto[] = [];
  truckTypesList: TrucksTypeSelectItemDto[] = [];
  selectedTruckTypes: number[] = [];
  selectedCapacity: number[] = [];
  capacityList: ISelectItemDto[] = [];
  truckStatusList: TruckTruckStatusLookupTableDto[] = [];
  truckFilterObj: TruckFilter = new TruckFilter();

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
    this.selectedCarrier = [];
    this.selectedTruckTypes = [];
    this.selectedCapacity = [];
    this.truckFilterObj = new TruckFilter();
    this.modal.hide();
  }

  search() {
    this.truckFilterObj.selectedCarrier = this.selectedCarrier.map((item) => {
      return this.carriersList.find((carrier) => Number(carrier.id) === item);
    });
    this.truckFilterObj.selectedTruckTypes = this.selectedTruckTypes.map((item) => {
      return this.transportTypeList.find((type) => Number(type.id) === item);
    });
    this.truckFilterObj.selectedCapacity = this.selectedCapacity.map((item) => {
      return this.capacityList.find((capacity) => Number(capacity.id) === item);
    });
    this.searchClicked.emit(this.truckFilterObj);
    this.close();
  }

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
