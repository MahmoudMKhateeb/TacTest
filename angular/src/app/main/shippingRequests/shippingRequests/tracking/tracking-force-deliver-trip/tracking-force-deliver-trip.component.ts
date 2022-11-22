import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from '@node_modules/rxjs/operators';
import { FileUpload } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';

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
          this.notify.success(this.l('SuccessfullyUpload'));
          this.close();
        } else if (response.error != null) {
          this.notify.error(this.l('UploadFailed'));
        }
      });
  }

  close() {
    this.active = false;
    this.modal.hide();
  }

  onUploadError() {}
}
