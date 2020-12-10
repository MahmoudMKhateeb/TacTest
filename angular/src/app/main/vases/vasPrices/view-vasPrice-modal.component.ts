import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetVasPriceForViewDto, VasPriceDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewVasPriceModal',
  templateUrl: './view-vasPrice-modal.component.html',
})
export class ViewVasPriceModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetVasPriceForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetVasPriceForViewDto();
    this.item.vasPrice = new VasPriceDto();
  }

  show(item: GetVasPriceForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
