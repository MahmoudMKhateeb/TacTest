import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Table } from 'primeng/table';
import * as _ from 'lodash';

import {
  ShippingRequestTripAccidentServiceProxy,
  IShippingRequestTripAccidentListDto,
  ShippingRequestsTripServiceProxy,
  ShippingRequestsTripListDto,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
@Component({
  templateUrl: './view-trip-accident-modal.component.html',
  selector: 'view-trip-accident-modal',
  styleUrls: ['./view-trip-accident-modal.component.scss'],
})
export class ViewTripAccidentModelComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalcanceltrip: EventEmitter<any> = new EventEmitter<any>();
  Accident: IShippingRequestTripAccidentListDto[] = [];
  Trip: ShippingRequestsTripListDto;
  active: boolean = false;
  IsStartSearch = false;
  ShowCancelButton = true;

  constructor(
    injector: Injector,
    private _ServiceProxy: ShippingRequestTripAccidentServiceProxy,
    private _tripServ: ShippingRequestsTripServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  getAll(trip: ShippingRequestsTripListDto): void {
    this.primengTableHelper.showLoadingIndicator();
    this.Trip = trip;

    this._ServiceProxy.getAll(undefined, this.Trip.id, undefined, this.primengTableHelper.getSorting(this.dataTable)).subscribe((result) => {
      this.IsStartSearch = true;
      this.primengTableHelper.totalRecordsCount = result.items.length;
      this.primengTableHelper.records = result.items;
      this.primengTableHelper.hideLoadingIndicator();
      this.active = true;
      this.modal.show();

      if (this.feature.isEnabled('App.Shipper')) {
        this.ShowCancelButton = !this.Trip.isApproveCancledByShipper;
      } else if (this.feature.isEnabled('App.Carrier')) {
        this.ShowCancelButton = !this.Trip.isApproveCancledByCarrier;
      } else if (this.Trip.isApproveCancledByCarrier && this.Trip.isApproveCancledByShipper) {
        this.ShowCancelButton = false;
      }
    });
  }
  downloadDocument(id: number): void {
    this._ServiceProxy.getFile(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  close(): void {
    this.modal.hide();
    this.active = false;
  }
  CancelTrip(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._tripServ.cancelByAccident(this.Trip.id).subscribe(() => {
          this.notify.success(this.l('SuccessfullyTripCancled'));
          this.close();
          this.modalcanceltrip.emit(null);
        });
      }
    });
  }
}