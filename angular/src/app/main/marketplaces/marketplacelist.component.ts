import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';

import { MarketPlaceServiceProxy, MarketPlaceListDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

import * as _ from 'lodash';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
@Component({
  templateUrl: './marketplacelist.component.html',
  styleUrls: ['./marketplacelist.component.scss'],
  animations: [appModuleAnimation()],
})
export class MarketPlaceListComponent extends ScrollPagnationComponentBase implements OnInit {
  Marketplaces: MarketPlaceListDto[] = [];
  fromDate: moment.Moment | null | undefined;
  toDate: moment.Moment | null | undefined;
  ReferenceNo: string | null | undefined;
  TenantId: number | undefined = undefined;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean = false;
  dir = 'ltr';
  constructor(injector: Injector, private _CurrentServ: MarketPlaceServiceProxy, private _fileDownloadService: FileDownloadService) {
    super(injector);
  }
  ngOnInit(): void {
    this.dir = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.LoadData();
    console.log(`(${window.innerHeight} + ${window.scrollY}) >= ${document.body.offsetHeight}`);
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
