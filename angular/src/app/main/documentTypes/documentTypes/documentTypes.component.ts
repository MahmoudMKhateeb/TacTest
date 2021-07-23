import { AfterViewInit, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  CreateOrEditDocumentTypeDto,
  DocumentsEntitiesEnum,
  DocumentTypeDto,
  DocumentTypesServiceProxy,
  SelectItemDto,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditDocumentTypeModalComponent } from './create-or-edit-documentType-modal.component';

import { ViewDocumentTypeModalComponent } from './view-documentType-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { FileDownloadService } from '@shared/utils/file-download.service';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';

@Component({
  templateUrl: './documentTypes.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
})
export class DocumentTypesComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('createOrEditDocumentTypeModal', { static: true }) createOrEditDocumentTypeModal: CreateOrEditDocumentTypeModalComponent;
  @ViewChild('viewDocumentTypeModalComponent', { static: true }) viewDocumentTypeModal: ViewDocumentTypeModalComponent;

  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;

  advancedFiltersAreShown = false;
  filterText = '';
  displayNameFilter = '';
  isRequiredFilter = -1;
  hasExpirationDateFilter = -1;
  requiredFromFilter: DocumentsEntitiesEnum;
  entityList: SelectItemDto[] = [];
  editions: SelectItemDto[] = [];

  // ADDED DEVEXTREME GRID
  dataSource: any = {};
  popupPosition = { of: window, at: 'top', my: 'top', offset: { y: 10 } };
  createOrEditDocumentTypeDto: CreateOrEditDocumentTypeDto = new CreateOrEditDocumentTypeDto();

  constructor(
    injector: Injector,
    private _documentTypesServiceProxy: DocumentTypesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDocumentsEntityLookUp();

    this.getEditionsLookUp();
    this.getDocumentTypes();
  }

  ngAfterViewInit(): void {
    this.primengTableHelper.adjustScroll(this.dataTable);
  }

  // getDocumentTypes(event?: LazyLoadEvent) {
  //   if (this.primengTableHelper.shouldResetPaging(event)) {
  //     this.paginator.changePage(0);
  //     return;
  //   }
  //
  //   this.primengTableHelper.showLoadingIndicator();
  //
  //   this._documentTypesServiceProxy
  //     .getAll(
  //       this.filterText,
  //       this.displayNameFilter,
  //       this.isRequiredFilter,
  //       this.hasExpirationDateFilter,
  //       this.requiredFromFilter,
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

  getDocumentTypes() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._documentTypesServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.items,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
      update: (key, values) => {
        console.log(JSON.stringify(key.id));
        console.log(JSON.stringify(values));
        let dto: CreateOrEditDocumentTypeDto = new CreateOrEditDocumentTypeDto();
        return self._documentTypesServiceProxy.createOrEdit(dto).toPromise();
      },
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  createDocumentType(): void {
    this.createOrEditDocumentTypeModal.show();
  }

  deleteDocumentType(documentType: DocumentTypeDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._documentTypesServiceProxy.delete(documentType.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  exportToExcel(): void {
    this._documentTypesServiceProxy
      .getDocumentTypesToExcel(this.filterText, this.displayNameFilter, this.isRequiredFilter, this.hasExpirationDateFilter, this.requiredFromFilter)
      .subscribe((result) => {
        this._fileDownloadService.downloadTempFile(result);
      });
  }

  getDocumentsEntityLookUp() {
    this._documentTypesServiceProxy.getDocumentEntitiesForTableDropdown().subscribe((result) => {
      this.entityList = result;
    });
  }

  getEditionsLookUp() {
    this._documentTypesServiceProxy.getEditionsForTableDropdown().subscribe((result) => {
      this.editions = result;
    });
  }

  downloadTemplate(id: number): void {
    this.notify.info(this.l('downloading'));
    this._documentTypesServiceProxy.getFileDto(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  onEditingStart($event: any) {
    console.log(JSON.stringify($event.data));
  }
}
