import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PricePackageComponent } from '@app/main/pricePackages/price-package/price-package.component';
import { PricePackagesProposalComponent } from '@app/main/pricePackages/price-packeges-proposal/price-packages-proposal.component';
import { PricePackageAppendixComponent } from '@app/main/pricePackages/price-package-appendix/price-package-appendix.component';

const routes: Routes = [
  { path: 'pricePackages', component: PricePackageComponent, data: { permission: 'Pages.PricePackages' } },
  {
    path: 'pricePackagesProposal',
    component: PricePackagesProposalComponent,
    data: { permission: 'Pages.PricePackageProposal' },
  },
  {
    path: 'pricePackageAppendices',
    component: PricePackageAppendixComponent,
    data: { permission: 'Pages.PricePackageAppendix' },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PricePackageRoutingModule {}
