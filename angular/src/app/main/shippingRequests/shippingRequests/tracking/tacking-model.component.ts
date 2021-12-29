import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter, NgZone } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';
import { TrackingPODModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-pod-model.component';
import { FileDownloadService } from '@shared/utils/file-download.service';

import {
  TrackingServiceProxy,
  TrackingListDto,
  ShippingRequestTripDriverRoutePointDto,
  PickingType,
  RoutePointStatus,
  ShippingRequestTripStatus,
  ShippingRequestTripDriverStatus,
  ShippingRequestsTripListDto,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { ViewTripAccidentModelComponent } from '../ShippingRequestTrips/accident/View-trip-accident-modal.component';
import { MapsAPILoader } from '@node_modules/@agm/core';

@Component({
  templateUrl: './tacking-model.component.html',
  styleUrls: ['./tacking-model.component.scss'],
  selector: 'tacking-model',
  animations: [appModuleAnimation()],
})
export class TrackingModelComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('modelconfirm', { static: false }) modelConfirm: TrackingConfirmModalComponent;
  @ViewChild('modelpod', { static: false }) modelpod: TrackingPODModalComponent;
  @ViewChild('ModelIncident', { static: false }) modelIncident: ViewTripAccidentModelComponent;
  @Output() onCloseModel: EventEmitter<any> = new EventEmitter<any>();
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
  private bounds: google.maps.LatLngBounds;
  constructor(
    injector: Injector,
    private _CurrentServ: TrackingServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private mapsAPILoader: MapsAPILoader
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.registerToEvents();
    this.initMap();
  }

  getForView() {
    this._CurrentServ.getForView(this.item.id).subscribe((result) => {
      this.routePoints = result.items;
      this.getPickup();
      this.getDrops();
      this.active = true;
      this.modal.show();
      console.log(this.routePoints);
    });
  }
  show(trip: TrackingListDto): void {
    console.log('the Trip :', trip);
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.item = trip;
    this.getCordinatesByCityName(this.item.origin, 'source');
    this.getCordinatesByCityName(this.item.destination, 'destanation');
    this.getForView();
  }
  close(): void {
    this.active = false;
    this.onCloseModel.emit('');
    this.modal.hide();
  }
  canChangeDropStatus(drop: ShippingRequestTripDriverRoutePointDto) {
    if (
      this.pickup.status == RoutePointStatus.FinishLoading &&
      this.item.status == ShippingRequestTripStatus.Intransit &&
      !this.saving &&
      this.item.isAssign
    ) {
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
          .pipe(
            finalize(() => {
              this.saving = false;
              //Refresh The Data After Status Change
              setTimeout(() => {
                this.getForView();
              }, 3000);
            })
          )
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
  start(): void {
    this._CurrentServ
      .start(this.item.id)
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

  nextLocation(point: ShippingRequestTripDriverRoutePointDto): void {
    this._CurrentServ
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
  change(point: ShippingRequestTripDriverRoutePointDto): void {
    this._CurrentServ
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
      this.updatePoint(<ShippingRequestTripDriverRoutePointDto>data.data);
    });

    abp.event.on('app.tracking.Shipment.Delivered', (data) => {
      let point = <ShippingRequestTripDriverRoutePointDto>data.data;
      this.updatePoint(point);
      if (this.item.id == point.shippingRequestTripId) {
        this.item.status = ShippingRequestTripStatus.Delivered;
        this.item.statusTitle = ShippingRequestTripStatus[ShippingRequestTripStatus.Delivered];
      }
    });
  }
  updatePoint(point: ShippingRequestTripDriverRoutePointDto): void {
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
      }
    }
  }

  dropOffStep(point: ShippingRequestTripDriverRoutePointDto): number {
    if (point.status == RoutePointStatus.DeliveryConfirmation) return 7;
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
  downloadPOD(id: number): void {
    this._CurrentServ.pOD(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  showIncident(): void {
    if (this.item.hasAccident) {
      let trip: ShippingRequestsTripListDto = new ShippingRequestsTripListDto();
      trip.id = this.item.id;
      trip.isApproveCancledByCarrier = this.item.isApproveCancledByCarrier;
      trip.isApproveCancledByShipper = this.item.isApproveCancledByShipper;
      trip.status = this.item.status;
      this.modelIncident.getAll(trip);
    }
  }
  canCreateAccident() {
    if (this.item.status == ShippingRequestTripStatus.Intransit) {
      if (!this.appSession.tenantId || this.feature.isEnabled('App.TachyonDealer')) {
        return this.item.isAssign;
      }

      return true;
    }
    return false;
  }
  accidentCallBack(): void {
    this.item.hasAccident = true;
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
