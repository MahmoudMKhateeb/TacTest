import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { FacilityForDropdownDto, ShippingRequestsTachyonDealerServiceProxy, TachyonDealerBidDtoInupt } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import Swal from 'sweetalert2';

@Component({
  selector: 'tachyonDealToBiddingModal',
  templateUrl: './tachyonDealToBiddingModalComponent.html',
})
export class TachyonDealToBiddingModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('tachyonDealToBiddingModal', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();

  bidsSettings: TachyonDealerBidDtoInupt = new TachyonDealerBidDtoInupt();
  facilityLoading = false;
  saving = false;
  loading = true;
  today = new Date();

  constructor(injector: Injector, private _ShippingRequestsTachyonDealer: ShippingRequestsTachyonDealerServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(shippingRequestId): void {
    this.bidsSettings.id = shippingRequestId;
    this.modal.show();
  }
  close(): void {
    this.modal.hide();
  }

  /**
   * this function Changes the Shipping Request Type From TachyonDeal to TachyonDeal&Bidding at the same time
   * @Parm shippingRequestId
   */
  makeShippingRequestaBidding() {
    Swal.fire({
      title: this.l('pleaseConfirmthisAction'),
      icon: 'info',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
    }).then((result) => {
      if (result.value) {
        this._ShippingRequestsTachyonDealer.startBid(this.bidsSettings).subscribe(() => {
          this.close();
          Swal.fire('Success!', `You Successfully Started the Bidding on Shipping Request ` + this.bidsSettings.id, 'success');
          this.modalsave.emit('');
        });
      }
    });
  }
}
