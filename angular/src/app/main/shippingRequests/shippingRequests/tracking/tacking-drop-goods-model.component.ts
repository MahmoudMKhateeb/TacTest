import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TrackingServiceProxy } from '@shared/service-proxies/service-proxies';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'tacking-drop-goods-model',
  templateUrl: './tacking-drop-goods-model.component.html',
})
export class TackingDropGoodsModelComponent extends AppComponentBase {
  @Output() modalConfirm: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active: boolean = false;
  saving: boolean = false;
  id: number;
  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: TrackingServiceProxy, private _httpClient: HttpClient) {
    super(injector);
  }

  public show(id: number): void {
    this.id = id;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
  }

  close(): void {
    this.modal.hide();
    this.modalConfirm.emit(null);
    this.active = false;
  }
}
