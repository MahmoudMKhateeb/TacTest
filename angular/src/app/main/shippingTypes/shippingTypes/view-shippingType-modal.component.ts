import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetShippingTypeForViewDto, ShippingTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewShippingTypeModal',
    templateUrl: './view-shippingType-modal.component.html'
})
export class ViewShippingTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetShippingTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetShippingTypeForViewDto();
        this.item.shippingType = new ShippingTypeDto();
    }

    show(item: GetShippingTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
