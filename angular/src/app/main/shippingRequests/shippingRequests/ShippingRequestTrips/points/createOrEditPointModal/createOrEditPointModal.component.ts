/* tslint:disable:triple-equals */
import { Component, Injector, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CreateOrEditRoutPointDto, PickingType, ShippingRequestRouteType } from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { NgForm } from '@angular/forms';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'createOrEditPointModal',
  styleUrls: ['./createOrEditPointModal.component.css'],
  templateUrl: './createOrEditPointModal.component.html',
})
export class CreateOrEditPointModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('createRouteStepModal', { static: true }) modal: ModalDirective;
  @ViewChild('createOrEditPintForm') public createOrEditPintForm: NgForm;
  goodDetailsListForView: any;
  @Input('isForDedicated') isForDedicated: boolean;
  @Input('isForPortsMovement') isForPortsMovement: boolean;
  @Input('isHomeDelivery') isHomeDelivery: boolean;
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
  //tripServiceSubscription$: any;
  pointsServiceSubscription$: any;
  usedInSubscription$: any;
  modalOpenedFor: 'note' | 'goodDetails' | 'receiver';

  constructor(injector: Injector, public _tripService: TripService, private _PointService: PointsService) {
    super(injector);
  }

  ngOnInit(): void {
    //take the Route Type From the Shared Service
    if (this._tripService.GetShippingRequestForViewOutput && this._tripService.GetShippingRequestForViewOutput.shippingRequest) {
      this.RouteType = this._tripService.GetShippingRequestForViewOutput.shippingRequest.routeTypeId;
      this.shippingRequestId = this._tripService.GetShippingRequestForViewOutput.shippingRequest.id;
    }

    //take the PointsList From The Shared Service
    this.pointsServiceSubscription$ = this._PointService.currentWayPointsList.subscribe((res) => (this.wayPointsList = res));
    //Where the using of this Component is coming from
    this.usedInSubscription$ = this._PointService.currentUsedIn.subscribe((res) => (this.usedIn = res));
    this._PointService.currentSingleWayPoint.subscribe((res) => {
      this.Point = res;
    });
  }

  show(id?: number, modalOpendFor?, goodDetailsListForView?) {
    console.log('modalOpendFor', modalOpendFor);
    console.log('id', id);
    this.modalOpenedFor = modalOpendFor;
    this.goodDetailsListForView = goodDetailsListForView;
    this.isAdditionalReceiverEnabled = this.modalOpenedFor === 'receiver';
    console.log('this.isAdditionalReceiverEnabled', this.isAdditionalReceiverEnabled);

    //if view disable the form otherwise enable it
    console.log('this.wayPointsList', this.wayPointsList);
    this.active = true;
    if (!isNaN(id)) {
      this.pointIdForEdit = id;
      //this is edit point action
      this.Point = this.wayPointsList[id];
    }
    console.log('this.Point', this.Point);
    //tell the service that i have this SinglePoint Active Right Now
    this.isAdditionalReceiverEnabled = this.Point.receiverFullName ? true : false;
    this._PointService.updateSinglePoint(this.Point);
    this.modal.show();
  }

  close() {
    if (this.createOrEditPintForm.valid) {
      this.Point = new CreateOrEditRoutPointDto();
      this._PointService.updateSinglePoint(this.Point);
      this.pointIdForEdit = null;
      this.isAdditionalReceiverEnabled = false;
      this.active = false;
    } else {
      this.Point.receiverPhoneNumber = null;
      this.Point.receiverEmailAddress = null;
      this.Point.receiverCardIdNumber = null;
      this.Point.receiverFullName = null;
    }
    this.modal.hide();
    this._PointService.currentPointIndex = null;
  }

  ngOnDestroy() {
    //this.tripServiceSubscription$.unsubscribe();
    this.pointsServiceSubscription$.unsubscribe();
    this.usedInSubscription$.unsubscribe();
  }
}
