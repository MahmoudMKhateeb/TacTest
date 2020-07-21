import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
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

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'shippingRequests/shippingRequests', component: ShippingRequestsComponent, data: { permission: 'Pages.ShippingRequests' }  },
                    { path: 'shippingRequests/shippingRequests/createOrEdit', component: CreateOrEditShippingRequestComponent, data: { permission: 'Pages.ShippingRequests.Create' }  },
                    { path: 'shippingRequests/shippingRequests/view', component: ViewShippingRequestComponent, data: { permission: 'Pages.ShippingRequests' }  },
                    { path: 'goodsDetails/goodsDetails', component: GoodsDetailsComponent, data: { permission: 'Pages.GoodsDetails' }  },
                    { path: 'offers/offers', component: OffersComponent, data: { permission: 'Pages.Offers' }  },
                    { path: 'routSteps/routSteps', component: RoutStepsComponent, data: { permission: 'Pages.RoutSteps' }  },
                    { path: 'routs/routes', component: RoutesComponent, data: { permission: 'Pages.Routes' }  },
                    { path: 'routs/routes/createOrEdit', component: CreateOrEditRouteComponent, data: { permission: 'Pages.Routes.Create' }  },
                    { path: 'routs/routes/view', component: ViewRouteComponent, data: { permission: 'Pages.Routes' }  },
                    { path: 'cities/cities', component: CitiesComponent, data: { permission: 'Pages.Cities' }  },
                    { path: 'countries/counties', component: CountiesComponent, data: { permission: 'Pages.Counties' }  },
                    { path: 'routTypes/routTypes', component: RoutTypesComponent, data: { permission: 'Pages.RoutTypes' }  },
                    { path: 'goodCategories/goodCategories', component: GoodCategoriesComponent, data: { permission: 'Pages.GoodCategories' }  },
                    { path: 'trailers/trailers', component: TrailersComponent, data: { permission: 'Pages.Trailers' }  },
                    { path: 'trailerStatuses/trailerStatuses', component: TrailerStatusesComponent, data: { permission: 'Pages.TrailerStatuses' }  },
                    { path: 'payloadMaxWeight/payloadMaxWeights', component: PayloadMaxWeightsComponent, data: { permission: 'Pages.PayloadMaxWeights' }  },
                    { path: 'trailerTypes/trailerTypes', component: TrailerTypesComponent, data: { permission: 'Pages.TrailerTypes' }  },
                    { path: 'trucks/trucks', component: TrucksComponent, data: { permission: 'Pages.Trucks' }  },
                    { path: 'trucksTypes/trucksTypes', component: TrucksTypesComponent, data: { permission: 'Pages.TrucksTypes' }  },
                    { path: 'dashboard', component: DashboardComponent, data: { permission: 'Pages.Tenant.Dashboard' } },
                    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
                    { path: '**', redirectTo: 'dashboard' }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class MainRoutingModule { }
