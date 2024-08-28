import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  ActorDto,
  ActorsServiceProxy,
  ActorTypesEnum,
  TenantCityLookupTableDto,
  TenantRegistrationServiceProxy,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { LogService, NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditActorModalComponent } from './create-or-edit-actor-modal.component';

import { ViewActorModalComponent } from './view-actor-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  templateUrl: './actors.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class ActorsComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditActorModal', { static: true }) createOrEditActorModal: CreateOrEditActorModalComponent;
  @ViewChild('viewActorModalComponent', { static: true }) viewActorModal: ViewActorModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  companyNameFilter = '';
  actorTypeFilter = -1;
  moiNumberFilter = '';
  addressFilter = '';
  mobileNumberFilter = '';
  emailFilter = '';

  actorTypesEnum = ActorTypesEnum;
  isSaab = this.feature.isEnabled('App.Sab');
  cities: TenantCityLookupTableDto[];

  constructor(injector: Injector, private _actorsServiceProxy: ActorsServiceProxy, private _countriesServiceProxy: TenantRegistrationServiceProxy) {
    super(injector);
  }
  ngOnInit() {
    this.getAllcities();
  }

  getActors(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
        return;
      }
    }

    this.primengTableHelper.showLoadingIndicator();

    this._actorsServiceProxy
      .getAll(
        this.filterText,
        this.companyNameFilter,
        this.actorTypeFilter,
        this.moiNumberFilter,
        this.addressFilter,
        this.mobileNumberFilter,
        this.emailFilter,
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

  createActor(): void {
    this.createOrEditActorModal.show();
  }

  deleteActor(actor: ActorDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._actorsServiceProxy.delete(actor.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  getCityDisplayName(CityId: string) {
    if (this.cities?.length == 0) return;
    return this.cities.find((x) => x.id == CityId)?.displayName || '';
  }

  getAllcities() {
    //2 for saudi Arabia
    this._countriesServiceProxy.getAllCitiesForTableDropdown(2).subscribe((result) => {
      this.cities = result;
    });
  }
}
