import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EmailTemplateDto, EmailTemplatesServiceProxy, LoadOptionsInput, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditEmailTemplateModalComponent } from './create-or-edit-emailTemplate-modal.component';

import { ViewEmailTemplateModalComponent } from './view-emailTemplate-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';

import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/api';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './emailTemplates.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class EmailTemplatesComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditEmailTemplateModal', { static: true }) createOrEditEmailTemplateModal: CreateOrEditEmailTemplateModalComponent;
  @ViewChild('viewEmailTemplateModalComponent', { static: true }) viewEmailTemplateModal: ViewEmailTemplateModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  nameFilter = '';
  displayNameFilter = '';
  dataSource: any = {};

  constructor(
    injector: Injector,
    private _emailTemplatesServiceProxy: EmailTemplatesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getEmailTemplates(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._emailTemplatesServiceProxy
      .getAll(
        this.filterText,
        this.nameFilter,
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

  createEmailTemplate(): void {
    this.createOrEditEmailTemplateModal.show();
  }

  deleteEmailTemplate(emailTemplate: EmailTemplateDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._emailTemplatesServiceProxy.delete(emailTemplate.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  DxGetAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        let Input = new LoadOptionsInput();
        Input.loadOptions = JSON.stringify(loadOptions);

        return self._emailTemplatesServiceProxy
          .dxGetAll(Input)
          .toPromise()
          .then((response) => {
            console.log(response.totalCount);
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
    });
  }

  ngOnInit(): void {
    this.DxGetAll();
  }
}
