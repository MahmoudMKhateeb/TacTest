import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenService } from '@node_modules/abp-ng2-module';
import {
  CreateOrEditDocumentFileDto,
  DocumentFileDto,
  DocumentFilesServiceProxy,
  DocumentsEntitiesEnum,
  GetTenantSubmittedDocumnetForView,
} from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Table } from 'primeng/table';
import * as moment from '@node_modules/moment';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Router } from '@angular/router';
import { RequiredDocumentFormChildComponent } from '@app/shared/common/required-document-form-child/required-document-form-child.component';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-required-document-files',
  templateUrl: './not-required-document-files.component.html',
  animations: [appModuleAnimation()],
  providers: [DateFormatterService],
})
export class NotRequiredDocumentFilesComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('requiredDocumentFormChildComponent', { static: false }) requiredDocumentFormChildComponent: RequiredDocumentFormChildComponent;
  @ViewChild('TenantRequiredDocumentsForm', { static: false }) TenantRequiredDocumentsForm: NgForm;
  active = false;
  saving = false;
  GregValue: moment.Moment;
  isFormSubmitted = false;
  submittedDocumentsList: GetTenantSubmittedDocumnetForView[] = [];
  createOrEditDocumentFileDtos: CreateOrEditDocumentFileDto[];
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayMoment = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  @ViewChild('requiredDocumentsCard') private myScrollContainer: ElementRef;
  documentsEntitiesEnum = DocumentsEntitiesEnum;

  constructor(
    injector: Injector,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _tokenService: TokenService,
    private _fileDownloadService: FileDownloadService,
    private _router: Router
  ) {
    super(injector);

    this.getTenantNotRrquiredDocuments();
    this.getAllsubmittedDocumentsStatusList();
    this.createOrEditDocumentFileDtos = [];
  }
  /**
   * Getter That Checks if All Documents Are Accepted
   * @constructor
   */
  get IsAllDocumentsAccepted(): boolean {
    let i = 0;
    this.submittedDocumentsList.forEach((x) => {
      if (x.isAccepted) i++;
    });
    return i === this.submittedDocumentsList.length ? true : false;
  }

  public saveInternal() {
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
  }

  save(): void {
    if (this.TenantRequiredDocumentsForm.invalid) {
      return;
    }
    this.saving = true;
    if (this.requiredDocumentFormChildComponent.DocsUploader.queue?.length > 0) {
      //this.normalizeDates();
      this.requiredDocumentFormChildComponent.DocsUploader.uploadAll();
    }
  }

  getAllsubmittedDocumentsStatusList() {
    this._documentFilesServiceProxy.getAllTenantSubmittedDocumentsWithStatuses(false).subscribe((result) => {
      if (result.length > 0) {
        this.isFormSubmitted = true;
        this.submittedDocumentsList = result;

        let isAccepted = this.submittedDocumentsList.every((x) =>
          x.isAccepted === true && x.expirationDate === undefined ? true : x.expirationDate >= moment(abp.clock.now())
        );
        if (isAccepted) {
          //  this._router.navigate(['app/main/dashboard']);
        }
      } else {
        this.isFormSubmitted = false;
        this.submittedDocumentsList = [];
      }
    });
  }

  getTenantNotRrquiredDocuments() {
    this._documentFilesServiceProxy.getTenentMissingDocuments(false).subscribe((result) => {
      result.forEach((x) => (x.expirationDate = null));
      this.createOrEditDocumentFileDtos = result;

      this.active = true;
      if (this.createOrEditDocumentFileDtos.length > 0) {
        this.scrollToRequiredDocumentsList();
      }
    });
  }

  reload() {
    this.getTenantNotRrquiredDocuments();
    this.getAllsubmittedDocumentsStatusList();
  }

  // checkIfIsNumberUnique(documnetType: DocumentTypeDto, index, number) {
  //   if (!documnetType.isNumberUnique) {
  //     this.uniqueNumberIsInvalideIndexList[index] = false;
  //     return;
  //   }
  //   let documnet = new DocumentUniqueCheckOutput();
  //   documnet.number = number;
  //   documnet.documentTypeId = documnetType.id.toString();
  //   this._documentFilesServiceProxy.isDocumentTypeNumberUnique(documnet).subscribe((result) => {
  //     this.uniqueNumberIsInvalideIndexList[index] = !result;
  //   });
  // }

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
}
