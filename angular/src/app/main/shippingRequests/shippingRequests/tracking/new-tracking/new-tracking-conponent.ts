import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Injector,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  InvokeStatusInputDto,
  PickingType,
  PointTransactionDto,
  RoutePointStatus,
  RoutPointTransactionDto,
  ShippingRequestRouteType,
  ShippingRequestTripDriverRoutePointDto,
  ShippingRequestTripDriverStatus,
  ShippingRequestTripStatus,
  TrackingListDto,
  TrackingServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { TrackingConfirmModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-confirm-code-model.component';
import { TrackingPODModalComponent } from '@app/main/shippingRequests/shippingRequests/tracking/tacking-pod-model.component';
import { UploadDeliveryNoteDocumentModelComponent } from '@app/main/shippingRequests/shippingRequests/tracking/upload-delivery-note-document-model/upload-delivery-note-document-model.component';
import { NgbDropdownConfig } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
  selector: 'new-tracking-conponent',
  templateUrl: './new-tracking-conponent.html',
  styleUrls: ['./new-tracking-conponent.css'],
  providers: [NgbDropdownConfig],
  animations: [appModuleAnimation()],
})
export class NewTrackingConponent extends AppComponentBase implements OnChanges {
  @ViewChild('modelconfirm', { static: false }) modelConfirmCode: TrackingConfirmModalComponent;
  @ViewChild('modelpod', { static: false }) modelpod: TrackingPODModalComponent;
  @ViewChild('modelDeliveryNote', { static: false }) modelDeliveryNote: UploadDeliveryNoteDocumentModelComponent;
  @Input() trip: TrackingListDto = new TrackingListDto();
  active = false;
  item: number;
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  routePoints: ShippingRequestTripDriverRoutePointDto[];
  pointsIsLoading = false;
  distance: string;
  duration: string;
  readonly zoom: number = 15;
  saving = false;
  markerLong: number;
  markerLat: number;
  markerFacilityName: string;
  activeIndex: number = 1;
  driverStatusesEnum = ShippingRequestTripDriverStatus;
  tripStatusesEnum = ShippingRequestTripStatus;
  pickingTypeEnum = PickingType;
  routeTypeEnum = ShippingRequestRouteType;
  canStartAnotherPoint = false;

  constructor(injector: Injector, private elRef: ElementRef, private _trackingServiceProxy: TrackingServiceProxy, config: NgbDropdownConfig) {
    super(injector);
    config.autoClose = true;
    config.container = 'body';
  }

  /**
   * to Detect Changes On Variables
   * @param changes
   */
  ngOnChanges(changes: SimpleChanges) {
    this.getForView();
    this.getCordinatesByCityName(this.trip.origin, 'source');
    this.getCordinatesByCityName(this.trip.destination, 'destanation');
    console.log('Changes happening \n');
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
    this.pointsIsLoading = true;
    this._trackingServiceProxy
      .getForView(this.trip.id)
      .pipe(
        finalize(() => {
          this.pointsIsLoading = false;
        })
      )
      .subscribe((result) => {
        this.trip.status = result.status;
        this.trip.driverStatus = result.driverStatus;
        this.routePoints = result.routPoints;
        this.handleCanGoNextLocation(result.routPoints);
      });
  }

