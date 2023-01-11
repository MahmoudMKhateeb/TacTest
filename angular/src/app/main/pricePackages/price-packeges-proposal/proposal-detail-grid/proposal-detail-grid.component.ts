import { AfterViewInit, Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import DataSource from '@node_modules/devextreme/data/data_source';
import { TmsPricePackageServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { AppComponentBase } from '@shared/common/app-component-base';
import {isNotNullOrUndefined} from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-proposal-detail-grid',
  templateUrl: './proposal-detail-grid.component.html',
  styleUrls: ['./proposal-detail-grid.component.css'],
})
export class ProposalDetailGridComponent extends AppComponentBase implements OnInit {
  @Input() proposalId: number;
  pricePackageDataSource: any;

  constructor(private _injector: Injector, private _tmsPricePackageServiceProxy: TmsPricePackageServiceProxy) {
    super(_injector);
  }

  ngOnInit(): void {
    let self = this;
    this.pricePackageDataSource = {};
    this.pricePackageDataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        if (!isNotNullOrUndefined(loadOptions)) {
            loadOptions = {};
        }
        loadOptions.filter = [];
        (loadOptions.filter as any[]).push(['ProposalId', '=', self.proposalId]);
        return self._tmsPricePackageServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((result) => {
            return {
              data: result.data,
              totalCount: result.totalCount,
              summary: result.summary,
              groupCount: result.groupCount,
            };
          })
          .catch(() => {
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
