import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetOfferForViewDto, OfferDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewOfferModal',
    templateUrl: './view-offer-modal.component.html'
})
export class ViewOfferModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetOfferForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetOfferForViewDto();
        this.item.offer = new OfferDto();
    }

    show(item: GetOfferForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
