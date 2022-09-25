import { AppConsts } from '@shared/AppConsts';
import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetActorForViewDto, ActorDto, ActorTypesEnum } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewActorModal',
  templateUrl: './view-actor-modal.component.html',
})
export class ViewActorModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  item: GetActorForViewDto;
  actorTypesEnum = ActorTypesEnum;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetActorForViewDto();
    this.item.actor = new ActorDto();
  }

  show(item: GetActorForViewDto): void {
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
