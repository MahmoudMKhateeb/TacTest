import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from '@app/main/profile/profile-routing.module';
import { CompleteProfileComponent } from './complete-profile/complete-profile.component';
import { TenantsProfileComponent } from './tenants-profile/tenants-profile.component';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { SideProfileComponent } from './side-profile/side-profile.component';

@NgModule({
  declarations: [CompleteProfileComponent, TenantsProfileComponent, SideProfileComponent],
  imports: [CommonModule, ProfileRoutingModule, AgmCoreModule],
})
export class ProfileModule {}
