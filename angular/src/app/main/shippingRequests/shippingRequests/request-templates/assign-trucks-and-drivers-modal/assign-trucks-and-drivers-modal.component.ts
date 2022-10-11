import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  AssignDedicatedTrucksAndDriversInput,
  DedicatedShippingRequestDriversDto,
  DedicatedShippingRequestsServiceProxy,
  DedicatedShippingRequestTrucksDto,
  GetShippingRequestForPriceOfferListDto,
} from '@shared/service-proxies/service-proxies';

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
  trucksOptions: DedicatedShippingRequestTrucksDto[] = [];
  selectedTrucks: DedicatedShippingRequestTrucksDto[] = [];

  driversOptions: DedicatedShippingRequestDriversDto[] = [];
  selectedDrivers: DedicatedShippingRequestDriversDto[] = [];

  constructor(injector: Injector, private _dedicatedShippingRequestService: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  show(dedicatedShippingRequest: GetShippingRequestForPriceOfferListDto) {
    this.getAllTrucksAndDriversForRequest();
    this.dedicatedShippingRequest = dedicatedShippingRequest;
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.dedicatedShippingRequest = null;
    this.modal.hide();
  }

  getAllTrucksAndDriversForRequest() {
    this.loading = true;
    this._dedicatedShippingRequestService.getAllTrucksAndDriversForRequest(this.dedicatedShippingRequest.id).subscribe((res) => {
      this.trucksOptions = res.dedicatedShippingRequestTrucksDtos;
      this.driversOptions = res.dedicatedShippingRequestDriversDtos;
      this.loading = false;
    });
  }

  save() {
    const assignDedicatedTrucksAndDriversInput = new AssignDedicatedTrucksAndDriversInput({
      shippingRequestId: this.dedicatedShippingRequest.id,
      driversList: this.selectedDrivers,
      trucksList: this.selectedTrucks,
    });
    this.loading = true;
    this._dedicatedShippingRequestService.assignDedicatedTrucksAndDrivers(assignDedicatedTrucksAndDriversInput).subscribe((res) => {
      this.loading = false;
      this.close();
    });
  }
}
