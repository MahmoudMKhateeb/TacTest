import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  DedicatedShippingRequestsServiceProxy,
  DedicatedShippingRequestTrucksAndDriversDto,
  GetShippingRequestForPriceOfferListDto,
  ReplaceDriverDto,
  ReplaceDriverInput,
  ReplaceTruckDto,
  ReplaceTruckInput,
  SelectItemDto,
  TrucksServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid/index';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { Router } from '@angular/router';

let _self;
@Component({
  selector: 'replace-trucks-and-drivers-modal',
  templateUrl: './replace-trucks-and-drivers-modal.component.html',
  styleUrls: ['./replace-trucks-and-drivers-modal.component.css'],
})
export class ReplaceTrucksAndDriversModalComponent extends AppComponentBase {
  @ViewChild('dataGrid', { static: false }) public dataGrid: DxDataGridComponent;
  @ViewChild('replaceTrucksAndDriversModal', { static: true }) public modal: ModalDirective;
  active = false;
  loading: boolean;
  dedicatedShippingRequest: GetShippingRequestForPriceOfferListDto;

  allDrivers: SelectItemDto[] = [];
  selectedDrivers: SelectItemDto[] = [];
  allTrucks: SelectItemDto[] = [];
  selectedTrucks: DedicatedShippingRequestTrucksAndDriversDto[] = [];
  isForTruck: boolean;
  private dedicatedShippingRequestId: number;
  dataSource: any;
  selectedItems: any[] = [];
  private dedicatedTruckTypeId: number;
  private dedicatedTruckId: number;
  private dedicatedDriverId: number;
  public get shouldDisable(): boolean {
    return (
      this.selectedItems.length === 0 ||
      (this.selectedItems.length > 0 &&
        this.dataGrid.instance.getSelectedRowsData().some((item) => {
          return (
            (this.isForTruck && !isNotNullOrUndefined(item.selectedTruckId)) ||
            (!this.isForTruck && !isNotNullOrUndefined(item.selectedDriverId)) ||
            !isNotNullOrUndefined(item.replacementIntervalInDays) ||
            item.replacementIntervalInDays?.toString().length === 0
          );
        }))
    );
  }

  constructor(
    injector: Injector,
    private _dedicatedShippingRequestService: DedicatedShippingRequestsServiceProxy,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _router: Router
  ) {
    super(injector);
    _self = this;
  }

  show(dedicatedShippingRequest: GetShippingRequestForPriceOfferListDto, isForTruck: boolean) {
    this.isForTruck = isForTruck;
    this.dedicatedShippingRequestId = dedicatedShippingRequest.id;
    this.dedicatedShippingRequest = dedicatedShippingRequest;
    this.dedicatedTruckTypeId = dedicatedShippingRequest.trucksTypeId;
    if (isForTruck) {
      this.getTrucksForReplace();
      this.getReplacementTrucksForDropDown();
    } else {
      this.getDriversForReplace();
      this.getReplacementDriversForDropDown();
    }
    this.active = true;
    this.modal.show();
  }

  showFromNotification(dedicatedShippingRequestId: number, isForTruck: boolean, driverId: number, truckId: number, truckTypeId: number) {
    this.isForTruck = isForTruck;
    this.dedicatedShippingRequestId = dedicatedShippingRequestId;
    this.dedicatedDriverId = driverId;
    this.dedicatedTruckId = truckId;
    this.dedicatedTruckTypeId = truckTypeId;
    // this.dedicatedShippingRequest = dedicatedShippingRequest;
    if (isForTruck) {
      this.getTrucksForReplace();
      this.getReplacementTrucksForDropDown();
    } else {
      this.getDriversForReplace();
      this.getReplacementDriversForDropDown();
    }
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.dedicatedShippingRequest = null;
    this.allDrivers = [];
    this.selectedDrivers = [];
    this.allTrucks = [];
    this.selectedItems = [];
    this.selectedTrucks = [];
    this._router.navigate([]);
    this.modal.hide();
  }

