import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import {
  CreateOrEditDocumentFileDto,
  DocumentFilesServiceProxy,
  DocumentTypesServiceProxy,
  UpdateDocumentFileInput,
  UserEditDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import { ControlContainer, NgForm } from '@angular/forms';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'app-required-document-form-child',
  templateUrl: './required-document-form-child.component.html',
  styleUrls: ['./required-document-form-child.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }],
})
export class RequiredDocumentFormChildComponent extends AppComponentBase implements OnInit {
  @Output() onDocsUploaderCompleteAll: EventEmitter<any> = new EventEmitter();
  @Input() createOrEditDocumentFileDtos!: CreateOrEditDocumentFileDto[];
  @Input() parentForm: NgForm;

  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  selectedDateTypeHijri = DateType.Hijri; // or DateType.Gregorian
  selectedDateTypeGregorian = DateType.Gregorian; // or DateType.Gregorian

  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;

  fileFormateIsInvalideIndexList: number[] = [];
  fileisDuplicateList: number[] = [];
  /**
   * required documents fileUploader
   */
  public DocsUploader: FileUploader;
  /**
   * required documents fileUploader options
   * @private
   */
  private _DocsUploaderOptions: FileUploaderOptions = {};
  selectedDate: any;

  constructor(
    injector: Injector,
    private cdr: ChangeDetectorRef,
    private _tokenService: TokenService,
    private _fileDownloadService: FileDownloadService,
    public _documentTypesServiceProxy: DocumentTypesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    console.log('Child ngOnInit');
    this.cdr.detectChanges();
    this.intilizedates();
    this.initDocsUploader();
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto, index: number): void {
    item.extn = event.target.files[0].type;
    item.name = event.target.files[0].name;

    //size validation
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      item.name = '';
      return;
    }

    //extention validation
    if (item.extn !== 'image/jpeg' && item.extn !== 'image/png' && item.extn !== 'application/pdf') {
      this.fileFormateIsInvalideIndexList.push(index);
      this.message.warn(this.l('PleaseChooseAvalidFormat'));
      item.name = '';
      return;
    } else {
      this.fileFormateIsInvalideIndexList = this.fileFormateIsInvalideIndexList.filter((x) => x !== index);
    }

    //not allow uploading the same document twice
    for (let i = 0; i < this.createOrEditDocumentFileDtos.length; i++) {
      const element = this.createOrEditDocumentFileDtos[i];
      if (element.name === item.name && element.extn === item.extn && i !== index) {
        item.name = '';
        item.extn = '';
        this.fileisDuplicateList.push(index);
        this.message.warn(this.l('DuplicateFileUploadMsg', element.name, element.extn));
        return;
      } else {
        //remove it if exist in validation list
        this.fileisDuplicateList = this.fileisDuplicateList.filter((x) => x !== index);
      }
    }

    this.DocsUploader.addToQueue(event.target.files);
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
        //attach each fileToken to his CreateOrEditDocumentFileDto

        this.createOrEditDocumentFileDtos.find((x) => x.name === item.file.name && x.extn === item.file.type).updateDocumentFileInput =
          new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
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

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }

  intilizedates() {
    this.createOrEditDocumentFileDtos.forEach((element) => {
      if (element.documentTypeDto.hasExpirationDate) {
        element.expirationDate = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
      }
    });
  }

  isFileFormatValid(i: number): boolean {
    return this.fileFormateIsInvalideIndexList.every((x) => x !== i);
  }

  isFileDuplicate(i: number): boolean {
    return !this.fileisDuplicateList.every((x) => x !== i);
  }

  isAllFileFormatValid(): boolean {
    return this.fileFormateIsInvalideIndexList?.length === 0;
  }

  isAllFileDuplicatePass(): boolean {
    return this.fileisDuplicateList?.length === 0;
  }

  onlyNumberKey(event) {
    return event.charCode == 8 || event.charCode == 0 ? null : event.charCode >= 48 && event.charCode <= 57;
  }
  downloadTemplate(id: number): void {
    this._documentTypesServiceProxy.getFileDto(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}
