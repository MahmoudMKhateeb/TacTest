import { Component, EventEmitter, Injector, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';

@Component({
  selector: 'app-automation-setup-modal',
  templateUrl: './automation-setup-modal.component.html',
  styleUrls: ['./automation-setup-modal.component.scss'],
})
export class AutomationSetupModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  automationSetupModel: any = {
    generationFrequency: null,
    sendBy: null,
    emailAddress: null,
    emailSubject: null,
    emailBody: null,
  };

  saving = false;
  allGenerationFrequencies: any[] = [];
  allSendBy: any[] = [
    {
      displayName: this.l('Platform'),
      id: 1,
    },
    {
      displayName: this.l('Email'),
      id: 2,
    },
  ];

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}

  show() {
    this.modal.show();
  }

  ngOnDestroy(): void {}

  save() {
    this.modalSave.emit(this.automationSetupModel);
    this.close();
  }

  close() {
    this.modal.hide();
  }

  changedSendBy() {
    if (this.automationSetupModel.sendBy != 2) {
      this.automationSetupModel.emailAddress = null;
      this.automationSetupModel.emailSubject = null;
      this.automationSetupModel.emailBody = null;
    }
  }
}
