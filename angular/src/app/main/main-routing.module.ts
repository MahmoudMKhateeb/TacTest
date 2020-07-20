import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
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
