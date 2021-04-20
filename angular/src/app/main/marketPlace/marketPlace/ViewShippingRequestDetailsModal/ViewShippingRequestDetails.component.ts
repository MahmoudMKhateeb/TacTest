import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetAllBidShippingRequestsForCarrierOutput,
  ShippingRequestBidsServiceProxy,
  CreatOrEditShippingRequestBidDto,
  CancelShippingRequestBidInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';
import Swal from 'sweetalert2';

@Component({
  selector: 'ViewShippingRequestDetailsModal',
  styleUrls: ['../marketPlace/marketPlaceStyling.css'],
  templateUrl: './view-shipping-request.modal.html',
})
export class ViewShippingRequestDetailsComponent extends AppComponentBase {
  @ViewChild('ViewShippingRequestModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  price: number | undefined | null = null;
  showSuccess = false;
  active = false;
  saving = false;
  invalid = false;
  record: GetAllBidShippingRequestsForCarrierOutput;
  placeBidInputs: CreatOrEditShippingRequestBidDto = new CreatOrEditShippingRequestBidDto();
  CancelShippingRequestBidInput: CancelShippingRequestBidInput = new CancelShippingRequestBidInput();
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };

  distance: string;
  duration: string;

  loading = true;
  constructor(injector: Injector, private router: Router, private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy) {
    super(injector);
    this.record = new GetAllBidShippingRequestsForCarrierOutput();
  }

  show(record: GetAllBidShippingRequestsForCarrierOutput): void {
    this.record = record;
    this.getCordinatesByCityName(record.sourceCityName, 'source');
    this.getCordinatesByCityName(record.destinationCityName, 'destanation');
    this.price = this.record.myBidPrice ?? null;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.showSuccess = false;
    this.record = new GetAllBidShippingRequestsForCarrierOutput();
    this.placeBidInputs.id = null;
    this.saving = false;
    this.origin = { lat: null, lng: null };
    this.destination = { lat: null, lng: null };
    this.loading = true;
    this.modal.hide();
    //this.price = null;
  }

  /**
   * Places A Bid On A Shipping Request In MarketPlace
   * @param ShippingReqid
   * @constructor
   */
  placeBid(ShippingReqid: number) {
    let x;
    x = Swal.fire(this.l('Success'), this.l('bidPlacedSuccessfully'), 'success');
    if (this.price > 0) {
      this.invalid = false;
      this.saving = true;
      if (this.record.myBidPrice !== 0) {
        this.placeBidInputs.id = this.record.myBidId;
        x = Swal.fire(this.l('Success'), this.l('bidUpdatedSuccessfully'), 'success');
      }
      this.placeBidInputs.shippingRequestId = ShippingReqid;
      this.placeBidInputs.basePrice = this.price;
      this._shippingRequestBidsServiceProxy
        .createOrEditShippingRequestBid(this.placeBidInputs)
        .pipe(
          finalize(() => {
            this.saving = false;
          })
        )
        .subscribe(() => {
          this.close();
          this.modalSave.emit(null);
          x;
        });
    } else {
      this.saving = false;
      this.showSuccess = false;
      this.invalid = true;
    }
  }

  /**
   * Delets Carrier Shipping Request Bid
   * @param bidId
   * @constructor
   */
  CancelBid(bidId: number) {
    Swal.fire({
      title: this.l('areYouSure?'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.saving = true;
        this.CancelShippingRequestBidInput.shippingRequestBidId = bidId;
        this.CancelShippingRequestBidInput.cancledReason = null;
        this._shippingRequestBidsServiceProxy
          .cancelShippingRequestBid(this.CancelShippingRequestBidInput)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          )
          .subscribe(() => {
            this.close();
            Swal.fire(this.l('Success'), this.l('bidRequestRemoved'), 'success');
            this.modalSave.emit(null);
          });
      } //end of if
    });
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
    this.loading = false;
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
