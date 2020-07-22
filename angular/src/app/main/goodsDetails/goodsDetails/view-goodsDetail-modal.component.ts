import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetGoodsDetailForViewDto, GoodsDetailDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewGoodsDetailModal',
  templateUrl: './view-goodsDetail-modal.component.html',
})
export class ViewGoodsDetailModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetGoodsDetailForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetGoodsDetailForViewDto();
    this.item.goodsDetail = new GoodsDetailDto();
  }

  show(item: GetGoodsDetailForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
