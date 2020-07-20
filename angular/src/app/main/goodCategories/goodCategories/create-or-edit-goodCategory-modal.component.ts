import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { GoodCategoriesServiceProxy, CreateOrEditGoodCategoryDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditGoodCategoryModal',
    templateUrl: './create-or-edit-goodCategory-modal.component.html'
})
export class CreateOrEditGoodCategoryModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    goodCategory: CreateOrEditGoodCategoryDto = new CreateOrEditGoodCategoryDto();



    constructor(
        injector: Injector,
        private _goodCategoriesServiceProxy: GoodCategoriesServiceProxy
    ) {
        super(injector);
    }

    show(goodCategoryId?: number): void {

        if (!goodCategoryId) {
            this.goodCategory = new CreateOrEditGoodCategoryDto();
            this.goodCategory.id = goodCategoryId;

            this.active = true;
            this.modal.show();
        } else {
            this._goodCategoriesServiceProxy.getGoodCategoryForEdit(goodCategoryId).subscribe(result => {
                this.goodCategory = result.goodCategory;


                this.active = true;
                this.modal.show();
            });
        }
        
    }

    save(): void {
            this.saving = true;

			
            this._goodCategoriesServiceProxy.createOrEdit(this.goodCategory)
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
