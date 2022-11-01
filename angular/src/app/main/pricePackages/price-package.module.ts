import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PricePackageRoutingModule } from './price-package-routing.module';
import { ViewNormalPricePackageModalComponent } from './normal-price-package/view-normal-price-package-modal.component';
import { NormalPricePackageComponent } from './normal-price-package/normal-price-package.component';
import { CreateOrEditNormalPricePackageModalComponent } from './normal-price-package/create-or-edit-normal-price-package-modal.component';
import { TableModule } from 'primeng/table';
import { UtilsModule } from '@shared/utils/utils.module';
import { PaginatorModule } from 'primeng/paginator';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { DevExtremeModule, DxButtonModule } from '@node_modules/devextreme-angular';
import { TmsPricePackageComponent } from '@app/main/pricePackages/tms-price-package/tms-price-package.component';
import { CreateTmsPricePackageModalComponent } from '@app/main/pricePackages/tms-price-package/create-tms-price-package-modal/create-tms-price-package-modal.component';
import { PricePackagesProposalComponent } from '@app/main/pricePackages/price-packeges-proposal/price-packages-proposal.component';
import { CreateOrEditPricePackegeProposalComponent } from './price-packeges-proposal/create-or-edit-price-packege-proposal/create-or-edit-price-packege-proposal.component';
import { BsDatepickerModule } from '@node_modules/ngx-bootstrap/datepicker';
import { ViewPricePackageProposalComponent } from './price-packeges-proposal/view-price-package-proposal/view-price-package-proposal.component';
import { PricePackageAppendixComponent } from './price-package-appendix/price-package-appendix.component';
import { CreateOrEditPricePackageAppendixComponent } from './price-package-appendix/create-or-edit-price-package-appendix/create-or-edit-price-package-appendix.component';
import { ViewPricePackageAppendixComponent } from './price-package-appendix/view-price-package-appendix/view-price-package-appendix.component';

@NgModule({
  declarations: [
    ViewNormalPricePackageModalComponent,
    NormalPricePackageComponent,
    CreateOrEditNormalPricePackageModalComponent,
    TmsPricePackageComponent,
    CreateTmsPricePackageModalComponent,
    PricePackagesProposalComponent,
    CreateOrEditPricePackegeProposalComponent,
    ViewPricePackageProposalComponent,
    PricePackageAppendixComponent,
    CreateOrEditPricePackageAppendixComponent,
    ViewPricePackageAppendixComponent,
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
  ],
})
export class PricePackageModule {}
