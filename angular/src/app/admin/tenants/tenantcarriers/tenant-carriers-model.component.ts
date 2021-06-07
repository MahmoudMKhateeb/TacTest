import { Component, OnInit, Injector, ViewChild, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { TenantCarrierServiceProxy, TenantCarriersListDto, CommonLookupServiceProxy } from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
@Component({
  templateUrl: './tenant-carriers-model.component.html',
  selector: 'tenant-carriers-model',
  styleUrls: ['./tenant-carriers-model.component.scss'],

  animations: [appModuleAnimation()],
})
export class TenantCarriersModel extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  TenantId: number;
  IsStartSearch: boolean = false;

  constructor(injector: Injector, private _CurrentServ: TenantCarrierServiceProxy, private _CommonServ: CommonLookupServiceProxy) {
    super(injector);
  }
  getAll(event?: LazyLoadEvent): void {
    if (!this.active) return;
    this.primengTableHelper.showLoadingIndicator();
    this._CurrentServ
      .getAll(
        this.TenantId,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.IsStartSearch = true;
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  show(id: number): void {
    this.primengTableHelper.records = [];
    this.active = true;
    this.modal.show();
    this.TenantId = id;
    this.getAll();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  delete(input: TenantCarriersListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.delete(input.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }
}
