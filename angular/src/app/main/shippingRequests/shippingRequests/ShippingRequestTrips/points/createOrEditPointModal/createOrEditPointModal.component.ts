import { Component, Injector, OnChanges, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  CreateOrEditRoutPointDto,
  FacilityForDropdownDto,
  PickingType,
  ReceiverFacilityLookupTableDto,
  ReceiversServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';

@Component({
  selector: 'createOrEditPointModal',
  styleUrls: ['./createOrEditPointModal.component.css'],
  templateUrl: './createOrEditPointModal.component.html',
})
export class CreateOrEditPointModalComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private _tripService: TripService,
    private _PointService: PointsService,
    private _receiversServiceProxy: ReceiversServiceProxy
  ) {
    super(injector);
  }
  @ViewChild('createRouteStepModal', { static: true }) modal: ModalDirective;
  @ViewChild('createOrEditFacilityModal') public createOrEditFacilityModal: ModalDirective;

  allFacilities: FacilityForDropdownDto[];
  allReceivers: ReceiverFacilityLookupTableDto[];

  RouteType: number; //filled in onInit from the Trip Shared Service
  PickingType = PickingType;
  zoom: Number = 13; //map zoom
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  wayPointsList: CreateOrEditRoutPointDto[] = [];
  singleWayPoint: CreateOrEditRoutPointDto = new CreateOrEditRoutPointDto();

  active = false;
  facilityLoading = false;
  receiversLoading = false;
  isAdditionalReceiverEnabled: boolean;

  currentShippingRequestSubscription: any;
  currentWayPointsListSubscription: any;
  currentActiveTripSubscription: any;
  ngOnInit(): void {
    //take the Route Type From the Shared Service
    this.currentShippingRequestSubscription = this._tripService.currentShippingRequest.subscribe((res) => (this.RouteType = 1));
    //take the PointsList From The Shared Service
    this.currentWayPointsListSubscription = this._PointService.currentWayPointsList.subscribe((res) => {
      this.wayPointsList = res;
    });
    //if Route Type is Single Drop Make the Single Drop Point Facility Same as in the Trip
    this.currentActiveTripSubscription = this._tripService.currentActiveTrip.subscribe((res) => {
      this.singleWayPoint.facilityId = res.destinationFacilityId;
      this.loadReceivers(res.destinationFacilityId);
    });

    setInterval(() => {
      console.log('current Single Point', this.singleWayPoint);
      console.log('way point length:', this.wayPointsList.length);
      console.log('Current Way Points ', this.wayPointsList);
    }, 3000);

    //take the facilites from the Shared Service Insted Of Loading Them Again
    this.allFacilities = this._tripService.allFacilities;
  }

  show(id?) {
    if (id) {
      //this is edit point action
      this.singleWayPoint = this.wayPointsList[id];
    }
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.modal.hide();
  }

  /**
   * Extracts Facility Coordinates for single Point (Map Drawing)
   */
  RouteStepCordSetter() {
    this.singleWayPoint.latitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.lat;
    this.singleWayPoint.longitude = this.allFacilities.find((x) => x.id == this.singleWayPoint.facilityId)?.long;
  }

  /**
   * Add Route Step
   */
  AddRouteStep() {
    this._PointService.updateSinglePoint(this.singleWayPoint);
    //Route Type Single Drop ?
    this.wayPointsList[1] = this.singleWayPoint;
    this._PointService.updateWayPoints(this.wayPointsList);
    // this._PointService.currentWayPointsList
    this.close();
  }

  /**
   * loads a list of Receivers by facility Id
   * @param facilityId
   */
  loadReceivers(facilityId) {
    this.receiversLoading = true;
    //to be Changed
    this._receiversServiceProxy.getAllReceiversByFacilityForTableDropdown(facilityId).subscribe((result) => {
      this.allReceivers = result;
      this.receiversLoading = false;
    });
  }

  /**
   * resets Additional Receiver if the Additional Receiver checkbox is not selected
   */
  resetAdditionalReviverFields() {
    this.singleWayPoint.receiverAddress = undefined;
    this.singleWayPoint.receiverCardIdNumber = undefined;
    this.singleWayPoint.receiverEmailAddress = undefined;
    this.singleWayPoint.receiverPhoneNumber = undefined;
    this.singleWayPoint.receiverFullName = undefined;
  }
}
