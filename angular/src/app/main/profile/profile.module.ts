import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from '@app/main/profile/profile-routing.module';
import { CompleteProfileComponent } from './complete-profile/complete-profile.component';
import { TenantsProfileComponent } from './tenants-profile/tenants-profile.component';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { SideProfileComponent } from './side-profile/side-profile.component';
import { AppModule } from '@app/app.module';
import { ProfileComponent } from './profile/profile.component';
import { TableModule } from '@node_modules/primeng/table';

@NgModule({
  declarations: [CompleteProfileComponent, TenantsProfileComponent, SideProfileComponent, ProfileComponent],
  imports: [CommonModule, ProfileRoutingModule, AgmCoreModule, AppModule, TableModule],
})
export class ProfileModule {}
