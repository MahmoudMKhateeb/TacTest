import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';

import { PackingTypesComponent } from './packingTypes/packingTypes/packingTypes.component';
import { ViewPackingTypeModalComponent } from './packingTypes/packingTypes/view-packingType-modal.component';
import { CreateOrEditPackingTypeModalComponent } from './packingTypes/packingTypes/create-or-edit-packingType-modal.component';

import { ShippingTypesComponent } from './shippingTypes/shippingTypes/shippingTypes.component';
import { ViewShippingTypeModalComponent } from './shippingTypes/shippingTypes/view-shippingType-modal.component';
import { CreateOrEditShippingTypeModalComponent } from './shippingTypes/shippingTypes/create-or-edit-shippingType-modal.component';

import { TruckCapacitiesTranslationsComponent } from './truckCapacitiesTranslations/truckCapacitiesTranslations/truckCapacitiesTranslations.component';
import { ViewTruckCapacitiesTranslationModalComponent } from './truckCapacitiesTranslations/truckCapacitiesTranslations/view-truckCapacitiesTranslation-modal.component';
import { CreateOrEditTruckCapacitiesTranslationModalComponent } from './truckCapacitiesTranslations/truckCapacitiesTranslations/create-or-edit-truckCapacitiesTranslation-modal.component';

import { TruckStatusesTranslationsComponent } from './truckStatusesTranslations/truckStatusesTranslations/truckStatusesTranslations.component';
import { ViewTruckStatusesTranslationModalComponent } from './truckStatusesTranslations/truckStatusesTranslations/view-truckStatusesTranslation-modal.component';
import { CreateOrEditTruckStatusesTranslationModalComponent } from './truckStatusesTranslations/truckStatusesTranslations/create-or-edit-truckStatusesTranslation-modal.component';

import { CitiesTranslationsComponent } from './citiesTranslations/citiesTranslations/citiesTranslations.component';
import { ViewCitiesTranslationModalComponent } from './citiesTranslations/citiesTranslations/view-citiesTranslation-modal.component';
import { CreateOrEditCitiesTranslationModalComponent } from './citiesTranslations/citiesTranslations/create-or-edit-citiesTranslation-modal.component';

import { CountriesTranslationsComponent } from './countriesTranslations/countriesTranslations/countriesTranslations.component';
import { ViewCountriesTranslationModalComponent } from './countriesTranslations/countriesTranslations/view-countriesTranslation-modal.component';
import { CreateOrEditCountriesTranslationModalComponent } from './countriesTranslations/countriesTranslations/create-or-edit-countriesTranslation-modal.component';

import { PlateTypesComponent } from './plateTypes/plateTypes/plateTypes.component';
import { ViewPlateTypeModalComponent } from './plateTypes/plateTypes/view-plateType-modal.component';
import { CreateOrEditPlateTypeModalComponent } from './plateTypes/plateTypes/create-or-edit-plateType-modal.component';

import { NationalitiesComponent } from './nationalities/nationalities/nationalities.component';
import { ViewNationalityModalComponent } from './nationalities/nationalities/view-nationality-modal.component';
import { CreateOrEditNationalityModalComponent } from './nationalities/nationalities/create-or-edit-nationality-modal.component';

import { MasterDetailChild_Nationality_NationalityTranslationsComponent } from './nationalitiesTranslation/nationalityTranslations/masterDetailChild_Nationality_nationalityTranslations.component';
import { MasterDetailChild_Nationality_ViewNationalityTranslationModalComponent } from './nationalitiesTranslation/nationalityTranslations/masterDetailChild_Nationality_view-nationalityTranslation-modal.component';
import { MasterDetailChild_Nationality_CreateOrEditNationalityTranslationModalComponent } from './nationalitiesTranslation/nationalityTranslations/masterDetailChild_Nationality_create-or-edit-nationalityTranslation-modal.component';

import { NationalityTranslationsComponent } from './nationalitiesTranslation/nationalityTranslations/nationalityTranslations.component';
import { ViewNationalityTranslationModalComponent } from './nationalitiesTranslation/nationalityTranslations/view-nationalityTranslation-modal.component';
import { CreateOrEditNationalityTranslationModalComponent } from './nationalitiesTranslation/nationalityTranslations/create-or-edit-nationalityTranslation-modal.component';