  /**
   * starts a trip
   */
  start(): void {
    this.saving = true;
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
              this.saving = false;
              this.getForView();
            })
          )
          .subscribe((result) => {
            this.notify.info(this.l('SuccessfullyAccepted'));
          });
      }
    });
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

  // TODO - change this methoud param to point id only if needed
  handleUploadPod(point: ShippingRequestTripDriverRoutePointDto, transaction: PointTransactionDto) {
    this.modelpod.show(point.id, transaction.action);
    abp.event.on('PodUploadedSuccess', () => {
      this.getForView();
    });
  }

  /**
   * handles the Trip Confirmation code process
   * opens the modal the send a request with the code to the invoke service and if code is correct refresh the data (getforView)
   * @param point
   * @param transaction
   */
  handleDeliveryConfirmationCode(point: ShippingRequestTripDriverRoutePointDto, transaction: PointTransactionDto) {
    //show the modal
    this.modelConfirmCode.show(point.id, transaction.action);
    abp.event.on('trackingConfirmCodeSubmitted', () => {
      this.getForView();
    });
  }

  /**
   * handels the upload proccess of a delivery note for the points
   * @param point
   * @param transaction
   * @private
   */
  private handleUploadDeliveryNotes(point: ShippingRequestTripDriverRoutePointDto, transaction: PointTransactionDto) {
    this.modelpod.show(point.id, transaction.action);
    abp.event.on('tripDeliveryNotesUploadSuccess', () => {
      this.getForView();
    });
  }

  invokeStatus(point: ShippingRequestTripDriverRoutePointDto, transaction: PointTransactionDto) {
    this.saving = true;
    const invokeRequestBody = new InvokeStatusInputDto();
    invokeRequestBody.id = point.id;
    invokeRequestBody.action = transaction.action;
    //TODO  - Dont Forget to take the secound acound that requires pod for karam
    if (transaction.action === 'FinishOffLoadShipmentDeliveryConfirmation' || transaction.action === 'DeliveryConfirmation') {
      //handle upload Pod
      this.saving = false;
      return this.handleUploadPod(point, transaction);
    }
    if (transaction.action === 'UplodeDeliveryNote') {
      this.saving = false;
      return this.handleUploadDeliveryNotes(point, transaction);
    }
    if (transaction.action === 'ReceiverConfirmed' || transaction.action === 'DeliveryConfirmationReceiverConfirmed') {
      this.saving = false;
      return this.handleDeliveryConfirmationCode(point, transaction);
    }

    this._trackingServiceProxy
      .invokeStatus(invokeRequestBody)
      .pipe(
        finalize(() => {
          this.saving = false;
          console.log(point, transaction);
        })
      )
      .toPromise()
      .then(() => {
        this.getForView();
      });
    console.log('invoke was Clicked for point ', point);
  }

  /**
   * check if the Current User can start a trip and the trip is startable
   * @param point
   * @param trip
   */
  canStartTrip(point: ShippingRequestTripDriverRoutePointDto, trip: TrackingListDto): boolean {
    return point.pickingType == 1 && trip.driverStatus == 0 && trip.canStartTrip ? true : false;
  }

  getStepperSteps(statues: RoutPointTransactionDto[]): {} {
    let items = [];
    //TODO change active index back to null
    this.activeIndex = null;
    for (let i = 0; i < statues.length; i++) {
      items.push({
        label: statues[i].name,
        styleClass: 'completed',
      });
      if (statues[i].isDone) {
        this.activeIndex = i;
      }
    }

    return items;
  }

  /**
   * go to next Location
   * @param point
   */
  nextLocation(point: ShippingRequestTripDriverRoutePointDto): void {
    this.saving = true;
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

  private handleCanGoNextLocation(routPoints: ShippingRequestTripDriverRoutePointDto[]): boolean {
    const singleDrop = this.routeTypeEnum.SingleDrop;
    const MultipleDrops = this.routeTypeEnum.MultipleDrops;
    const canStartAnotherPoint = routPoints.find((item) => item.canGoToNextLocation === true) ? true : false;
    //single Drop
    if (this.trip.routeTypeId === singleDrop && canStartAnotherPoint) {
      this.saving = true;
      this.nextLocation(this.routePoints[1]);
      this.notify.info('CanGoNextForSingleDrop');
    }
    //for Multible Drops
    if (this.trip.routeTypeId === MultipleDrops && canStartAnotherPoint) {
      this.notify.info('CanGoNext');
      console.log('can Go Next For MultiDrops');
      this.canStartAnotherPoint = routPoints.find((item) => item.canGoToNextLocation === true) ? true : false;
    }
    return (this.canStartAnotherPoint = canStartAnotherPoint);
  }
}
