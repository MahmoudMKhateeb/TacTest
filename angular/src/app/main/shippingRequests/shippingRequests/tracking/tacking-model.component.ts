import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter, NgZone } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';

import {
  TrackingServiceProxy,
  TrackingListDto,
  ShippingRequestTripDriverRoutePointDto,
  PickingType,
  RoutePointStatus,
  ShippingRequestTripStatus,
  ShippingRequestTripDriverStatus,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './tacking-model.component.html',
  styleUrls: ['./tacking-model.component.scss'],
  selector: 'tacking-model',
  animations: [appModuleAnimation()],
})
export class TrackingModelComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('modelconfirm', { static: false }) modelConfirm: TrackingConfirmModalComponent;
  _zone: NgZone;
  item: TrackingListDto = new TrackingListDto();
  routePoints: ShippingRequestTripDriverRoutePointDto[] = [];
  pickup: ShippingRequestTripDriverRoutePointDto = new ShippingRequestTripDriverRoutePointDto();
  drops: ShippingRequestTripDriverRoutePointDto[] = [];
  active = false;
  saving = false;
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  distance: string;
  duration: string;
  direction: string;
  constructor(injector: Injector, private _CurrentServ: TrackingServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.registerToEvents();
  }

  show(trip: TrackingListDto): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.item = trip;
    this.getCordinatesByCityName(this.item.origin, 'source');
    this.getCordinatesByCityName(this.item.destination, 'destanation');
    this._CurrentServ.getForView(this.item.id).subscribe((result) => {
      this.routePoints = result.items;
      this.getPickup();
      this.getDrops();
      this.active = true;
      this.modal.show();
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  canChangeDropStatus(drop: ShippingRequestTripDriverRoutePointDto) {
    if (this.pickup.status == RoutePointStatus.FinishLoading && this.item.status == ShippingRequestTripStatus.Intransit && !this.saving) {
      if (this.drops.findIndex((x) => x.isActive && x.id != drop.id) == -1) {
        return true;
      }
    }
    return false;
  }
  getPickup() {
    this.pickup = this.routePoints.find((x) => x.pickingType == PickingType.Pickup);
  }
  getDrops() {
    this.drops = this.routePoints.filter((x) => x.pickingType == PickingType.Dropoff);
  }

  accept(): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this._CurrentServ
          .accept(this.item.id)
          .pipe(finalize(() => (this.saving = false)))
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyAccepted'));
          });
      }
    });
  }

  changeStatus(point: ShippingRequestTripDriverRoutePointDto): void {
    if (point.status == RoutePointStatus.FinishOffLoadShipment) {
      this.modelConfirm.show(point.id);
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
  start(): void {
    this._CurrentServ
      .start(this.item.id)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyStarted'));
      });
  }

  nextLocation(point: ShippingRequestTripDriverRoutePointDto): void {
    this._CurrentServ
      .nextLocation(point.id)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyChanged'));
      });
  }
  change(point: ShippingRequestTripDriverRoutePointDto): void {
    this._CurrentServ
      .changeStatus(point.id)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SuccessfullyChanged'));
      });
  }

  confimred(): void {}

  registerToEvents() {
    abp.event.on('app.tracking.accepted', (data) => {
      let item: TrackingListDto = <TrackingListDto>data.data;
      if (item.id == this.item.id) {
        this.item.driverStatus = ShippingRequestTripDriverStatus.Accepted;
        this.item.driverStatusTitle = ShippingRequestTripDriverStatus[ShippingRequestTripDriverStatus.Accepted];
      }
    });

    abp.event.on('app.tracking.started', (data) => {
      let item: TrackingListDto = <TrackingListDto>data.data;
      if (item.id == this.item.id) {
        this.item.status = ShippingRequestTripStatus.Intransit;
        this.item.statusTitle = ShippingRequestTripStatus[ShippingRequestTripStatus.Intransit];
        this.pickup.status = RoutePointStatus.StartedMovingToLoadingLocation;
        this.pickup.statusTitle = RoutePointStatus[this.pickup.status];
        this.pickup.nextStatus = RoutePointStatus[RoutePointStatus.ArriveToLoadingLocation];
        this.pickup.isActive = true;
      }
    });

    abp.event.on('app.tracking.changed', (data) => {
      let point: ShippingRequestTripDriverRoutePointDto = <ShippingRequestTripDriverRoutePointDto>data.data;
      let currentpoint = this.routePoints.find((x) => x.id == point.id);
      if (currentpoint) {
        if (currentpoint.pickingType == PickingType.Pickup) {
          this.pickup.isActive = point.isActive;
          this.pickup.isComplete = point.isComplete;
          this.pickup.endTime = point.endTime;
          this.pickup.status = point.status;
          this.pickup.statusTitle = point.statusTitle;
          this.pickup.nextStatus = point.nextStatus;
        } else {
          currentpoint.isActive = point.isActive;
          currentpoint.isComplete = point.isComplete;
          currentpoint.endTime = point.endTime;
          currentpoint.status = point.status;
          currentpoint.statusTitle = point.statusTitle;
          currentpoint.nextStatus = point.nextStatus;
          console.log(this.routePoints);
        }
      }
    });
  }

  /**
  getNextStatus(status: RoutePointStatus, type: PickingType) {
    switch (status) {
      case RoutePointStatus.StandBy:
        if (type == PickingType.Pickup)
          return {
            status: RoutePointStatus.StartedMovingToLoadingLocation,
            title: RoutePointStatus[RoutePointStatus.StartedMovingToLoadingLocation],
          };
        else
          return {
            status: RoutePointStatus.StartedMovingToOfLoadingLocation,
            title: RoutePointStatus[RoutePointStatus.StartedMovingToOfLoadingLocation],
          };

      case RoutePointStatus.StartedMovingToLoadingLocation:
        return { status: RoutePointStatus.ArriveToLoadingLocation, title: RoutePointStatus[RoutePointStatus.ArriveToLoadingLocation] };
      case RoutePointStatus.ArriveToLoadingLocation:
        return { status: RoutePointStatus.StartLoading, title: RoutePointStatus[RoutePointStatus.StartLoading] };
      case RoutePointStatus.StartLoading:
        return { status: RoutePointStatus.FinishLoading, title: RoutePointStatus[RoutePointStatus.FinishLoading] };
      case RoutePointStatus.StartedMovingToOfLoadingLocation:
        return { status: RoutePointStatus.ArrivedToDestination, title: RoutePointStatus[RoutePointStatus.ArrivedToDestination] };
      case RoutePointStatus.ArrivedToDestination:
        return { status: RoutePointStatus.StartOffloading, title: RoutePointStatus[RoutePointStatus.StartOffloading] };
      case RoutePointStatus.StartOffloading:
        return { status: RoutePointStatus.FinishOffLoadShipment, title: RoutePointStatus[RoutePointStatus.FinishOffLoadShipment] };
      case RoutePointStatus.FinishOffLoadShipment:
        return { status: RoutePointStatus.ReceiverConfirmed, title: RoutePointStatus[RoutePointStatus.ReceiverConfirmed] };
      case RoutePointStatus.ReceiverConfirmed:
        return { status: RoutePointStatus.DeliveryConfirmation, title: RoutePointStatus[RoutePointStatus.DeliveryConfirmation] };
      default:
        return { status: 0, title: '' };
    }
  }*/

  dropOffStep(point: ShippingRequestTripDriverRoutePointDto): number {
    if (!this.canChangeDropStatus(point)) return 0;
    switch (point.status) {
      case RoutePointStatus.StandBy:
        return 1;
      case RoutePointStatus.StartedMovingToOfLoadingLocation:
        return 2;
      case RoutePointStatus.ArrivedToDestination:
        return 3;
      case RoutePointStatus.StartOffloading:
        return 4;
      case RoutePointStatus.FinishOffLoadShipment:
        return 5;
      case RoutePointStatus.ReceiverConfirmed:
        return 6;
      default:
        return 7;
    }
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
