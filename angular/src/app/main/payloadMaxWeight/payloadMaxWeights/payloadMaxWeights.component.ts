﻿import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PayloadMaxWeightsServiceProxy, PayloadMaxWeightDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditPayloadMaxWeightModalComponent } from './create-or-edit-payloadMaxWeight-modal.component';

import { ViewPayloadMaxWeightModalComponent } from './view-payloadMaxWeight-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  templateUrl: './payloadMaxWeights.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class PayloadMaxWeightsComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditPayloadMaxWeightModal', { static: true }) createOrEditPayloadMaxWeightModal: CreateOrEditPayloadMaxWeightModalComponent;
  @ViewChild('viewPayloadMaxWeightModalComponent', { static: true }) viewPayloadMaxWeightModal: ViewPayloadMaxWeightModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  maxMaxWeightFilter: number;
  maxMaxWeightFilterEmpty: number;
  minMaxWeightFilter: number;
  minMaxWeightFilterEmpty: number;

  _entityTypeFullName = 'TACHYON.Trailers.PayloadMaxWeight.PayloadMaxWeight';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _payloadMaxWeightsServiceProxy: PayloadMaxWeightsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
  }

  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }

  getPayloadMaxWeights(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._payloadMaxWeightsServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
        this.maxMaxWeightFilter == null ? this.maxMaxWeightFilterEmpty : this.maxMaxWeightFilter,
        this.minMaxWeightFilter == null ? this.minMaxWeightFilterEmpty : this.minMaxWeightFilter,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createPayloadMaxWeight(): void {
    this.createOrEditPayloadMaxWeightModal.show();
  }

  showHistory(payloadMaxWeight: PayloadMaxWeightDto): void {
    this.entityTypeHistoryModal.show({
      entityId: payloadMaxWeight.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deletePayloadMaxWeight(payloadMaxWeight: PayloadMaxWeightDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._payloadMaxWeightsServiceProxy.delete(payloadMaxWeight.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._payloadMaxWeightsServiceProxy
      .getPayloadMaxWeightsToExcel(
        this.filterText,
        this.displayNameFilter,
        this.maxMaxWeightFilter == null ? this.maxMaxWeightFilterEmpty : this.maxMaxWeightFilter,
        this.minMaxWeightFilter == null ? this.minMaxWeightFilterEmpty : this.minMaxWeightFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
