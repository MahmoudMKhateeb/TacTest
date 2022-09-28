/* tslint:disable:triple-equals */
import { Component, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  CreateOrEditRoutPointDto,
  FacilitiesServiceProxy,
  PickingType,
  RoutStepsServiceProxy,
  ShippingRequestRouteType,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'createOrEditPointModal',
  styleUrls: ['./createOrEditPointModal.component.css'],
  templateUrl: './createOrEditPointModal.component.html',
})
export class CreateOrEditPointModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  goodDetailsListForView: any;
  constructor(injector: Injector, public _tripService: TripService, private _PointService: PointsService) {
    super(injector);
  }
  @ViewChild('createRouteStepModal', { static: true }) modal: ModalDirective;
  @ViewChild('createOrEditPintForm') public createOrEditPintForm: NgForm;
  RouteType: number; //filled in onInit from the Trip Shared Service
  PickingType = PickingType;
  RouteTypes = ShippingRequestRouteType;

  zoom: Number = 13; //map zoom
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  wayPointsList: CreateOrEditRoutPointDto[];
  Point: CreateOrEditRoutPointDto;

  active = false;
  shippingRequestId: number;

  isAdditionalReceiverEnabled: boolean;
  pointIdForEdit = null;
  usedIn: 'view' | 'createOrEdit';
  tripServiceSubscription$: any;
  pointsServiceSubscription$: any;
  usedInSubscription$: any;
  modalOpenedFor: 'note' | 'goodDetails' | 'receiver';
  ngOnDestroy() {
    this.tripServiceSubscription$.unsubscribe();
    this.pointsServiceSubscription$.unsubscribe();
    this.usedInSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    //take the Route Type From the Shared Service
    this.tripServiceSubscription$ = this._tripService.currentShippingRequest.subscribe((res) => {
      if (res && res.shippingRequest) {
        this.RouteType = res.shippingRequest.routeTypeId;
        this.shippingRequestId = res.shippingRequest.id;
      }
    });
    //take the PointsList From The Shared Service
    this.pointsServiceSubscription$ = this._PointService.currentWayPointsList.subscribe((res) => (this.wayPointsList = res));
    //Where the using of this Component is coming from
    this.usedInSubscription$ = this._PointService.currentUsedIn.subscribe((res) => (this.usedIn = res));
    this._PointService.currentSingleWayPoint.subscribe((res) => {
      this.Point = res;
    });
  }

  show(id?, modalOpendFor?, goodDetailsListForView?) {
    this.modalOpenedFor = modalOpendFor;
    this.goodDetailsListForView = goodDetailsListForView;
    this.isAdditionalReceiverEnabled = this.modalOpenedFor === 'receiver';
    console.log('this.isAdditionalReceiverEnabled', this.isAdditionalReceiverEnabled);

    //if view disable the form otherwise enable it
    this.active = true;
    if (id) {
      this.pointIdForEdit = id;
      //this is edit point action
      this.Point = this.wayPointsList[id];
    }
    //tell the service that i have this SinglePoint Active Right Now
    this.isAdditionalReceiverEnabled = this.Point.receiverFullName ? true : false;
    this._PointService.updateSinglePoint(this.Point);
    this.modal.show();
  }

  close() {
    this.Point = new CreateOrEditRoutPointDto();
    this._PointService.updateSinglePoint(this.Point);
    this.pointIdForEdit = null;
    this.isAdditionalReceiverEnabled = false;
    this.active = false;
    this.modal.hide();
  }
}
