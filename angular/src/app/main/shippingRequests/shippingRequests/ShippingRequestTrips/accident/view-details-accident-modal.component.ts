import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { ShippingRequestTripAccidentServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'app-view-details-accident-modal',
  templateUrl: './view-details-accident-modal.component.html',
})
export class ViewDetailsAccidentModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('viewAccidentDetails', { static: false }) public modal: ModalDirective;
  active = false;
  accidentDetails: any;
  @Input() allReasons: any;
  constructor(
    injector: Injector,
    private _shippingRequestTripAccident: ShippingRequestTripAccidentServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  show(id: number): void {
    this._shippingRequestTripAccident.get(id).subscribe((result) => {
      this.accidentDetails = result;
      this.active = true;
      this.modal.show();
    });
  }

  downloadDocument(id: number): void {
    this._shippingRequestTripAccident.getFile(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  close() {
    this.active = false;
    this.modal.hide();
  }
}
