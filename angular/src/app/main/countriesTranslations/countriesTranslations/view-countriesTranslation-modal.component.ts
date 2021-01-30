import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetCountriesTranslationForViewDto, CountriesTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewCountriesTranslationModal',
    templateUrl: './view-countriesTranslation-modal.component.html'
})
export class ViewCountriesTranslationModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetCountriesTranslationForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetCountriesTranslationForViewDto();
        this.item.countriesTranslation = new CountriesTranslationDto();
    }

    show(item: GetCountriesTranslationForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
