import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PricePackageModuleRoutingModule } from './price-package-module-routing.module';
import { ViewNormalPricePackageModalComponent } from '../normal-price-package/view-normal-price-package-modal.component';
import { NormalPricePackageComponent } from '../normal-price-package/normal-price-package.component';
import { CreateOrEditNormalPricePackageModalComponent } from '../normal-price-package/create-or-edit-normal-price-package-modal.component';
import { TableModule } from 'primeng/table';
import { UtilsModule } from '@shared/utils/utils.module';
import { PaginatorModule } from 'primeng/paginator';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@NgModule({
  declarations: [ViewNormalPricePackageModalComponent, NormalPricePackageComponent, CreateOrEditNormalPricePackageModalComponent],
  imports: [
    PricePackageModuleRoutingModule,
    UtilsModule,
    AppCommonModule,
    CommonModule,
    PaginatorModule,
    TableModule,
    ModalModule,
    BsDropdownModule.forRoot(),
  ],
})
export class PricePackageModuleModule {}
