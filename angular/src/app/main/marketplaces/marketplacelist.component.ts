import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { LazyLoadEvent } from 'primeng/public_api';
import { MarketPlaceServiceProxy, MarketPlaceListDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: './marketplacelist.component.html',
  styleUrls: ['./marketplacelist.component.css'],
  animations: [appModuleAnimation()],
})
export class MarketPlaceListComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  Marketplaces: MarketPlaceListDto[] = [];
  IsStartSearch: boolean = false;
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  ReferenceNo: string | null | undefined;
  TenantId: number | undefined = undefined;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  constructor(injector: Injector, private _CurrentServ: MarketPlaceServiceProxy, private _fileDownloadService: FileDownloadService) {
    super(injector);
  }
  ngOnInit(): void {
    this.getAll();
  }
  getAll(event?: LazyLoadEvent): void {
    if (this.creationDateRangeActive) {
      this.fromDate = moment(this.creationDateRange[0]);
      this.toDate = moment(this.creationDateRange[1]);
    } else {
      this.fromDate = null;
      this.toDate = null;
    }

    this._CurrentServ.getAll(undefined, 0, 2).subscribe((result) => {
      this.IsStartSearch = true;
      this.Marketplaces = result.items;
      console.log(result.items);
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  delete(input: MarketPlaceListDto): void {
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
