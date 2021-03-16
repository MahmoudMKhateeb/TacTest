import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CountriesTranslationsServiceProxy, CreateOrEditCountriesTranslationDto ,CountriesTranslationCountyLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCountriesTranslationModal',
    templateUrl: './create-or-edit-countriesTranslation-modal.component.html'
})
export class CreateOrEditCountriesTranslationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    countriesTranslation: CreateOrEditCountriesTranslationDto = new CreateOrEditCountriesTranslationDto();

    countyDisplayName = '';

	allCountys: CountriesTranslationCountyLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _countriesTranslationsServiceProxy: CountriesTranslationsServiceProxy
    ) {
        super(injector);
    }
    
    show(countriesTranslationId?: number): void {
    

        if (!countriesTranslationId) {
            this.countriesTranslation = new CreateOrEditCountriesTranslationDto();
            this.countriesTranslation.id = countriesTranslationId;
            this.countyDisplayName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._countriesTranslationsServiceProxy.getCountriesTranslationForEdit(countriesTranslationId).subscribe(result => {
                this.countriesTranslation = result.countriesTranslation;

                this.countyDisplayName = result.countyDisplayName;

                this.active = true;
                this.modal.show();
            });
        }
        this._countriesTranslationsServiceProxy.getAllCountyForTableDropdown().subscribe(result => {						
						this.allCountys = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
			
            this._countriesTranslationsServiceProxy.createOrEdit(this.countriesTranslation)
             .pipe(finalize(() => { this.saving = false;}))
             .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
             });
    }







    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
