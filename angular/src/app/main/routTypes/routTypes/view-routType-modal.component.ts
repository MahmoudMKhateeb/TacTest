import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetRoutTypeForViewDto, RoutTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewRoutTypeModal',
    templateUrl: './view-routType-modal.component.html'
})
export class ViewRoutTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetRoutTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetRoutTypeForViewDto();
        this.item.routType = new RoutTypeDto();
    }

    show(item: GetRoutTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
