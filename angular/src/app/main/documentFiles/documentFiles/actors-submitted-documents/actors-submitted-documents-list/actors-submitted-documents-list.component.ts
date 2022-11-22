import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { ViewDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/view-documentFile-modal.component';
import {
  ActorTypesEnum,
  DocumentFileDto,
  DocumentFilesServiceProxy,
  DocumentsEntitiesEnum,
  TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@node_modules/abp-ng2-module';
import { ActivatedRoute } from '@angular/router';
import { FileDownloadService } from '@shared/utils/file-download.service';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import * as _ from 'lodash';

@Component({
  selector: 'app-actors-submitted-documents-list',
  templateUrl: './actors-submitted-documents-list.component.html',
  styleUrls: ['./actors-submitted-documents-list.component.css'],
})
export class ActorsSubmittedDocumentsListComponent extends AppComponentBase implements OnInit {
  @Input() actorId: any;
  @ViewChild('entityTypeHistoryModal', { static: true })
  entityTypeHistoryModal: EntityTypeHistoryModalComponent;
  @ViewChild('createOrEditDocumentFileModal', { static: true }) createOrEditDocumentFileModal: CreateOrEditDocumentFileModalComponent;
  @ViewChild('viewDocumentFileModalComponent', { static: true }) viewDocumentFileModal: ViewDocumentFileModalComponent;

  isHost = true;
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  entityType = DocumentsEntitiesEnum.Tenant;

  _entityTypeFullName = 'TACHYON.Documents.DocumentFiles.DocumentFile';
  entityHistoryEnabled = false;
  entityIdFilter = '';

  todayMoment = this.dateFormatterService.NgbDateStructToMoment(this.todayGregorian);
  statuses: any[];
  items: any[] = [];
  actorTypes = Object.entries(ActorTypesEnum);

  // ADDED DEVEXTREME GRID
  dataSource: any = {};
  popupPosition: any = { of: window, at: 'top', my: 'top', offset: { y: 10 } };

  constructor(
    injector: Injector,
    private _documentFilesServiceProxy: DocumentFilesServiceProxy,
    private _notifyService: NotifyService,
    private _tokenAuth: TokenAuthServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.entityHistoryEnabled = this.setIsEntityHistoryEnabled();
  }

  getDocumentFiles() {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._documentFilesServiceProxy
          .getAllActorsSubmittedDocuments(JSON.stringify(loadOptions), self.actorId)
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  getDocumentFilesByDriverId(driverId: any) {
    this.actorId = driverId;
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._documentFilesServiceProxy
          .getAllDriversSubmittedDocuments(JSON.stringify(loadOptions), driverId)
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          })
          .finally(() => {});
      },
    });
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
    //this.getDocumentFiles();
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

  getActorTypeTitle(actorTypeValue) {
    let actorType = this.actorTypes.find((x) => x[0] == actorTypeValue);
    return this.l(actorType[1].toString());
  }
}
