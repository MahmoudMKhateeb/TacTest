import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetUnitOfMeasureForViewDto, UnitOfMeasureDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewUnitOfMeasureModal',
  templateUrl: './view-unitOfMeasure-modal.component.html',
})
export class ViewUnitOfMeasureModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetUnitOfMeasureForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetUnitOfMeasureForViewDto();
    this.item.unitOfMeasure = new UnitOfMeasureDto();
  }

  show(item: GetUnitOfMeasureForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
