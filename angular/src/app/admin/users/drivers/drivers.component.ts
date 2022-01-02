import { AfterViewInit, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UsersComponent } from '@app/admin/users/users.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { DocumentsEntitiesEnum, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';
import { ViewOrEditEntityDocumentsModalComponent } from '@app/main/documentFiles/documentFiles/documentFilesViewComponents/view-or-edit-entity-documents-modal.componant';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DriverTrackingModalComponent } from '@app/admin/users/drivers/driver-tracking-modal/driver-tracking-modal.component';

@Component({
  selector: 'app-drivers',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './drivers.component.html',
  styleUrls: ['../users.component.less'],
  animations: [appModuleAnimation()],
})
export class DriversComponent extends UsersComponent implements AfterViewInit, OnInit {
  @ViewChild('DriverTrackingModal') DriverTrackingModal: DriverTrackingModalComponent;
  @ViewChild('viewOrEditEntityDocumentsModal', { static: true }) viewOrEditEntityDocumentsModal: ViewOrEditEntityDocumentsModalComponent;
  isArabic = false;
  driverId: number;
  tripId: number;
  documentsEntitiesEnum = DocumentsEntitiesEnum;
  dataSource: any = {};

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

  getDrivers(filter) {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        if (!loadOptions.filter) {
          loadOptions.filter = [];
          (loadOptions.filter as any[]).push(['isActive', '=', filter]);
        }
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
    var filter = this._activatedRoute.snapshot.queryParams['isActive'];
    this.getDrivers(filter);
  }
}
