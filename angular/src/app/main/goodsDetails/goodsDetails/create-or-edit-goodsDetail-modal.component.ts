import { Component, ViewChild, Injector, Output, EventEmitter} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { GoodsDetailsServiceProxy, CreateOrEditGoodsDetailDto ,GoodsDetailGoodCategoryLookupTableDto
					} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'createOrEditGoodsDetailModal',
    templateUrl: './create-or-edit-goodsDetail-modal.component.html'
})
export class CreateOrEditGoodsDetailModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    goodsDetail: CreateOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();

    goodCategoryDisplayName = '';

	allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
					
    constructor(
        injector: Injector,
        private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
    ) {
        super(injector);
    }

    show(goodsDetailId?: number): void {

        if (!goodsDetailId) {
            this.goodsDetail = new CreateOrEditGoodsDetailDto();
            this.goodsDetail.id = goodsDetailId;
            this.goodCategoryDisplayName = '';

            this.active = true;
            this.modal.show();
        } else {
            this._goodsDetailsServiceProxy.getGoodsDetailForEdit(goodsDetailId).subscribe(result => {
                this.goodsDetail = result.goodsDetail;

                this.goodCategoryDisplayName = result.goodCategoryDisplayName;

                this.active = true;
                this.modal.show();
            });
        }
        this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown().subscribe(result => {						
						this.allGoodCategorys = result;
					});
					
    }

    save(): void {
            this.saving = true;

			
            this._goodsDetailsServiceProxy.createOrEdit(this.goodsDetail)
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
