import { Component, Injector, ViewChild, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { HttpClient } from '@angular/common/http';
import { FileUpload } from '@node_modules/primeng/fileupload';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from '@node_modules/rxjs/operators';

import * as _ from 'lodash';
import {
  AppLocalizationServiceProxy,
  IAppLocalizationListDto,
  AppLocalizationFilterInput,
  ComboboxItemDto,
  EditionServiceProxy,
  TerminologyVersion,
  TerminologyPlatForm,
  TerminologyAppVersion,
  TerminologySection,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EnumToArrayPipe } from '../../../shared/common/pipes/enum-to-array.pipe';
@Component({
  templateUrl: './applocalization.component.html',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class AppLocalizationComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @ViewChild('ExcelFileUpload', { static: false }) excelFileUpload: FileUpload;

  input: AppLocalizationFilterInput = new AppLocalizationFilterInput();
  AppLocalizations: IAppLocalizationListDto[] = [];
  editions: ComboboxItemDto[] = [];
  IsStartSearch = false;
  uploadUrl: string;
  terminologyVersion: any;
  terminologyPlatForm: any;
  terminologyAppVersion: any;
  terminologySection: any;
  constructor(
    injector: Injector,
    private _ServiceProxy: AppLocalizationServiceProxy,
    private _editionService: EditionServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _httpClient: HttpClient,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportTerminologyFromExcel';
    this._editionService.getEditionComboboxItems(0, true, false).subscribe((editions) => {
      this.editions = editions;
    });
    this.terminologyVersion = this.enumToArray.transform(TerminologyVersion);
    this.terminologyPlatForm = this.enumToArray.transform(TerminologyPlatForm);
    this.terminologyAppVersion = this.enumToArray.transform(TerminologyAppVersion);
    this.terminologySection = this.enumToArray.transform(TerminologySection);
  }
  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();

    this._ServiceProxy
      .getAll(
        this.input.filter,
        this.input.editionId,
        this.input.page,
        this.input.platForm,
        this.input.appVersion,
        this.input.version,
        this.input.section,
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
  delete(localize: IAppLocalizationListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.delete(localize.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          this.reloadPage();
        });
      }
    });
  }

  exportToExcel(): void {
    this.input.sorting = this.primengTableHelper.getSorting(this.dataTable);
    this._ServiceProxy.exports(this.input).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  restore(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.restore().subscribe(() => {
          this.notify.success(this.l('SuccessfullyRestore'));
          this.reloadPage();
        });
      }
    });
  }
  generate(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.generate().subscribe(() => {
          this.notify.success(this.l('SuccessfullyGenerate'));
        });
      }
    });
  }

  uploadExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    formData.append('file', file, file.name);

    this._httpClient
      .post<any>(this.uploadUrl, formData)
      .pipe(finalize(() => this.excelFileUpload.clear()))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success(this.l('ImportUsersProcessStart'));
        } else if (response.error != null) {
          this.notify.error(this.l('ImportUsersUploadFailed'));
        }
      });
  }

  onUploadExcelError(): void {
    this.notify.error(this.l('ImportUsersUploadFailed'));
  }
}
