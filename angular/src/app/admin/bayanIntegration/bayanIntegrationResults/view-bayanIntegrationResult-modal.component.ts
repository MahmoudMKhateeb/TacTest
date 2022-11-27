import { AppConsts } from '@shared/AppConsts';
import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetBayanIntegrationResultForViewDto, BayanIntegrationResultDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewBayanIntegrationResultModal',
  templateUrl: './view-bayanIntegrationResult-modal.component.html',
})
export class ViewBayanIntegrationResultModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetBayanIntegrationResultForViewDto;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetBayanIntegrationResultForViewDto();
    this.item.bayanIntegrationResult = new BayanIntegrationResultDto();
  }

  show(item: GetBayanIntegrationResultForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
