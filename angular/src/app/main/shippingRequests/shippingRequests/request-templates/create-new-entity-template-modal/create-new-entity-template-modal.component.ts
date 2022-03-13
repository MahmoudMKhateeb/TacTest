import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { CreateOrEditEntityTemplateInputDto, EntityTemplateServiceProxy, SavedEntityType } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'create-new-entity-template-modal',
  templateUrl: './create-new-entity-template-modal.component.html',
  styleUrls: ['./create-new-entity-template-modal.component.css'],
})
export class CreateNewEntityTemplateModalComponent extends AppComponentBase {
  @ViewChild('AddNewEntityModalComponent', { static: false }) public modal: ModalDirective;
  active = false;
  saving = false;
  private entityType: SavedEntityType = SavedEntityType.TripTemplate;
  constructor(injector: Injector, private _templateService: EntityTemplateServiceProxy) {
    super(injector);
  }
  entityId: number;
  templateName: string;
  show(id: number) {
    this.entityId = id;
    this.active = true;
    this.modal.show();
  }
  close() {
    this.active = false;
    this.modal.hide();
  }

  /**
   * create template in backend
   */
  save() {
    this.saving = true;
    let entityTemplateInput: CreateOrEditEntityTemplateInputDto = new CreateOrEditEntityTemplateInputDto();
    entityTemplateInput.savedEntityId = this.entityId.toString();
    entityTemplateInput.entityType = this.entityType;
    entityTemplateInput.templateName = this.templateName;
    this._templateService
      .createOrEdit(entityTemplateInput)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.close();
        })
      )
      .subscribe(() => {
        this.notify.success('TemplateSavedSuccessfully');
      });
  }
}
