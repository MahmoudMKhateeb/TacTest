import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { NationalitiesServiceProxy, CreateOrEditNationalityDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';


@Component({
    selector: 'createOrEditNationalityModal',
    templateUrl: './create-or-edit-nationality-modal.component.html'
})
export class CreateOrEditNationalityModalComponent extends AppComponentBase {
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    nationality: CreateOrEditNationalityDto = new CreateOrEditNationalityDto();



    constructor(
        injector: Injector,
        private _nationalitiesServiceProxy: NationalitiesServiceProxy
    ) {
        super(injector);
    }
    
    show(nationalityId?: number): void {
    

        if (!nationalityId) {
            this.nationality = new CreateOrEditNationalityDto();
            this.nationality.id = nationalityId;

            this.active = true;
            this.modal.show();
        } else {
            this._nationalitiesServiceProxy.getNationalityForEdit(nationalityId).subscribe(result => {
                this.nationality = result.nationality;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
			
            this._nationalitiesServiceProxy.createOrEdit(this.nationality)
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
