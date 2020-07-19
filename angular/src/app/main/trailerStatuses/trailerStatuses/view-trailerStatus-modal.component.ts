import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTrailerStatusForViewDto, TrailerStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewTrailerStatusModal',
    templateUrl: './view-trailerStatus-modal.component.html'
})
export class ViewTrailerStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetTrailerStatusForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetTrailerStatusForViewDto();
        this.item.trailerStatus = new TrailerStatusDto();
    }

    show(item: GetTrailerStatusForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
