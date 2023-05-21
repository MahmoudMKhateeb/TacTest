import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarriersForDropDownDto,
  DedicatedShippingRequestsServiceProxy,
  ISelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DriverFilter } from '@app/admin/users/drivers/driver-filter/driver-filter-model';
import { TruckFilter } from '@app/main/trucks/trucks/truck-filter/truck-filter-model';

@Component({
  selector: 'app-driver-filter-modal',
  templateUrl: './driver-filter-modal.component.html',
  styleUrls: ['./driver-filter-modal.component.css'],
})
export class DriverFilterModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() searchClicked: EventEmitter<DriverFilter> = new EventEmitter<DriverFilter>();
  loading = false;
  namePattern: any = /^[^0-9]+$/;
  maxDate: Date = new Date();
  driverStatusesList: ISelectItemDto[] = [];
  carriersList: CarriersForDropDownDto[] = [];
  selectedCarrier: number[] = [];
  searchObj: DriverFilter = new DriverFilter();
  _shouldClearInputs: boolean;
  @Input()
  set shouldClearInputs(val: boolean) {
    if (val) {
      this.clear();
    }
  }
  get shouldClearInputs(): boolean {
    return this._shouldClearInputs;
  }

  constructor(injector: Injector, private _shippingRequestService: ShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.driverStatusesList = [
      ISelectItemDto.fromJS({ id: 'true', displayName: this.l('Active') }),
      ISelectItemDto.fromJS({ id: 'false', displayName: this.l('InActive') }),
    ];
  }

  show(): void {
    this.getAllCarriersForDropDown();
    this.modal.show();
  }

  close(): void {
    this.modal.hide();
  }

  clear() {
    this.searchObj = new DriverFilter();
    this.selectedCarrier = [];
  }

  search() {
    console.log('searchObj', this.searchObj);
    this.searchObj.selectedCarriers = this.selectedCarrier?.map((item) => {
      return this.carriersList.find((carrier) => carrier.id === item);
    });
    this.searchClicked.emit(this.searchObj);
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

  logEvent(event) {
    console.log('event', event);
    console.log('carriersList', this.carriersList);
    console.log('selectedCarrier', this.selectedCarrier);
  }

  onSelectionChanged(event) {
    console.log('onSelectionChanged event', event);
    console.log('carriersList', this.carriersList);
    console.log('selectedCarrier', this.selectedCarrier);
    // const index = this.searchObj.selectedCarriers.findIndex(item => item.id === )
  }
}
