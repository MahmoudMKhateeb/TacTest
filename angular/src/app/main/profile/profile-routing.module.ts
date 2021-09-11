import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompleteProfileComponent } from '@app/main/profile/complete-profile/complete-profile.component';
import { SideProfileComponent } from '@app/main/profile/side-profile/side-profile.component';

const routes: Routes = [
  {
    path: '',
    component: CompleteProfileComponent,
    children: [
      { path: 'completeProfile', component: CompleteProfileComponent },
      { path: '', redirectTo: 'overview', pathMatch: 'full' },
      { path: '**', redirectTo: 'overview', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
