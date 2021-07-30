import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

import {
  TransactionServiceProxy,
  TransactionListDto,
  ISelectItemDto,
  CommonLookupServiceProxy,
  ChannelType,
  EditionServiceProxy,
  ComboboxItemDto,
  TransactionFilterInput,
} from '@shared/service-proxies/service-proxies';

import { FileDownloadService } from '@shared/utils/file-download.service';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import CustomStore from '@node_modules/devextreme/data/custom_store';
@Component({
  templateUrl: 'transaction-list.component.html',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class TransactionListComponent extends AppComponentBase implements OnInit {
  ChannelType: any;
  Tenants: ISelectItemDto[];
  editions: ComboboxItemDto[] = [];
  dataSource: any = {};
  loadOptions: any;

  constructor(
    injector: Injector,
    private _CurrentServ: TransactionServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private enumToArray: EnumToArrayPipe,
    private _editionService: EditionServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.ChannelType = this.enumToArray.transform(ChannelType);
    if (!this.appSession.tenantId || this.feature.isEnabled('App.TachyonDealer')) {
      this._editionService.getEditionComboboxItems(0, true, false).subscribe((editions) => {
        this.editions = editions;
      });
    }

    this.fillDataSource();
  }

  exportToExcel(): void {
    let input = new TransactionFilterInput();
    input.loadOptions = JSON.stringify(this.loadOptions);
    this._CurrentServ.exports(input).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  fillDataSource() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        self.loadOptions = loadOptions;
        return self._CurrentServ
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
    });
  }
}
