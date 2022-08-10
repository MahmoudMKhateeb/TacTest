import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrEditDocumentFileDto,
  CreateOrEditShippingRequestAndTripNotesDto,
  ShippingRequestAndTripNotesServiceProxy,
  VisibilityNotes,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-view-notes',
  templateUrl: './view-notes.component.html',
  styleUrls: ['./view-notes.component.css'],
})
export class ViewNotesComponent extends AppComponentBase implements OnInit {
  @ViewChild('ViewNoteModal', { static: true }) modal: ModalDirective;
  @Input() shippingRequestId: number;
  @Input() tripId: number;
  @Input() type: string;
  saving = false;
  item = new CreateOrEditShippingRequestAndTripNotesDto();
  loading = true;
  visibility = VisibilityNotes;
  files: CreateOrEditDocumentFileDto[] = [];
  open = false;
  others = 1;

  constructor(
    injector: Injector,
    private _shippingRequestAndTripNotesServiceProxy: ShippingRequestAndTripNotesServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  show(id) {
    if (id != null) {
      this.item.noteId = id;
      this._shippingRequestAndTripNotesServiceProxy
        .getForEdit(id)
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe((result) => {
          this.item = result;
          if (this.item.visibility > 0) {
            this.item.visibility = this.others;
            this.open = true;
          }
          this.files = this.item.createOrEditDocumentFileDto;
          this.modal.show();
        });
    }
  }

  close(): void {
    this.item = new CreateOrEditShippingRequestAndTripNotesDto();
    this.modal.hide();
    this.loading = false;
  }

  downloadTemplate(documentId: string, fileName: string, type: string): void {
    this._fileDownloadService.downloadFileByBinary(documentId, fileName, type);
  }
}
