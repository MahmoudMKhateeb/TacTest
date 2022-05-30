import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { SrPostPriceUpdateAction, SrPostPriceUpdateServiceProxy, ViewSrPostPriceUpdateDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'ViewSrPostPriceUpdatesModal',
  templateUrl: './view-sr-post-price-update-modal.component.html',
  styleUrls: ['./view-sr-post-price-update-modal.component.css'],
})
export class ViewSrPostPriceUpdateModalComponent extends AppComponentBase {
  @ViewChild('viewSrUpdateModal') modal: ModalDirective;
  srUpdateDto: ViewSrPostPriceUpdateDto;
  rejectAction = SrPostPriceUpdateAction.Reject;
  active: boolean;
  loading: boolean;

  constructor(private injector: Injector, private _serviceProxy: SrPostPriceUpdateServiceProxy) {
    super(injector);
    this.active = false;
    this.loading = false;
  }

  close() {
    this.modal.hide();
    this.active = false;
    this.srUpdateDto = undefined;
  }

  show(updateId: number) {
    this.srUpdateDto = new ViewSrPostPriceUpdateDto();
    this.active = true;
    this.modal.show();
    this.getSrPostPriceUpdateForView(updateId);
  }

  private getSrPostPriceUpdateForView(srUpdateId: number) {
    this.loading = true;
    this._serviceProxy.getForView(srUpdateId).subscribe((result) => {
      this.srUpdateDto = result;
      this.loading = false;
    });
  }
}
