import { AfterViewInit, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UsersComponent } from '@app/admin/users/users.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CarriersForDropDownDto, DocumentsEntitiesEnum, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';
import { ViewOrEditEntityDocumentsModalComponent } from '@app/main/documentFiles/documentFiles/documentFilesViewComponents/view-or-edit-entity-documents-modal.componant';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DriverTrackingModalComponent } from '@app/admin/users/drivers/driver-tracking-modal/driver-tracking-modal.component';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DriverFilterModalComponent } from '@app/admin/users/drivers/driver-filter/driver-filter-modal.component';
import { DriverFilter } from '@app/admin/users/drivers/driver-filter/driver-filter-model';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import * as moment from '@node_modules/moment';

@Component({
  selector: 'app-drivers',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './drivers.component.html',
  styleUrls: ['../users.component.less'],
  animations: [appModuleAnimation()],
})
export class DriversComponent extends UsersComponent implements AfterViewInit, OnInit {
  @ViewChild(DxDataGridComponent, { static: false }) dataGrid: DxDataGridComponent;
  @ViewChild('DriverTrackingModal') DriverTrackingModal: DriverTrackingModalComponent;
  @ViewChild('driverFilterModal') driverFilter: DriverFilterModalComponent;
  @ViewChild('viewOrEditEntityDocumentsModal', { static: true }) viewOrEditEntityDocumentsModal: ViewOrEditEntityDocumentsModalComponent;
  isArabic = false;
  driverId: number;
  tripId: number;
  documentsEntitiesEnum = DocumentsEntitiesEnum;
  dataSource: any = {};
  showClearSearchFilters: boolean;
  shouldClearInputs: boolean;

  constructor(
    injector: Injector,
    public _impersonationService: ImpersonationService,
    public _userServiceProxy: UserServiceProxy,
    public _fileDownloadService: FileDownloadService,
    public _activatedRoute: ActivatedRoute,
    public _httpClient: HttpClient,
    public _localStorageService: LocalStorageService
  ) {
    super(injector, _impersonationService, _userServiceProxy, _fileDownloadService, _activatedRoute, _httpClient, _localStorageService);
    this.onlyDrivers = true;
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Users/ImportDriversFromExcel';
    this.isArabic = abp.localization.currentLanguage.name.startsWith('ar');
  }

  ngAfterViewInit(): void {
    this.primengTableHelper.adjustScroll(this.dataTable);
  }

  showDriverkDocuments(driverId) {
    this.viewOrEditEntityDocumentsModal.show(driverId, DocumentsEntitiesEnum.Driver);
  }

  getDrivers(searchOptions?: LoadOptions) {
    var filter = this._activatedRoute.snapshot.queryParams['isActive'];
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        if (!loadOptions.filter) {
          loadOptions.filter = [];
          if (isNotNullOrUndefined(filter)) {
            (loadOptions.filter as any[]).push(['isActive', '=', filter]);
          }
        }
        // console.log('searchOptions', searchOptions);
        // debugger
        // if (isNotNullOrUndefined(searchOptions)) {
        //     (searchOptions.filter as any[]).map(item => {
        //         (loadOptions.filter as any[]).push(item);
        //     });
        //     searchOptions = null;
        // }
        console.log('loadOptions', loadOptions);
        return self._userServiceProxy
          .getDrivers(JSON.stringify(loadOptions))
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

  ngOnInit(): void {
    this.getDrivers();
    abp.event.on('UserDeletedEvent', () => {
      this.getDrivers();
      this.notify.success(this.l('SuccessfullyDeleted'));
    });
  }

  search(filterObject: DriverFilter) {
    const loadOptions: LoadOptions = {
      filter: [],
    };
    const keys = Object.keys(filterObject);
    for (let i = 0; i < keys.length; i++) {
      const key = keys[i];
      const val = filterObject[key];
      if (isNotNullOrUndefined(val)) {
        if (key === 'selectedCarriers' && val.length > 0) {
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
        if (key === 'creationTime' && !!val) {
          const array = [];
          const date = val as Date;
          const dateValue = moment.utc({ y: date.getFullYear(), M: date.getMonth(), d: date.getDate() });
          array.push(['creationTime', '>=', dateValue.toISOString()]);
          array.push('and');
          array.push(['creationTime', '<', dateValue.add(1, 'd').toISOString()]);
          loadOptions.filter.push(array);
          loadOptions.filter.push('and');
          continue;
        }
        if (key === 'driverName' && !!val) {
          const array = [];
          array.push(['name', '=', val]);
          array.push('or');
          array.push(['surname', '=', val]);
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
