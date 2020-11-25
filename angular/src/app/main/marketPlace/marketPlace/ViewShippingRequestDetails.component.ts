import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  ViewShipperBidsReqDetailsOutput,
  ShippingRequestBidsServiceProxy,
  CreatOrEditShippingRequestBidDto,
  ShippingRequestBidInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';
import Swal from 'sweetalert2';

@Component({
  selector: 'ViewShippingRequestDetailsModal',
  styleUrls: ['./marketPlaceStyling.css'],
  templateUrl: './view-shipping-request.modal.html',
})
export class ViewShippingRequestDetailsComponent extends AppComponentBase {
  @ViewChild('ViewShippingRequestModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  price = null;
  showSuccess = false;
  active = false;
  saving = false;
  invalid = false;

  record: ViewShipperBidsReqDetailsOutput;
  placeBidInputs: CreatOrEditShippingRequestBidDto = new CreatOrEditShippingRequestBidDto();

  DeleteBidinputs: ShippingRequestBidInput = new ShippingRequestBidInput();

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
    this.record = new ViewShipperBidsReqDetailsOutput();
    this.placeBidInputs.id = null;
    this.saving = false;
    this.modal.hide();
    //this.price = null;
    console.log('closed');
  }

  MakeABid(ShippingReqid: number) {
    let x;
    x = Swal.fire(this.l('Success'), this.l('Bid Placed Successfully'), 'success');
    if (this.price > 0) {
      this.invalid = false;
      this.saving = true;
      if (this.record.firstBidId !== 0) {
        this.placeBidInputs.id = this.record.firstBidId;
        x = Swal.fire(this.l('Success'), this.l('Bid Updated Successfully'), 'success');
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
          this.close();
          this.modalSave.emit(null);
          x;
        });
      console.log('Created Bid :', this.price);
    } else {
      this.saving = false;
      this.showSuccess = false;
      this.invalid = true;
    }
  }

  DeleteBid(bidId: number) {
    Swal.fire({
      title: this.l('Are you sure you want Cancel this bid ?'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.saving = true;
        this.DeleteBidinputs.shippingRequestBidId = bidId;
        this.DeleteBidinputs.cancledReason = null;
        this._shippingRequestBidsServiceProxy
          .cancelBidRequest(this.DeleteBidinputs)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          )
          .subscribe(() => {
            this.close();
            Swal.fire(this.l('Success'), this.l('Bid Request Removed'), 'success');
            this.modalSave.emit(null);
          });
      } //end of if
    });
  }
}
