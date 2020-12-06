import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { UsersComponent } from '@app/admin/users/users.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { AppConsts } from '@shared/AppConsts';
import { ViewOrEditEntityDocumentsModalComponent } from '@app/main/documentFiles/documentFiles/documentFilesViewComponents/view-or-edit-entity-documents-modal.componant';

@Component({
  selector: 'app-drivers',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './drivers.component.html',
  styleUrls: ['../users.component.less'],
  animations: [appModuleAnimation()],
})
export class DriversComponent extends UsersComponent {
  @ViewChild('viewOrEditEntityDocumentsModal', { static: true }) viewOrEditEntityDocumentsModal: ViewOrEditEntityDocumentsModalComponent;

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
  }

  showDriverkDocuments(driverId) {
    this.viewOrEditEntityDocumentsModal.show(driverId, 'Driver');
  }
}
