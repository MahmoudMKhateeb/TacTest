import { Component, ViewChild, Injector, Output, EventEmitter, OnInit} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RoutesServiceProxy, CreateOrEditRouteDto , RouteRoutTypeLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {Observable} from '@node_modules/rxjs';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';

@Component({
    templateUrl: './create-or-edit-route.component.html',
    animations: [appModuleAnimation()]
})
export class CreateOrEditRouteComponent extends AppComponentBase implements OnInit {
    active = false;
    saving = false;

    route: CreateOrEditRouteDto = new CreateOrEditRouteDto();
    routeId: any;

    routTypeDisplayName = '';

	allRoutTypes: RouteRoutTypeLookupTableDto[];

breadcrumbs: BreadcrumbItem[] = [
                        new BreadcrumbItem(this.l('Route'), '/app/main/routs/routes'),
                        new BreadcrumbItem(this.l('Entity_Name_Plural_Here') + '' + this.l('Details')),
                    ];
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _routesServiceProxy: RoutesServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.routeId = this._activatedRoute.snapshot.queryParams['id'];
        this.show(this.routeId );
    }

    show(routeId?: number): void {

        if (!routeId) {
            this.route = new CreateOrEditRouteDto();
            this.route.id = routeId;
            this.routTypeDisplayName = '';

            this.active = true;
        } else {
            this._routesServiceProxy.getRouteForEdit(routeId).subscribe(result => {
                this.route = result.route;

                this.routTypeDisplayName = result.routTypeDisplayName;

                this.active = true;
            });
        }
        this._routesServiceProxy.getAllRoutTypeForTableDropdown().subscribe(result => {
						this.allRoutTypes = result;
					});

    }

    private saveInternal(): Observable<number> {
            this.saving = true;


        return this._routesServiceProxy.createOrEdit(this.route)
         .pipe(finalize(() => {
            this.saving = false;
            this.notify.info(this.l('SavedSuccessfully'));
         }));
    }

    save(): void {
        this.saveInternal().subscribe(routeId => {
             this._router.navigate( ['/app/main/routs/routes/createOrEdit'], { queryParams: { id: routeId }, skipLocationChange: false, replaceUrl: true });
        this.routeId = routeId;
        });
    }









}
