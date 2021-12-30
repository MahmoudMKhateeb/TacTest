import { Component, Injector, ViewEncapsulation, ViewChild, ChangeDetectorRef, AfterViewInit, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TrucksServiceProxy, TruckDto, DocumentsEntitiesEnum, CreateOrEditDocumentTypeDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTruckModalComponent } from './create-or-edit-truck-modal.component';

import { ViewTruckModalComponent } from './view-truck-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
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
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './trucks.component.html',
  styleUrls: ['./trucks.Component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrucksComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTruckModal', { static: true }) createOrEditTruckModal: CreateOrEditTruckModalComponent;
  @ViewChild('viewTruckModalComponent', { static: true }) viewTruckModal: ViewTruckModalComponent;
  @ViewChild('viewOrEditEntityDocumentsModal', { static: false }) viewOrEditEntityDocumentsModal: ViewOrEditEntityDocumentsModalComponent;
  @ViewChild('truckUserLookupTableModal', { static: true }) truckUserLookupTableModal: TruckUserLookupTableModalComponent;

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
  documentsEntitiesEnum = DocumentsEntitiesEnum;
  dataSource: any = {};

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
    this.getAllTrucks();
  }

  ngAfterViewInit(): void {}

  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }

  reloadPage(): void {}

  showTruckDocuments(truckId) {
    this.viewOrEditEntityDocumentsModal.show(truckId, DocumentsEntitiesEnum.Truck);
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
          this.notify.success(this.l('ImportTrucksProcessStart'));
        } else if (response.error != null) {
          this.notify.error(this.l('ImportTrucksUploadFailed'));
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

  getAllTrucks() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._trucksServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
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
}
