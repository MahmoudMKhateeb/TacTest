import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetAllBidShippingRequestsForCarrierOutput,
  ShippingRequestBidsServiceProxy,
  CreatOrEditShippingRequestBidDto,
  CancelShippingRequestBidInput,
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
  record: GetAllBidShippingRequestsForCarrierOutput;
  placeBidInputs: CreatOrEditShippingRequestBidDto = new CreatOrEditShippingRequestBidDto();
  CancelShippingRequestBidInput: CancelShippingRequestBidInput = new CancelShippingRequestBidInput();

  constructor(injector: Injector, private router: Router, private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy) {
    super(injector);
    this.record = new GetAllBidShippingRequestsForCarrierOutput();
  }

  show(record: GetAllBidShippingRequestsForCarrierOutput): void {
    this.record = record;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.showSuccess = false;
    this.record = new GetAllBidShippingRequestsForCarrierOutput();
    this.placeBidInputs.id = null;
    this.saving = false;
    this.modal.hide();
    //this.price = null;
  }

  MakeABid(ShippingReqid: number) {
    let x;
    x = Swal.fire(this.l('Success'), this.l('bidPlacedSuccessfully'), 'success');
    if (this.price > 0) {
      this.invalid = false;
      this.saving = true;
      if (this.record.myBidPrice !== 0) {
        this.placeBidInputs.id = this.record.myBidId;
        x = Swal.fire(this.l('Success'), this.l('bidUpdatedSuccessfully'), 'success');
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
      title: this.l('areYouSure?'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.saving = true;
        this.CancelShippingRequestBidInput.shippingRequestBidId = bidId;
        this.CancelShippingRequestBidInput.cancledReason = null;
        this._shippingRequestBidsServiceProxy
          .cancelShippingRequestBid(this.CancelShippingRequestBidInput)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          )
          .subscribe(() => {
            this.close();
            Swal.fire(this.l('Success'), this.l('bidRequestRemoved'), 'success');
            this.modalSave.emit(null);
          });
      } //end of if
    });
  }
}
