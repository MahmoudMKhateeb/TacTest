import { Component, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TrailersServiceProxy, TrailerDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTrailerModalComponent } from './create-or-edit-trailer-modal.component';

import { ViewTrailerModalComponent } from './view-trailer-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  templateUrl: './trailers.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrailersComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTrailerModal', { static: true }) createOrEditTrailerModal: CreateOrEditTrailerModalComponent;
  @ViewChild('viewTrailerModalComponent', { static: true }) viewTrailerModal: ViewTrailerModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  trailerCodeFilter = '';
  plateNumberFilter = '';
  modelFilter = '';
  yearFilter = '';
  maxWidthFilter: number;
  maxWidthFilterEmpty: number;
  minWidthFilter: number;
  minWidthFilterEmpty: number;
  maxHeightFilter: number;
  maxHeightFilterEmpty: number;
  minHeightFilter: number;
  minHeightFilterEmpty: number;
  maxLengthFilter: number;
  maxLengthFilterEmpty: number;
  minLengthFilter: number;
  minLengthFilterEmpty: number;
  isLiftgateFilter = -1;
  isReeferFilter = -1;
  isVentedFilter = -1;
  isRollDoorFilter = -1;
  trailerStatusDisplayNameFilter = '';
  trailerTypeDisplayNameFilter = '';
  payloadMaxWeightDisplayNameFilter = '';
  truckPlateNumberFilter = '';

  _entityTypeFullName = 'TACHYON.Trailers.Trailer';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _trailersServiceProxy: TrailersServiceProxy,
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

  getTrailers(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._trailersServiceProxy
      .getAll(
        this.filterText,
        this.trailerCodeFilter,
        this.plateNumberFilter,
        this.modelFilter,
        this.yearFilter,
        this.maxWidthFilter == null ? this.maxWidthFilterEmpty : this.maxWidthFilter,
        this.minWidthFilter == null ? this.minWidthFilterEmpty : this.minWidthFilter,
        this.maxHeightFilter == null ? this.maxHeightFilterEmpty : this.maxHeightFilter,
        this.minHeightFilter == null ? this.minHeightFilterEmpty : this.minHeightFilter,
        this.maxLengthFilter == null ? this.maxLengthFilterEmpty : this.maxLengthFilter,
        this.minLengthFilter == null ? this.minLengthFilterEmpty : this.minLengthFilter,
        this.isLiftgateFilter,
        this.isReeferFilter,
        this.isVentedFilter,
        this.isRollDoorFilter,
        this.trailerStatusDisplayNameFilter,
        this.trailerTypeDisplayNameFilter,
        this.payloadMaxWeightDisplayNameFilter,
        this.truckPlateNumberFilter,
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

  createTrailer(): void {
    this.createOrEditTrailerModal.show();
  }

  showHistory(trailer: TrailerDto): void {
    this.entityTypeHistoryModal.show({
      entityId: trailer.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteTrailer(trailer: TrailerDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._trailersServiceProxy.delete(trailer.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._trailersServiceProxy
      .getTrailersToExcel(
        this.filterText,
        this.trailerCodeFilter,
        this.plateNumberFilter,
        this.modelFilter,
        this.yearFilter,
        this.maxWidthFilter == null ? this.maxWidthFilterEmpty : this.maxWidthFilter,
        this.minWidthFilter == null ? this.minWidthFilterEmpty : this.minWidthFilter,
        this.maxHeightFilter == null ? this.maxHeightFilterEmpty : this.maxHeightFilter,
        this.minHeightFilter == null ? this.minHeightFilterEmpty : this.minHeightFilter,
        this.maxLengthFilter == null ? this.maxLengthFilterEmpty : this.maxLengthFilter,
        this.minLengthFilter == null ? this.minLengthFilterEmpty : this.minLengthFilter,
        this.isLiftgateFilter,
        this.isReeferFilter,
        this.isVentedFilter,
        this.isRollDoorFilter,
        this.trailerStatusDisplayNameFilter,
        this.trailerTypeDisplayNameFilter,
        this.payloadMaxWeightDisplayNameFilter,
        this.truckPlateNumberFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }
}
