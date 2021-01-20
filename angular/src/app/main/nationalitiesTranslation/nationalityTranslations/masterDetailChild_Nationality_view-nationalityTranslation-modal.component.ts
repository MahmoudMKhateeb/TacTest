import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetNationalityTranslationForViewDto, NationalityTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'masterDetailChild_Nationality_viewNationalityTranslationModal',
    templateUrl: './masterDetailChild_Nationality_view-nationalityTranslation-modal.component.html'
})
export class MasterDetailChild_Nationality_ViewNationalityTranslationModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetNationalityTranslationForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetNationalityTranslationForViewDto();
        this.item.nationalityTranslation = new NationalityTranslationDto();
    }

    show(item: GetNationalityTranslationForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
