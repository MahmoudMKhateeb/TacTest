import {AppConsts} from "@shared/AppConsts";
import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetRedemptionCodeForViewDto, RedemptionCodeDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'viewRedemptionCodeModal',
    templateUrl: './view-redemptionCode-modal.component.html'
})
export class ViewRedemptionCodeModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    item: GetRedemptionCodeForViewDto;


    constructor(
        injector: Injector
    ) {
        super(injector);
        this.item = new GetRedemptionCodeForViewDto();
        this.item.redemptionCode = new RedemptionCodeDto();
    }

    show(item: GetRedemptionCodeForViewDto): void {
        this.item = item;
        this.active = true;
        this.modal.show();
    }
    
    

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
