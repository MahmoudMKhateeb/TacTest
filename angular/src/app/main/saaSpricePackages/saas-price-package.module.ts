import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SaasPricePackageRoutingModule } from './saas-price-package-routing.module';
import { TableModule } from 'primeng/table';
import { UtilsModule } from '@shared/utils/utils.module';
import { PaginatorModule } from 'primeng/paginator';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from '@node_modules/ngx-bootstrap/datepicker';
import { SplitButtonModule } from '@node_modules/primeng/splitbutton';
import { MultiSelectModule } from '@node_modules/primeng/multiselect';

// devextreme imports
import { DxDataGridModule } from '@node_modules/devextreme-angular/ui/data-grid';
import { DxPopoverModule } from '@node_modules/devextreme-angular/ui/popover';
import { DxValidationGroupModule } from '@node_modules/devextreme-angular/ui/validation-group';
import { DxValidatorModule } from '@node_modules/devextreme-angular/ui/validator';
import { DxValidationSummaryModule } from '@node_modules/devextreme-angular/ui/validation-summary';
import { DxDateBoxModule } from '@node_modules/devextreme-angular/ui/date-box';
import { DxTextBoxModule } from '@node_modules/devextreme-angular/ui/text-box';
import { DxFileUploaderModule } from '@node_modules/devextreme-angular/ui/file-uploader';
import { DxCheckBoxModule } from '@node_modules/devextreme-angular/ui/check-box';
import { DxNumberBoxModule } from '@node_modules/devextreme-angular/ui/number-box';
import { DxLoadPanelModule } from '@node_modules/devextreme-angular/ui/load-panel';
import { DxButtonModule } from '@node_modules/devextreme-angular/ui/button';
import { DxTreeListModule } from '@node_modules/devextreme-angular/ui/tree-list';
import { DxSelectBoxModule } from '@node_modules/devextreme-angular/ui/select-box';
import { DxDropDownBoxModule } from '@node_modules/devextreme-angular/ui/drop-down-box';
import { DxSchedulerModule } from '@node_modules/devextreme-angular/ui/scheduler';
import { DxPopupModule } from '@node_modules/devextreme-angular/ui/popup';
import { DxScrollViewModule } from '@node_modules/devextreme-angular/ui/scroll-view';
import { DxTextAreaModule } from '@node_modules/devextreme-angular/ui/text-area';
import { CreateOrEditSaasPricePackageModalComponent } from '@app/main/saaSpricePackages/saas-price-package/create-or-edit-saas-price-package-modal/create-or-edit-saas-price-package-modal.component';
import { SaasPricePackageComponent } from '@app/main/saaSpricePackages/saas-price-package/saas-price-package.component';

@NgModule({
  declarations: [SaasPricePackageComponent, CreateOrEditSaasPricePackageModalComponent],
  imports: [
    SaasPricePackageRoutingModule,
    UtilsModule,
    AppCommonModule,
    CommonModule,
    PaginatorModule,
    TableModule,
    ModalModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule,
    SplitButtonModule,
    MultiSelectModule,
    DxDataGridModule,
    DxPopoverModule,
    DxPopupModule,
    DxValidationGroupModule,
    DxDateBoxModule,
    DxValidatorModule,
    DxValidationSummaryModule,
    DxTextBoxModule,
    DxFileUploaderModule,
    DxCheckBoxModule,
    DxNumberBoxModule,
    DxLoadPanelModule,
    DxButtonModule,
    DxTreeListModule,
    DxSelectBoxModule,
    DxDropDownBoxModule,
    DxSchedulerModule,
    DxScrollViewModule,
    DxTextAreaModule,
  ],
})
export class SaasPricePackageModule {}
