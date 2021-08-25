import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TrucksTypesServiceProxy, TrucksTypeDto, LoadOptionsInput } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTrucksTypeModalComponent } from './create-or-edit-trucksType-modal.component';

import { ViewTrucksTypeModalComponent } from './view-trucksType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import * as moment from 'moment';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './trucksTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TrucksTypesComponent extends AppComponentBase implements OnInit {
  dataSource: any = {};

  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditTrucksTypeModal', { static: true }) createOrEditTrucksTypeModal: CreateOrEditTrucksTypeModalComponent;
  @ViewChild('viewTrucksTypeModalComponent', { static: true }) viewTrucksTypeModal: ViewTrucksTypeModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';

  _entityTypeFullName = 'TACHYON.Trucks.TrucksTypes.TrucksType';
  entityHistoryEnabled = false;

  transportTypes: any;

  constructor(
    injector: Injector,
    private _trucksTypesServiceProxy: TrucksTypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._trucksTypesServiceProxy.getAllTransportTypeForTableDropdown().subscribe((result) => {
      this.transportTypes = result;
    });
    this.DxGetAll();
  }
  DxGetAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        let Input = new LoadOptionsInput();
        Input.loadOptions = JSON.stringify(loadOptions);

        return self._trucksTypesServiceProxy
          .dxGetAll(Input)
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
        return self._trucksTypesServiceProxy.createOrEdit(values).toPromise();
      },
      update: (key, values) => {
        return self._trucksTypesServiceProxy.createOrEdit(values).toPromise();
      },
    });
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

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createTrucksType(): void {
    this.createOrEditTrucksTypeModal.show();
  }

  showHistory(trucksType: TrucksTypeDto): void {
    this.entityTypeHistoryModal.show({
      entityId: trucksType.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  // deleteTrucksType(trucksType: TrucksTypeDto): void {
  //   this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
  //     if (isConfirmed) {
  //       this._trucksTypesServiceProxy.delete(trucksType.id).subscribe(() => {
  //         this.reloadPage();
  //         this.notify.success(this.l('SuccessfullyDeleted'));
  //       });
  //     }
  //   });
  // }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
