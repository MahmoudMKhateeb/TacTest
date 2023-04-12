import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PricePackageRoutingModule } from './price-package-routing.module';
import { TableModule } from 'primeng/table';
import { UtilsModule } from '@shared/utils/utils.module';
import { PaginatorModule } from 'primeng/paginator';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PricePackageComponent } from '@app/main/pricePackages/price-package/price-package.component';
import { CreateOrEditPricePackageModalComponent } from '@app/main/pricePackages/price-package/create-or-edit-price-package-modal/create-or-edit-price-package-modal.component';
import { PricePackagesProposalComponent } from '@app/main/pricePackages/price-packeges-proposal/price-packages-proposal.component';
import { CreateOrEditPricePackegeProposalComponent } from './price-packeges-proposal/create-or-edit-price-packege-proposal/create-or-edit-price-packege-proposal.component';
import { BsDatepickerModule } from '@node_modules/ngx-bootstrap/datepicker';
import { ViewPricePackageProposalComponent } from './price-packeges-proposal/view-price-package-proposal/view-price-package-proposal.component';
import { PricePackageAppendixComponent } from './price-package-appendix/price-package-appendix.component';
import { CreateOrEditPricePackageAppendixComponent } from './price-package-appendix/create-or-edit-price-package-appendix/create-or-edit-price-package-appendix.component';
import { ViewPricePackageAppendixComponent } from './price-package-appendix/view-price-package-appendix/view-price-package-appendix.component';
import { SplitButtonModule } from '@node_modules/primeng/splitbutton';
import { ViewMatchingPricePackageComponent } from './price-package/view-matching-price-package/view-matching-price-package.component';
import { ProposalDetailGridComponent } from './price-packeges-proposal/proposal-detail-grid/proposal-detail-grid.component';
import { AppendixDetailGridComponent } from './price-package-appendix/appendix-detail-grid/appendix-detail-grid.component';
import { MultiSelectModule } from '@node_modules/primeng/multiselect';
import { NormalPricePackageCalculationComponent } from '@app/main/shippingRequests/shippingRequests/normal-price-packages/normal-price-package-calculation/normal-price-package-calculation-component';

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

@NgModule({
  declarations: [
    NormalPricePackageCalculationComponent,
    PricePackageComponent,
    CreateOrEditPricePackageModalComponent,
    PricePackagesProposalComponent,
    CreateOrEditPricePackegeProposalComponent,
    ViewPricePackageProposalComponent,
    PricePackageAppendixComponent,
    CreateOrEditPricePackageAppendixComponent,
    ViewPricePackageAppendixComponent,
    ViewMatchingPricePackageComponent,
    ProposalDetailGridComponent,
    AppendixDetailGridComponent,
  ],
  imports: [
    PricePackageRoutingModule,
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
  exports: [ViewMatchingPricePackageComponent, NormalPricePackageCalculationComponent],
})
export class PricePackageModule {}
