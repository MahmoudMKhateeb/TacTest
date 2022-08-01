import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NormalPricePackageComponent } from '../normal-price-package/normal-price-package.component';

const routes: Routes = [{ path: '', component: NormalPricePackageComponent, data: { permission: 'Pages.NormalPricePackages' } }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PricePackageModuleRoutingModule {}
