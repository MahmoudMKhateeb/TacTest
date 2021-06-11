import { Component, OnInit, Injector, Input } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GetShippingRequestForPriceOfferListDto, PriceOfferChannel, PriceOfferServiceProxy } from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { ScrollPagnationComponentBase } from '@shared/common/scroll/scroll-pagination-component-base';
@Component({
  templateUrl: './shipping-request-card-template.component.html',
  styleUrls: ['/assets/custom/css/style.scss'],
  selector: 'shipping-request-card-template',
  animations: [appModuleAnimation()],
})
export class ShippingRequestCardTemplateComponent extends ScrollPagnationComponentBase implements OnInit {
  Items: GetShippingRequestForPriceOfferListDto[] = [];
  @Input() Channel: PriceOfferChannel | null | undefined = undefined;
  @Input() Title: string;
  @Input() ShippingRequestId: number | null | undefined = undefined;
  direction = 'ltr';
  constructor(injector: Injector, private _currentServ: PriceOfferServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.LoadData();
  }
  LoadData() {
    this._currentServ
      .getAllShippingRequest(undefined, this.ShippingRequestId, this.Channel, '', this.skipCount, this.maxResultCount)
      .subscribe((result) => {
        this.IsLoading = false;
        if (result.items.length < this.maxResultCount) {
          this.StopLoading = true;
        }
        this.Items.push(...result.items);
      });
  }

  /*delete(input: GetShippingRequestForPriceOfferListDto): void {
        this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
            if (isConfirmed) {
                this._CurrentServ.delete(input.id).subscribe(() => {
                    this.notify.success(this.l('SuccessfullyDeleted'));
                });
            }
        });
    }
    */
  getWordTitle(n: any, word: string): string {
    if (parseInt(n) == 1) {
      return this.l(word);
    }
    return this.l(`${word}s`);
  }
}
