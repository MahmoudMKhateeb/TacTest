import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CountiesServiceProxy, CreateOrEditCountyDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditCountyModal',
    templateUrl: './create-or-edit-county-modal.component.html'
})
export class CreateOrEditCountyModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    county: CreateOrEditCountyDto = new CreateOrEditCountyDto();



    constructor(
        injector: Injector,
        private _countiesServiceProxy: CountiesServiceProxy
    ) {
        super(injector);
    }

    show(countyId?: number): void {

        if (!countyId) {
            this.county = new CreateOrEditCountyDto();
            this.county.id = countyId;

            this.active = true;
            this.modal.show();
        } else {
            this._countiesServiceProxy.getCountyForEdit(countyId).subscribe(result => {
                this.county = result.county;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._countiesServiceProxy.createOrEdit(this.county)
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
