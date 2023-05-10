import { AfterViewChecked, Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid';
import {
  PickingType,
  RoutPointTransactionDto,
  ShipmentTrackingMode,
  ShippingRequestDriverServiceProxy,
  ShippingRequestFlag,
  ShippingTypeEnum,
  TrackingListDto,
  TrackingServiceProxy,
  TrackingShippingRequestTripDto,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ActivatedRoute, Router } from '@angular/router';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { TrackingSearchInput } from '@app/shared/common/search/TrackingSearchInput';
import { finalize } from '@node_modules/rxjs/operators';
import { LoadOptions } from 'devextreme/data/load_options';
import CustomStore from 'devextreme/data/custom_store';
import { Workbook } from 'exceljs';
import { saveAs } from 'file-saver';
import { exportDataGrid } from 'devextreme/excel_exporter';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-tracking-table',
  templateUrl: './tracking-table-view.component.html',
  styleUrls: ['./tracking-table-view.component.scss'],
  animations: [appModuleAnimation()],
})
export class TrackingTableViewComponent extends AppComponentBase implements OnInit, AfterViewChecked {
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  items: TrackingListDto[] = [];
  private _searchInput: TrackingSearchInput = new TrackingSearchInput();
  @Input()
  set searchInput(val: TrackingSearchInput) {
    this._searchInput = val;
    if (!this.isFirstLoad && isNotNullOrUndefined(val)) {
      this.search();
    }
  }
  @Input() shipmentType: 'normalShipment' | 'directShipment';
  get searchInput(): TrackingSearchInput {
    return this._searchInput;
  }
  isFirstLoad = true;
  IsLoading: boolean;
  skipCount = 0;
  maxResultCount = 20;
  StopLoading = false;
  directRequestId!: number;
  activeShippingRequestId!: number;
  ShippingRequestFlag = ShippingRequestFlag;
  requestType = ShippingRequestFlag.Normal;
  waybillNumber: number;
  tripSubDetails: TrackingShippingRequestTripDto = new TrackingShippingRequestTripDto();
  showSubTable = true;
  dataSource: any = {};

  constructor(
    injector: Injector,
    private _currentServ: TrackingServiceProxy,
    private _localStorageService: LocalStorageService,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _shippingRequestDriverServiceProxy: ShippingRequestDriverServiceProxy,
    private _trackingServiceProxy: TrackingServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    super(injector);
    this.directRequestId = this._activatedRoute.snapshot.queryParams['directRequestId'];
    this.activeShippingRequestId = this._activatedRoute.snapshot.queryParams['srId'];
  }

  ngOnInit() {
    this.IsLoading = true;
    this.LoadData();
  }

  ngAfterViewChecked() {
    this.watchForDataGridReachedBottom();
  }

  private watchForDataGridReachedBottom() {
    if (isNotNullOrUndefined(this.dataGrid?.instance?.getScrollable())) {
      this.dataGrid.instance.getScrollable().on('scroll', (e) => {
        // console.log('e', e);
        // console.log('e.component.scrollHeight()', e.component.scrollHeight());
        // && !element.find('.dx-datagrid-bottom-load-panel').length
        if (
          Math.ceil(e.scrollOffset.top + e.element.clientHeight) === Math.ceil(e.component.scrollHeight()) &&
          !this.StopLoading &&
          !this.IsLoading
        ) {
          console.log('bottom reached');
          this.skipCount += this.maxResultCount;
          this.IsLoading = true;
          this.LoadData();
        }
      });
    }
  }

