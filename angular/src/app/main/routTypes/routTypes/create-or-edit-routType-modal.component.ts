import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RoutTypesServiceProxy, CreateOrEditRoutTypeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditRoutTypeModal',
    templateUrl: './create-or-edit-routType-modal.component.html'
})
export class CreateOrEditRoutTypeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    routType: CreateOrEditRoutTypeDto = new CreateOrEditRoutTypeDto();



    constructor(
        injector: Injector,
        private _routTypesServiceProxy: RoutTypesServiceProxy
    ) {
        super(injector);
    }

    show(routTypeId?: number): void {

        if (!routTypeId) {
            this.routType = new CreateOrEditRoutTypeDto();
            this.routType.id = routTypeId;

            this.active = true;
            this.modal.show();
        } else {
            this._routTypesServiceProxy.getRoutTypeForEdit(routTypeId).subscribe(result => {
                this.routType = result.routType;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._routTypesServiceProxy.createOrEdit(this.routType)
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
