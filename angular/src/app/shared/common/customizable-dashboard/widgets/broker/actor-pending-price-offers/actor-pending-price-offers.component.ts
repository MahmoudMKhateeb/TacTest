import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { BrokerDashboardServiceProxy, NewPriceOfferListDto } from '@shared/service-proxies/service-proxies';
import { Router } from '@angular/router';

@Component({
  selector: 'app-actor-pending-price-offers-widget',
  templateUrl: './actor-pending-price-offers.component.html',
  styleUrls: ['./actor-pending-price-offers.component.scss'],
})
export class ActorPendingPriceOffersComponent extends AppComponentBase implements OnInit {
  pendingOffers: NewPriceOfferListDto[] = [];

  constructor(injector: Injector, private _brokerDashboardServiceProxy: BrokerDashboardServiceProxy, private router: Router) {
    super(injector);
  }

  ngOnInit(): void {
    this.getPendingPriceOffers();
  }

  getPendingPriceOffers(): void {
    this._brokerDashboardServiceProxy.getPendingPriceOffers().subscribe((res) => {
      this.pendingOffers = res;
    });
  }

  goToPriceOffer(offer: NewPriceOfferListDto) {
    this.router.navigateByUrl(`/app/main/shippingRequests/shippingRequests/view?id=${offer.shippingRequestId}`);
  }
}
