import { Component, Injector, ViewEncapsulation, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentFilesServiceProxy, DocumentFileDto, SelectItemDto, DocumentsEntitiesEnum } from '@shared/service-proxies/service-proxies';
import { NotifyService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { EntityTypeHistoryModalComponent } from '@app/shared/common/entityHistory/entity-type-history-modal.component';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import * as _ from 'lodash';
import * as moment from 'moment';
import session = abp.session;
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { CreateOrEditDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/create-or-edit-documentFile-modal.component';
import { ViewDocumentFileModalComponent } from '@app/main/documentFiles/documentFiles/view-documentFile-modal.component';
import { DriverSubmitedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/drivers-submitted-documents/driver-submited-documents-list/driver-submited-documents-list.component';

@Component({
  selector: 'app-drivers-submitted-documents',
  templateUrl: './drivers-submitted-documents.component.html',
  styleUrls: ['./drivers-submitted-documents.component.css'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
  providers: [DateFormatterService],
})
export class DriversSubmittedDocumentsComponent extends AppComponentBase implements OnInit {
  @ViewChild('driverSubmitedDocumentsListComponent', { static: true }) driverSubmitedDocumentsListComponent: DriverSubmitedDocumentsListComponent;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.driverSubmitedDocumentsListComponent.getDocumentFiles();
  }
}
