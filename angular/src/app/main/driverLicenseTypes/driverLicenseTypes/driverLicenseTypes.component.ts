import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DriverLicenseTypeDto, DriverLicenseTypesServiceProxy, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDriverLicenseTypeModalComponent } from './create-or-edit-driverLicenseType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { NotifyService } from 'abp-ng2-module';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  templateUrl: './driverLicenseTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class DriverLicenseTypesComponent extends AppComponentBase {
  @ViewChild('createOrEditDriverLicenseTypeModal', { static: true }) createOrEditDriverLicenseTypeModal: CreateOrEditDriverLicenseTypeModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';

  constructor(
    injector: Injector,
    private _driverLicenseTypesServiceProxy: DriverLicenseTypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getDriverLicenseTypes(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._driverLicenseTypesServiceProxy
      .getAll(
        this.filterText,
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

  createDriverLicenseType(): void {
    this.createOrEditDriverLicenseTypeModal.show();
  }

  deleteDriverLicenseType(driverLicenseType: DriverLicenseTypeDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._driverLicenseTypesServiceProxy.delete(driverLicenseType.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }
}
