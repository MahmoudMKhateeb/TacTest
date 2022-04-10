import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';

@Component({
  selector: 'shared-file-viewer',
  templateUrl: './file-viwer.component.html',
  styleUrls: ['./file-viwer.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class FileViwerComponent extends AppComponentBase {
  @ViewChild('modal', { static: true }) modal: ModalDirective;
  @ViewChild('pdfViewerAutoLoad', { static: true }) pdfViewerAutoLoad;
  fileUrl: any;
  fileType = 'pdf';

  constructor(injector: Injector) {
    super(injector);
  }

  /**
   * show the modal
   * @param fileurl
   */
  show(fileurl: any, fileType: 'pdf' | 'img') {
    this.fileUrl = fileurl;
    this.fileType = fileType;
    if (fileType === 'pdf') {
      this.downloadFile(fileurl).subscribe((res) => {
        this.pdfViewerAutoLoad.pdfSrc = res;
        this.pdfViewerAutoLoad.refresh();
      });
    }
    this.modal.show();
  }

  /**
   * close the modal
   */
  close() {
    this.fileUrl = null;
    this.modal.hide();
  }
}
