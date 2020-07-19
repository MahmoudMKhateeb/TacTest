import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TrailerStatusesServiceProxy, CreateOrEditTrailerStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditTrailerStatusModal',
    templateUrl: './create-or-edit-trailerStatus-modal.component.html'
})
export class CreateOrEditTrailerStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    trailerStatus: CreateOrEditTrailerStatusDto = new CreateOrEditTrailerStatusDto();



    constructor(
        injector: Injector,
        private _trailerStatusesServiceProxy: TrailerStatusesServiceProxy
    ) {
        super(injector);
    }

    show(trailerStatusId?: number): void {

        if (!trailerStatusId) {
            this.trailerStatus = new CreateOrEditTrailerStatusDto();
            this.trailerStatus.id = trailerStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._trailerStatusesServiceProxy.getTrailerStatusForEdit(trailerStatusId).subscribe(result => {
                this.trailerStatus = result.trailerStatus;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._trailerStatusesServiceProxy.createOrEdit(this.trailerStatus)
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