import { TrucksTypesTranslationsComponent } from './trucksTypesTranslations/trucksTypesTranslations/trucksTypesTranslations.component';
import { ViewTrucksTypesTranslationModalComponent } from './trucksTypesTranslations/trucksTypesTranslations/view-trucksTypesTranslation-modal.component';
import { CreateOrEditTrucksTypesTranslationModalComponent } from './trucksTypesTranslations/trucksTypesTranslations/create-or-edit-trucksTypesTranslation-modal.component';

import { TransportTypesTranslationsComponent } from './transportTypesTranslations/transportTypesTranslations/transportTypesTranslations.component';
import { ViewTransportTypesTranslationModalComponent } from './transportTypesTranslations/transportTypesTranslations/view-transportTypesTranslation-modal.component';
import { CreateOrEditTransportTypesTranslationModalComponent } from './transportTypesTranslations/transportTypesTranslations/create-or-edit-transportTypesTranslation-modal.component';

import { VasPricesComponent } from './vases/vasPrices/vasPrices.component';
import { ViewVasPriceModalComponent } from './vases/vasPrices/view-vasPrice-modal.component';
import { CreateOrEditVasPriceModalComponent } from './vases/vasPrices/create-or-edit-vasPrice-modal.component';

import { ReceiversComponent } from './receivers/receivers/receivers.component';
import { ViewReceiverModalComponent } from './receivers/receivers/view-receiver-modal.component';
import { CreateOrEditReceiverModalComponent } from './receivers/receivers/create-or-edit-receiver-modal.component';

import { TermAndConditionsComponent } from './termsAndConditions/termAndConditions/termAndConditions.component';
import { ViewTermAndConditionModalComponent } from './termsAndConditions/termAndConditions/view-termAndCondition-modal.component';
import { CreateOrEditTermAndConditionModalComponent } from './termsAndConditions/termAndConditions/create-or-edit-termAndCondition-modal.component';

import { CapacitiesComponent } from './truckCapacities/capacities/capacities.component';
import { ViewCapacityModalComponent } from './truckCapacities/capacities/view-capacity-modal.component';
import { CreateOrEditCapacityModalComponent } from './truckCapacities/capacities/create-or-edit-capacity-modal.component';

import { TransportTypesComponent } from './transportTypes/transportTypes/transportTypes.component';
import { ViewTransportTypeModalComponent } from './transportTypes/transportTypes/view-transportType-modal.component';
import { CreateOrEditTransportTypeModalComponent } from './transportTypes/transportTypes/create-or-edit-transportType-modal.component';

import { DocumentTypeTranslationsComponent } from './documentTypeTranslations/documentTypeTranslations/documentTypeTranslations.component';
import { ViewDocumentTypeTranslationModalComponent } from './documentTypeTranslations/documentTypeTranslations/view-documentTypeTranslation-modal.component';
import { CreateOrEditDocumentTypeTranslationModalComponent } from './documentTypeTranslations/documentTypeTranslations/create-or-edit-documentTypeTranslation-modal.component';

import { DocumentsEntitiesComponent } from './documentsEntities/documentsEntities/documentsEntities.component';
import { ViewDocumentsEntityModalComponent } from './documentsEntities/documentsEntities/view-documentsEntity-modal.component';
import { CreateOrEditDocumentsEntityModalComponent } from './documentsEntities/documentsEntities/create-or-edit-documentsEntity-modal.component';

import { PortsComponent } from './ports/ports/ports.component';
import { ViewPortModalComponent } from './ports/ports/view-port-modal.component';
import { CreateOrEditPortModalComponent } from './ports/ports/create-or-edit-port-modal.component';

import { PickingTypesComponent } from './pickingTypes/pickingTypes/pickingTypes.component';
import { ViewPickingTypeModalComponent } from './pickingTypes/pickingTypes/view-pickingType-modal.component';
import { CreateOrEditPickingTypeModalComponent } from './pickingTypes/pickingTypes/create-or-edit-pickingType-modal.component';

