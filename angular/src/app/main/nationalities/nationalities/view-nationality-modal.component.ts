import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetNationalityForViewDto, NationalityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewNationalityModal',
    templateUrl: './view-nationality-modal.component.html'
})
export class ViewNationalityModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetNationalityForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetNationalityForViewDto();
        this.item.nationality = new NationalityDto();
    }

    show(item: GetNationalityForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
