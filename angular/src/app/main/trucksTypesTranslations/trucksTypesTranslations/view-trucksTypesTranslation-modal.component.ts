import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetTrucksTypesTranslationForViewDto, TrucksTypesTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewTrucksTypesTranslationModal',
  templateUrl: './view-trucksTypesTranslation-modal.component.html',
})
export class ViewTrucksTypesTranslationModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetTrucksTypesTranslationForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetTrucksTypesTranslationForViewDto();
    this.item.trucksTypesTranslation = new TrucksTypesTranslationDto();
  }

  show(item: GetTrucksTypesTranslationForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
