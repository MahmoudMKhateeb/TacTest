import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NewPriceOfferListDto, ShipperDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-new-offers-widget',
  templateUrl: './new-offers-widget.component.html',
  styleUrls: ['./new-offers-widget.component.css'],
})
export class NewOffersWidgetComponent extends AppComponentBase implements OnInit {
  newOffers: NewPriceOfferListDto[] = [];

  constructor(injector: Injector, private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy, private router: Router) {
    super(injector);
  }

  ngOnInit(): void {
    this.getNewOffers();
  }

  getNewOffers(): void {
    this._shipperDashboardServiceProxy.getNewPriceOffers().subscribe((res) => {
      this.newOffers = res;
    });
  }

  goToPriceOffer(offer: NewPriceOfferListDto) {
    console.log('offer', offer);
    this.router.navigateByUrl(`/app/main/shippingRequests/shippingRequests/view?id=${offer.shippingRequestId}`);
  }
}
