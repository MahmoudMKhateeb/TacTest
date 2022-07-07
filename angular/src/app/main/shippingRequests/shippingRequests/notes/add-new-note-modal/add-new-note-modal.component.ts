import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  CreateOrEditDocumentFileDto,
  VisibilityNotes,
  CreateOrEditShippingRequestAndTripNotesDto,
  ShippingRequestAndTripNotesServiceProxy,
  DocumentTypesServiceProxy,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { IAjaxResponse, TokenService } from 'abp-ng2-module';
import { FileItem, FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-new-note-modal',
  templateUrl: './add-new-note-modal.component.html',
  styleUrls: ['./add-new-note-modal.component.css'],
})
export class AddNewNoteModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('AddNewNoteModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Output() onDocsUploaderCompleteAll: EventEmitter<any> = new EventEmitter();
  @Input() shippingRequestId: number;
  @Input() tripId: number;
  saving = false;
  item = new CreateOrEditShippingRequestAndTripNotesDto();
  alldocumentsValid = false;
  public DocsUploader: FileUploader;
  private _DocsUploaderOptions: FileUploaderOptions = {};
  fileToken: string;
  loading = true;
  visibility = VisibilityNotes;
  files: CreateOrEditDocumentFileDto[] = [];

  open = false;
  others = 1;

  docProgressFileName: any;
  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;

  constructor(
    injector: Injector,
    private cdr: ChangeDetectorRef,
    private _tokenService: TokenService,
    public _documentTypesServiceProxy: DocumentTypesServiceProxy,
    private _shippingRequestAndTripNotesServiceProxy: ShippingRequestAndTripNotesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  show(id = null) {
    this.loading = true;
    this.item.visibility = this.visibility.Internal;
    if (id == null) {
      this.item = new CreateOrEditShippingRequestAndTripNotesDto();
      this.loading = false;
      this.modal.show();
    } else {
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
          this.files = this.item.createOrEditDocumentFileDto;
          if (this.item.visibility > 0) {
            this.item.visibility = this.others;
            this.open = true;
          }
          this.modal.show();
        });
    }
    this.initDocsUploader();
    this.cdr.detectChanges();
  }
  createOrEditNote() {
    this.item.shippingRequetId = this.shippingRequestId;
    this.item.tripId = this.tripId;
    this.item.createOrEditDocumentFileDto = this.files;
    this.saving = true;
    this._shippingRequestAndTripNotesServiceProxy
      .createOrEdit(this.item)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.close();
        this.modalSave.emit(null);
        this.notify.info(this.l('SuccessfullySaved'));
      });
  }
  close(): void {
    this.modal.hide();
    this.loading = false;
    this.open = false;
    this.files = [];
  }

  /**
   * initialize required documents fileUploader
   */
  public initDocsUploader(): void {
    console.log('nitialize required documents fileUploader');
    this.DocsUploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Helper/UploadDocumentFile' });
    this._DocsUploaderOptions.autoUpload = false;
    this._DocsUploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
    this._DocsUploaderOptions.removeAfterUpload = true;

    this.DocsUploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.DocsUploader.onBuildItemForm = (fileItem: FileItem, form: any) => {
      form.append('FileType', fileItem.file.type);
      form.append('FileName', fileItem.file.name);
      form.append('FileToken', this.guid());
    };

    this.DocsUploader.onSuccessItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
      if (resp.success) {
        this.files.find((r) => r.name.includes(item.file.name)).updateDocumentFileInput = new UpdateDocumentFileInput({
          fileToken: resp.result.fileToken,
        });
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploader.onCompleteAll = () => {
      this.onDocsUploaderCompleteAll.emit();
      // this.saveInternal();
    };

    //for progressBar
    this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.docProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploader.setOptions(this._DocsUploaderOptions);
  }

  DocFileChangeEvent(event: any, items: CreateOrEditDocumentFileDto[]): void {
    let file = event.target.files[0];

    if (event.target.files[0].size > 5242880) {
      //5MB
      file.error = this.l('DocumentFile_Warn_SizeLimit');
      this.alldocumentsValid = false;
    }
    let extn = event.target.files[0].type;
    if (extn != 'image/jpeg' && extn != 'image/png' && extn != 'application/pdf') {
      file.error = this.l('DocumentFile_Warn_Extention');
      this.alldocumentsValid = false;
    }
    if (this.files.filter((r) => r.name.includes(file.name)).length > 0) {
      return;
    } else {
      var item = new CreateOrEditDocumentFileDto();
      item.name = event.target.files[0].name;
      item.extn = extn;
      item.notes = file.error;
      this.files.push(item);
    }
    this.alldocumentsValid = true;

    this.DocsUploader.addToQueue(event.target.files);
    this.DocsUploader.uploadAll();
  }

  get isFileInputValid() {
    return this.files.length > 0 && this.files.find((r) => r.updateDocumentFileInput == null) != null ? false : true;
  }

  removeFile(name1: string) {
    let index = this.files.findIndex((r) => r.name.includes(name1));
    this.files.splice(index, 1);
    if (this.files.length == 0) this.docProgress = undefined;
  }
}
