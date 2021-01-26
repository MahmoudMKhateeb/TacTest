import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TruckCapacitiesTranslationsServiceProxy, CreateOrEditTruckCapacitiesTranslationDto ,TruckCapacitiesTranslationCapacityLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditTruckCapacitiesTranslationModal',
    templateUrl: './create-or-edit-truckCapacitiesTranslation-modal.component.html'
})
export class CreateOrEditTruckCapacitiesTranslationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    truckCapacitiesTranslation: CreateOrEditTruckCapacitiesTranslationDto = new CreateOrEditTruckCapacitiesTranslationDto();

    capacityDisplayName = '';

	allCapacitys: TruckCapacitiesTranslationCapacityLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _truckCapacitiesTranslationsServiceProxy: TruckCapacitiesTranslationsServiceProxy
    ) {
        super(injector);
    }
    
    show(truckCapacitiesTranslationId?: number): void {
    

        if (!truckCapacitiesTranslationId) {
            this.truckCapacitiesTranslation = new CreateOrEditTruckCapacitiesTranslationDto();
            this.truckCapacitiesTranslation.id = truckCapacitiesTranslationId;
            this.capacityDisplayName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._truckCapacitiesTranslationsServiceProxy.getTruckCapacitiesTranslationForEdit(truckCapacitiesTranslationId).subscribe(result => {
                this.truckCapacitiesTranslation = result.truckCapacitiesTranslation;

                this.capacityDisplayName = result.capacityDisplayName;

                this.active = true;
                this.modal.show();
            });
        }
        this._truckCapacitiesTranslationsServiceProxy.getAllCapacityForTableDropdown().subscribe(result => {						
						this.allCapacitys = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
			
            this._truckCapacitiesTranslationsServiceProxy.createOrEdit(this.truckCapacitiesTranslation)
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
