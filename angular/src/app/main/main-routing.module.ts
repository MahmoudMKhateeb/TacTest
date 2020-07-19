import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
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