  LoadData() {
    if (isNotNullOrUndefined(this.waybillNumber)) {
      this.searchInput.WaybillNumber = this.waybillNumber;
    }

    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        if (self.shipmentType === AppConsts.Tracking_NormalShipment) {
          let trackingMode = this.isTachyonDealerOrHost ? ShipmentTrackingMode.NormalShipment : ShipmentTrackingMode.Mixed;
          return self._currentServ
            .getAllDx(
              JSON.stringify(loadOptions),
              self.searchInput.status,
              self.searchInput.shipper,
              self.searchInput.carrier,
              self.searchInput.WaybillNumber,
              self.searchInput.transportTypeId,
              self.searchInput.truckTypeId,
              self.searchInput.truckCapacityId,
              self.searchInput.originId,
              self.searchInput.destinationId,
              self.searchInput.pickupFromDate,
              self.searchInput.pickupToDate,
              self.searchInput.fromDate,
              self.searchInput.toDate,
              self.searchInput.shippingRequestReferance,
              self.searchInput.routeTypeId,
              self.searchInput.packingTypeId,
              self.searchInput.goodsOrSubGoodsCategoryId,
              self.searchInput.plateNumberId,
              self.searchInput.driverNameOrMobile,
              self.searchInput.deliveryFromDate,
              self.searchInput.deliveryToDate,
              self.searchInput.containerNumber,
              self.searchInput.isInvoiceIssued,
              self.searchInput.isSubmittedPOD,
              self.searchInput.requestTypeId,
              trackingMode,
              '',
              self.skipCount,
              self.maxResultCount
            )
            .toPromise()
            .then((response) => {
              self.isFirstLoad = false;
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
        }

        return self._currentServ
          .getAllDx(
            JSON.stringify(loadOptions),
            self.searchInput.status,
            self.searchInput.shipper,
            self.searchInput.carrier,
            self.searchInput.WaybillNumber,
            self.searchInput.transportTypeId,
            self.searchInput.truckTypeId,
            self.searchInput.truckCapacityId,
            self.searchInput.originId,
            self.searchInput.destinationId,
            self.searchInput.pickupFromDate,
            self.searchInput.pickupToDate,
            self.searchInput.fromDate,
            self.searchInput.toDate,
            self.searchInput.shippingRequestReferance,
            self.searchInput.routeTypeId,
            self.searchInput.packingTypeId,
            self.searchInput.goodsOrSubGoodsCategoryId,
            self.searchInput.plateNumberId,
            self.searchInput.driverNameOrMobile,
            self.searchInput.deliveryFromDate,
            self.searchInput.deliveryToDate,
            self.searchInput.containerNumber,
            self.searchInput.isInvoiceIssued,
            self.searchInput.isSubmittedPOD,
            self.searchInput.requestTypeId,
            ShipmentTrackingMode.DirectShipment,
            '',
            self.skipCount,
            self.maxResultCount
          )
          .toPromise()
          .then((response) => {
            self.isFirstLoad = false;
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

  search(): void {
    this.IsLoading = true;
    this.StopLoading = false;
    this.skipCount = 0;
    this.items = [];
    this.LoadData();
  }

  getForView(selectedRow: any) {
    setTimeout(() => {
      selectedRow.component.updateDimensions();
    }, 500);
    if (selectedRow.key instanceof Object) {
      // this.dataGrid.instance.collapseAll(-1);
      // this.dataGrid.instance.expandRow(selectedRow.key);
      selectedRow.key.showSubTable = true;
      this._trackingServiceProxy.getForView(selectedRow.key.id).subscribe((result) => {
        this.tripSubDetails = result;
      });
      setTimeout(() => {
        this.dataGrid.instance.getScrollable().update();
      }, 500);
    }
  }

  track(record: any) {
    console.log('data', record);
    this.dataGrid.instance.collapseAll(-1);
    this.dataGrid.instance.expandRow(record.key);
    record.data.showSubTable = false;
  }

  log(masterDetailItem) {
    console.log('data', masterDetailItem);
  }

  collapsedRow($event: any) {
    console.log('event', $event);
  }

  getForViewReady() {
    setTimeout(() => {
      this.dataGrid.instance.getScrollable().update();
    }, 500);
  }

  onExporting(e) {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Employees');

    exportDataGrid({
      component: e.component,
      worksheet,
      autoFilterEnabled: true,
      customizeCell: ({ gridCell, excelCell }) => {
        if (gridCell.rowType === 'data') {
          if (gridCell.column.dataField === 'waybillNumber') {
            excelCell.value = parseInt(gridCell.value).toString();
          }
        }
        if (gridCell.rowType === 'group') {
          excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'BEDFE6' } };
        }
      },
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'DataGrid.xlsx');
      });
    });
    e.cancel = true;
  }
}
