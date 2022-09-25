import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ActorDto, ActorsServiceProxy, ActorTypesEnum, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
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
export class ActorsComponent extends AppComponentBase {
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

  constructor(
    injector: Injector,
    private _actorsServiceProxy: ActorsServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
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

  generateInvoice(actor: any) {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._actorsServiceProxy.generateShipperInvoices(actor.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('Success'));
        });
      }
    });
  }
}