import { FacilitiesComponent } from './addressBook/facilities/facilities.component';
import { ViewFacilityModalComponent } from './addressBook/facilities/view-facility-modal.component';
import { CreateOrEditFacilityModalComponent } from './addressBook/facilities/create-or-edit-facility-modal.component';
import { FileUploadModule } from 'primeng/fileupload';

import { DocumentFilesComponent } from './documentFiles/documentFiles/documentFiles.component';
import { ViewDocumentFileModalComponent } from './documentFiles/documentFiles/view-documentFile-modal.component';

import { DocumentTypesComponent } from './documentTypes/documentTypes/documentTypes.component';
import { ViewDocumentTypeModalComponent } from './documentTypes/documentTypes/view-documentType-modal.component';
import { CreateOrEditDocumentTypeModalComponent } from './documentTypes/documentTypes/create-or-edit-documentType-modal.component';

import { ShippingRequestsComponent } from './shippingRequests/shippingRequests/shippingRequests.component';
import { ViewShippingRequestComponent } from './shippingRequests/shippingRequests/view-shippingRequest.component';
import { CreateOrEditShippingRequestComponent } from './shippingRequests/shippingRequests/create-or-edit-shippingRequest.component';
import { ShippingrequestsDetailsModelComponent } from './shippingRequests/shippingRequests/details/shippingrequests-details-model.component';

import { GoodsDetailsComponent } from './goodsDetails/goodsDetails/goodsDetails.component';
import { ViewGoodsDetailModalComponent } from './goodsDetails/goodsDetails/view-goodsDetail-modal.component';
import { CreateOrEditGoodsDetailModalComponent } from './goodsDetails/goodsDetails/create-or-edit-goodsDetail-modal.component';

import { OffersComponent } from './offers/offers/offers.component';
import { ViewOfferModalComponent } from './offers/offers/view-offer-modal.component';
import { CreateOrEditOfferModalComponent } from './offers/offers/create-or-edit-offer-modal.component';

import { RoutStepsComponent } from './routSteps/routSteps/routSteps.component';
import { ViewRoutStepModalComponent } from './routSteps/routSteps/view-routStep-modal.component';
import { CreateOrEditRoutStepModalComponent } from './routSteps/routSteps/create-or-edit-routStep-modal.component';

import { RoutesComponent } from './routs/routes/routes.component';
import { ViewRouteComponent } from './routs/routes/view-route.component';
import { CreateOrEditRouteComponent } from './routs/routes/create-or-edit-route.component';

import { CitiesComponent } from './cities/cities/cities.component';
import { ViewCityModalComponent } from './cities/cities/view-city-modal.component';
import { CreateOrEditCityModalComponent } from './cities/cities/create-or-edit-city-modal.component';

import { CountiesComponent } from './countries/counties/counties.component';
import { ViewCountyModalComponent } from './countries/counties/view-county-modal.component';
import { CreateOrEditCountyModalComponent } from './countries/counties/create-or-edit-county-modal.component';

import { RoutTypesComponent } from './routTypes/routTypes/routTypes.component';
import { ViewRoutTypeModalComponent } from './routTypes/routTypes/view-routType-modal.component';
import { CreateOrEditRoutTypeModalComponent } from './routTypes/routTypes/create-or-edit-routType-modal.component';

import { GoodCategoriesComponent } from './goodCategories/goodCategories/goodCategories.component';
import { ViewGoodCategoryModalComponent } from './goodCategories/goodCategories/view-goodCategory-modal.component';
import { CreateOrEditGoodCategoryModalComponent } from './goodCategories/goodCategories/create-or-edit-goodCategory-modal.component';

import { TrailersComponent } from './trailers/trailers/trailers.component';
import { ViewTrailerModalComponent } from './trailers/trailers/view-trailer-modal.component';
import { CreateOrEditTrailerModalComponent } from './trailers/trailers/create-or-edit-trailer-modal.component';

