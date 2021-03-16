import { Component, Injector, ViewEncapsulation, ViewChild, ChangeDetectorRef, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TrucksServiceProxy, TruckDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckModalComponent } from './create-or-edit-truck-modal.component';

import { ViewTruckModalComponent } from './view-truck-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';
import { finalize } from '@node_modules/rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileUpload } from '@node_modules/primeng/fileupload';
import { ViewOrEditEntityDocumentsModalComponent } from '@app/main/documentFiles/documentFiles/documentFilesViewComponents/view-or-edit-entity-documents-modal.componant';
import { TruckUserLookupTableModalComponent } from './truck-user-lookup-table-modal.component';
import { AppSessionService } from '@shared/common/session/app-session.service';

@Component({
  templateUrl: './trucks.component.html',
  styleUrls: ['./trucks.Component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrucksComponent extends AppComponentBase implements OnInit {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTruckModal', { static: true }) createOrEditTruckModal: CreateOrEditTruckModalComponent;
  @ViewChild('viewTruckModalComponent', { static: true }) viewTruckModal: ViewTruckModalComponent;
  @ViewChild('viewOrEditEntityDocumentsModal', { static: false }) viewOrEditEntityDocumentsModal: ViewOrEditEntityDocumentsModalComponent;
  @ViewChild('truckUserLookupTableModal', { static: true }) truckUserLookupTableModal: TruckUserLookupTableModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('ExcelFileUpload', { static: false }) excelFileUpload: FileUpload;

  advancedFiltersAreShown = false;
  filterText = '';
  plateNumberFilter = '';
  modelNameFilter = '';
  modelYearFilter = '';
  isAttachableFilter = -1;
  trucksTypeDisplayNameFilter = '';
  truckStatusDisplayNameFilter = '';

  _entityTypeFullName = 'TACHYON.Trucks.Truck';
  entityHistoryEnabled = false;
  isArabic = false;
  uploadUrl: string;

  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService,
    private _httpClient: HttpClient,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super(injector);
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportTrucksFromExcel';
  }

  ngOnInit(): void {
    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
    this.isArabic = abp.localization.currentLanguage.name.startsWith('ar');
  }

  ngAfterViewInit(): void {
    this.primengTableHelper.adjustScroll(this.dataTable);
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

  getTrucks(event?: LazyLoadEvent) {
    // this.reloadPage();

    this.changeDetectorRef.detectChanges();
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._trucksServiceProxy
      .getAll(
        this.filterText,
        this.plateNumberFilter,
        this.modelNameFilter,
        this.modelYearFilter,
        this.isAttachableFilter,
        this.trucksTypeDisplayNameFilter,
        this.truckStatusDisplayNameFilter,
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

  showTruckDocuments(truckId) {
    this.viewOrEditEntityDocumentsModal.show(truckId, 'Truck');
  }

  createTruck(): void {
    this.createOrEditTruckModal.show();
  }

  showHistory(truck: TruckDto): void {
    this.entityTypeHistoryModal.show({
      entityId: truck.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteTruck(truck: TruckDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._trucksServiceProxy.delete(truck.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._trucksServiceProxy
      .getTrucksToExcel(
        this.filterText,
        this.plateNumberFilter,
        this.modelNameFilter,
        this.modelYearFilter,
        this.isAttachableFilter,
        this.trucksTypeDisplayNameFilter,
        this.truckStatusDisplayNameFilter
      )
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }

  uploadExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    formData.append('file', file, file.name);

    this._httpClient
      .post<any>(this.uploadUrl, formData)
      .pipe(finalize(() => this.excelFileUpload.clear()))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success(this.l('ImportUsersProcessStart'));
        } else if (response.error != null) {
          this.notify.error(this.l('ImportUsersUploadFailed'));
        }
      });
  }

  onUploadExcelError(): void {
    this.notify.error(this.l('ImportUsersUploadFailed'));
  }

  openSelectUserModal() {
    this.truckUserLookupTableModal.id = null;
    this.truckUserLookupTableModal.displayName = '';
    this.truckUserLookupTableModal.show();
  }
}
