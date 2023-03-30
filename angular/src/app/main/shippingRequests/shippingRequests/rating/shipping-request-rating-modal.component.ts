import { AfterViewInit, Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateShipperRateByCarrierDto, RatingServiceProxy } from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from 'codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-shipping-request-rating-modal',
  templateUrl: './shipping-request-rating-modal.component.html',
  styleUrls: ['./shipping-request-rating-modal.component.css'],
})
export class ShippingRequestRatingModalComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  rating: CreateShipperRateByCarrierDto = new CreateShipperRateByCarrierDto();
  ratingTripId: any;
  active: boolean = false;
  saving: boolean = false;
  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: RatingServiceProxy, private _Router: ActivatedRoute) {
    super(injector);
  }
  ngOnInit(): void {
    this.ratingTripId = this._Router.snapshot.queryParams['ratingTripId'];
  }

  public show(id: number): void {
    this.rating.tripId = id;
    this.active = true;
    this.modal.show();
  }

  ngAfterViewInit() {
    if (isNotNullOrUndefined(this.ratingTripId)) {
      this.show(this.ratingTripId);
    }
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