import { TrailerStatusesComponent } from './trailerStatuses/trailerStatuses/trailerStatuses.component';
import { ViewTrailerStatusModalComponent } from './trailerStatuses/trailerStatuses/view-trailerStatus-modal.component';
import { CreateOrEditTrailerStatusModalComponent } from './trailerStatuses/trailerStatuses/create-or-edit-trailerStatus-modal.component';

import { PayloadMaxWeightsComponent } from './payloadMaxWeight/payloadMaxWeights/payloadMaxWeights.component';
import { ViewPayloadMaxWeightModalComponent } from './payloadMaxWeight/payloadMaxWeights/view-payloadMaxWeight-modal.component';
import { CreateOrEditPayloadMaxWeightModalComponent } from './payloadMaxWeight/payloadMaxWeights/create-or-edit-payloadMaxWeight-modal.component';

import { TrailerTypesComponent } from './trailerTypes/trailerTypes/trailerTypes.component';
import { ViewTrailerTypeModalComponent } from './trailerTypes/trailerTypes/view-trailerType-modal.component';
import { CreateOrEditTrailerTypeModalComponent } from './trailerTypes/trailerTypes/create-or-edit-trailerType-modal.component';

import { TrucksComponent } from './trucks/trucks/trucks.component';
import { ViewTruckModalComponent } from './trucks/trucks/view-truck-modal.component';
import { CreateOrEditTruckModalComponent } from './trucks/trucks/create-or-edit-truck-modal.component';
import { TruckUserLookupTableModalComponent } from './trucks/trucks/truck-user-lookup-table-modal.component';
import { TrucksTypesComponent } from './trucksTypes/trucksTypes/trucksTypes.component';
import { ViewTrucksTypeModalComponent } from './trucksTypes/trucksTypes/view-trucksType-modal.component';
import { CreateOrEditTrucksTypeModalComponent } from './trucksTypes/trucksTypes/create-or-edit-trucksType-modal.component';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PaginatorModule } from 'primeng/paginator';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask';
import { TableModule } from 'primeng/table';

