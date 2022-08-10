import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { EntityTemplateServiceProxy, SavedEntityType } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'load-entity-template-modal',
  templateUrl: './load-entity-template-modal.component.html',
  styleUrls: ['./load-entity-template-modal.component.css'],
})
export class LoadEntityTemplateModalComponent extends AppComponentBase {
  @ViewChild('loadTemplateModal', { static: false }) public modal: ModalDirective;
  active = false;
  templates: any;
  entityId: number;
  templateName: string;
  entityTypesEnum = SavedEntityType;
  loading: boolean;
  templateId: number;

  constructor(injector: Injector, private _templateService: EntityTemplateServiceProxy, private _router: Router) {
    super(injector);
  }

  show(id: SavedEntityType) {
    this.loadTemplate();

    this.entityId = id;
    this.active = true;
    this.modal.show();
  }
  close() {
    this.active = false;
    this.modal.hide();
  }

  /**
   * get Templates List From the Helper Class
   */
  loadTemplate() {
    this.loading = true;
    this._templateService.getAllForDropdown(this.entityTypesEnum.ShippingRequestTemplate, null).subscribe((res) => {
      this.templates = res;
      this.loading = false;
    });
  }

  ApplyTemplate() {
    this._router
      .navigate(['/app/main/shippingRequests/shippingRequestWizard'], {
        queryParams: {
          templateId: this.templateId,
        },
      })
      .then(() => {
        this.close();
      });
  }
}
