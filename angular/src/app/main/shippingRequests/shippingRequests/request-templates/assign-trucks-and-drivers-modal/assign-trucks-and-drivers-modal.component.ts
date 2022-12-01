import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  AssignDedicatedTrucksAndDriversInput,
  AssignTrucksAndDriversForDedicatedInput,
  DedicatedDriversDto,
  DedicatedShippingRequestsServiceProxy,
  DedicatedShippingRequestTrucksAndDriversDto,
  DedicatedTruckDto,
  GetAllTrucksWithDriversListDto,
  GetShippingRequestForPriceOfferListDto,
  SelectItemDto,
  TrucksServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid/index';

let _self;
@Component({
  selector: 'assign-trucks-and-drivers-modal',
  templateUrl: './assign-trucks-and-drivers-modal.component.html',
  styleUrls: ['./assign-trucks-and-drivers-modal.component.css'],
})
export class AssignTrucksAndDriversModalComponent extends AppComponentBase {
  @ViewChild('dataGrid', { static: false }) public dataGrid: DxDataGridComponent;
  @ViewChild('assignTrucksAndDriversModal', { static: false }) public modal: ModalDirective;
  active = false;
  loading: boolean;
  dedicatedShippingRequest: GetShippingRequestForPriceOfferListDto;

  allDrivers: SelectItemDto[] = [];
  selectedDrivers: SelectItemDto[] = [];
  allTrucks: GetAllTrucksWithDriversListDto[] = [];
  selectedTrucks: DedicatedShippingRequestTrucksAndDriversDto[] = [];

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
    const trucks = this.dataGrid.instance.getSelectedRowsData().map((truck) => {
      const driverName = this.allDrivers.find((driver) => Number(driver.id) === truck.driverUserId).displayName;
      const truckForDedicated = new DedicatedShippingRequestTrucksAndDriversDto({
        truckId: truck.truckId,
        truckName: truck.truckName,
        driverName: driverName,
        driverId: truck.driverUserId,
      });
      return truckForDedicated;
    });
    const assignDedicatedTrucksAndDriversInput = new AssignTrucksAndDriversForDedicatedInput({
      shippingRequestId: this.dedicatedShippingRequest.id,
      dedicatedShippingRequestTrucksAndDriversDtos: trucks,
    });
    this.loading = true;
    this._dedicatedShippingRequestService.assignTrucksAndDriversForDedicated(assignDedicatedTrucksAndDriversInput).subscribe((res) => {
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
        this.allDrivers = res.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
      });
    }
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks(truckTypeId) {
    if (this.feature.isEnabled('App.Carrier') || this.isTachyonDealerOrHost) {
      this._dedicatedShippingRequestService
        .getAllTrucksWithDriversList(truckTypeId, this.dedicatedShippingRequest.carrierTenantId)
        .subscribe((res) => {
          this.allTrucks = res;
        });
    }
  }

  print(d) {
    console.log('d', d);
  }

  onDataGridCellClick(event: any) {
    console.log('onDataGridCellClick', event);
    if (event.columnIndex === 2 || event.columnIndex === 3) {
      event.event.stopPropagation();
      event.event.stopImmediatePropagation();
    }
    if (event.column.dataField === 'driverUserId' && !event.row.isSelected) {
      event.row.isSelected = true;
    }
  }

  onSelectionChanged(row) {
    if (row.selectedRowKeys.length > this.dedicatedShippingRequest?.numberOfTrucks) {
      this.dataGrid.instance.deselectRows(row.currentSelectedRowKeys);
    }
  }
}