import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MainRoutingModule } from './main-routing.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { BsDatepickerConfig, BsDatepickerModule, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { ImageCropperModule } from '@node_modules/ngx-image-cropper';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { UpdatePriceShippingRequestModalComponent } from './shippingRequests/shippingRequests/update-price-shipping-request-modal/update-price-shipping-request-modal.component';
import { AdminModule } from '@app/admin/admin.module';
import { ShippingRequestsListComponent } from '@app/main/shippingRequests/shippingRequestslist.component';
import { MarketPlaceListComponent } from '@app/main/marketplaces/marketPlacelist.component';
import { DirectRequestViewComponent } from '@app/main/directrequests/direct-request-view.component';
import { OffersListComponent } from '@app/main/offer/offers.component';
import { PriceOfferModelComponent } from '@app/main/priceoffer/price-offer-model-component';
import { PriceOfferViewModelComponent } from '@app/main/priceoffer/price-offer-view-model-component';
import { PriceOfferListModelComponent } from '@app/main/priceoffer/price-offer-list-model-component';
import { PriceOfferRejectModelComponent } from '@app/main/priceoffer/price-offer-reject-model-component';
import { ShippingRequestOffersList } from '@app/main/shippingRequests/shippingRequests/offers/shipping-request-offers-list.component';
import { RejectionReasonModalComponent } from './documentFiles/documentFiles/rejectionReason-modal.component';
import { ViewShippingRequestPriceResponseModalComponent } from './shippingRequests/shippingRequests/shipping-request-Response/view-shipping-request-response-modal.component';
import { ListboxModule, MultiSelectModule, PickListModule, SelectButtonModule, StepsModule } from '@node_modules/primeng';
import { PointsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.component';

import { AgmDirectionModule } from '@node_modules/agm-direction';
import { TripsForViewShippingRequestComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/tripsForViewShippingRequest.component';
import { CreateOrEditTripComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/createOrEditTripModal/createOrEditTrip.component';
import { CreateOrEditTripAccidentModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/create-or-edit-trip-accident-modal.component';
import { ViewTripAccidentModelComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/View-trip-accident-modal.component';
import { CreateOrEditTripResolveAccidentModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/accident/resolves/create-or-edit-resolve-trip-accident-modal.component';
import { TripRejectReasonComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/rejectreason/trip-reject-reason.component';
import { TripRejectReasonModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/rejectreason/create-or-edit-trip-reject-reason-modal.component';

import { InvoicePeriodsListComponent } from '@app/main/invoices/invoice-periods-list/invoice-periods-list.component';
import { InvoicePeriodsModalComponent } from '@app/main/invoices/invoice-periods-modal/invoice-periods-modal.component';
import { InvoicesListComponent } from '@app/main/invoices/invoices-list/invoices-list.component';
import { BalancesListComponent } from '@app/main/invoices/balances/balances-list/balances-list.component';
import { DemanModelComponent } from '@app/main/invoices/invoice-tenants/model/deman-model.component';
import { BalanceRechargeModelComponent } from '@app/main/invoices/balances/balance-recharge-model/balance-recharge-model.component';
import { InvoiceTenantComponent } from '@app/main/invoices/invoice-tenants/invoice-tenant.component';
import { InvoiceTenantDetailsComponent } from '@app/main/invoices/invoice-tenants/invoice-tenant-details.component';
import { SubmitInvoiceRejectedModelComponent } from '@app/main/invoices/invoice-tenants/model/Rejected-model.component';

import { InvoiceDetailComponent } from '@app/main/invoices/invoice-detail/invoice-detail.component';
import { TransactionListComponent } from '@app/main/invoices/transaction/transaction-list/transaction-list.component';
import { InvoicePaymentMethodComponent } from '@app/main/invoices/invoice-payment-methods/invoice-payment-method.component';
import { InvoicePaymentMethodModelComponent } from '@app/main/invoices/invoice-payment-methods/invoice-payment-method-model.component';
import { InvoiceDemandModelComponent } from '@app/main/invoices/invoices-list/invoices-ondeman-model.component';
import { InvoiceTemplateComponent } from '@app/main/invoices/template/invoice-template.component';
import { ProformaListComponent } from '@app/main/invoices/proformas/proforma-list.component';
import { AccidentReasonComponent } from '@app/main/accidents/reasons/reason.component';
import { AccidentReasonComponentModalComponent } from '@app/main/accidents/reasons/create-or-edit-reason-modal.component';
import { ViewTripModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/viewTripModal/viewTripModal.component';
import { TachyonDealToBiddingModalComponent } from '@app/main/shippingRequests/shippingRequests/tachyonDeal/tachyonDealToBiddingModal/tachyonDealToBiddingModal.component';
import { DirectRequestComponent } from '@app/main/shippingRequests/shippingRequests/directrequest/direct-request.component';
import { DirectRequestTenantModelComponent } from '@app/main/shippingRequests/shippingRequests/directrequest/direct-request-tenant-model.component';
import { ShippingRequestCardTemplateComponent } from '@app/main/shippingRequests/shippingRequests/template/shipping-request-card-template.component';
import { ShippingRequestCardSearchModelComponent } from '@app/main/shippingRequests/shippingRequests/template/shipping-request-card-search-model.component';
import { ShippingRequestCardCancelModelComponent } from '@app/main/shippingRequests/shippingRequests/template/shipping-request-card-cancel-model.component';
import { AppLocalizationComponent } from '@app/main/applocalizations/applocalization.component';
import { ApplocalizationModalComponent } from '@app/main/applocalizations/create-or-edit-applocalization-modal.component';
import { ViewApplocalizationModalComponent } from '@app/main/applocalizations/view-applocalization-modal.component';
import { ComingSoonComponent } from '@app/main/commingSoon/comingSoon.component';
import { TrackingComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking.component';
import { TrackinSearchModelComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-search-model.component';
import { TMSRequestListComponent } from '@app/main/tms/tms-request-list.component';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
  imports: [
    UtilsModule,
    AppCommonModule,
    CommonModule,
    FormsModule,
    FileUploadModule,
    AutoCompleteModule,
    PaginatorModule,
    EditorModule,
    InputMaskModule,
    TableModule,
    ModalModule,
    TabsModule,
    TooltipModule,
    MainRoutingModule,
    CountoModule,
    NgxChartsModule,
    BsDatepickerModule.forRoot(),
    BsDropdownModule.forRoot(),
    PopoverModule.forRoot(),
    ImageCropperModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyDKKZqDW_xX5azTqBV2oXSb6P3nwCAzOpw',
      libraries: ['places'],
    }),
    AdminModule,
    MultiSelectModule,
    PickListModule,
    ListboxModule,
    AgmDirectionModule,
    StepsModule,
    SelectButtonModule,
  ],
  declarations: [
    TrucksTypesTranslationsComponent,
    ViewTrucksTypesTranslationModalComponent,
    CreateOrEditTrucksTypesTranslationModalComponent,
    CitiesTranslationsComponent,
    TruckCapacitiesTranslationsComponent,
    ViewTruckCapacitiesTranslationModalComponent,
    CreateOrEditTruckCapacitiesTranslationModalComponent,
    TruckStatusesTranslationsComponent,
    ViewTruckStatusesTranslationModalComponent,
    CreateOrEditTruckStatusesTranslationModalComponent,
    PackingTypesComponent,
    ViewPackingTypeModalComponent,
    CreateOrEditPackingTypeModalComponent,
    ShippingTypesComponent,
    ViewShippingTypeModalComponent,
    CreateOrEditShippingTypeModalComponent,
    ViewCitiesTranslationModalComponent,
    CreateOrEditCitiesTranslationModalComponent,
    CountriesTranslationsComponent,
    ViewCountriesTranslationModalComponent,
    CreateOrEditCountriesTranslationModalComponent,
    PlateTypesComponent,
    ViewPlateTypeModalComponent,
    CreateOrEditPlateTypeModalComponent,
    NationalitiesComponent,
    ViewNationalityModalComponent,
    CreateOrEditNationalityModalComponent,
    MasterDetailChild_Nationality_NationalityTranslationsComponent,
    MasterDetailChild_Nationality_ViewNationalityTranslationModalComponent,
    MasterDetailChild_Nationality_CreateOrEditNationalityTranslationModalComponent,
    NationalityTranslationsComponent,
    ViewNationalityTranslationModalComponent,
    CreateOrEditNationalityTranslationModalComponent,
    TransportTypesTranslationsComponent,
    ViewTransportTypesTranslationModalComponent,
    CreateOrEditTransportTypesTranslationModalComponent,
    VasPricesComponent,
    ViewShippingRequestPriceResponseModalComponent,
    ViewVasPriceModalComponent,
    CreateOrEditVasPriceModalComponent,
    ReceiversComponent,
    ViewReceiverModalComponent,
    CreateOrEditReceiverModalComponent,
    TermAndConditionsComponent,
    ViewTermAndConditionModalComponent,
    CreateOrEditTermAndConditionModalComponent,
    CapacitiesComponent,
    RejectionReasonModalComponent,
    ViewCapacityModalComponent,
    CreateOrEditCapacityModalComponent,
    TransportTypesComponent,
    ViewTransportTypeModalComponent,
    CreateOrEditTransportTypeModalComponent,
    DocumentTypeTranslationsComponent,
    ViewDocumentTypeTranslationModalComponent,
    CreateOrEditDocumentTypeTranslationModalComponent,
    DocumentsEntitiesComponent,
    ViewDocumentsEntityModalComponent,
    CreateOrEditDocumentsEntityModalComponent,
    PortsComponent,
    ViewPortModalComponent,
    CreateOrEditPortModalComponent,
    PickingTypesComponent,
    ViewPickingTypeModalComponent,
    CreateOrEditPickingTypeModalComponent,
    FacilitiesComponent,
    ViewFacilityModalComponent,
    CreateOrEditFacilityModalComponent,
    DocumentFilesComponent,
    ViewDocumentFileModalComponent,
    DocumentTypesComponent,
    ViewDocumentTypeModalComponent,
    CreateOrEditDocumentTypeModalComponent,
    ShippingRequestsComponent,
    ViewShippingRequestComponent,
    CreateOrEditShippingRequestComponent,
    GoodsDetailsComponent,
    ViewGoodsDetailModalComponent,
    CreateOrEditGoodsDetailModalComponent,
    OffersComponent,
    ViewOfferModalComponent,
    CreateOrEditOfferModalComponent,
    RoutStepsComponent,
    ViewRoutStepModalComponent,
    CreateOrEditRoutStepModalComponent,
    RoutesComponent,
    ViewRouteComponent,
    CreateOrEditRouteComponent,
    CitiesComponent,
    ViewCityModalComponent,
    CreateOrEditCityModalComponent,
    CountiesComponent,
    ViewCountyModalComponent,
    CreateOrEditCountyModalComponent,
    RoutTypesComponent,
    ViewRoutTypeModalComponent,
    CreateOrEditRoutTypeModalComponent,
    GoodCategoriesComponent,
    ViewGoodCategoryModalComponent,
    CreateOrEditGoodCategoryModalComponent,
    TrailersComponent,
    ViewTrailerModalComponent,
    CreateOrEditTrailerModalComponent,
    TrailerStatusesComponent,
    ViewTrailerStatusModalComponent,
    CreateOrEditTrailerStatusModalComponent,
    PayloadMaxWeightsComponent,
    ViewPayloadMaxWeightModalComponent,
    CreateOrEditPayloadMaxWeightModalComponent,
    TrailerTypesComponent,
    ViewTrailerTypeModalComponent,
    CreateOrEditTrailerTypeModalComponent,
    TrucksComponent,
    ViewTruckModalComponent,
    CreateOrEditTruckModalComponent,
    TruckUserLookupTableModalComponent,
    TrucksTypesComponent,
    ViewTrucksTypeModalComponent,
    CreateOrEditTrucksTypeModalComponent,
    DashboardComponent,
    UpdatePriceShippingRequestModalComponent,
    MarketPlaceListComponent,
    ShippingRequestsListComponent,
    DirectRequestViewComponent,
    OffersListComponent,
    PriceOfferModelComponent,
    PriceOfferViewModelComponent,
    PriceOfferListModelComponent,
    PriceOfferRejectModelComponent,
    ShippingRequestOffersList,
    InvoicePeriodsListComponent,
    InvoicePeriodsModalComponent,
    InvoicesListComponent,
    BalancesListComponent,
    DemanModelComponent,
    BalanceRechargeModelComponent,
    InvoiceDetailComponent,
    SubmitInvoiceRejectedModelComponent,
    InvoiceTenantComponent,
    InvoiceTenantDetailsComponent,
    TransactionListComponent,
    InvoicePaymentMethodComponent,
    InvoicePaymentMethodModelComponent,
    InvoiceDemandModelComponent,
    InvoiceTemplateComponent,
    ProformaListComponent,
    PointsComponent,
    TripsForViewShippingRequestComponent,
    CreateOrEditTripComponent,
    CreateOrEditTripAccidentModalComponent,
    ViewTripModalComponent,
    ViewTripAccidentModelComponent,
    CreateOrEditTripResolveAccidentModalComponent,
    AccidentReasonComponent,
    AccidentReasonComponentModalComponent,
    TripRejectReasonComponent,
    TripRejectReasonModalComponent,
    //  tachyondealer
    TachyonDealToBiddingModalComponent,
    DirectRequestComponent,
    ShippingRequestCardTemplateComponent,
    ShippingRequestCardSearchModelComponent,
    ShippingRequestCardCancelModelComponent,
    DirectRequestTenantModelComponent,
    AppLocalizationComponent,
    ApplocalizationModalComponent,
    ViewApplocalizationModalComponent,
    ShippingrequestsDetailsModelComponent,
    ComingSoonComponent,
    TrackingComponent,
    TrackinSearchModelComponent,
    TMSRequestListComponent,
  ],
  providers: [
    { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
    { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
    { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
  ],
})
export class MainModule {}
