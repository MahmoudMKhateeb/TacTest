import { Component, Injector, ViewChild } from '@angular/core';
import { AppendixForViewDto, PricePackageAppendixServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-view-price-package-appendix',
  templateUrl: './view-price-package-appendix.component.html',
  styleUrls: ['./view-price-package-appendix.component.css'],
})
export class ViewPricePackageAppendixComponent extends AppComponentBase {
  appendix: AppendixForViewDto;
  @ViewChild('ViewAppendixModal') viewAppendixModal: ModalDirective;
  isModalActive = false;

  constructor(
    private _appendixServiceProxy: PricePackageAppendixServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private injector: Injector
  ) {
    super(injector);
  }

  show(appendixId: number) {
    this.isModalActive = true;
    this.appendix = new AppendixForViewDto();
    this._appendixServiceProxy.getForView(appendixId).subscribe((result) => {
      this.appendix = result;
    });
    this.viewAppendixModal.show();
  }

  close(): void {
    this.appendix = new AppendixForViewDto();
    this.isModalActive = false;
    this.viewAppendixModal.hide();
  }

  downloadAppendixFile(appendixFileId: string) {
    this._fileDownloadService.downloadFileByBinary(appendixFileId, this.appendix.contractName, 'application/pdf');
  }
}
