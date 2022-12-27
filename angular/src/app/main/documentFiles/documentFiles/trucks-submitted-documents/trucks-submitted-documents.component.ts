import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  DocumentFileDto,
  DocumentFilesServiceProxy,
  DocumentsEntitiesEnum,
  SelectItemDto,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import * as _ from 'lodash';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { ViewDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/view-documentFile-modal.component';
import { TruckSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/trucks-submitted-documents/truck-submited-documents-list/truck-submited-documents-list.component';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import session = abp.session;

@Component({
  selector: 'app-trucks-submitted-documents',
  templateUrl: './trucks-submitted-documents.component.html',
  styleUrls: ['./trucks-submitted-documents.component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
  providers: [DateFormatterService, EnumToArrayPipe],
})
export class TrucksSubmittedDocumentsComponent extends AppComponentBase implements OnInit {
  @ViewChild('truckSubmitedDocumentsListComponent', { static: true }) truckSubmitedDocumentsListComponent: TruckSubmitedDocumentsListComponent;
  @ViewChild('entityTypeHistoryModal', { static: true }) entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;
  @ViewChild('viewDocumentFileModalComponent', { static: true }) viewDocumentFileModal: ViewDocumentFileModalComponent;

  isHost = true;
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  entityType = DocumentsEntitiesEnum.Tenant;
  entityTypesList: SelectItemDto[] = [];

  _entityTypeFullName = 'TACHYON.Documents.DocumentFiles.DocumentFile';
  entityHistoryEnabled = false;
  entityIdFilter = '';

  todayMoment = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
  statuses: any[];
  items: any[] = [];

  // ADDED DEVEXTREME GRID
  dataSource: any = {};
  popupPosition = { of: window, at: 'top', my: 'top', offset: { y: 10 } };

  constructor(
    injector: Injector,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.isHost = session.tenantId == null;
    this.truckSubmitedDocumentsListComponent.getDocumentFiles();

    this.entityTypesList = this.enumToArray.transform(DocumentsEntitiesEnum);

    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
  }

  private setIsEntityHistoryEnabled(): boolean {
    let customSettings = (abp as any).custom;
    return (
      this.isGrantedAny('Pages.Administration.AuditLogs') &&
      customSettings.EntityHistory &&
      customSettings.EntityHistory.isEnabled &&
      _.filter(customSettings.EntityHistory.enabledEntities, (entityType) => entityType === this._entityTypeFullName).length === 1
    );
  }

  reloadPage(): void {
    this.truckSubmitedDocumentsListComponent.getDocumentFiles();
  }

  showHistory(documentFile: DocumentFileDto): void {
    this.entityTypeHistoryModal.show({
      entityId: documentFile.id.toString(),
      entityTypeFullName: this._entityTypeFullName,
      entityTypeDescription: '',
    });
  }

  deleteDocumentFile(documentFile: DocumentFileDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this._documentFilesServiceProxy.delete(documentFile.id).subscribe(() => {
          this.reloadPage();
          this.notify.success(this.l('SuccessfullyDeleted'));
        });
      }
    });
  }

  downloadDocument(documentFile: DocumentFileDto) {
    this._documentFilesServiceProxy.getDocumentFileDto(documentFile.id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  acceptDocumentFile(id: string) {
    this._documentFilesServiceProxy.accept(id).subscribe(() => {
      this.reloadPage();
      this.notify.success(this.l('SuccessfullyAccepted'));
    });
  }
}
