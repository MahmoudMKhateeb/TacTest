import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TrucksTypesComponent } from './trucksTypes/trucksTypes/trucksTypes.component';
import { DashboardComponent } from './dashboard/dashboard.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
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
