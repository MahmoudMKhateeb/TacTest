import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DriverLicenseTypesComponent } from './driverLicenseTypes/driverLicenseTypes/driverLicenseTypes.component';
import { EmailTemplatesComponent } from './emailTemplates/emailTemplates/emailTemplates.component';
import { DangerousGoodTypesComponent } from './goods/dangerousGoodTypes/dangerousGoodTypes.component';
import { PackingTypesComponent } from './packingTypes/packingTypes/packingTypes.component';
import { ShippingTypesComponent } from './shippingTypes/shippingTypes/shippingTypes.component';
import { CitiesTranslationsComponent } from './citiesTranslations/citiesTranslations/citiesTranslations.component';
import { CountriesTranslationsComponent } from './countriesTranslations/countriesTranslations/countriesTranslations.component';
import { PlateTypesComponent } from './plateTypes/plateTypes/plateTypes.component';
import { TruckCapacitiesTranslationsComponent } from './truckCapacitiesTranslations/truckCapacitiesTranslations/truckCapacitiesTranslations.component';
import { TruckStatusesTranslationsComponent } from './truckStatusesTranslations/truckStatusesTranslations/truckStatusesTranslations.component';
import { NationalitiesComponent } from './nationalities/nationalities/nationalities.component';
import { NationalityTranslationsComponent } from './nationalitiesTranslation/nationalityTranslations/nationalityTranslations.component';
import { TrucksTypesTranslationsComponent } from './trucksTypesTranslations/trucksTypesTranslations/trucksTypesTranslations.component';
import { TransportTypesTranslationsComponent } from './transportTypesTranslations/transportTypesTranslations/transportTypesTranslations.component';
import { VasPricesComponent } from './vases/vasPrices/vasPrices.component';
import { ReceiversComponent } from './receivers/receivers/receivers.component';
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
import { CreateOrEditShippingRequestComponent } from './shippingRequests/shippingRequests/create-or-edit-shippingRequest.component';
import { ViewShippingRequestComponent } from './shippingRequests/shippingRequests/view-shippingRequest.component';
//import { OffersComponent } from './offers/offers/offers.component';
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
import { MarketPlaceListComponent } from '@app/main/marketplaces/marketPlacelist.component';
import { ShippingRequestsListComponent } from '@app/main/shippingRequests/shippingRequestslist.component';
import { DirectRequestViewComponent } from '@app/main/directrequests/direct-request-view.component';
import { OffersListComponent } from '@app/main/offer/offers.component';
import { InvoiceDetailResolverService } from '@app/main/invoices/invoice-detail/Invoice-detail-resolver.service';
import { BalancesListComponent } from '@app/main/invoices/balances/balances-list/balances-list.component';
import { InvoiceTenantComponent } from '@app/main/invoices/invoice-tenants/invoice-tenant.component';
import { InvoiceTenantDetailsComponent } from '@app/main/invoices/invoice-tenants/invoice-tenant-details.component';
import { InvoiceDetailComponent } from '@app/main/invoices/invoice-detail/invoice-detail.component';
import { InvoicesListComponent } from '@app/main/invoices/invoices-list/invoices-list.component';
import { InvoicePeriodsListComponent } from '@app/main/invoices/invoice-periods-list/invoice-periods-list.component';
import { InvoicePaymentMethodComponent } from '@app/main/invoices/invoice-payment-methods/invoice-payment-method.component';
import { TransactionListComponent } from '@app/main/invoices/transaction/transaction-list/transaction-list.component';
import { InvoiceTenantDetailsService } from '@app/main/invoices/invoice-tenants/invoice-tenant-details.service';
import { ProformaListComponent } from '@app/main/invoices/proformas/proforma-list.component';
import { AccidentReasonComponent } from '@app/main/accidents/reasons/reason.component';
import { TripRejectReasonComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/rejectreason/trip-reject-reason.component';
import { ComingSoonComponent } from '@app/main/commingSoon/comingSoon.component';
import { CreateOrEditShippingRequestWizardComponent } from '@app/main/shippingRequests/shippingRequests/shippingRequestWizard/create-or-edit-shipping-request-wizard.component';
import { TMSRequestListComponent } from '@app/main/tms/tms-request-list.component';
import { TrucksSubmittedDocumentsComponent } from '@app/main/documentFiles/documentFiles/trucks-submitted-documents/trucks-submitted-documents.component';
import { DriversSubmittedDocumentsComponent } from '@app/main/documentFiles/documentFiles/drivers-submitted-documents/drivers-submitted-documents.component';
import { ShipmentHistoryComponent } from '@app/main/shippingRequests/shippingRequests/shipment-history/shipment-history.component';
import { RequestTemplatesComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/request-templates.component';
import { PenaltiesListComponent } from './Penalties/penalties-list/penalties-list.component';
import { TrackingComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tracking.component';
import { InvoiceNoteListComponent } from './invoices/invoice-note/invoice-note-list/invoice-note-list.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: '',
        children: [
          { path: 'emailTemplates/emailTemplates', component: EmailTemplatesComponent, data: { permission: 'Pages.EmailTemplates' } },
          { path: 'driverLicenseTypes/driverLicenseTypes', component: DriverLicenseTypesComponent, data: { permission: 'Pages.DriverLicenseTypes' } },
          { path: 'goods/dangerousGoodTypes', component: DangerousGoodTypesComponent, data: { permission: 'Pages.DangerousGoodTypes' } },
          {
            path: 'truckCapacitiesTranslations/truckCapacitiesTranslations',
            component: TruckCapacitiesTranslationsComponent,
            data: { permission: 'Pages.TruckCapacitiesTranslations' },
          },
          {
            path: 'truckStatusesTranslations/truckStatusesTranslations',
            component: TruckStatusesTranslationsComponent,
            data: { permission: 'Pages.TruckStatusesTranslations' },
          },
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
          { path: 'citiesTranslations/citiesTranslations', component: CitiesTranslationsComponent, data: { permission: 'Pages.CitiesTranslations' } },
          {
            path: 'countriesTranslations/countriesTranslations',
            component: CountriesTranslationsComponent,
            data: { permission: 'Pages.CountriesTranslations' },
          },
          { path: 'plateTypes/plateTypes', component: PlateTypesComponent, data: { permission: 'Pages.Capacities' } },
          { path: 'citiesTranslations/citiesTranslations', component: CitiesTranslationsComponent, data: { permission: 'Pages.CitiesTranslations' } },
          {
            path: 'countriesTranslations/countriesTranslations',
            component: CountriesTranslationsComponent,
            data: { permission: 'Pages.CountriesTranslations' },
          },
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
          {
            path: 'truckCapacitiesTranslations/truckCapacitiesTranslations',
            component: TruckCapacitiesTranslationsComponent,
            data: { permission: 'Pages.TruckCapacitiesTranslations' },
          },
          {
            path: 'truckStatusesTranslations/truckStatusesTranslations',
            component: TruckStatusesTranslationsComponent,
            data: { permission: 'Pages.TruckStatusesTranslations' },
          },
          {
            path: 'truckCapacitiesTranslations/truckCapacitiesTranslations',
            component: TruckCapacitiesTranslationsComponent,
            data: { permission: 'Pages.TruckCapacitiesTranslations' },
          },
          {
            path: 'truckStatusesTranslations/truckStatusesTranslations',
            component: TruckStatusesTranslationsComponent,
            data: { permission: 'Pages.TruckStatusesTranslations' },
          },
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
          { path: 'citiesTranslations/citiesTranslations', component: CitiesTranslationsComponent, data: { permission: 'Pages.CitiesTranslations' } },
          {
            path: 'countriesTranslations/countriesTranslations',
            component: CountriesTranslationsComponent,
            data: { permission: 'Pages.CountriesTranslations' },
          },
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
          { path: 'receivers/receivers', component: ReceiversComponent, data: { permission: 'Pages.Receivers' } },
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
          {
            path: 'documentFiles/TrucksSubmittedDocuments',
            component: TrucksSubmittedDocumentsComponent,
            data: { permission: 'Pages.DocumentFiles' },
          },
          {
            path: 'documentFiles/DriversSubmittedDocuments',
            component: DriversSubmittedDocumentsComponent,
            data: { permission: 'Pages.DocumentFiles' },
          },
          { path: 'documentTypes/documentTypes', component: DocumentTypesComponent, data: { permission: 'Pages.DocumentTypes' } },
          { path: 'shippingRequests/shippingRequests', component: ShippingRequestsListComponent },
          {
            path: 'shippingRequests/ShipmentHistory',
            component: ShipmentHistoryComponent,

            data: { permission: 'Pages.ShippingRequests' },
          },
          { path: 'tms/shippingRequests', component: TMSRequestListComponent },
          {
            path: 'shippingRequests/shippingRequests/createOrEdit',
            component: CreateOrEditShippingRequestComponent,
            data: { permission: 'Pages.ShippingRequests' },
          },
          {
            path: 'shippingRequests/shippingRequestWizard',
            component: CreateOrEditShippingRequestWizardComponent,
            data: { permission: 'Pages.ShippingRequests' },
          },
          {
            path: 'shippingRequests/requestsTemplates',
            component: RequestTemplatesComponent,
          },

          { path: 'shippingRequests/shippingRequests/view', component: ViewShippingRequestComponent, data: { permission: 'Pages.ShippingRequests' } },
          { path: 'marketplace/list', component: MarketPlaceListComponent },
          {
            path: 'tracking',
            loadChildren: () => import('@app/main/shippingRequests/shippingRequests/tracking/tracking.module').then((m) => m.TrackingModule), //Lazy load main module
          },
          { path: 'directrequest/list', component: DirectRequestViewComponent },
          { path: 'offers', component: OffersListComponent },
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
          { path: 'invoices/view', component: InvoicesListComponent, data: { permission: 'Pages.Invoices' } },
          { path: 'penalties/view', component: PenaltiesListComponent, data: { permission: 'Pages.Invoices' } },

          { path: 'invoicenote/view', component: InvoiceNoteListComponent, data: { permission: 'Pages.Invoices' } },

          {
            path: 'invoices/detail/:id',
            component: InvoiceDetailComponent,
            data: { permission: 'Pages.Invoices' },
            resolve: {
              invoiceinfo: InvoiceDetailResolverService,
            },
          },
          { path: 'invoices/periods', component: InvoicePeriodsListComponent, data: { permission: 'Pages.Administration.Host.Invoices.Periods' } },
          {
            path: 'invoices/paymentlist',
            component: InvoicePaymentMethodComponent,
            data: { permission: 'Pages.Administration.Host.Invoices.PaymentMethods' },
          },

          { path: 'invoices/transaction', component: TransactionListComponent, data: { permission: 'Pages.Invoices.Transaction' } },
          { path: 'invoices/proformas', component: ProformaListComponent },
          { path: 'invoices/submitinvoice', component: InvoiceTenantComponent, data: { permission: 'Pages.Invoices.SubmitInvoices' } },
          {
            path: 'invoices/balnacerecharges',
            component: BalancesListComponent,
            data: { permission: 'Pages.Administration.Host.Invoices.Balances' },
          },
          {
            path: 'invoices/submitinvoices/detail/:id',
            component: InvoiceTenantDetailsComponent,
            data: { permission: 'Pages.Invoices.SubmitInvoices' },
            resolve: {
              invoiceinfo: InvoiceTenantDetailsService,
            },
          },
          { path: 'accidents/reasons', component: AccidentReasonComponent, data: { permission: 'Pages.ShippingRequestResoneAccidents' } },
          { path: 'trip/reject/reasons', component: TripRejectReasonComponent, data: { permission: 'Pages.ShippingRequestTrips.Reject.Reason' } },
          // { path: 'lanaguages/applocalizations', component: AppLocalizationComponent, data: { permission: 'Pages.AppLocalizations' } },

          {
            path: 'profile',
            loadChildren: () => import('app/main/profile/profile.module').then((m) => m.ProfileModule), //Lazy load main module
            data: { preload: true },
          },
          {
            path: 'pricePackages/normalPricePackages',
            loadChildren: () =>
              import('app/main/pricePackages/price-package-module/price-package-module.module').then((m) => m.PricePackageModuleModule), //Lazy load main module
            data: { preload: true },
          },
          { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
          //TODO:// to be removed after menu Structure work is complete
          //
          { path: 'page-not-found', component: ComingSoonComponent },
          //
          { path: '', redirectTo: 'page-not-found' },
          { path: '**', redirectTo: 'page-not-found' },
        ],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class MainRoutingModule {}
