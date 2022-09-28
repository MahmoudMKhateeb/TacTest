import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditPriceOfferInput,
  CreateSrPostPriceUpdateActionDto,
  CreateSrPostPriceUpdateOfferActionDto,
  SrPostPriceUpdateAction,
  SrPostPriceUpdateListDto,
  SrPostPriceUpdateOfferAction,
  SrPostPriceUpdateOfferStatus,
  SrPostPriceUpdateServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { PriceOfferModelComponent } from '@app/main/priceoffer/price-offer-model-component';
import { OfferPostPriceResponse } from './offer-post-price-response';
import { isNotNullOrUndefined } from 'codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'SrPostPriceUpdatesModal',
  templateUrl: './sr-post-price-update.component.html',
  styleUrls: ['./sr-post-price-update.component.css'],
})
export class SrPostPriceUpdateComponent extends AppComponentBase {
  @ViewChild('srUpdateModal') modal: ModalDirective;
  @ViewChild('PriceOfferModal') priceOfferModal: PriceOfferModelComponent;
  @ViewChild('Paginator') paginator: Paginator;
  @ViewChild('UpdatesTable') table: Table;
  updates: SrPostPriceUpdateListDto[];
  loading: boolean;
  activeUpdateIdForRepricing: number;
  shippingRequestId: number;

  constructor(private injector: Injector, private _serviceProxy: SrPostPriceUpdateServiceProxy, private cdref: ChangeDetectorRef) {
    super(injector);
  }

  show(shippingRequestId: number) {
    this.modal.show();
    this.shippingRequestId = shippingRequestId;
    this.getSrUpdates();
  }

  close() {
    this.modal.hide();
    this.shippingRequestId = undefined;
    this.updates = undefined;
  }

  getSrUpdates(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.loading = true;
    if (!isNotNullOrUndefined(this.shippingRequestId)) {
      this.loading = false;
      return;
    }
    this._serviceProxy
      .getAll(
        this.shippingRequestId,
        this.primengTableHelper.getSorting(this.table) ?? 'creationTime DECS',
        this.primengTableHelper.getMaxResultCount(this.paginator, event),
        this.primengTableHelper.getSkipCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.updates = result.items;
        this.loading = false;
      });
  }

  refreshPage() {
    this.paginator.changePage(0);
  }

  acceptChanges(srUpdateId: number) {
    let dto = new CreateSrPostPriceUpdateActionDto();
    dto.id = srUpdateId;
    dto.action = SrPostPriceUpdateAction.Accept;
    this.loading = true;
    this._serviceProxy.createUpdateAction(dto).subscribe(() => {
      this.refreshPage();
      this.notify.success(this.l('SavedSuccessfully'));
    });
  }

  acceptChangesWithReprice(srUpdate: SrPostPriceUpdateListDto) {
    this.activeUpdateIdForRepricing = srUpdate.id;
    this.priceOfferModal.show(this.shippingRequestId, undefined, undefined, undefined, true);
  }

  sendPostPriceOffer(event: CreateOrEditPriceOfferInput) {
    let dto = new CreateSrPostPriceUpdateActionDto();
    dto.id = this.activeUpdateIdForRepricing;
    dto.action = SrPostPriceUpdateAction.ChangePrice;
    dto.offer = event;
    this.loading = true;
    this._serviceProxy.createUpdateAction(dto).subscribe(() => {
      this.refreshPage();
      this.notify.success(this.l('SavedSuccessfully'));
    });
  }

  takeOfferAction(response: OfferPostPriceResponse) {
    if (!isNotNullOrUndefined(response.isOfferRejected)) return;
    let dto = new CreateSrPostPriceUpdateOfferActionDto();
    dto.id = this.activeUpdateIdForRepricing;

    if (!response.isOfferRejected) {
      dto.offerAction = SrPostPriceUpdateOfferAction.Accept;

      this._serviceProxy.createOfferAction(dto).subscribe(() => {
        this.notify.success(this.l('SuccessfullyAccepted'));
        this.refreshPage();
      });
    } else {
      dto.offerAction = SrPostPriceUpdateOfferAction.Reject;
      dto.offerRejectionReason = response.rejectionReason;

      this._serviceProxy.createOfferAction(dto).subscribe(() => {
        this.notify.success(this.l('SuccessfullyRejected'));
        this.refreshPage();
      });
    }
  }

  setActiveId(updateId: number) {
    this.activeUpdateIdForRepricing ??= updateId;
  }

  isUpdateHasOffer(update: SrPostPriceUpdateListDto): boolean {
    return update.action === SrPostPriceUpdateAction.ChangePrice && isNotNullOrUndefined(update.priceOfferId);
  }

  getUpdateStatus(update: SrPostPriceUpdateListDto) {
    if (update.offerStatus !== SrPostPriceUpdateOfferStatus.None) {
      return this.l(update.offerStatusTitle);
    }

    if (update.action === SrPostPriceUpdateAction.Pending) {
      return this.l('PostPriceUpdateWaitingForCarrierAction');
    }
    return update.isApplied ? this.l('Applied') : this.l('NotApplied');
  }

  ngAfterContentChecked() {
    this.cdref.detectChanges();
  }
}
