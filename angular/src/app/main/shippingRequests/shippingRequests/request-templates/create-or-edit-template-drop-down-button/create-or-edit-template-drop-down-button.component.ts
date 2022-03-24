import { Component, Injector, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { CreateOrEditEntityTemplateInputDto, EntityTemplateServiceProxy, SavedEntityType } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'create-or-edit-template-drop-down-button',
  templateUrl: './create-or-edit-template-drop-down-button.component.html',
  styleUrls: ['./create-or-edit-template-drop-down-button.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class CreateOrEditTemplateDropDownButtonComponent extends AppComponentBase {
  @Input() entityType: SavedEntityType;
  @Input() sourceEntityId: string;
  @Input() customClass: string;
  @Input() jsonData: string;
  @Input() dropDirection: 'up' | 'down';
  @Input() disabled: boolean;

  templateName: string;
  loading: boolean;
  templateIdForEdit: string = this._activeRoute.snapshot.queryParams['templateId'] || null;
  constructor(
    injector: Injector,
    private _templateService: EntityTemplateServiceProxy,
    private _router: Router,
    private _activeRoute: ActivatedRoute
  ) {
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
    entityTemplateInput.templateName = this.templateName;
    entityTemplateInput.savedEntityId = this.sourceEntityId;
    entityTemplateInput.entityType = this.entityType;
    if (isNotNullOrUndefined(this.templateIdForEdit)) {
      entityTemplateInput.id = Number(this.templateIdForEdit);
    }
    // there is 2 ways of Creating Entity Template  1.By Providing EntityId example: TripId / ShippingRequestID
    // or 2. by providing a json data of the entity for example Json Data Of the Trip/Shipping Request
    // Handle if there is Json Data
    if (isNotNullOrUndefined(this.jsonData)) {
      entityTemplateInput.savedEntityId = undefined;
      entityTemplateInput.savedEntity = this.jsonData;
    }

    this._templateService
      .createOrEdit(entityTemplateInput)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.templateIdForEdit = res;
        this.updateUrl(res);
        this.notify.success(this.l('TemplateSavedSuccessfully'));
      });
  }

  /**
   * Add Created Template Id To The Url To allow Edits
   * @param TemplateId
   */
  updateUrl(TemplateId: string) {
    this._router.navigate([], {
      queryParams: {
        templateId: TemplateId,
      },
      queryParamsHandling: 'merge',
    });
  }
}
