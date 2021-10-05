import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GoodCategoriesServiceProxy, GoodCategoryDto, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditGoodCategoryModalComponent } from './create-or-edit-goodCategory-modal.component';

import { ViewGoodCategoryModalComponent } from './view-goodCategory-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './goodCategories.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class GoodCategoriesComponent extends AppComponentBase {
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditGoodCategoryModal', { static: true }) createOrEditGoodCategoryModal: CreateOrEditGoodCategoryModalComponent;
  @ViewChild('viewGoodCategoryModalComponent', { static: true }) viewGoodCategoryModal: ViewGoodCategoryModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';

  _entityTypeFullName = 'TACHYON.Goods.GoodCategories.GoodCategory';
  entityHistoryEnabled = false;
  store: CustomStore;
  dataSource: any = {};

  constructor(
    injector: Injector,
    private _goodCategoriesServiceProxy: GoodCategoriesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);

    this.getAll();
  }

  ngOnInit(): void {
    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
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

  getGoodCategories(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._goodCategoriesServiceProxy
      .getAll(
        this.filterText,
        this.displayNameFilter,
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

  createGoodCategory(): void {
    this.createOrEditGoodCategoryModal.show();
  }

  showHistory(goodCategory: GoodCategoryDto): void {
    this.entityTypeHistoryModal.show({
      entityId: goodCategory.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  // deleteGoodCategory(goodCategory: GoodCategoryDto): void {
  //   this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
  //     if (isConfirmed) {
  //       this._goodCategoriesServiceProxy.delete(goodCategory.id).subscribe(() => {
  //         this.reloadPage();
  //         this.notify.success(this.l('SuccessfullyDeleted'));
  //       });
  //     }
  //   });
  // }

  exportToExcel(): void {
    this._goodCategoriesServiceProxy.getGoodCategoriesToExcel(this.filterText, this.displayNameFilter).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  getAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._goodCategoriesServiceProxy
          .getAllDx(
            loadOptions.requireTotalCount,
            loadOptions.requireGroupCount,
            undefined,
            undefined,
            loadOptions.skip,
            loadOptions.take,
            loadOptions.sort,
            loadOptions.group,
            loadOptions.filter,
            loadOptions.totalSummary,
            loadOptions.groupSummary,
            loadOptions.select,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined
          )
          .toPromise()
          .then((response) => {
            return { data: response.data, totalCount: response.totalCount };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }
}
