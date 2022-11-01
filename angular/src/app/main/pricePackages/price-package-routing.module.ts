import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NormalPricePackageComponent } from './normal-price-package/normal-price-package.component';
import { TmsPricePackageComponent } from '@app/main/pricePackages/tms-price-package/tms-price-package.component';
import { PricePackagesProposalComponent } from '@app/main/pricePackages/price-packeges-proposal/price-packages-proposal.component';
import {
    PricePackageAppendixComponent
} from '@app/main/pricePackages/price-package-appendix/price-package-appendix.component';

const routes: Routes = [
  { path: 'normalPricePackages', component: NormalPricePackageComponent, data: { permission: 'Pages.NormalPricePackages' } },
  { path: 'tmsPricePackages', component: TmsPricePackageComponent, data: { permission: 'Pages.TmsPricePackages' } },
  { path: 'pricePackagesProposal', component: PricePackagesProposalComponent, data: { permission: 'Pages.PricePackageProposal' } },
  { path: 'pricePackageAppendices', component: PricePackageAppendixComponent, data: { permission: 'Pages.PricePackageAppendix' } },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PricePackageRoutingModule {}
