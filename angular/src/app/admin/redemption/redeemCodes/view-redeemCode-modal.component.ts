import {AppConsts} from "@shared/AppConsts";
import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetRedeemCodeForViewDto, RedeemCodeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewRedeemCodeModal',
    templateUrl: './view-redeemCode-modal.component.html'
})
export class ViewRedeemCodeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetRedeemCodeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetRedeemCodeForViewDto();
        this.item.redeemCode = new RedeemCodeDto();
    }

    show(item: GetRedeemCodeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }
    
    

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
