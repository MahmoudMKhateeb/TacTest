import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActorsSubmittedDocumentsListComponent } from '@app/main/documentFiles/documentFiles/actors-submitted-documents/actors-submitted-documents-list/actors-submitted-documents-list.component';

@Component({
  selector: 'app-actors-submitted-documents',
  templateUrl: './actors-submitted-documents.component.html',
  styleUrls: ['./actors-submitted-documents.component.css'],
})
export class ActorsSubmittedDocumentsComponent extends AppComponentBase implements OnInit {
  @ViewChild('actorsSubmitedDocumentsListComponentList', { static: true })
  actorsSubmitedDocumentsListComponentList: ActorsSubmittedDocumentsListComponent;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.actorsSubmitedDocumentsListComponentList.getDocumentFiles();
  }
}
