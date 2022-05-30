import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateSrPostPriceUpdateActionDto, SrPostPriceUpdateAction, SrPostPriceUpdateServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'RejectSrPostPriceUpdateModal',
  templateUrl: './reject-post-price-update.component.html',
  styleUrls: ['./reject-post-price-update.component.css'],
})
export class RejectPostPriceUpdateComponent extends AppComponentBase implements OnInit {
  @ViewChild('rejectUpdateModal') modal: ModalDirective;
  @Output('rejectSubmitted') modalSave: EventEmitter<void>;
  updateAction: CreateSrPostPriceUpdateActionDto;
  saving: boolean;

  constructor(private injector: Injector, private _serviceProxy: SrPostPriceUpdateServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.updateAction = new CreateSrPostPriceUpdateActionDto();
    this.updateAction.action = SrPostPriceUpdateAction.Reject;
    this.modalSave = new EventEmitter<void>();
    this.saving = false;
  }

  show(updateId: number) {
    this.updateAction.id = updateId;
    this.modal.show();
  }

  close() {
    this.updateAction = undefined;
    this.modal.hide();
  }

  rejectUpdate() {
    this.saving = true;

    this._serviceProxy
      .createUpdateAction(this.updateAction)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('RejectedSuccessfully'));
        this.modalSave.emit();
        this.close();
      });
  }
}
