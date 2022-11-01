import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CarriersForDropDownDto,
  DedicatedShippingRequestsServiceProxy,
  ISelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-driver-filter-modal',
  templateUrl: './driver-filter-modal.component.html',
  styleUrls: ['./driver-filter-modal.component.css'],
})
export class DriverFilterModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  loading = false;
  namePattern: any = /^[^0-9]+$/;
  maxDate: Date = new Date();
  driverStatusesList: ISelectItemDto[] = [];
  carriersList: CarriersForDropDownDto[] = [];
  selectedCarrier: any;

  constructor(injector: Injector, private _shippingRequestService: ShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit() {
    this.getAllCarriersForDropDown();
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
