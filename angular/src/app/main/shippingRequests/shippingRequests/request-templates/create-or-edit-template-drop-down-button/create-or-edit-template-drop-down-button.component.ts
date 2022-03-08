import { Component, Injector, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { CreateOrEditEntityTemplateInputDto, EntityTemplateServiceProxy, SavedEntityType } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'create-or-edit-template-drop-down-button',
  templateUrl: './create-or-edit-template-drop-down-button.component.html',
  styleUrls: ['./create-or-edit-template-drop-down-button.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class CreateOrEditTemplateDropDownButtonComponent extends AppComponentBase {
  @Input() entityType: SavedEntityType;
  @Input() sourceEntityId: string;
  @Input() templateIdForEdit: number;
  @Input() customClass: string;
  templateName: string;
  loading: boolean;

  constructor(injector: Injector, private _templateService: EntityTemplateServiceProxy) {
    super(injector);
  }
  /**
   * save Template
   * @constructor
   */

  SaveAsTemplate(): void {
    if (!isNotNullOrUndefined(this.templateName) || this.templateName === '') {
      return;
    }
    this.loading = true;
    let entityTemplateInput: CreateOrEditEntityTemplateInputDto = new CreateOrEditEntityTemplateInputDto();
    if (isNotNullOrUndefined(this.templateIdForEdit)) {
      entityTemplateInput.id = this.templateIdForEdit;
    }
    entityTemplateInput.templateName = this.templateName;
    entityTemplateInput.savedEntityId = this.sourceEntityId;
    entityTemplateInput.entityType = this.entityType;
    this._templateService
      .createOrEdit(entityTemplateInput)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe(() => {
        this.notify.success(this.l('TemplateSavedSuccessfully'));
      });
  }
}
