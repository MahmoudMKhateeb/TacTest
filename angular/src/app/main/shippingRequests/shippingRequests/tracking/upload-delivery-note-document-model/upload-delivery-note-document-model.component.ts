import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { FileUpload } from '@node_modules/primeng/fileupload';
import { TrackingServiceProxy } from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'upload-delivery-note-document-model',
  templateUrl: './upload-delivery-note-document-model.component.html',
  styleUrls: ['./upload-delivery-note-document-model.component.css'],
})
export class UploadDeliveryNoteDocumentModelComponent extends AppComponentBase {
  @Output() modalConfirm: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('FileUpload', { static: false }) fileUpload: FileUpload;

  active: boolean = false;
  saving: boolean = false;
  id: number;
  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: TrackingServiceProxy, private _httpClient: HttpClient) {
    super(injector);
  }

  public show(id: number): void {
    this.id = id;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
  }

  close(): void {
    this.modal.hide();
    this.modalConfirm.emit(null);
    this.active = false;
  }

  upload(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    formData.append('file', file, file.name);
    formData.append('id', this.id.toString());
    this._httpClient
      .post<any>(AppConsts.remoteServiceBaseUrl + '/api/services/app/ShippingRequestDriver/UploadDeliveryNoteDocument', formData)
      .pipe(finalize(() => this.fileUpload.clear()))
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