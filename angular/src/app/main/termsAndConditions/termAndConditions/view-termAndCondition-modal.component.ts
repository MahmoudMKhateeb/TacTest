import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTermAndConditionForViewDto, TermAndConditionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTermAndConditionModal',
  templateUrl: './view-termAndCondition-modal.component.html',
})
export class ViewTermAndConditionModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTermAndConditionForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTermAndConditionForViewDto();
    this.item.termAndCondition = new TermAndConditionDto();
  }

  show(item: GetTermAndConditionForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
