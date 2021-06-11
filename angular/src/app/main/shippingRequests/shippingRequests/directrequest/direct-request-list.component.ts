import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';

import { MarketPlaceServiceProxy, MarketPlaceListDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
@Component({
  templateUrl: './direct-request-list.component.html',
  styleUrls: ['/assets/custom/css/style.scss'],
  animations: [appModuleAnimation()],
})
export class DirectRequestListComponent extends ScrollPagnationComponentBase implements OnInit {
  Marketplaces: MarketPlaceListDto[] = [];
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  ReferenceNo: string | null | undefined;
  TenantId: number | undefined = undefined;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  direction = 'ltr';
  constructor(injector: Injector, private _CurrentServ: MarketPlaceServiceProxy, private _fileDownloadService: FileDownloadService) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.LoadData();
  }

  LoadData() {
    if (this.creationDateRangeActive) {
      this.fromDate = moment(this.creationDateRange[0]);
      this.toDate = moment(this.creationDateRange[1]);
    } else {
      this.fromDate = null;
      this.toDate = null;
    }

    this._CurrentServ.getAll(undefined, this.skipCount, this.maxResultCount).subscribe((result) => {
      this.IsLoading = false;
      if (result.items.length == 0) {
        this.StopLoading = true;
        return;
      }
      this.Marketplaces.push(...result.items);
    });
  }

  delete(input: MarketPlaceListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._CurrentServ.delete(input.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          //this.reloadPage();
        });
      }
    });
  }

  getWordTitle(n: any, word: string): string {
    if (parseInt(n) == 1) {
      return this.l(word);
    }
    return this.l(`${word}s`);
  }
}
