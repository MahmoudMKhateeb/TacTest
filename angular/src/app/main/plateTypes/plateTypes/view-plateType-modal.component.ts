import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetPlateTypeForViewDto, PlateTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewPlateTypeModal',
    templateUrl: './view-plateType-modal.component.html'
})
export class ViewPlateTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetPlateTypeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetPlateTypeForViewDto();
        this.item.plateType = new PlateTypeDto();
    }

    show(item: GetPlateTypeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
