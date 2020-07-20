import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
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
import { InputMaskModule } from 'primeng/inputmask';import { FileUploadModule } from 'primeng/fileupload';
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
import {ImageCropperModule} from '@node_modules/ngx-image-cropper';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
    imports: [
        FileUploadModule,
        AutoCompleteModule,
        PaginatorModule,
        EditorModule,
        InputMaskModule, TableModule,

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
        PopoverModule.forRoot(), ImageCropperModule
    ],
    declarations: [
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
        DashboardComponent
    ],
    providers: [
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale }
    ]
})
export class MainModule { }
