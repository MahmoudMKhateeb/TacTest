import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SaasPricePackageComponent } from '@app/main/saaSpricePackages/saas-price-package/saas-price-package.component';

const routes: Routes = [{ path: 'saaSpricePackages', component: SaasPricePackageComponent, data: { permission: 'Pages.PricePackages' } }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SaasPricePackageRoutingModule {}
