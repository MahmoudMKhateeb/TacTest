import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateShipperRateByCarrierDto, RatingServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-shipping-request-rating-modal',
  templateUrl: './shipping-request-rating-modal.component.html',
  styleUrls: ['./shipping-request-rating-modal.component.css'],
})
export class ShippingRequestRatingModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  rating: CreateShipperRateByCarrierDto = new CreateShipperRateByCarrierDto();

  active: boolean = false;
  saving: boolean = false;
  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: RatingServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}

  public show(id: number): void {
    this.rating.tripId = id;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
    if (this.feature.isEnabled('App.Shipper')) {
      this._Service
        .createCarrierRatingByShipper(this.rating)
        .pipe(finalize(() => (this.saving = false)))
        .subscribe(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.close();
          this.modalSave.emit();
          this.rating.rate = 0;
          this.rating.note = ' ';
        });
    }
    if (this.feature.isEnabled('App.Carrier')) {
      this._Service
        .createShipperRatingByCarrier(this.rating)
        .pipe(finalize(() => (this.saving = false)))
        .subscribe(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.close();
          this.modalSave.emit();
          this.rating.rate = 0;
          this.rating.note = ' ';
        });
    }
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
