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
import { FormsModule } from '@angular/forms';
import { RatingModule } from '@node_modules/primeng/rating';
import { UtilsModule } from '@shared/utils/utils.module';
import { PaginatorModule } from '@node_modules/primeng/paginator';
import { CarrierVasesComponent } from './tenants-profile/carrier-vases/carrier-vases.component';
import { CarrierFleetInformationComponent } from './tenants-profile/carrier-fleet-information/carrier-fleet-information.component';
import { ShipperFacilitiesComponent } from './tenants-profile/shipper-facilities/shipper-facilities.component';
import { ShipperInvoicingComponent } from './tenants-profile/shipper-invoicing/shipper-invoicing.component';
import { NgbRatingModule } from '@node_modules/@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    CompleteProfileComponent,
    TenantsProfileComponent,
    SideProfileComponent,
    ProfileComponent,
    CarrierVasesComponent,
    CarrierFleetInformationComponent,
    ShipperFacilitiesComponent,
    ShipperInvoicingComponent,
  ],
  imports: [
    CommonModule,
    ProfileRoutingModule,
    AgmCoreModule,
    AppModule,
    TableModule,
    FormsModule,
    RatingModule,
    UtilsModule,
    PaginatorModule,
    NgbRatingModule,
  ],
})
export class ProfileModule {}
