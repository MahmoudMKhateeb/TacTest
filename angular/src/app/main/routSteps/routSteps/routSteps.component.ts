import { Component, Injector, ViewEncapsulation, ViewChild, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RoutStepsServiceProxy, RoutStepDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditRoutStepModalComponent } from './create-or-edit-routStep-modal.component';

import { ViewRoutStepModalComponent } from './view-routStep-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  selector: 'routSteps',
  templateUrl: './routSteps.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class RoutStepsComponent extends AppComponentBase implements OnInit {
  @Input() routeId: any;

  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditRoutStepModal', { static: true }) createOrEditRoutStepModal: CreateOrEditRoutStepModalComponent;
  @ViewChild('viewRoutStepModalComponent', { static: true }) viewRoutStepModal: ViewRoutStepModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  latitudeFilter = '';
  longitudeFilter = '';
  maxOrderFilter: number;
  maxOrderFilterEmpty: number;
  minOrderFilter: number;
  minOrderFilterEmpty: number;
  cityDisplayNameFilter = '';
  cityDisplayName2Filter = '';
  trucksTypeDisplayNameFilter = '';
  trailerTypeDisplayNameFilter = '';
  goodsDetailNameFilter = '';

  _entityTypeFullName = 'TACHYON.Routs.RoutSteps.RoutStep';
  entityHistoryEnabled = false;

  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
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

  getRoutSteps(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._routStepsServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
        this.latitudeFilter,
        this.longitudeFilter,
        this.maxOrderFilter == null ? this.maxOrderFilterEmpty : this.maxOrderFilter,
        this.minOrderFilter == null ? this.minOrderFilterEmpty : this.minOrderFilter,
        this.cityDisplayNameFilter,
        this.cityDisplayName2Filter,
        this.trucksTypeDisplayNameFilter,
        this.trailerTypeDisplayNameFilter,
        // this.goodsDetailNameFilter,
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

  createRoutStep(): void {
    this.createOrEditRoutStepModal.show();
  }

  showHistory(routStep: RoutStepDto): void {
    this.entityTypeHistoryModal.show({
      entityId: routStep.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteRoutStep(routStep: RoutStepDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._routStepsServiceProxy.delete(routStep.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  // exportToExcel(): void {
  //   this._routStepsServiceProxy
  //     .getRoutStepsToExcel(
  //       this.filterText,
  //       this.displayNameFilter,
  //       this.latitudeFilter,
  //       this.longitudeFilter,
  //       this.maxOrderFilter == null ? this.maxOrderFilterEmpty : this.maxOrderFilter,
  //       this.minOrderFilter == null ? this.minOrderFilterEmpty : this.minOrderFilter,
  //       this.cityDisplayNameFilter,
  //       this.cityDisplayName2Filter,
  //       this.routeDisplayNameFilter
  //     )
  //     .subscribe((result) => {
  //       this._fileDownloadService.downloadTempFile(result);
  //     });
  // }
}
