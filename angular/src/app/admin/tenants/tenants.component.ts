import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ComboboxItemDto,
  CommonLookupServiceProxy,
  EditionServiceProxy,
  EntityDtoOfInt64,
  FindUsersInput,
  NameValueDto,
  TenantListDto,
  TenantServiceProxy,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { CreateTenantModalComponent } from './create-tenant-modal.component';
import { EditTenantModalComponent } from './edit-tenant-modal.component';
import { TenantFeaturesModalComponent } from './tenant-features-modal.component';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import CustomStore from '@node_modules/devextreme/data/custom_store';

@Component({
  templateUrl: './tenants.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class TenantsComponent extends AppComponentBase implements OnInit {
  @ViewChild('impersonateUserLookupModal', { static: true }) impersonateUserLookupModal: CommonLookupModalComponent;
  @ViewChild('createTenantModal', { static: true }) createTenantModal: CreateTenantModalComponent;
  @ViewChild('editTenantModal', { static: true }) editTenantModal: EditTenantModalComponent;
  @ViewChild('tenantFeaturesModal', { static: true }) tenantFeaturesModal: TenantFeaturesModalComponent;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;

  subscriptionDateRange: Date[] = [moment().startOf('day').toDate(), moment().add(30, 'days').endOf('day').toDate()];
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];

  _entityTypeFullName = 'TACHYON.MultiTenancy.Tenant';
  entityHistoryEnabled = false;

  filters: {
    filterText: string;
    creationDateRangeActive: boolean;
    subscriptionEndDateRangeActive: boolean;
    selectedEditionId: number;
  } = <any>{};
  dataSource: any = {};
  editions: ComboboxItemDto[] = [];
  shipperEditionDisplayName = 'shipper';

  constructor(
    injector: Injector,
    private _tenantService: TenantServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _commonLookupService: CommonLookupServiceProxy,
    private _impersonationService: ImpersonationService,
    private _editionService: EditionServiceProxy,
    private _router: Router
  ) {
    super(injector);
    this.setFiltersFromRoute();
  }

  setFiltersFromRoute(): void {
    if (this._activatedRoute.snapshot.queryParams['subscriptionEndDateStart'] != null) {
      this.filters.subscriptionEndDateRangeActive = true;
      this.subscriptionDateRange[0] = moment(this._activatedRoute.snapshot.queryParams['subscriptionEndDateStart']).toDate();
    } else {
      this.subscriptionDateRange[0] = moment().startOf('day').toDate();
    }

    if (this._activatedRoute.snapshot.queryParams['subscriptionEndDateEnd'] != null) {
      this.filters.subscriptionEndDateRangeActive = true;
      this.subscriptionDateRange[1] = moment(this._activatedRoute.snapshot.queryParams['subscriptionEndDateEnd']).toDate();
    } else {
      this.subscriptionDateRange[1] = moment().add(30, 'days').endOf('day').toDate();
    }

    if (this._activatedRoute.snapshot.queryParams['creationDateStart'] != null) {
      this.filters.creationDateRangeActive = true;
      this.creationDateRange[0] = moment(this._activatedRoute.snapshot.queryParams['creationDateStart']).toDate();
    } else {
      this.creationDateRange[0] = moment().add(-7, 'days').startOf('day').toDate();
    }

    if (this._activatedRoute.snapshot.queryParams['creationDateEnd'] != null) {
      this.filters.creationDateRangeActive = true;
      this.creationDateRange[1] = moment(this._activatedRoute.snapshot.queryParams['creationDateEnd']).toDate();
    } else {
      this.creationDateRange[1] = moment().endOf('day').toDate();
    }

    if (this._activatedRoute.snapshot.queryParams['editionId'] != null) {
      this.filters.selectedEditionId = parseInt(this._activatedRoute.snapshot.queryParams['editionId']);
    }
  }

  ngOnInit(): void {
    this.filters.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';

    this.setIsEntityHistoryEnabled();

    this.impersonateUserLookupModal.configure({
      title: this.l('SelectAUser'),
      dataSource: (skipCount: number, maxResultCount: number, filter: string, tenantId?: number) => {
        let input = new FindUsersInput();
        input.filter = filter;
        input.maxResultCount = maxResultCount;
        input.skipCount = skipCount;
        input.tenantId = tenantId;
        input.excludeDrivers = true;
        return this._commonLookupService.findUsers(input);
      },
    });

    this._editionService.getEditionComboboxItems(0, true, false).subscribe((editions) => {
      this.editions = editions;
    });

    this.getAllTenants();
  }

  private setIsEntityHistoryEnabled(): void {
    let customSettings = (abp as any).custom;
    this.entityHistoryEnabled =
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1;
  }

  getTenants(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);

      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._tenantService
      .getTenants(
        this.filters.filterText,
        this.filters.subscriptionEndDateRangeActive ? moment(this.subscriptionDateRange[0]) : undefined,
        this.filters.subscriptionEndDateRangeActive ? moment(this.subscriptionDateRange[1]).endOf('day') : undefined,
        this.filters.creationDateRangeActive ? moment(this.creationDateRange[0]) : undefined,
        this.filters.creationDateRangeActive ? moment(this.creationDateRange[1]).endOf('day') : undefined,
        this.filters.selectedEditionId,
        this.filters.selectedEditionId !== undefined && this.filters.selectedEditionId + '' !== '-1',
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getMaxResultCount(this.paginator, event),
        this.primengTableHelper.getSkipCount(this.paginator, event)
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  showUserImpersonateLookUpModal(record: any): void {
    this.impersonateUserLookupModal.tenantId = record.id;
    this.impersonateUserLookupModal.show();
  }

  unlockUser(record: any): void {
    this._tenantService.unlockTenantAdmin(new EntityDtoOfInt64({ id: record.id })).subscribe(() => {
      this.notify.success(this.l('UnlockedTenandAdmin', record.name));
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
    console.log('page Reloaded');
  }

  createTenant(): void {
    this.createTenantModal.show();
  }

  // deleteTenant(tenant: TenantListDto): void {
  //   this.message.confirm(this.l('TenantDeleteWarningMessage', tenant.tenancyName), this.l('AreYouSure'), (isConfirmed) => {
  //     if (isConfirmed) {
  //       this._tenantService.deleteTenant(tenant.id).subscribe(() => {
  //         this.reloadPage();
  //         this.notify.success(this.l('SuccessfullyDeleted'));
  //       });
  //     }
  //   });
  // } $$## This is Added By Mahmoud ##$$

  showHistory(tenant: TenantListDto): void {
    this.entityTypeHistoryModal.show({
      entityId: tenant.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: tenant.tenancyName,
    });
  }

  impersonateUser(item: NameValueDto): void {
    this._impersonationService.impersonate(parseInt(item.value), this.impersonateUserLookupModal.tenantId);
  }

  getAllTenants() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._tenantService
          .getAllTenants(JSON.stringify(loadOptions))
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

  /**
   * opens Tenant Profile View in a new tab
   */
  viewTenantProfile(tenantId: number) {
    // Converts the route into a string that can be used
    // with the window.open() function
    const url = this._router.serializeUrl(this._router.createUrlTree([`/app/main/profile`, tenantId]));

    window.open(url, '_blank');
  }
}
