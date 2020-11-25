import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetDocumentFileForViewDto,
  DocumentFileDto,
  ViewShipperBidsReqDetailsOutput,
  ShippingRequestBidsServiceProxy,
  StopShippingRequestBidInput,
  CreatOrEditShippingRequestBidDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'ViewShippingRequestDetailsModal',
  styleUrls: ['./marketPlaceStyling.css'],
  templateUrl: './view-shipping-request.modal.html',
})
export class ViewShippingRequestDetailsComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  price = null;
  showSuccess = false;
  active = false;
  saving = false;

  record: ViewShipperBidsReqDetailsOutput;
  placeBidInputs: CreatOrEditShippingRequestBidDto = new CreatOrEditShippingRequestBidDto();

  constructor(injector: Injector, private router: Router, private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy) {
    super(injector);
    this.record = new ViewShipperBidsReqDetailsOutput();
  }

  show(record: ViewShipperBidsReqDetailsOutput): void {
    this.record = record;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.showSuccess = false;
    this.modal.hide();
  }

  MakeABid(ShippingReqid: number) {
    if (this.price > 0) {
      this.showSuccess = true;
      this.saving = true;
      if (this.record.bidsNo !== 0) {
        this.placeBidInputs.id = 1;
        console.log('edited To :', this.price);
      }
      this.placeBidInputs.shippingRequestId = ShippingReqid;
      this.placeBidInputs.price = this.price;

      this._shippingRequestBidsServiceProxy
        .createOrEditShippingRequestBid(this.placeBidInputs)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          //  this.notify.info(this.l('SavedSuccessfully'));
          this.close();
          this.modalSave.emit(null);
        });
      console.log('Created Bid :', this.price);
    }
  }

  EditMyBid(id: number, newPrice: number) {}
}
