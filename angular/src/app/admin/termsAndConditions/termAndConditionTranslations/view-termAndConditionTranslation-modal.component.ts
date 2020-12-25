import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTermAndConditionTranslationForViewDto, TermAndConditionTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewTermAndConditionTranslationModal',
    templateUrl: './view-termAndConditionTranslation-modal.component.html'
})
export class ViewTermAndConditionTranslationModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetTermAndConditionTranslationForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetTermAndConditionTranslationForViewDto();
        this.item.termAndConditionTranslation = new TermAndConditionTranslationDto();
    }

    show(item: GetTermAndConditionTranslationForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
