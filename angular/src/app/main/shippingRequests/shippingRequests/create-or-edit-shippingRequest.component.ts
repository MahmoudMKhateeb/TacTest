import {Component, Injector, OnInit} from '@angular/core';
import {finalize} from 'rxjs/operators';
import {
    CreateOrEditShippingRequestDto,
    ShippingRequestGoodsDetailLookupTableDto,
    ShippingRequestRouteLookupTableDto,
    ShippingRequestsServiceProxy,
    ShippingRequestTrailerTypeLookupTableDto,
    ShippingRequestTrucksTypeLookupTableDto
} from '@shared/service-proxies/service-proxies';
import {AppComponentBase} from '@shared/common/app-component-base';
import {ActivatedRoute, Router} from '@angular/router';
import {appModuleAnimation} from '@shared/animations/routerTransition';
import {Observable} from '@node_modules/rxjs';
import {BreadcrumbItem} from '@app/shared/common/sub-header/sub-header.component';

@Component({
    templateUrl: './create-or-edit-shippingRequest.component.html',
    animations: [appModuleAnimation()]
})
export class CreateOrEditShippingRequestComponent extends AppComponentBase implements OnInit {
    active = false;
    saving = false;

    shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();

    trucksTypeDisplayName = '';
    trailerTypeDisplayName = '';
    goodsDetailName = '';
    routeDisplayName = '';

    allTrucksTypes: ShippingRequestTrucksTypeLookupTableDto[];
    allTrailerTypes: ShippingRequestTrailerTypeLookupTableDto[];
    allGoodsDetails: ShippingRequestGoodsDetailLookupTableDto[];
    allRoutes: ShippingRequestRouteLookupTableDto[];

    breadcrumbs: BreadcrumbItem[] = [
        new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests'),
        new BreadcrumbItem(this.l('Entity_Name_Plural_Here') + '' + this.l('Details')),
    ];

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.show(this._activatedRoute.snapshot.queryParams['id']);
    }

    show(shippingRequestId?: number): void {

        if (!shippingRequestId) {
            this.shippingRequest = new CreateOrEditShippingRequestDto();
            this.shippingRequest.id = shippingRequestId;
            this.trucksTypeDisplayName = '';
            this.trailerTypeDisplayName = '';
            this.goodsDetailName = '';
            this.routeDisplayName = '';

            this.active = true;
        } else {
            this._shippingRequestsServiceProxy.getShippingRequestForEdit(shippingRequestId).subscribe(result => {
                this.shippingRequest = result.shippingRequest;

                this.trucksTypeDisplayName = result.trucksTypeDisplayName;
                this.trailerTypeDisplayName = result.trailerTypeDisplayName;
                this.goodsDetailName = result.goodsDetailName;
                this.routeDisplayName = result.routeDisplayName;

                this.active = true;
            });
        }
        this._shippingRequestsServiceProxy.getAllTrucksTypeForTableDropdown().subscribe(result => {
            this.allTrucksTypes = result;
        });
        this._shippingRequestsServiceProxy.getAllTrailerTypeForTableDropdown().subscribe(result => {
            this.allTrailerTypes = result;
        });
        this._shippingRequestsServiceProxy.getAllGoodsDetailForTableDropdown().subscribe(result => {
            this.allGoodsDetails = result;
        });
        this._shippingRequestsServiceProxy.getAllRouteForTableDropdown().subscribe(result => {
            this.allRoutes = result;
        });

    }

    save(): void {
        this.saveInternal().subscribe(x => {
            this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
        });
    }

    saveAndNew(): void {
        this.saveInternal().subscribe(x => {
            this.shippingRequest = new CreateOrEditShippingRequestDto();
        });
    }

    private saveInternal(): Observable<void> {
        this.saving = true;


        return this._shippingRequestsServiceProxy.createOrEdit(this.shippingRequest)
            .pipe(finalize(() => {
                this.saving = false;
                this.notify.info(this.l('SavedSuccessfully'));
            }));
    }


}
