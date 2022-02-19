import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EmailEditorComponent } from '@node_modules/angular-email-editor';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { CreateOrEditEmailTemplateTranslationDto, EmailTemplatesServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-create-or-edit-email-template-translation-modal',
  templateUrl: './create-or-edit-email-template-translation-modal.component.html',
})
export class CreateOrEditEmailTemplateTranslationModalComponent extends AppComponentBase implements OnInit {
  title = 'angular-email-editor';

  @ViewChild(EmailEditorComponent)
  private emailEditor: EmailEditorComponent;
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  emailTemplate: CreateOrEditEmailTemplateTranslationDto = new CreateOrEditEmailTemplateTranslationDto();

  constructor(injector: Injector, private _emailTemplatesServiceProxy: EmailTemplatesServiceProxy) {
    super(injector);
  }

  show(coreId: number, emailTemplateTranslationId?: number): void {
    this._emailTemplatesServiceProxy.getEmailTemplateTranslationForCreateOrEdit(emailTemplateTranslationId, coreId).subscribe((result) => {
      this.emailTemplate = result.emailTemplate;
      this.emailTemplate.coreId = coreId;
      this.active = true;
      this.modal.show();
    });
  }

  save(): void {
    this.saving = true;

    this.emailEditor.editor.exportHtml((data) => {
      this.emailTemplate.translatedContent = JSON.stringify(data);
      this._emailTemplatesServiceProxy
        .createOrEditTranslation(this.emailTemplate)
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

  ngOnInit(): void {}

  // called when the editor is created
  editorLoaded($event: any) {
    console.log('editorLoaded');
    // load the design json here
    // this.emailEditor.editor.loadDesign({});
  }

  // called when the editor has finished loading
  editorReady($event: any) {
    this.emailEditor.editor.loadDesign(JSON.parse(this.emailTemplate.translatedContent).design);
  }
}
