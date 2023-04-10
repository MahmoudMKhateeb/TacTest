import { AfterViewChecked, Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid';
import {
  PickingType,
  RoutPointTransactionDto,
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
    this._currentServ
      .getAll(
        this.searchInput.status,
        this.searchInput.shipper,
        this.searchInput.carrier,
        this.searchInput.WaybillNumber,
        this.searchInput.transportTypeId,
        this.searchInput.truckTypeId,
        this.searchInput.truckCapacityId,
        this.searchInput.originId,
        this.searchInput.destinationId,
        this.searchInput.pickupFromDate,
        this.searchInput.pickupToDate,
        this.searchInput.fromDate,
        this.searchInput.toDate,
        this.searchInput.shippingRequestReferance,
        this.searchInput.routeTypeId,
        this.searchInput.packingTypeId,
        this.searchInput.goodsOrSubGoodsCategoryId,
        this.searchInput.plateNumberId,
        this.searchInput.driverNameOrMobile,
        this.searchInput.deliveryFromDate,
        this.searchInput.deliveryToDate,
        this.searchInput.containerNumber,
        this.searchInput.isInvoiceIssued,
        this.searchInput.isSubmittedPOD,
        this.searchInput.requestTypeId,
        '',
        this.skipCount,
        this.maxResultCount
      )
      .subscribe((result) => {
        this.IsLoading = false;
        this.isFirstLoad = false;
        this.StopLoading = result.items.length < this.maxResultCount;
        result.items.map((item) => ((item as any).showSubTable = true));
        this.items.push(...result.items);
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
}
