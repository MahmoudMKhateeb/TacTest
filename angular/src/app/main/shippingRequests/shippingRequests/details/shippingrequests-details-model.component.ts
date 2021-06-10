import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

import { ShippingRequestsServiceProxy, GetShippingRequestForPricingOutput } from '@shared/service-proxies/service-proxies';

import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
@Component({
  templateUrl: './shippingrequests-details-model.component.html',
  styleUrls: ['./shippingrequests-details-model.component.scss'],
  selector: 'shippingrequests-details-model',
  animations: [appModuleAnimation()],
})
export class ShippingrequestsDetailsModelComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  item: GetShippingRequestForPricingOutput = new GetShippingRequestForPricingOutput();
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  distance: string;
  duration: string;
  direction: string;
  constructor(injector: Injector, private _CurrentServ: ShippingRequestsServiceProxy) {
    super(injector);
  }

  show(id: number): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this._CurrentServ.getShippingRequestForPricing(id).subscribe((result) => {
      this.item = result;
      this.active = true;
      this.getCordinatesByCityName(this.item.originCity, 'source');
      this.getCordinatesByCityName(this.item.destinationCity, 'destanation');
      this.modal.show();
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  save(): void {
    this.saving = true;
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
