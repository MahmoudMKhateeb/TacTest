import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { TransportSubtypesComponent } from './transportSubtypes/transportSubtypes/transportSubtypes.component';
import { ViewTransportSubtypeModalComponent } from './transportSubtypes/transportSubtypes/view-transportSubtype-modal.component';
import { CreateOrEditTransportSubtypeModalComponent } from './transportSubtypes/transportSubtypes/create-or-edit-transportSubtype-modal.component';

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
import { CreateOrEditDocumentFileModalComponent } from './documentFiles/documentFiles/create-or-edit-documentFile-modal.component';

import { DocumentTypesComponent } from './documentTypes/documentTypes/documentTypes.component';
import { ViewDocumentTypeModalComponent } from './documentTypes/documentTypes/view-documentType-modal.component';
import { CreateOrEditDocumentTypeModalComponent } from './documentTypes/documentTypes/create-or-edit-documentType-modal.component';

import { ShippingRequestsComponent } from './shippingRequests/shippingRequests/shippingRequests.component';
import { ViewShippingRequestComponent } from './shippingRequests/shippingRequests/view-shippingRequest.component';
import { CreateOrEditShippingRequestComponent } from './shippingRequests/shippingRequests/create-or-edit-shippingRequest.component';

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

import { BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { ImageCropperModule } from '@node_modules/ngx-image-cropper';
import { AgmCoreModule } from '@node_modules/@agm/core';
import { UpdatePriceShippingRequestModalComponent } from './shippingRequests/shippingRequests/update-price-shipping-request-modal/update-price-shipping-request-modal.component';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
  imports: [
    FileUploadModule,

    AutoCompleteModule,
    PaginatorModule,
    EditorModule,
    InputMaskModule,
    TableModule,

    CommonModule,
    FormsModule,
    ModalModule,
    TabsModule,
    TooltipModule,
    AppCommonModule,
    UtilsModule,
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
  ],
  declarations: [
    TransportSubtypesComponent,

    ViewTransportSubtypeModalComponent,
    CreateOrEditTransportSubtypeModalComponent,
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
    CreateOrEditDocumentFileModalComponent,
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
  ],
  providers: [
    { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
    { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
    { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
  ],
})
export class MainModule {}
