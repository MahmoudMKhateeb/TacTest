import { Component, Injector, Input, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DriverLicenseTypesServiceProxy, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDriverLicenseTypeModalComponent } from './create-or-edit-driverLicenseType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { NotifyService } from 'abp-ng2-module';
import { LazyLoadEvent } from 'primeng/api';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  templateUrl: './driverLicenseTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class DriverLicenseTypesComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditDriverLicenseTypeModal', { static: true }) createOrEditDriverLicenseTypeModal: CreateOrEditDriverLicenseTypeModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  dataSource: any = {};
  @Input() CoreId: any;

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
  ngOnInit(): void {
    this.getAll();
  }

  getAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._driverLicenseTypesServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
      insert: (values) => {
        return self._driverLicenseTypesServiceProxy.createOrEdit(values).toPromise();
      },
      update: (key, values) => {
        return self._driverLicenseTypesServiceProxy.createOrEdit(values).toPromise();
      },
      remove: (key) => {
        return self._driverLicenseTypesServiceProxy.delete(key).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
