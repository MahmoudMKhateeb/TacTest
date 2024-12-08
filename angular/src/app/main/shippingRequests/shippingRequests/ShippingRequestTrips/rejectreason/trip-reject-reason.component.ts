import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import * as _ from 'lodash';
import {
  ShippingRequestTripRejectReasonServiceProxy,
  ShippingRequestTripRejectReasonListDto,
  FilterInput,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  templateUrl: './trip-reject-reason.component.html',
  animations: [appModuleAnimation()],
})
export class TripRejectReasonComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  advancedFiltersAreShown: boolean = false;
  displayNameFilter: string = '';
  filterText: string = '';
  Reasons: ShippingRequestTripRejectReasonListDto[] = [];
  IsStartSearch = false;
  constructor(
    injector: Injector,
    private _ServiceProxy: ShippingRequestTripRejectReasonServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getAll(event?: LazyLoadEvent): void {
    this.primengTableHelper.showLoadingIndicator();
    this._ServiceProxy.getAllRejectReason(this.filterText, this.primengTableHelper.getSorting(this.dataTable)).subscribe((result) => {
      this.IsStartSearch = true;
      this.primengTableHelper.totalRecordsCount = result.items.length;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      console.log(result.items);
    });
  }

  delete(reason: ShippingRequestTripRejectReasonListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._ServiceProxy.delete(reason.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyDeleted'));
          _.remove(this.primengTableHelper.records, reason);
        });
      }
    });
  }

  exportToExcel(): void {
    var data = {
      filter: this.filterText,
      sorting: this.primengTableHelper.getSorting(this.dataTable),
    };
    this._ServiceProxy.exports(data as FilterInput).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
