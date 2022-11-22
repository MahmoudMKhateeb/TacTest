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
import { CarrierNormalPricePackagesComponent } from './tenants-profile/carrier-normal-price-package/carrier-normal-price-package.component';
import { CarrierVasesComponent } from './tenants-profile/carrier-vases/carrier-vases.component';
import { CarrierFleetInformationComponent } from './tenants-profile/carrier-fleet-information/carrier-fleet-information.component';
import { ShipperFacilitiesComponent } from './tenants-profile/shipper-facilities/shipper-facilities.component';
import { ShipperInvoicingComponent } from './tenants-profile/shipper-invoicing/shipper-invoicing.component';
import { NgbRatingModule } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { TooltipModule } from '@node_modules/ngx-bootstrap/tooltip';
import { CarrierServiceAreasComponent } from './tenants-profile/carrier-service-areas/carrier-service-areas.component';
import { AccordionModule } from '@node_modules/primeng/accordion';
import { MultiSelectModule } from '@node_modules/primeng/multiselect';

@NgModule({
  declarations: [
    CompleteProfileComponent,
    TenantsProfileComponent,
    SideProfileComponent,
    ProfileComponent,
    CarrierNormalPricePackagesComponent,
    CarrierVasesComponent,
    CarrierFleetInformationComponent,
    ShipperFacilitiesComponent,
    ShipperInvoicingComponent,
    CarrierServiceAreasComponent,
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
    TooltipModule,
    AccordionModule,
    MultiSelectModule,
  ],
})
export class ProfileModule {}
