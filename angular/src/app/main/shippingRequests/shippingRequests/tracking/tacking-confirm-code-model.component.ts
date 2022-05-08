import { Component, Injector, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';
import { InvokeStatusInputDto, TrackingServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'tacking-confirm-code-model',
  templateUrl: './tacking-confirm-code-model.component.html',
})
export class TrackingConfirmModalComponent extends AppComponentBase {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  active = false;
  saving = false;
  code: string;
  pointId: number;
  action: string;
  constructor(injector: Injector, private _trackingServiceProxy: TrackingServiceProxy) {
    super(injector);
  }
  public show(pointId: number, action: string): void {
    this.pointId = pointId;
    this.action = action;
    this.active = true;
    this.code = undefined;
    this.modal.show();
  }
  save(): void {
    this.saving = true;
    const invokeRequestBody = new InvokeStatusInputDto();
    invokeRequestBody.code = this.code;
    invokeRequestBody.id = this.pointId;
    invokeRequestBody.action = this.action;
    this._trackingServiceProxy
      .invokeStatus(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(
        () => {
          this.close();
          abp.event.trigger('trackingConfirmCodeSubmitted');
        },
        () => {
          this.notify.error(this.l('InvalidCode'));
        }
      );
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
