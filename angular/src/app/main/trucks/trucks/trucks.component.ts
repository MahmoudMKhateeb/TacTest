import { AfterViewInit, ChangeDetectorRef, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  CarriersForDropDownDto,
  DocumentsEntitiesEnum,
  ISelectItemDto,
  TokenAuthServiceProxy,
  TruckDto,
  TrucksServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditTruckModalComponent } from './create-or-edit-truck-modal.component';

import { ViewTruckModalComponent } from './view-truck-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import { finalize } from '@node_modules/rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { HttpClient } from '@angular/common/http';
import { FileUpload } from '@node_modules/primeng/fileupload';
import { ViewOrEditEntityDocumentsModalComponent } from '@app/main/documentFiles/documentFiles/documentFilesViewComponents/view-or-edit-entity-documents-modal.componant';
import { TruckUserLookupTableModalComponent } from './truck-user-lookup-table-modal.component';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { TruckFilter } from '@app/main/trucks/trucks/truck-filter/truck-filter-model';
import * as moment from '@node_modules/moment';

@Component({
  templateUrl: './trucks.component.html',
  styleUrls: ['./trucks.Component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrucksComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild(DxDataGridComponent, { static: false }) dataGrid: DxDataGridComponent;
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
  showClearSearchFilters: boolean;
  shouldClearInputs: boolean;

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
    this.refreshData();
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

  getTrucks() {
    var filter = this._activatedRoute.snapshot.queryParams['Active'];
    this.getAllTrucks(filter);
  }

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
          this.getTrucks();
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.refreshData();
          var filter = this._activatedRoute.snapshot.queryParams['Active'];
          this.getAllTrucks(filter);
        });
      }
    });
  }

  refreshData() {
    var filter = this._activatedRoute.snapshot.queryParams['Active'];
    this.getAllTrucks(filter);
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

  getAllTrucks(filter) {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        if (!loadOptions.filter) {
          loadOptions.filter = [];
          (loadOptions.filter as any[]).push(['truckStatusDisplayName', 'contains', filter]);
        }
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

  search(filterObject: TruckFilter) {
    const loadOptions: LoadOptions = {
      filter: [],
    };
    const keys = Object.keys(filterObject);
    for (let i = 0; i < keys.length; i++) {
      const key = keys[i];
      const val = filterObject[key];
      if (isNotNullOrUndefined(val)) {
        if (key === 'selectedCarrier' && val.length > 0) {
          const array = [];
          (val as CarriersForDropDownDto[]).map((item, index) => {
            array.push(['companyName', '=', item.displayName]);
            if (index < val.length - 1) {
              array.push('or');
            }
          });
          loadOptions.filter.push(array);
          loadOptions.filter.push('and');
          continue;
        }
        if (key === 'selectedTruckTypes' && val.length > 0) {
          const array = [];
          (val as ISelectItemDto[]).map((item, index) => {
            array.push(['trucksTypeId', '=', item.id]);
            array.push('or');
            array.push(['trucksTypeDisplayName', '=', item.displayName]);
            if (index < val.length - 1) {
              array.push('or');
            }
          });
          loadOptions.filter.push(array);
          loadOptions.filter.push('and');
          continue;
        }
        if (key === 'selectedCapacity' && val.length > 0) {
          const array = [];
          (val as ISelectItemDto[]).map((item, index) => {
            array.push(['capacityId', '=', item.id]);
            array.push('or');
            array.push(['capacityDisplayName', '=', item.displayName]);
            if (index < val.length - 1) {
              array.push('or');
            }
          });
          loadOptions.filter.push(array);
          loadOptions.filter.push('and');
          continue;
        }
        if (key === 'creationDate' && !!val) {
          const array = [];
          const date = val as Date;
          const dateValue = moment.utc({ y: date.getFullYear(), M: date.getMonth(), d: date.getDate() });
          array.push(['creationDate', '>=', dateValue.toISOString()]);
          array.push('and');
          array.push(['creationDate', '<', dateValue.add(1, 'd').toISOString()]);
          loadOptions.filter.push(array);
          loadOptions.filter.push('and');
          continue;
        }
        if (!(val instanceof Array) && !!val) {
          loadOptions.filter.push([key, '=', val]);
          if (i < keys.length - 1) {
            loadOptions.filter.push('and');
          }
        }
      }
    }
    this.dataGrid.instance.clearFilter();
    this.dataGrid.instance.filter(loadOptions.filter);
    this.showClearSearchFilters = true;
    this.shouldClearInputs = false;
  }

  clearFilters() {
    this.dataGrid.instance.clearFilter();
    this.showClearSearchFilters = false;
    this.shouldClearInputs = true;
  }
}
