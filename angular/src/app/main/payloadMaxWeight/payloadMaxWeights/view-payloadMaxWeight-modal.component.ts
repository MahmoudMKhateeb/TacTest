import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetPayloadMaxWeightForViewDto, PayloadMaxWeightDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewPayloadMaxWeightModal',
    templateUrl: './view-payloadMaxWeight-modal.component.html'
})
export class ViewPayloadMaxWeightModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetPayloadMaxWeightForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetPayloadMaxWeightForViewDto();
        this.item.payloadMaxWeight = new PayloadMaxWeightDto();
    }

    show(item: GetPayloadMaxWeightForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
