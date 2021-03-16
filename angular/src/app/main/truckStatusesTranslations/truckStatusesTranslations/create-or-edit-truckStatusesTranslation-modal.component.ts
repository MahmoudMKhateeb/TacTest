import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TruckStatusesTranslationsServiceProxy, CreateOrEditTruckStatusesTranslationDto ,TruckStatusesTranslationTruckStatusLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditTruckStatusesTranslationModal',
    templateUrl: './create-or-edit-truckStatusesTranslation-modal.component.html'
})
export class CreateOrEditTruckStatusesTranslationModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    truckStatusesTranslation: CreateOrEditTruckStatusesTranslationDto = new CreateOrEditTruckStatusesTranslationDto();

    truckStatusDisplayName = '';

	allTruckStatuss: TruckStatusesTranslationTruckStatusLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _truckStatusesTranslationsServiceProxy: TruckStatusesTranslationsServiceProxy
    ) {
        super(injector);
    }
    
    show(truckStatusesTranslationId?: number): void {
    

        if (!truckStatusesTranslationId) {
            this.truckStatusesTranslation = new CreateOrEditTruckStatusesTranslationDto();
            this.truckStatusesTranslation.id = truckStatusesTranslationId;
            this.truckStatusDisplayName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._truckStatusesTranslationsServiceProxy.getTruckStatusesTranslationForEdit(truckStatusesTranslationId).subscribe(result => {
                this.truckStatusesTranslation = result.truckStatusesTranslation;

                this.truckStatusDisplayName = result.truckStatusDisplayName;

                this.active = true;
                this.modal.show();
            });
        }
        this._truckStatusesTranslationsServiceProxy.getAllTruckStatusForTableDropdown().subscribe(result => {						
						this.allTruckStatuss = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
			
            this._truckStatusesTranslationsServiceProxy.createOrEdit(this.truckStatusesTranslation)
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
