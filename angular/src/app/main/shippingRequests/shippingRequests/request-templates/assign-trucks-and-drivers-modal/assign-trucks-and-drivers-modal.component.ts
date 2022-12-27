import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  AssignDedicatedTrucksAndDriversInput,
  DedicatedDriversDto,
  DedicatedShippingRequestsServiceProxy,
  DedicatedTruckDto,
  GetShippingRequestForPriceOfferListDto,
  SelectItemDto,
  TrucksServiceProxy,
} from '@shared/service-proxies/service-proxies';

let _self;
@Component({
  selector: 'assign-trucks-and-drivers-modal',
  templateUrl: './assign-trucks-and-drivers-modal.component.html',
  styleUrls: ['./assign-trucks-and-drivers-modal.component.css'],
})
export class AssignTrucksAndDriversModalComponent extends AppComponentBase {
  @ViewChild('assignTrucksAndDriversModal', { static: false }) public modal: ModalDirective;
  active = false;
  loading: boolean;
  dedicatedShippingRequest: GetShippingRequestForPriceOfferListDto;

  allDrivers: SelectItemDto[] = [];
  selectedDrivers: SelectItemDto[] = [];
  allTrucks: SelectItemDto[] = [];
  selectedTrucks: SelectItemDto[] = [];

  constructor(
    injector: Injector,
    private _dedicatedShippingRequestService: DedicatedShippingRequestsServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy
  ) {
    super(injector);
    _self = this;
  }

  show(dedicatedShippingRequest: GetShippingRequestForPriceOfferListDto) {
    this.dedicatedShippingRequest = dedicatedShippingRequest;
    this.getAllDrivers();
    this.getAllTrucks(this.dedicatedShippingRequest.trucksTypeId);
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.dedicatedShippingRequest = null;
    this.allDrivers = [];
    this.selectedDrivers = [];
    this.allTrucks = [];
    this.selectedTrucks = [];
    this.modal.hide();
  }

  save() {
    console.log('this.selectedDrivers', this.selectedDrivers);
    console.log('this.selectedTrucks', this.selectedTrucks);
    const drivers = this.selectedDrivers.map((driver) => {
      return new DedicatedDriversDto({ driverName: driver.displayName, id: Number(driver.id) });
    });
    const trucks = this.selectedTrucks.map((truck) => {
      return new DedicatedTruckDto({ truckName: truck.displayName, id: Number(truck.id) });
    });
    const assignDedicatedTrucksAndDriversInput = new AssignDedicatedTrucksAndDriversInput({
      shippingRequestId: this.dedicatedShippingRequest.id,
      driversList: drivers,
      trucksList: trucks,
    });
    console.log('assignDedicatedTrucksAndDriversInput', assignDedicatedTrucksAndDriversInput);
    this.loading = true;
    this._dedicatedShippingRequestService.assignDedicatedTrucksAndDrivers(assignDedicatedTrucksAndDriversInput).subscribe((res) => {
      this.loading = false;
      this.close();
    });
  }

  /**
   * Driver Assignation Section
   * this method is for Getting All Carriers Drivers For DD
   */
  getAllDrivers() {
    if (this.feature.isEnabled('App.Carrier') || this.isTachyonDealerOrHost) {
      this._dedicatedShippingRequestService.getAllDriversForDropDown(this.dedicatedShippingRequest.carrierTenantId).subscribe((res) => {
        this.allDrivers = res;
      });
    }
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks(truckTypeId) {
    if (this.feature.isEnabled('App.Carrier') || this.isTachyonDealerOrHost) {
      this._dedicatedShippingRequestService
        .getAllCarrierTrucksByTruckTypeForDropDown(truckTypeId, this.dedicatedShippingRequest.carrierTenantId)
        .subscribe((res) => {
          this.allTrucks = res;
        });
    }
  }

  asyncValidationOnNumberOfTrucks(params) {
    console.log('params', params);
    return new Promise((resolve) => {
      resolve(params.value.length === _self.dedicatedShippingRequest.numberOfTrucks);
    });
  }
}
