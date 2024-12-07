﻿import { Component, Injector, ViewEncapsulation, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UnitOfMeasuresServiceProxy, UnitOfMeasureDto, LoadOptionsInput } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditUnitOfMeasureModalComponent } from './create-or-edit-unitOfMeasure-modal.component';

import { ViewUnitOfMeasureModalComponent } from './view-unitOfMeasure-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import CustomStore from 'devextreme/data/custom_store';
import { LoadOptions } from 'devextreme/data/load_options';

@Component({
  templateUrl: './unitOfMeasures.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class UnitOfMeasuresComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditUnitOfMeasureModal', { static: true }) createOrEditUnitOfMeasureModal: CreateOrEditUnitOfMeasureModalComponent;
  @ViewChild('viewUnitOfMeasureModalComponent', { static: true }) viewUnitOfMeasureModal: ViewUnitOfMeasureModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  dataSource: any = {};
  constructor(
    injector: Injector,
    private _unitOfMeasuresServiceProxy: UnitOfMeasuresServiceProxy,
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

  // getUnitOfMeasures(event?: LazyLoadEvent) {
  //   if (this.primengTableHelper.shouldResetPaging(event)) {
  //     this.paginator.changePage(0);
  //     return;
  //   }

  //   this.primengTableHelper.showLoadingIndicator();

  //   this._unitOfMeasuresServiceProxy
  //     .getAll(
  //       this.filterText,
  //       this.displayNameFilter,
  //       this.primengTableHelper.getSorting(this.dataTable),
  //       this.primengTableHelper.getSkipCount(this.paginator, event),
  //       this.primengTableHelper.getMaxResultCount(this.paginator, event)
  //     )
  //     .subscribe((result) => {
  //       this.primengTableHelper.totalRecordsCount = result.totalCount;
  //       this.primengTableHelper.records = result.items;
  //       this.primengTableHelper.hideLoadingIndicator();
  //     });
  // }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createUnitOfMeasure(): void {
    this.createOrEditUnitOfMeasureModal.show();
  }

  deleteUnitOfMeasure(unitOfMeasure: UnitOfMeasureDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._unitOfMeasuresServiceProxy.delete(unitOfMeasure.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  getAll() {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._unitOfMeasuresServiceProxy
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
        return self._unitOfMeasuresServiceProxy.createOrEdit(values).toPromise();
      },
      update: (key, values) => {
        return self._unitOfMeasuresServiceProxy.createOrEdit(values).toPromise();
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
