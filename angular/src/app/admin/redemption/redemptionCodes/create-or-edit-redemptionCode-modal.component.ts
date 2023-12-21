import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RedemptionCodesServiceProxy, CreateOrEditRedemptionCodeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';




@Component({
    selector: 'createOrEditRedemptionCodeModal',
    templateUrl: './create-or-edit-redemptionCode-modal.component.html'
})
export class CreateOrEditRedemptionCodeModalComponent extends AppComponentBase implements OnInit{
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    redemptionCode: CreateOrEditRedemptionCodeDto = new CreateOrEditRedemptionCodeDto();

    redeemCodeCode = '';



    constructor(
        injector: Injector,
        private _redemptionCodesServiceProxy: RedemptionCodesServiceProxy
    ) {
        super(injector);
    }
    
    show(redemptionCodeId?: number): void {
    

        if (!redemptionCodeId) {
            this.redemptionCode = new CreateOrEditRedemptionCodeDto();
            this.redemptionCode.id = redemptionCodeId;
            this.redeemCodeCode = '';


            this.active = true;
            this.modal.show();
        } else {
            this._redemptionCodesServiceProxy.getRedemptionCodeForEdit(redemptionCodeId).subscribe(result => {
                this.redemptionCode = result.redemptionCode;

                this.redeemCodeCode = result.redeemCodeCode;


                this.active = true;
                this.modal.show();
            });
        }
        
        
    }

    save(): void {
            this.saving = true;
            
			
			
            this._redemptionCodesServiceProxy.createOrEdit(this.redemptionCode)
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
    
     ngOnInit(): void {
        
     }    
}
