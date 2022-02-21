import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EmailTemplatesServiceProxy } from '@shared/service-proxies/service-proxies';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { CreateOrEditEmailTemplateTranslationModalComponent } from '@app/main/emailTemplates/emailTemplates/create-or-edit-email-template-translation-modal.component';

@Component({
  selector: 'app-email-template-translation-template',
  templateUrl: './email-template-translation-template.component.html',
})
export class EmailTemplateTranslationTemplateComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditEmailTemplateTranslationModal', { static: true })
  createOrEditEmailTemplateTranslationModal: CreateOrEditEmailTemplateTranslationModalComponent;
  dataSource: any = {};
  @Input() CoreId: any;

  constructor(injector: Injector, private _emailTemplatesServiceProxy: EmailTemplatesServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.DxGetAll();
  }

  DxGetAll() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      key: 'id',
      load(loadOptions: LoadOptions) {
        return self._emailTemplatesServiceProxy
          .getAllTranslations(self.CoreId, JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
      // ,
      // insert: (values) => {
      //   values = { ...values, ...{ coreId: self.CoreId } };
      //   return self._trucksTypesServiceProxy.createOrEditTranslation(values).toPromise();
      // },
      // update: (key, values) => {
      //   values = { ...values, ...{ id: key } };
      //   return self._trucksTypesServiceProxy.createOrEditTranslation(values).toPromise();
      // },
    });
  }

  updateRow(options) {
    options.newData = { ...options.oldData, ...options.newData };
  }

  createEmailTemplate(): void {
    this.createOrEditEmailTemplateTranslationModal.show(this.CoreId);
  }
}
