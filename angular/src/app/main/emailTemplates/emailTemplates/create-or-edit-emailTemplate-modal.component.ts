import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CreateOrEditEmailTemplateDto, EmailTemplatesServiceProxy, EmailTemplateTypesEnum } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EmailEditorComponent } from '@node_modules/angular-email-editor';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'createOrEditEmailTemplateModal',
  templateUrl: './create-or-edit-emailTemplate-modal.component.html',
  providers: [EnumToArrayPipe],
})
export class CreateOrEditEmailTemplateModalComponent extends AppComponentBase implements OnInit {
  title = 'angular-email-editor';

  @ViewChild(EmailEditorComponent)
  private emailEditor: EmailEditorComponent;
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  emailTemplate: CreateOrEditEmailTemplateDto = new CreateOrEditEmailTemplateDto();

  types: any;

  constructor(injector: Injector, private _emailTemplatesServiceProxy: EmailTemplatesServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }

  show(emailTemplateId?: number): void {
    if (!emailTemplateId) {
      this.emailTemplate = new CreateOrEditEmailTemplateDto();
      this.emailTemplate.id = emailTemplateId;

      this.active = true;
      this.modal.show();
    } else {
      this._emailTemplatesServiceProxy.getEmailTemplateForEdit(emailTemplateId).subscribe((result) => {
        this.emailTemplate = result.emailTemplate;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this.emailEditor.editor.exportHtml((data) => {
      this.emailTemplate.content = JSON.stringify(data);
      this._emailTemplatesServiceProxy
        .createOrEdit(this.emailTemplate)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.close();
          this.modalSave.emit(null);
        });
      console.log(JSON.stringify(data));
    });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  ngOnInit(): void {
    this.types = this.enumToArray.transform(EmailTemplateTypesEnum);
  }

  // called when the editor is created
  editorLoaded($event: any) {
    console.log('editorLoaded');
    // load the design json here
    // this.emailEditor.editor.loadDesign({});
  }

  // called when the editor has finished loading
  editorReady($event: any) {
    this.emailEditor.editor.loadDesign(JSON.parse(this.emailTemplate.content).design);
  }
}
