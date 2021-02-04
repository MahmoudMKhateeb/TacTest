import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { CreateOrEditGoodsDetailDto, GoodsDetailGoodCategoryLookupTableDto, GoodsDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'createOrEditGoodsDetailModal',
  templateUrl: './create-or-edit-goodsDetail-modal.component.html',
})
export class CreateOrEditGoodsDetailModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Input() CreateOrEditGoodDetailList: CreateOrEditGoodsDetailDto[];
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Output() modalEdit: EventEmitter<CreateOrEditGoodsDetailDto> = new EventEmitter<CreateOrEditGoodsDetailDto>();

  active = false;
  saving = false;

  goodsDetail: CreateOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();

  goodCategoryDisplayName = '';

  allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  private index: number;

  constructor(injector: Injector, private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy) {
    super(injector);
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown().subscribe((result) => {
      this.allGoodCategorys = result;
    });
  }

  show(): void {
    if (!this.goodsDetail) {
      this.goodsDetail = new CreateOrEditGoodsDetailDto();
    }
    this.active = true;
    this.modal.show();
  }

  edit(index?: number) {
    this.goodsDetail = this.CreateOrEditGoodDetailList[index];
    this.index = index;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;

    if (!this.CreateOrEditGoodDetailList) {
      this.CreateOrEditGoodDetailList = [];
    }

    if (this.index !== undefined) {
      this.CreateOrEditGoodDetailList[this.index] = this.goodsDetail;
    } else {
      this.CreateOrEditGoodDetailList.push(this.goodsDetail);
    }

    this.modalSave.emit(null);
    this.notify.info(this.l('SavedSuccessfully'));

    this.saving = false;
    this.close();
  }

  close(): void {
    this.index = undefined;
    this.goodsDetail = new CreateOrEditGoodsDetailDto();
    this.active = false;
    this.modal.hide();
  }
}
