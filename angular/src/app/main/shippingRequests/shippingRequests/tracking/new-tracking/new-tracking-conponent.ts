import { Component, Injector, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  PickingType,
  RoutePointStatus,
  ShippingRequestTripDriverRoutePointDto,
  ShippingRequestTripStatus,
  TrackingListDto,
  TrackingServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';
import { TrackingPODModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-pod-model.component';
import { UploadDeliveryNoteDocumentModelComponent } from '@app/main/shippingRequests/shippingRequests/tracking/upload-delivery-note-document-model/upload-delivery-note-document-model.component';

@Component({
  selector: 'new-tracking-conponent',
  templateUrl: './new-tracking-conponent.html',
  styleUrls: ['./new-tracking-conponent.css'],
})
export class NewTrackingConponent extends AppComponentBase implements OnChanges {
  @ViewChild('modelconfirm', { static: false }) modelConfirm: TrackingConfirmModalComponent;
  @ViewChild('modelpod', { static: false }) modelpod: TrackingPODModalComponent;
  @ViewChild('modelDeliveryNote', { static: false }) modelDeliveryNote: UploadDeliveryNoteDocumentModelComponent;
  @Input() trip: TrackingListDto = new TrackingListDto();
  active = false;
  item: number;
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  routePoints: ShippingRequestTripDriverRoutePointDto[];
  pointsIsLoading = true;
  distance: string;
  duration: string;
  readonly zoom: number = 15;
  saving = false;
  markerLong: number;
  markerLat: number;
  markerFacilityName: string;

  constructor(injector: Injector, private _trackingServiceProxy: TrackingServiceProxy) {
    super(injector);
  }

  /**
   * to Detect Changes On Variables
   * @param changes
   */
  ngOnChanges(changes: SimpleChanges) {
    this.getForView();
    this.getCordinatesByCityName(this.trip.origin, 'source');
    this.getCordinatesByCityName(this.trip.destination, 'destanation');
  }

  /**
   * Show Item
   * @param item
   */
  show(item: TrackingListDto) {
    this.active = true;
    this.trip = item;
    this.getForView();
    // this.modal.show();
  }

  getForView() {
    this._trackingServiceProxy
      .getForView(this.trip.id)
      .pipe(
        finalize(() => {
          this.pointsIsLoading = false;
        })
      )
      .subscribe((result) => {
        this.routePoints = result.items;
      });
  }

  start(): void {
    this._trackingServiceProxy
      .start(this.trip.id)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.getForView();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyStarted'));
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

  /**
   * accepts the trip
   */
  accept(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this._trackingServiceProxy
          .accept(this.trip.id)
          .pipe(
            finalize(() => {
              //Refresh The Data After Status Change
              setTimeout(() => {
                this.getForView();
                this.saving = false;
              }, 5000);
            })
          )
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyAccepted'));
          });
      }
    });
  }

  /**
   * changes Point Status To the Next Status
   * @param point
   */
  changeStatus(point: ShippingRequestTripDriverRoutePointDto): void {
    if (point.status == RoutePointStatus.FinishOffLoadShipment) {
      this.modelConfirm.show(point.id);
      return;
    } else if (point.status == RoutePointStatus.ReceiverConfirmed) {
      this.modelpod.show(point.id);
      return;
    }

    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        if (point.pickingType == PickingType.Pickup && point.status == RoutePointStatus.StandBy) {
          this.start();
        } else if (point.pickingType == PickingType.Dropoff && point.status == RoutePointStatus.StandBy) {
          this.nextLocation(point);
        } else {
          this.change(point);
        }
      }
    });
  }

  /**
   * go to next Location
   * @param point
   */
  nextLocation(point: ShippingRequestTripDriverRoutePointDto): void {
    this._trackingServiceProxy
      .nextLocation(point.id)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.getForView();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyChanged'));
      });
  }

  /**
   * change Status
   * @param point
   */
  change(point: ShippingRequestTripDriverRoutePointDto): void {
    this._trackingServiceProxy
      .changeStatus(point.shippingRequestTripId)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.getForView();
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyChanged'));
      });
  }

  /**
   * checks if the Current Point is Allowed to go to the next one
   * @param point
   * @param index
   */
  canGoNext(point: ShippingRequestTripDriverRoutePointDto, index: number) {
    //if its the last Point hide the Go next Button
    if (index == this.routePoints.length - 1) return false;
    if (point.pickingType == 2 && point.status == 8 && this.routePoints[index + 1]?.status < 4) {
      return true;
    }
    return false;
  }

  /**
   * time line Panel Click
   * @param $event
   */
  timeLinePanelClick(point: ShippingRequestTripDriverRoutePointDto) {
    this.item = point.id;
    if (this.markerLong == point.lng && this.markerLat == point.lat) {
      this.markerLong = null;
      this.markerLat = null;
      this.markerFacilityName = '';
      return;
    }
    this.markerLong = point.lng;
    this.markerLat = point.lat;
    this.markerFacilityName = point.facility;
  }

  /**
   * checks if the user and point allowed to Upload DeliveryNote
   * @param point
   */
  canUploadDeliveryNote(point: ShippingRequestTripDriverRoutePointDto) {
    if (this.trip.needsDeliveryNote && !point.isDeliveryNoteUploaded && point.status === 9) {
      return true;
    }
    return false;
  }
}
