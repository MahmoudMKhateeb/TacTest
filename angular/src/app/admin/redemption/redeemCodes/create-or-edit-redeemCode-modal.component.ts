import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { RedeemCodesServiceProxy, CreateOrEditRedeemCodeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';




@Component({
    selector: 'createOrEditRedeemCodeModal',
    templateUrl: './create-or-edit-redeemCode-modal.component.html'
})
export class CreateOrEditRedeemCodeModalComponent extends AppComponentBase implements OnInit{
   
    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    redeemCode: CreateOrEditRedeemCodeDto = new CreateOrEditRedeemCodeDto();




    constructor(
        injector: Injector,
        private _redeemCodesServiceProxy: RedeemCodesServiceProxy
    ) {
        super(injector);
    }
    
    show(redeemCodeId?: number): void {
    

        if (!redeemCodeId) {
            this.redeemCode = new CreateOrEditRedeemCodeDto();
            this.redeemCode.id = redeemCodeId;
            this.redeemCode.expiryDate = moment().startOf('day');


            this.active = true;
            this.modal.show();
        } else {
            this._redeemCodesServiceProxy.getRedeemCodeForEdit(redeemCodeId).subscribe(result => {
                this.redeemCode = result.redeemCode;



                this.active = true;
                this.modal.show();
            });
        }
        
        
    }

    save(): void {
            this.saving = true;
            
			
			
            this._redeemCodesServiceProxy.createOrEdit(this.redeemCode)
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
