import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ShippingRequestStatusesServiceProxy, CreateOrEditShippingRequestStatusDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditShippingRequestStatusModal',
    templateUrl: './create-or-edit-shippingRequestStatus-modal.component.html'
})
export class CreateOrEditShippingRequestStatusModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    shippingRequestStatus: CreateOrEditShippingRequestStatusDto = new CreateOrEditShippingRequestStatusDto();



    constructor(
        injector: Injector,
        private _shippingRequestStatusesServiceProxy: ShippingRequestStatusesServiceProxy
    ) {
        super(injector);
    }

    show(shippingRequestStatusId?: number): void {

        if (!shippingRequestStatusId) {
            this.shippingRequestStatus = new CreateOrEditShippingRequestStatusDto();
            this.shippingRequestStatus.id = shippingRequestStatusId;

            this.active = true;
            this.modal.show();
        } else {
            this._shippingRequestStatusesServiceProxy.getShippingRequestStatusForEdit(shippingRequestStatusId).subscribe(result => {
                this.shippingRequestStatus = result.shippingRequestStatus;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._shippingRequestStatusesServiceProxy.createOrEdit(this.shippingRequestStatus)
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
