import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TrackingServiceProxy } from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { FileUpload } from '@node_modules/primeng/fileupload';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'tacking-pod-model',
  templateUrl: './tacking-pod-model.component.html',
})
export class TrackingPODModalComponent extends AppComponentBase {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('FileUpload', { static: false }) fileUpload: FileUpload;

  active: boolean = false;
  saving: boolean = false;
  id: number;
  Specifiedtime: Date = new Date();
  action: string;
  constructor(injector: Injector, private _Service: TrackingServiceProxy, private _httpClient: HttpClient) {
    super(injector);
  }

  public show(pointId: number, action: string): void {
    this.id = pointId;
    this.action = action;
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
    const formData: FormData = new FormData();
    const file = data.files[0];
    formData.append('file', file, file.name);
    formData.append('Action', this.action);
    formData.append('id', this.id.toString());
    this._httpClient
      .post<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/DropOffPointToDelivery', formData)
      .pipe(finalize(() => this.fileUpload.clear()))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success(this.l('SuccessfullyUpload'));
          this.action === 'UplodeDeliveryNote' ? abp.event.trigger('tripDeliveryNotesUploadSuccess') : abp.event.trigger('PodUploadedSuccess');
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
