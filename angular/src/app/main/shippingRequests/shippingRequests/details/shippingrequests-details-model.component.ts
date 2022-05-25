import { Component, EventEmitter, Injector, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import {
  GetShippingRequestForPriceOfferListDto,
  GetShippingRequestForPricingOutput,
  PriceOfferChannel,
  PriceOfferItemDto,
  PriceOfferServiceProxy,
  ShippingRequestBidStatus,
  ShippingRequestStatus,
  ShippingRequestDirectRequestStatus,
} from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: './shippingrequests-details-model.component.html',
  styleUrls: ['./shippingrequests-details-model.component.css'],
  selector: 'shippingrequests-details-model',
  animations: [appModuleAnimation()],
})
export class ShippingrequestsDetailsModelComponent extends AppComponentBase {
  @Input() Channel: PriceOfferChannel | null | undefined;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  request: GetShippingRequestForPricingOutput = new GetShippingRequestForPricingOutput();
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  distance: string;
  duration: string;
  direction: string;
  Items: PriceOfferItemDto[] = [];
  shippingrequest: GetShippingRequestForPriceOfferListDto = new GetShippingRequestForPriceOfferListDto();
  constructor(injector: Injector, private _CurrentServ: PriceOfferServiceProxy) {
    super(injector);
  }

  show(request: GetShippingRequestForPriceOfferListDto): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.shippingrequest = request;
    //console.log(this.shippingrequest);
    this._CurrentServ.getShippingRequestForPricing(this.Channel, this.shippingrequest.id).subscribe((result) => {
      this.request = result;
      this.Items = this.request.items;
      this.active = true;
      this.getCordinatesByCityName(this.request.originCity, 'source');
      this.getCordinatesByCityName(this.request.destinationCity, 'destanation');
      this.modal.show();
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  update(offerId: number) {
    this.shippingrequest.offerId = offerId;
    this.shippingrequest.isPriced = true;
  }
  markAsPriced(offerId: number) {
    this.shippingrequest.isPriced = true;
    if (this.shippingrequest.isBid) {
      this.shippingrequest.offerId = offerId;
    } else {
      this.shippingrequest.directRequestStatus = ShippingRequestDirectRequestStatus.Accepted;
    }
  }
  delete() {
    this.shippingrequest.offerId = undefined;
    this.shippingrequest.isPriced = false;
  }
  /**
   * Check the current user log in can set price or not
   */
  canSetPrice(): boolean {
    if (!this.Channel) return false;
    if (this.shippingrequest.status != ShippingRequestStatus.PrePrice && this.shippingrequest.status != ShippingRequestStatus.NeedsAction)
      return false;
    if (this.shippingrequest.isPriced) return false;
    if (this.shippingrequest.directRequestStatus != ShippingRequestDirectRequestStatus.New) return false;
    if (this.request.status != ShippingRequestStatus.NeedsAction && this.request.status != ShippingRequestStatus.PrePrice) return false;
    if (this.Channel == PriceOfferChannel.MarketPlace && this.request.bidStatus != ShippingRequestBidStatus.OnGoing) return false;
    if (this.feature.isEnabled('App.Shipper')) return false;
    if (this.feature.isEnabled('App.Carrier')) return true;
    if (this.feature.isEnabled('App.TachyonDealer') && !this.request.isTachyonDeal) return true;
    return false;
  }
  /**
   * Get City Cordinates By Providing its name
   * this finction is to draw the shipping Request Main Route in View SR Details in marketPlace
   * @param cityName
   * @param cityType   source/dest
   */
  getCordinatesByCityName(cityName: string, cityType: string) {
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode(
      {
        address: cityName,
      },
      (results, status) => {
        if (status == google.maps.GeocoderStatus.OK) {
          const Lat = results[0].geometry.location.lat();
          const Lng = results[0].geometry.location.lng();
          if (cityType == 'source') {
            this.origin = { lat: Lat, lng: Lng };
          } else {
            this.destination = { lat: Lat, lng: Lng };
            this.messuareDistance(this.origin, this.destination);
          }
        } else {
          console.log('Something got wrong ' + status);
        }
      }
    );
  }

  /**
   * Measure the Distance Between 2 Points using Cordinates
   * @param Oring { lat: null, lng: null }
   * @param destination { lat: null, lng: null }
   */

  messuareDistance(origin, destination) {
    origin = this.origin;
    destination = this.destination;
    const service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix(
      {
        origins: [{ lat: origin.lat, lng: origin.lng }],
        destinations: [{ lat: destination.lat, lng: destination.lng }],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false,
      },
      (response, status) => {
        this.distance = response.rows[0].elements[0].distance.text;
        this.duration = response.rows[0].elements[0].duration.text;
      }
    );
  }
}
