import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import {
  PricePackageOfferStatus,
  TmsPricePackageForPricingDto,
  TmsPricePackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-tms-price-package-for-pricing-modal',
  templateUrl: './tms-price-package-for-pricing-modal.component.html',
  styleUrls: ['./tms-price-package-for-pricing-modal.component.css'],
})
export class TmsPricePackageForPricingModalComponent extends AppComponentBase implements OnInit {
  pricePackage: TmsPricePackageForPricingDto;
  pricePackageStatus = PricePackageOfferStatus;
  @Input() shippingRequestId: number;
  isLoading: boolean;
  isAcceptLoading: boolean;

  @ViewChild('Modal', { static: true }) modal: ModalDirective;

  constructor(private injector: Injector, private _tmsPricePackageService: TmsPricePackageServiceProxy) {
    super(injector);
  }

  show(pricePackageId: number) {
    this._tmsPricePackageService.getForPricing(pricePackageId).subscribe((result) => {
      this.pricePackage = result;
      this.modal.show();
    });
  }

  close() {
    this.pricePackage = new TmsPricePackageForPricingDto();
    this.modal.hide();
    this.isLoading = false;
  }

  ngOnInit(): void {
    this.pricePackage = new TmsPricePackageForPricingDto();
    this.isLoading = false;
    this.isAcceptLoading = false;
  }

  sendOffer() {
    this.isLoading = true;
    this._tmsPricePackageService.sendOfferByPricePackage(this.pricePackage.id, this.shippingRequestId).subscribe(() => {

      this._tmsPricePackageService.getForPricing(this.pricePackage.id).subscribe((result) => {
        this.isLoading = false;
        this.notify.success(this.l('SavedSuccessfully'));
        this.pricePackage = result;
      });
    });
  }

    acceptOffer() {
      this.isAcceptLoading = true;
        this._tmsPricePackageService.acceptOfferByPricePackage(this.pricePackage.id).subscribe(() => {
            this._tmsPricePackageService.getForPricing(this.pricePackage.id).subscribe((result) => {
                this.isAcceptLoading = false;
                this.notify.success(this.l('SavedSuccessfully'));
                this.pricePackage = result;
            });
        });
    }
}
