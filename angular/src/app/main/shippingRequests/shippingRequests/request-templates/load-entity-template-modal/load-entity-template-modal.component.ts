import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  EntityTemplateServiceProxy,
  SavedEntityType,
  TemplateSelectItemDto,
  TemplateSelectItemGroupDto,
} from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';
import DataSource from '@node_modules/devextreme/data/data_source';
import CustomStore from '@node_modules/devextreme/data/custom_store';

@Component({
  selector: 'load-entity-template-modal',
  templateUrl: './load-entity-template-modal.component.html',
  styleUrls: ['./load-entity-template-modal.component.css'],
})
export class LoadEntityTemplateModalComponent extends AppComponentBase {
  @ViewChild('loadTemplateModal', { static: false }) public modal: ModalDirective;
  active = false;
  templates: TemplateSelectItemGroupDto[];
  entityId: number;
  templateName: string;
  loading: boolean;
  template: TemplateSelectItemDto;
  dataSource: DataSource;

  constructor(injector: Injector, private _templateService: EntityTemplateServiceProxy, private _router: Router) {
    super(injector);
  }

  // todo see trip template impact
  show(id: SavedEntityType) {
    let self = this;
    this.dataSource = new DataSource({
      store: new CustomStore({
        load() {
          return self._templateService
            .getShippingRequestTemplatesForDropdown()
            .toPromise()
            .then((result) => {
              return {
                data: result,
                groupCount: result.length,
              };
            });
        },
      }),
      key: 'id',
      map(item) {
        item.key = item.typeTitle;
        item.items = item.templates;
        return item;
      },
    });

    this.entityId = id;
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.template = undefined;
    this.modal.hide();
  }

  ApplyTemplate() {
    let route =
      this.template.type === SavedEntityType.DedicatedShippingRequestTemplate
        ? '/app/main/shippingRequests/dedicatedShippingRequestWizard'
        : '/app/main/shippingRequests/shippingRequestWizard';

    this._router
      .navigate([route], {
        queryParams: {
          templateId: this.template.id,
        },
      })
      .then(() => {
        this.close();
      });
  }
}
