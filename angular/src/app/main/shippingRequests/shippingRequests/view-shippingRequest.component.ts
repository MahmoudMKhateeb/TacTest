import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ShippingRequestsServiceProxy, GetShippingRequestForViewDto, ShippingRequestDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
@Component({
    templateUrl: './view-shippingRequest.component.html',
    animations: [appModuleAnimation()]
})
export class ViewShippingRequestComponent extends AppComponentBase implements OnInit {

    active = false;
    saving = false;

    item: GetShippingRequestForViewDto;

breadcrumbs: BreadcrumbItem[]= [
                        new BreadcrumbItem(this.l("ShippingRequest"),"/app/main/shippingRequests/shippingRequests"),
                        new BreadcrumbItem(this.l('ShippingRequests') + '' + this.l('Details')),
                    ];
    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
         private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy
    ) {
        super(injector);
        this.item = new GetShippingRequestForViewDto();
        this.item.shippingRequest = new ShippingRequestDto();        
    }

    ngOnInit(): void {
        this.show(this._activatedRoute.snapshot.queryParams['id']);
    }

    show(shippingRequestId: number): void {
      this._shippingRequestsServiceProxy.getShippingRequestForView(shippingRequestId).subscribe(result => {      
                 this.item = result;
                this.active = true;
            });       
    }
}
