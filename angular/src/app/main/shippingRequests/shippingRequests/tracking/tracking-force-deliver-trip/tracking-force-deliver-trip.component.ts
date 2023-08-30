import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from '@node_modules/rxjs/operators';
import { FileUpload } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';
import { ImportTripTransactionFromExcelDto } from '@shared/service-proxies/service-proxies';
import { isNullOrUndefined } from 'util';
import { ViewForceDeliverTripsFromExcelModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking-force-deliver-trip/view-force-deliver-trips-from-excel-modal/view-force-deliver-trips-from-excel-modal.component';

@Component({
  selector: 'app-tracking-force-deliver-trip',
  templateUrl: './tracking-force-deliver-trip.component.html',
  styleUrls: ['./tracking-force-deliver-trip.component.css'],
})
export class TrackingForceDeliverTripComponent extends AppComponentBase {
  active: boolean;
  loading: boolean;
  @ViewChild('DeliverTripModal') modal: ModalDirective;
  @ViewChild('DeliverTripFileUpdate') fileUpload: FileUpload;
  @ViewChild('viewForceDeliverTripsFromExcelModal', { static: false })
  viewForceDeliverTripsFromExcelModal: ViewForceDeliverTripsFromExcelModalComponent;
  tripsFromExcel: ImportTripTransactionFromExcelDto[] = [];

  constructor(injector: Injector, private _httpClient: HttpClient) {
    super(injector);
    this.active = false;
    this.loading = false;
  }

  show() {
    this.active = true;
    this.modal.show();
  }

  save() {
    this.loading = true;
  }
  upload(data: { files: File[] }): void {
    this.loading = true;
    let files: FormData = new FormData();
    data.files.forEach((x) => {
      files.append('file', x, x.name);
    });
    this._httpClient
      .post<any>(AppConsts.remoteServiceBaseUrl + '/Helper/BulkDeliverTripFromExcel', files)
      .pipe(
        finalize(() => {
          this.fileUpload.clear();
          this.loading = false;
        })
      )
      .subscribe((response) => {
        if (response.success) {
          this.tripsFromExcel = response.result.tripsFromExcel as ImportTripTransactionFromExcelDto[];
          this.viewForceDeliverTripsFromExcelModal.show();
          //   this.notify.success(this.l('SuccessfullyUpload'));
          // this.close();
        } else if (response.error != null) {
          this.notify.error(this.l('UploadFailed'));
        }
      });
  }

  close() {
    this.active = false;
    this.modal.hide();
  }

  onUploadError(error) {
    console.log('error', error);
  }

  onViewModalSave() {
    this.close();
  }
}
