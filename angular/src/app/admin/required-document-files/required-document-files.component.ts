import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileItem, FileUploader, FileUploaderOptions } from '@node_modules/ng2-file-upload';
import { AppConsts } from '@shared/AppConsts';
import { IAjaxResponse, TokenService } from '@node_modules/abp-ng2-module';
import {
  CreateOrEditDocumentFileDto,
  DocumentFileDto,
  DocumentFilesServiceProxy,
  DocumentTypeDto,
  DocumentUniqueCheckOutput,
  GetTenantSubmittedDocumnetForView,
  UpdateDocumentFileInput,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import * as moment from '@node_modules/moment';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { DateType } from '@app/shared/common/hijri-gregorian-datepicker/consts';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-required-document-files',
  templateUrl: './required-document-files.component.html',
  animations: [appModuleAnimation()],
  providers: [DateFormatterService],
})
export class RequiredDocumentFilesComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('requiredDocumentsCard') private myScrollContainer: ElementRef;
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  active = false;
  saving = false;
  GregValue: moment.Moment;
  isFormSubmitted = false;
  selectedDateTypeHijri = DateType.Hijri; // or DateType.Gregorian
  selectedDateTypeGregorian = DateType.Gregorian; // or DateType.Gregorian
  fileFormateIsInvalideIndexList: boolean[] = [];
  uniqueNumberIsInvalideIndexList: boolean[] = [];
  DateInvalideIndexList: boolean[] = [];
  submittedDocumentsList: GetTenantSubmittedDocumnetForView[] = [];
  // truckFiles: File[] = [];
  alldocumentsValid = false;
  todayMoment = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  allnumbersValid = false;
  numbersInValidList: boolean[] = [];
  allDatesValid = true;
  datesInValidList: boolean[] = [];
  fileisDuplicateList: boolean[] = [];
  alldocumentsNotDuplicated = false;
  /**
   * required documents fileUploader
   */
  public DocsUploader: FileUploader;
  /**
   * DocFileUploader onProgressItem progress
   */
  docProgress: any;
  /**
   * DocFileUploader onProgressItem file name
   */
  docProgressFileName: any;
  private _uploaderOptions: FileUploaderOptions = {};
  /**
   * required documents fileUploader options
   * @private
   */
  private _DocsUploaderOptions: FileUploaderOptions = {};

  createOrEditDocumentFileDtos: CreateOrEditDocumentFileDto[];
  constructor(
    injector: Injector,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _tokenService: TokenService,
    private _fileDownloadService: FileDownloadService,
    private _router: Router
  ) {
    super(injector);

    this.getTenantRrquiredDocuments();
    this.getAllsubmittedDocumentsStatusList();
    this.createOrEditDocumentFileDtos = [];
  }

  ngOnInit(): void {
    this.initDocsUploader();
  }

  /**
   * initialize required documents fileUploader
   */
  initDocsUploader(): void {
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
        this.createOrEditDocumentFileDtos.find(
          (x) => x.name === item.file.name && x.extn === item.file.type
        ).updateDocumentFileInput = new UpdateDocumentFileInput({ fileToken: resp.result.fileToken });
      } else {
        this.message.error(resp.error.message);
      }
    };

    this.DocsUploader.onErrorItem = (item, response, status) => {
      const resp = <IAjaxResponse>JSON.parse(response);
    };

    this.DocsUploader.onCompleteAll = () => {
      // create tenant req.
      if (!this.alldocumentsValid) {
        this.notify.error(this.l('makeSureThatYouFillAllRequiredFields'));
        return;
      }

      this.createOrEditDocumentFileDtos.forEach((element) => {
        if (element.documentTypeDto.hasExpirationDate) {
          let date = this.dateFormatterService.MomentToNgbDateStruct(element.expirationDate);
          let hijriDate = this.dateFormatterService.ToHijri(date);
          element.hijriExpirationDate = this.dateFormatterService.ToString(hijriDate);
        }
      });

      this._documentFilesServiceProxy
        .addTenantRequiredDocumentFiles(this.createOrEditDocumentFileDtos)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.isFormSubmitted = true;
          this.saving = false;
          this.reload();
          this.notify.info(this.l('SavedSuccessfully'));
        });
    };

    //for progressBar
    this.DocsUploader.onProgressItem = (fileItem: FileItem, progress: any) => {
      this.docProgress = progress;
      this.docProgressFileName = fileItem.file.name;
    };

    this.DocsUploader.setOptions(this._DocsUploaderOptions);
  }

  DocFileChangeEvent(event: any, item: CreateOrEditDocumentFileDto, index: number): void {
    if (event.target.files[0].size > 5242880) {
      //5MB
      this.message.warn(this.l('DocumentFile_Warn_SizeLimit', this.maxDocumentFileBytesUserFriendlyValue));
      this.isAllfileFormatesAccepted();
      item.name = '';
      return;
    }
    item.extn = event.target.files[0].type;
    if (item.extn != 'image/jpeg' && item.extn != 'image/png' && item.extn != 'application/pdf') {
      this.fileFormateIsInvalideIndexList[index] = true;
      item.name = '';
      // this.truckFiles[index] = null;
      this.isAllfileFormatesAccepted();
      return;
    }

    item.name = event.target.files[0].name;
    //not allow uploading the same document twice
    for (let i = 0; i < this.createOrEditDocumentFileDtos.length; i++) {
      const element = this.createOrEditDocumentFileDtos[i];
      if (element.name == event.target.files[0].name && element.extn == event.target.files[0].type && i != index) {
        item.name = '';
        item.extn = '';
        this.fileisDuplicateList[index] = true;
        this.isAllfileNotDuplicated();
        this.message.warn(this.l('DuplicateFileUploadMsg', element.name, element.extn));
        return;
      }
    }

    this.fileisDuplicateList[index] = false;
    this.isAllfileNotDuplicated();
    this.fileFormateIsInvalideIndexList[index] = false;
    // this.truckFiles[index] = event.target.files;
    this.isAllfileFormatesAccepted();
    this.DocsUploader.addToQueue(event.target.files);
  }

  guid(): string {
    function s4() {
      return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
  }

  save(): void {
    if (!this.alldocumentsValid || !this.alldocumentsNotDuplicated || !this.allDatesValid || !this.allnumbersValid) {
      this.notify.error(this.l('makeSureThatYouFillAllRequiredFields'));
      return;
    }
    this.saving = true;
    this.DocsUploader.uploadAll();
  }

  getAllsubmittedDocumentsStatusList() {
    this._documentFilesServiceProxy.getAllTenantSubmittedRequiredDocumentsWithStatuses().subscribe((result) => {
      if (result.length > 0) {
        this.isFormSubmitted = true;
        this.submittedDocumentsList = result;
        let isAccepted = this.submittedDocumentsList.every((x) =>
          x.isAccepted == true && x.expirationDate == undefined ? true : x.expirationDate >= this.todayMoment
        );
        if (isAccepted) {
          this._router.navigate(['app/main/dashboard']);
        }
      } else {
        this.isFormSubmitted = false;
        this.submittedDocumentsList = [];
      }
    });
  }
  getTenantRrquiredDocuments() {
    this._documentFilesServiceProxy.getTenentMissingDocuments().subscribe((result) => {
      result.forEach((x) => (x.expirationDate = null));
      this.createOrEditDocumentFileDtos = result;
      this.intilizedates();

      this.active = true;
      if (this.createOrEditDocumentFileDtos.length > 0) {
        this.scrollToRequiredDocumentsList();
      }
    });
  }

  reload() {
    this.getTenantRrquiredDocuments();
    this.getAllsubmittedDocumentsStatusList();
  }

  checkIfIsNumberUnique(documnetType: DocumentTypeDto, index, number) {
    if (!documnetType.isNumberUnique) {
      this.uniqueNumberIsInvalideIndexList[index] = false;
      return;
    }
    let documnet = new DocumentUniqueCheckOutput();
    documnet.number = number;
    documnet.documentTypeId = documnetType.id.toString();
    this._documentFilesServiceProxy.isDocumentTypeNumberUnique(documnet).subscribe((result) => {
      this.uniqueNumberIsInvalideIndexList[index] = !result;
    });
  }

  downloadDocument(documentFile: DocumentFileDto) {
    this._documentFilesServiceProxy.getDocumentFileDto(documentFile.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }

  deleteDocumentFile(documentFile: DocumentFileDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._documentFilesServiceProxy.delete(documentFile.id).subscribe(() => {
          this.reload();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  scrollToRequiredDocumentsList() {
    this.myScrollContainer.nativeElement.scrollIntoView({
      top: this.myScrollContainer.nativeElement.scrollHeight,
      left: 0,
      behavior: 'smooth',
    });
  }
  isAllfileFormatesAccepted() {
    if (
      this.fileFormateIsInvalideIndexList.every((x) => x === false) &&
      this.fileFormateIsInvalideIndexList.length == this.createOrEditDocumentFileDtos.length
    ) {
      this.alldocumentsValid = true;
    } else {
      this.alldocumentsValid = false;
    }
  }

  numberChange(item: CreateOrEditDocumentFileDto, index: number) {
    if (item.documentTypeDto.numberMinDigits <= item.number.length && item.number.length <= item.documentTypeDto.numberMaxDigits) {
      this.numbersInValidList[index] = false;
      this.isNumbersValid();
    } else {
      this.numbersInValidList[index] = true;
      this.isNumbersValid();
    }
  }
  isNumbersValid() {
    if (this.numbersInValidList.every((x) => x === false) && this.numbersInValidList.length == this.createOrEditDocumentFileDtos.length) {
      this.allnumbersValid = true;
    } else {
      this.allnumbersValid = false;
    }
  }

  // dateSelected() {
  //   for (let index = 0; index < this.createOrEditDocumentFileDtos.length; index++) {
  //     const element = this.createOrEditDocumentFileDtos[index];
  //     if (element.documentTypeDto.hasExpirationDate) {
  //       this.datesInValidList[index] = element.expirationDate == null;
  //     }
  //   }
  //   if (
  //     this.datesInValidList.every((x) => x === false) &&
  //     this.createOrEditDocumentFileDtos.filter((x) => x.documentTypeDto.hasExpirationDate).length == this.datesInValidList.length
  //   ) {
  //     this.allDatesValid = true;
  //   } else {
  //     this.allDatesValid = false;
  //   }
  // }

  isAllfileNotDuplicated() {
    if (this.fileisDuplicateList.every((x) => x === false) && this.fileisDuplicateList.length == this.createOrEditDocumentFileDtos.length) {
      this.alldocumentsNotDuplicated = true;
    } else {
      this.alldocumentsNotDuplicated = false;
    }
  }

  intilizedates() {
    this.createOrEditDocumentFileDtos.forEach((element) => {
      if (element.documentTypeDto.hasExpirationDate) {
        element.expirationDate = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
      }
    });
  }
}
