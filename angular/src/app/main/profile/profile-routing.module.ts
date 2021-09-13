import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompleteProfileComponent } from '@app/main/profile/complete-profile/complete-profile.component';
import { TenantsProfileComponent } from '@app/main/profile/tenants-profile/tenants-profile.component';
import { ProfileComponent } from '@app/main/profile/profile/profile.component';

const routes: Routes = [
  {
    path: ':id',
    component: ProfileComponent,
    children: [
      { path: 'complete', component: CompleteProfileComponent },
      { path: 'view', component: TenantsProfileComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
