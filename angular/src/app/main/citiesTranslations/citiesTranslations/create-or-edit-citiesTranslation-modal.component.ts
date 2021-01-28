import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CitiesTranslationsServiceProxy, CreateOrEditCitiesTranslationDto ,CitiesTranslationCityLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditCitiesTranslationModal',
    templateUrl: './create-or-edit-citiesTranslation-modal.component.html'
})
export class CreateOrEditCitiesTranslationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    citiesTranslation: CreateOrEditCitiesTranslationDto = new CreateOrEditCitiesTranslationDto();

    cityDisplayName = '';

	allCitys: CitiesTranslationCityLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _citiesTranslationsServiceProxy: CitiesTranslationsServiceProxy
    ) {
        super(injector);
    }
    
    show(citiesTranslationId?: number): void {
    

        if (!citiesTranslationId) {
            this.citiesTranslation = new CreateOrEditCitiesTranslationDto();
            this.citiesTranslation.id = citiesTranslationId;
            this.cityDisplayName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._citiesTranslationsServiceProxy.getCitiesTranslationForEdit(citiesTranslationId).subscribe(result => {
                this.citiesTranslation = result.citiesTranslation;

                this.cityDisplayName = result.cityDisplayName;

                this.active = true;
                this.modal.show();
            });
        }
        this._citiesTranslationsServiceProxy.getAllCityForTableDropdown().subscribe(result => {						
						this.allCitys = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
			
            this._citiesTranslationsServiceProxy.createOrEdit(this.citiesTranslation)
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
