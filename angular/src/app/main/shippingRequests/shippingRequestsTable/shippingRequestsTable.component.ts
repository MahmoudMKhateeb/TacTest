import { AfterViewChecked, AfterViewInit, Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  GetShippingRequestForPriceOfferListDto,
  PriceOfferChannel,
  PriceOfferServiceProxy,
  ShippingRequestDirectRequestServiceProxy,
  ShippingRequestDirectRequestStatus,
  ShippingRequestFlag,
  ShippingRequestStatus,
  ShippingRequestType,
  ShippingTypeEnum,
} from '@shared/service-proxies/service-proxies';
import { ShippingRequestForPriceOfferGetAllInput } from '@app/shared/common/search/ShippingRequestForPriceOfferGetAllInput';
import { ActivatedRoute, Router } from '@angular/router';
import { ShippingrequestsDetailsModelComponent } from '@app/main/shippingRequests/shippingRequests/details/shippingrequests-details-model.component';
import * as _ from 'lodash';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { AssignTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/assign-trucks-and-drivers-modal/assign-trucks-and-drivers-modal.component';
import { ReplaceTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/replace-trucks-and-drivers-modal/replace-trucks-and-drivers-modal.component';
import { DedicatedShippingRequestAttendanceSheetModalComponent } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-shipping-request-attendance-sheet-modal.component';

@Component({
  selector: 'app-shipping-requests-table',
  templateUrl: './shippingRequestsTable.component.html',
  styleUrls: ['./shippingRequestsTable.component.scss'],
  animations: [appModuleAnimation()],
})
export class ShippingRequestsTableComponent extends AppComponentBase implements OnInit, AfterViewChecked {
  @ViewChild('Model', { static: false }) modalMore: ShippingrequestsDetailsModelComponent;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild('assignTrucksAndDriversModal', { static: false }) assignTrucksAndDriversModal: AssignTrucksAndDriversModalComponent;
  @ViewChild('replaceTrucksAndDriversModal', { static: false }) replaceTrucksAndDriversModal: ReplaceTrucksAndDriversModalComponent;
  @ViewChild('attendanceModal', { static: true }) attendanceModal: DedicatedShippingRequestAttendanceSheetModalComponent;

  @Input() Title: string;
  @Input() isTMS = false;
  @Input() Channel: PriceOfferChannel | number | null | undefined = undefined;

  items: GetShippingRequestForPriceOfferListDto[] = [];
  searchInput: ShippingRequestForPriceOfferGetAllInput = new ShippingRequestForPriceOfferGetAllInput();
  IsLoading: boolean;
  skipCount = 0;
  maxResultCount = 20;
  StopLoading = false;
  directRequestId!: number;
  activeShippingRequestId!: number;
  ShippingRequestFlag = ShippingRequestFlag;
  ShippingTypeEnum = ShippingTypeEnum;
  selectedFilterShippingRequestFlag = ShippingRequestFlag.Normal;
  shippingRequestStatusEnum = ShippingRequestStatus;
  PriceOfferChannelEnum = PriceOfferChannel;
  requestType = ShippingRequestFlag.Normal;

  constructor(
    injector: Injector,
    private router: Router,
    private _activatedRoute: ActivatedRoute,
    private _currentServ: PriceOfferServiceProxy,
    private _directRequestSrv: ShippingRequestDirectRequestServiceProxy
  ) {
    super(injector);
    this.directRequestId = this._activatedRoute.snapshot.queryParams['directRequestId'];
    this.activeShippingRequestId = this._activatedRoute.snapshot.queryParams['srId'];
  }

  ngOnInit() {
    // let loadUrl = this.baseUrl + '/api/services/app/PriceOffer/GetAllShippingRequest?';
    // this.dataSource = AspNetData.createStore({
    //     key: 'id',
    //     loadUrl,
    // });
    this.searchInput.channel = this.Channel;
    this.searchInput.requestFlag = ShippingRequestFlag.Normal;
    if (this.isTMS) {
      this.searchInput.requestType = ShippingRequestType.TachyonManageService;
      this.searchInput.isTMS = true;
    }
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
        if (e.scrollOffset.top + e.element.clientHeight === e.component.scrollHeight() && !this.StopLoading && !this.IsLoading) {
          console.log('bottom reached');
          this.skipCount += this.maxResultCount;
          this.IsLoading = true;
          this.LoadData();
        }
      });
    }
  }

  createNewRequest() {
    this.router.navigateByUrl('/app/main/shippingRequests/shippingRequestWizard');
  }

  createNewDedicatedRequest() {
    this.router.navigateByUrl('/app/main/shippingRequests/dedicatedShippingRequestWizard');
  }

  search(): void {
    this.IsLoading = true;
    this.StopLoading = false;
    this.skipCount = 0;
    this.items = [];
    this.LoadData();
  }

  LoadData() {
    this._currentServ
      .getAllShippingRequest(
        this.searchInput.filter,
        this.searchInput.carrier,
        this.searchInput.shippingRequestId,
        this.searchInput.directRequestId,
        this.searchInput.channel,
        this.searchInput.requestType,
        this.searchInput.truckTypeId,
        this.searchInput.originId,
        this.searchInput.destinationId,
        this.searchInput.pickupFromDate,
        this.searchInput.pickupToDate,
        this.searchInput.fromDate,
        this.searchInput.toDate,
        this.searchInput.routeTypeId,
        this.searchInput.status,
        this.searchInput.isTachyonDeal,
        this.searchInput.isTMS,
        this.searchInput.requestFlag,
        '',
        this.skipCount,
        this.maxResultCount
      )
      .subscribe((result) => {
        this.IsLoading = false;
        this.StopLoading = result.items.length < this.maxResultCount;
        result.items.forEach((r) => {
          if (this.isShipper) {
            if (r.requestType === ShippingRequestType.TachyonManageService && r.status === ShippingRequestStatus.NeedsAction) {
              r.statusTitle = this.l('New');
            }
          }
          // only in this case I need to use double equal not triple (type is difference)
          if (
            (this.directRequestId && r.directRequestId === this.directRequestId) ||
            (this.activeShippingRequestId && r.id === this.activeShippingRequestId)
          ) {
            this.moreRedirectTo(r);
            this.directRequestId = undefined;
          }
        });
        this.items.push(...result.items);
        // this.filterByRequestType(ShippingRequestFlag.Normal);
      });
  }

  moreRedirectTo(item: GetShippingRequestForPriceOfferListDto): void {
    if (!this.Channel && this.appSession.tenantId) {
      if (
        !this.isTachyonDealer ||
        (this.isTachyonDealer && item.requestType === ShippingRequestType.TachyonManageService) ||
        (this.isTachyonDealer && item.shippingRequestFlag === ShippingRequestFlag.Dedicated)
      ) {
        this.router.navigateByUrl(`/app/main/shippingRequests/shippingRequests/view?id=${item.id}`);
        return;
      }
    }
    this.modalMore.show(item);
  }

  delete(input: GetShippingRequestForPriceOfferListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._directRequestSrv.delete(input.directRequestId).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          _.remove(this.items, input);
        });
      }
    });
  }

  decline(input: GetShippingRequestForPriceOfferListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._directRequestSrv.decline(input.directRequestId).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeclined'));
          _.remove(this.items, input);
        });
      }
    });
  }

  canViewBrokerPrice(item: GetShippingRequestForPriceOfferListDto): boolean {
    if ((item.shipperActor || item.carrierActor) && !this.isTachyonDealerOrHost) {
      return false;
    }

    return true;
  }

  canSeeTotalOffers(item: GetShippingRequestForPriceOfferListDto) {
    if (item.price > 0) {
      return false;
    }
    if (item.totalOffers > 0 && !this.isCarrier && this.Channel != 2 && this.Channel != 10 && item.status == 2) {
      return true;
    }
    return false;
  }

  canSeeTotalOffersForShipperPrice(item: GetShippingRequestForPriceOfferListDto) {
    if (item.shipperPrice > 0) {
      return false;
    }
    if (item.totalOffers > 0 && !this.isCarrier && this.Channel != 2 && this.Channel != 10 && item.status == 2) {
      return true;
    }
    return false;
  }

  getWordTitle(n: any, word: string): string {
    if (parseInt(n) === 1) {
      return this.l(word);
    }
    return this.l(`${word}s`);
  }

  showAsList() {
    this.router.navigateByUrl(`/app/main/${this.isTMS ? 'tms' : 'shippingRequests'}/shippingRequests`);
  }

  filterByRequestType(requestType: ShippingRequestFlag) {
    this.searchInput.requestFlag = requestType;
    this.search();
  }

  canDeleteDirectRequest(input: GetShippingRequestForPriceOfferListDto) {
    if (
      this.Channel === PriceOfferChannel.DirectRequest &&
      (input.directRequestStatus === ShippingRequestDirectRequestStatus.New ||
        input.directRequestStatus === ShippingRequestDirectRequestStatus.Declined)
    ) {
      if ((this.isTachyonDealer && input.isTachyonDeal) || (this.isShipper && !input.isTachyonDeal)) {
        return true;
      }
    }
    return false;
  }

  isCarrierOwnRequest(request: GetShippingRequestForPriceOfferListDto): boolean {
    return this.isCarrierSaas && request.isSaas && this.appSession.tenantId === request.tenantId;
  }

  assignTrucksAndDrivers(item: GetShippingRequestForPriceOfferListDto) {
    console.log('item', item);
    this.assignTrucksAndDriversModal.show(item);
  }

  viewTrucksOrDrivers(item: GetShippingRequestForPriceOfferListDto, isForTruck: boolean) {
    console.log('item', item);
    this.replaceTrucksAndDriversModal.show(item, isForTruck);
  }

  openAttendanceModal(shippingRequest: GetShippingRequestForPriceOfferListDto) {
    this.attendanceModal.show(shippingRequest.status === this.shippingRequestStatusEnum.Completed, null, shippingRequest.id, {
      rentalStartDate: shippingRequest?.rentalStartDate,
      rentalEndDate: shippingRequest?.rentalEndDate,
    });
  }

  updateScrolling() {
    setTimeout(() => {
      this.dataGrid.instance.getScrollable().update();
    }, 500);
  }
}
