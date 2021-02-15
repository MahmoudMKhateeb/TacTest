import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { CreateOrEditGoodsDetailDto, GoodsDetailsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'GoodDetailsForCreateShippingRequest',
  templateUrl: './GoodDetailsForCreateShippingRequest.html',
})
export class GoodDetailsForCreateShippingRequstComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditGoodDetail') public createOrEditGoodDetail: ModalDirective;

  @Output() SelectedGoodDetailsFromChild: EventEmitter<CreateOrEditGoodsDetailDto[]> = new EventEmitter<CreateOrEditGoodsDetailDto[]>();
  constructor(injector: Injector, private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy) {
    super(injector);
  }
  // One Good Detail
  goodsDetail: CreateOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();
  //all the goods Details
  GoodDetailsList: CreateOrEditGoodsDetailDto[] = [];
  active: any;
  saving: boolean;
  index: number;
  allGoodCategorys: any;

  ngOnInit() {
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });
  }

  show(): void {
    if (!this.goodsDetail) {
      this.goodsDetail = new CreateOrEditGoodsDetailDto();
    }
    this.active = true;
    this.createOrEditGoodDetail.show();
  }

  edit(index?: number) {
    this.goodsDetail = this.GoodDetailsList[index];
    this.index = index;
    this.active = true;
    this.createOrEditGoodDetail.show();
  }
  delete(index: number) {
    this.GoodDetailsList.splice(index, 1);
    this.notify.info(this.l('SuccessfullyDeleted'));
    this.EmitToFather();
  }
  save(): void {
    this.saving = true;

    if (!this.GoodDetailsList) {
      this.GoodDetailsList = [];
    }

    if (this.index !== undefined) {
      this.GoodDetailsList[this.index] = this.goodsDetail;
    } else {
      this.GoodDetailsList.push(this.goodsDetail);
    }

    this.EmitToFather();
    this.notify.info(this.l('SavedSuccessfully'));
    console.log(this.GoodDetailsList);
    this.saving = false;
    this.close();
  }
  EmitToFather() {
    this.SelectedGoodDetailsFromChild.emit(this.GoodDetailsList);
    this.goodsDetail = new CreateOrEditGoodsDetailDto();
    this.createOrEditGoodDetail.hide();
  }

  close(): void {
    this.index = undefined;
    this.goodsDetail = new CreateOrEditGoodsDetailDto();
    this.active = false;
    this.createOrEditGoodDetail.hide();
  }
}