  save() {
    const items = this.dataGrid.instance.getSelectedRowsData().map((item) => {
      let parsedItem;
      if (this.isForTruck) {
        parsedItem = new ReplaceTruckDto({
          originalDedicatedTruckId: item.id,
          truckId: item.selectedTruckId,
          replacementReason: item.replacementReason,
          replacementIntervalInDays: item.replacementIntervalInDays,
        });
      } else {
        parsedItem = new ReplaceDriverDto({
          originalDedicatedDriverId: item.id,
          driverUserId: item.selectedDriverId,
          replacementReason: item.replacementReason,
          replacementIntervalInDays: item.replacementIntervalInDays,
        });
      }
      return parsedItem;
    });
    const replaceTrucksInput = new ReplaceTruckInput({
      shippingRequestId: this.dedicatedShippingRequestId,
      replaceTruckDtos: this.uniqueArray(items, 'originalDedicatedTruckId'),
    });
    const replaceDriversInput = new ReplaceDriverInput({
      shippingRequestId: this.dedicatedShippingRequestId,
      replaceDriverDtos: this.uniqueArray(items, 'originalDedicatedDriverId'),
    });
    console.log('replaceTrucksInput', replaceTrucksInput);
    console.log('replaceDriversInput', replaceDriversInput);
    this.loading = true;
    if (this.isForTruck) {
      this._dedicatedShippingRequestService.replaceTrucks(replaceTrucksInput).subscribe((res) => {
        this.loading = false;
        this.close();
      });
    } else {
      this._dedicatedShippingRequestService.replaceDrivers(replaceDriversInput).subscribe((res) => {
        this.loading = false;
        this.close();
      });
    }
  }

  /**
   * Driver Assignation Section
   * this method is for Getting All Carriers Drivers For DD
   */
  getReplacementDriversForDropDown() {
    if (this.feature.isEnabled('App.Carrier') || this.isTachyonDealerOrHost) {
      this._dedicatedShippingRequestService
        .getReplacementDriversForDropDown(this.dedicatedShippingRequestId, this.dedicatedShippingRequest.carrierTenantId)
        .subscribe((res) => {
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
  getReplacementTrucksForDropDown() {
    if (this.feature.isEnabled('App.Carrier') || this.isTachyonDealerOrHost) {
      this._dedicatedShippingRequestService
        .getReplacementTrucksForDropDown(this.dedicatedShippingRequestId, this.dedicatedTruckTypeId, this.dedicatedShippingRequest.carrierTenantId)
        .subscribe((res) => {
          this.allTrucks = res.map((item) => {
            (item.id as any) = Number(item.id);
            return item;
          });
        });
    }
  }

  print(d) {
    console.log('d', d);
  }

  onDataGridCellClick(event: any) {
    console.log('onDataGridCellClick', event);
    if (event.columnIndex > 0) {
      event.event.stopPropagation();
      event.event.stopImmediatePropagation();
    }
    // if (event.column.dataField === 'driverUserId' && !event.row.isSelected) {
    //   event.row.isSelected = true;
    // }
  }

  onSelectionChanged(row) {
    // if (row.selectedRowKeys.length > this.dedicatedShippingRequest?.numberOfTrucks) {
    //   this.dataGrid.instance.deselectRows(row.currentSelectedRowKeys);
    // }
  }

  private getTrucksForReplace() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._dedicatedShippingRequestService
          .getDedicatedTrucksForReplace(self.dedicatedShippingRequestId, JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            response.data.map((item) => {
              item.selectedTruckId = null;
              if (item.isRequestedToReplace) {
                self.selectedItems.push(item);
              } else {
                item.replacementIntervalInDays = null;
                item.replacementReason = null;
              }
            });
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
    });
  }

  private getDriversForReplace() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._dedicatedShippingRequestService
          .getDedicatedDriversForReplace(self.dedicatedShippingRequestId, JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            response.data.map((item) => {
              item.selectedDriverId = null;
              if (item.isRequestedToReplace) {
                self.selectedItems.push(item);
              } else {
                item.replacementIntervalInDays = null;
                item.replacementReason = null;
              }
            });
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
    });
  }

  uniqueArray(arr: any[], attributeToConsider: string) {
    let a = [];
    let uniqueIds = [];
    for (let i = 0, l = arr.length; i < l; i++) {
      if (uniqueIds.indexOf(arr[i][attributeToConsider]) === -1 && arr[i][attributeToConsider] !== '') {
        uniqueIds.push(arr[i][attributeToConsider]);
      }
    }
    a = uniqueIds.map((item) => {
      return arr.find((arrayItem) => arrayItem[attributeToConsider] === item);
    });
    return a;
  }
}
