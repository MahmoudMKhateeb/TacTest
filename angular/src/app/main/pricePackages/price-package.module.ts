import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PricePackageRoutingModule } from './price-package-routing.module';
import { TableModule } from 'primeng/table';
import { UtilsModule } from '@shared/utils/utils.module';
import { PaginatorModule } from 'primeng/paginator';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { DevExtremeModule, DxButtonModule } from '@node_modules/devextreme-angular';
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
    DxButtonModule,
    DevExtremeModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule,
    SplitButtonModule,
    MultiSelectModule,
  ],
  exports: [ViewMatchingPricePackageComponent, NormalPricePackageCalculationComponent],
})
export class PricePackageModule {}
