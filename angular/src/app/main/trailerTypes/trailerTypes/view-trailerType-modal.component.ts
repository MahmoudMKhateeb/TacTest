import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTrailerTypeForViewDto, TrailerTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewTrailerTypeModal',
    templateUrl: './view-trailerType-modal.component.html'
})
export class ViewTrailerTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetTrailerTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetTrailerTypeForViewDto();
        this.item.trailerType = new TrailerTypeDto();
    }

    show(item: GetTrailerTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
