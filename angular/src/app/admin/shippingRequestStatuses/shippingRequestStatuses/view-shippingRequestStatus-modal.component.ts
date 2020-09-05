import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetShippingRequestStatusForViewDto, ShippingRequestStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewShippingRequestStatusModal',
    templateUrl: './view-shippingRequestStatus-modal.component.html'
})
export class ViewShippingRequestStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetShippingRequestStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetShippingRequestStatusForViewDto();
        this.item.shippingRequestStatus = new ShippingRequestStatusDto();
    }

    show(item: GetShippingRequestStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
