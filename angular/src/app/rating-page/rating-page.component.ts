import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { pickBy } from 'lodash-es';
import {
  CreateDeliveryExpRateByReceiverDto,
  CreateDriverAndDERatingByReceiverDto,
  CreateDriverRatingDtoByReceiverDto,
} from '@shared/service-proxies/service-proxies';
import { RatingServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from '@node_modules/rxjs/operators';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-rating-page',
  templateUrl: './rating-page.component.html',
  styleUrls: ['./rating-page.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class RatingPageComponent extends AppComponentBase implements OnInit {
  val1: any;
  val2: any;
  saving = false;
  RatingTypeId: number;
  rateDto: CreateDriverAndDERatingByReceiverDto = new CreateDriverAndDERatingByReceiverDto();
  note: string;
  rate1: number;
  rate2: number;
  code = this._Activatedroute.snapshot.paramMap.get('code');

  constructor(injector: Injector, private _ratingServiceProxy: RatingServiceProxy, private _Activatedroute: ActivatedRoute) {
    super(injector);
  }

  ngOnInit(): void {}
  onSubmit() {
    this.saving = true;
    this.rateDto.createDriverRatingDtoByReceiverInput = new CreateDriverRatingDtoByReceiverDto();
    this.rateDto.createDeliveryExpRateByReceiverInput = new CreateDeliveryExpRateByReceiverDto();
    this.rateDto.createDeliveryExpRateByReceiverInput.rate = this.rate1;
    this.rateDto.createDriverRatingDtoByReceiverInput.rate = this.rate2;

    this.rateDto.createDeliveryExpRateByReceiverInput.code = this.code;
    this.rateDto.createDriverRatingDtoByReceiverInput.code = this.code;

    this.rateDto.createDeliveryExpRateByReceiverInput.note = this.note;
    this.rateDto.createDriverRatingDtoByReceiverInput.note = this.note;
    this._ratingServiceProxy

      .createDriverAndDERatingByReceiver(this.rateDto)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe((res) => {
        this.notify.success(this.l('RatingSuccessfully'));
      });
  }
}
