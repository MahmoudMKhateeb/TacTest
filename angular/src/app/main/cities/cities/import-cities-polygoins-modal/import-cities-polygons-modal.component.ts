import { Component, Injector, ViewChild, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { FileUpload } from '@node_modules/primeng/fileupload';
import { TrackingServiceProxy } from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from '@node_modules/rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'import-cities-polygons-modal',
  templateUrl: './import-cities-polygons-modal.component.html',
  styleUrls: ['./import-cities-polygons-modal.component.css'],
  encapsulation: ViewEncapsulation.Emulated,
})
export class ImportCitiesPolygonsModalComponent extends AppComponentBase {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('FileUpload', { static: false }) fileUpload: FileUpload;

  active: boolean = false;
  saving: boolean = false;
  loading = false;
  constructor(injector: Injector, private _Service: TrackingServiceProxy, private _httpClient: HttpClient) {
    super(injector);
  }

  public show(): void {
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }

  upload(data: { files: File }): void {
    this.loading = true;
    const formData: FormData = new FormData();
    const file = data.files[0];
    formData.append('file', file, file.name);
    this._httpClient
      .post<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/helper/ImportCitiesPolygon', formData)
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

  onUploadError(): void {
    this.notify.error(this.l('UploadFailed'));
  }
}
