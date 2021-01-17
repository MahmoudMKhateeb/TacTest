import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CitiesTranslationsComponent } from './citiesTranslations/citiesTranslations/citiesTranslations.component';
import { CountriesTranslationsComponent } from './countriesTranslations/countriesTranslations/countriesTranslations.component';
import { PlateTypesComponent } from './plateTypes/plateTypes/plateTypes.component';
import { TruckCapacitiesTranslationsComponent } from './truckCapacitiesTranslations/truckCapacitiesTranslations/truckCapacitiesTranslations.component';
import { TruckStatusesTranslationsComponent } from './truckStatusesTranslations/truckStatusesTranslations/truckStatusesTranslations.component';
import { TripStatusesComponent } from './tripStatuses/tripStatuses/tripStatuses.component';
import { PackingTypesComponent } from './packingTypes/packingTypes/packingTypes.component';
import { ShippingTypesComponent } from './shippingTypes/shippingTypes/shippingTypes.component';
import { NationalitiesComponent } from './nationalities/nationalities/nationalities.component';
import { NationalityTranslationsComponent } from './nationalitiesTranslation/nationalityTranslations/nationalityTranslations.component';
import { TrucksTypesTranslationsComponent } from './trucksTypesTranslations/trucksTypesTranslations/trucksTypesTranslations.component';
import { TransportTypesTranslationsComponent } from './transportTypesTranslations/transportTypesTranslations/transportTypesTranslations.component';
import { VasPricesComponent } from './vases/vasPrices/vasPrices.component';
import { TermAndConditionsComponent } from './termsAndConditions/termAndConditions/termAndConditions.component';
import { CapacitiesComponent } from './truckCapacities/capacities/capacities.component';
import { TransportTypesComponent } from './transportTypes/transportTypes/transportTypes.component';
import { DocumentTypeTranslationsComponent } from './documentTypeTranslations/documentTypeTranslations/documentTypeTranslations.component';
import { DocumentsEntitiesComponent } from './documentsEntities/documentsEntities/documentsEntities.component';
import { PickingTypesComponent } from './pickingTypes/pickingTypes/pickingTypes.component';
import { PortsComponent } from './ports/ports/ports.component';
import { FacilitiesComponent } from './addressBook/facilities/facilities.component';
import { DocumentFilesComponent } from './documentFiles/documentFiles/documentFiles.component';
import { DocumentTypesComponent } from './documentTypes/documentTypes/documentTypes.component';
import { ShippingRequestsComponent } from './shippingRequests/shippingRequests/shippingRequests.component';
import { CreateOrEditShippingRequestComponent } from './shippingRequests/shippingRequests/create-or-edit-shippingRequest.component';
import { ViewShippingRequestComponent } from './shippingRequests/shippingRequests/view-shippingRequest.component';
import { GoodsDetailsComponent } from './goodsDetails/goodsDetails/goodsDetails.component';
import { OffersComponent } from './offers/offers/offers.component';
import { RoutStepsComponent } from './routSteps/routSteps/routSteps.component';
import { RoutesComponent } from './routs/routes/routes.component';
import { CreateOrEditRouteComponent } from './routs/routes/create-or-edit-route.component';
import { ViewRouteComponent } from './routs/routes/view-route.component';
import { CitiesComponent } from './cities/cities/cities.component';
import { CountiesComponent } from './countries/counties/counties.component';
import { RoutTypesComponent } from './routTypes/routTypes/routTypes.component';
import { GoodCategoriesComponent } from './goodCategories/goodCategories/goodCategories.component';
import { TrailersComponent } from './trailers/trailers/trailers.component';
import { TrailerStatusesComponent } from './trailerStatuses/trailerStatuses/trailerStatuses.component';
import { PayloadMaxWeightsComponent } from './payloadMaxWeight/payloadMaxWeights/payloadMaxWeights.component';
import { TrailerTypesComponent } from './trailerTypes/trailerTypes/trailerTypes.component';
import { TrucksComponent } from './trucks/trucks/trucks.component';
import { TrucksTypesComponent } from './trucksTypes/trucksTypes/trucksTypes.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MarketplaceComponent } from '@app/main/marketPlace/marketPlace/marketplace.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        children: [
                    { path: 'citiesTranslations/citiesTranslations', component: CitiesTranslationsComponent, data: { permission: 'Pages.CitiesTranslations' }  },
                    { path: 'countriesTranslations/countriesTranslations', component: CountriesTranslationsComponent, data: { permission: 'Pages.CountriesTranslations' }  },
          { path: 'plateTypes/plateTypes', component: PlateTypesComponent, data: { permission: 'Pages.Capacities' } },
          { path: 'nationalities/nationalities', component: NationalitiesComponent, data: { permission: 'Pages.Nationalities' } },
          {
            path: 'nationalitiesTranslation/nationalityTranslations',
            component: NationalityTranslationsComponent,
            data: { permission: 'Pages.NationalityTranslations' },
          },
          {
            path: 'transportTypesTranslations/transportTypesTranslations',
            component: TransportTypesTranslationsComponent,
            data: { permission: 'Pages.TransportTypesTranslations' },
          },
                    { path: 'truckCapacitiesTranslations/truckCapacitiesTranslations', component: TruckCapacitiesTranslationsComponent, data: { permission: 'Pages.TruckCapacitiesTranslations' }  },
                    { path: 'truckStatusesTranslations/truckStatusesTranslations', component: TruckStatusesTranslationsComponent, data: { permission: 'Pages.TruckStatusesTranslations' }  },
                    { path: 'nationalities/nationalities', component: NationalitiesComponent, data: { permission: 'Pages.Nationalities' }  },
                    { path: 'nationalitiesTranslation/nationalityTranslations', component: NationalityTranslationsComponent, data: { permission: 'Pages.NationalityTranslations' }  },
                    { path: 'transportTypesTranslations/transportTypesTranslations', component: TransportTypesTranslationsComponent, data: { permission: 'Pages.TransportTypesTranslations' }  },
          { path: 'tripStatuses/tripStatuses', component: TripStatusesComponent, data: { permission: 'Pages.TripStatuses' } },
          { path: 'packingTypes/packingTypes', component: PackingTypesComponent, data: { permission: 'Pages.PackingTypes' } },
          { path: 'shippingTypes/shippingTypes', component: ShippingTypesComponent, data: { permission: 'Pages.ShippingTypes' } },
          { path: 'nationalities/nationalities', component: NationalitiesComponent, data: { permission: 'Pages.Nationalities' } },
          {
            path: 'nationalitiesTranslation/nationalityTranslations',
            component: NationalityTranslationsComponent,
            data: { permission: 'Pages.NationalityTranslations' },
          },
          {
            path: 'transportTypesTranslations/transportTypesTranslations',
            component: TransportTypesTranslationsComponent,
            data: { permission: 'Pages.TransportTypesTranslations' },
          },
          {
            path: 'trucksTypesTranslations/trucksTypesTranslations',
            component: TrucksTypesTranslationsComponent,
            data: { permission: 'Pages.TrucksTypesTranslations' },
          },
          {
            path: 'transportTypesTranslations/transportTypesTranslations',
            component: TransportTypesTranslationsComponent,
            data: { permission: 'Pages.TransportTypesTranslations' },
          },
          { path: 'vases/vasPrices', component: VasPricesComponent, data: { permission: 'Pages.VasPrices' } },
          { path: 'termsAndConditions/termAndConditions', component: TermAndConditionsComponent, data: { permission: 'Pages.TermAndConditions' } },
          { path: 'truckCapacities/capacities', component: CapacitiesComponent, data: { permission: 'Pages.Capacities' } },
          { path: 'transportTypes/transportTypes', component: TransportTypesComponent, data: { permission: 'Pages.TransportTypes' } },
          {
            path: 'documentTypeTranslations/documentTypeTranslations',
            component: DocumentTypeTranslationsComponent,
            data: { permission: 'Pages.DocumentTypeTranslations' },
          },
          { path: 'documentsEntities/documentsEntities', component: DocumentsEntitiesComponent, data: { permission: 'Pages.DocumentsEntities' } },
          { path: 'ports/ports', component: PortsComponent, data: { permission: 'Pages.Ports' } },
          { path: 'pickingTypes/pickingTypes', component: PickingTypesComponent, data: { permission: 'Pages.PickingTypes' } },
          { path: 'addressBook/facilities', component: FacilitiesComponent, data: { permission: 'Pages.Facilities' } },
          { path: 'documentFiles/documentFiles', component: DocumentFilesComponent, data: { permission: 'Pages.DocumentFiles' } },
          { path: 'documentTypes/documentTypes', component: DocumentTypesComponent, data: { permission: 'Pages.DocumentTypes' } },
          { path: 'shippingRequests/shippingRequests', component: ShippingRequestsComponent, data: { permission: 'Pages.ShippingRequests' } },
          {
            path: 'shippingRequests/shippingRequests/createOrEdit',
            component: CreateOrEditShippingRequestComponent,
            data: { permission: 'Pages.ShippingRequests.Create' },
          },
          { path: 'shippingRequests/shippingRequests/view', component: ViewShippingRequestComponent, data: { permission: 'Pages.ShippingRequests' } },
          { path: 'marketPlace/marketPlace', component: MarketplaceComponent },
          { path: 'offers/offers', component: OffersComponent, data: { permission: 'Pages.Offers' } },
          { path: 'routSteps/routSteps', component: RoutStepsComponent, data: { permission: 'Pages.RoutSteps' } },
          { path: 'routs/routes', component: RoutesComponent, data: { permission: 'Pages.Routes' } },
          { path: 'routs/routes/createOrEdit', component: CreateOrEditRouteComponent, data: { permission: 'Pages.Routes.Create' } },
          { path: 'routs/routes/view', component: ViewRouteComponent, data: { permission: 'Pages.Routes' } },
          { path: 'cities/cities', component: CitiesComponent, data: { permission: 'Pages.Cities' } },
          { path: 'countries/counties', component: CountiesComponent, data: { permission: 'Pages.Counties' } },
          { path: 'routTypes/routTypes', component: RoutTypesComponent, data: { permission: 'Pages.RoutTypes' } },
          { path: 'goodCategories/goodCategories', component: GoodCategoriesComponent, data: { permission: 'Pages.GoodCategories' } },
          { path: 'trailers/trailers', component: TrailersComponent, data: { permission: 'Pages.Trailers' } },
          { path: 'trailerStatuses/trailerStatuses', component: TrailerStatusesComponent, data: { permission: 'Pages.TrailerStatuses' } },
          { path: 'payloadMaxWeight/payloadMaxWeights', component: PayloadMaxWeightsComponent, data: { permission: 'Pages.PayloadMaxWeights' } },
          { path: 'trailerTypes/trailerTypes', component: TrailerTypesComponent, data: { permission: 'Pages.TrailerTypes' } },
          { path: 'trucks/trucks', component: TrucksComponent, data: { permission: 'Pages.Trucks' } },
          { path: 'trucksTypes/trucksTypes', component: TrucksTypesComponent, data: { permission: 'Pages.TrucksTypes' } },
          { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: '**', redirectTo: 'dashboard' },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class MainRoutingModule {}
