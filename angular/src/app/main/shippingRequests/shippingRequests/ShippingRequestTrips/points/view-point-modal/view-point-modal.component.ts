import { Component, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DropDownMenu, TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import {
  CreateOrEditRoutPointDto,
  FacilityForDropdownDto,
  ReceiverFacilityLookupTableDto,
  ReceiversServiceProxy,
  RoutPointDto,
  RoutStepsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';

@Component({
  selector: 'view-point-details-modal',
  templateUrl: './view-point-modal.component.html',
  styleUrls: ['./view-point-modal.component.css'],
})
export class ViewPointModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  constructor(
    injector: Injector,
    private _tripService: TripService,
    private _PointService: PointsService,
    private _receiversServiceProxy: ReceiversServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy
  ) {
    super(injector);
  }
  @ViewChild('viewPointDetailsModal', { static: true }) modal: ModalDirective;

  allFacilities: FacilityForDropdownDto[];
  allReceivers: ReceiverFacilityLookupTableDto[];

  zoom: Number = 13; //map zoom
  lat: Number = 24.717942;
  lng: Number = 46.675761;
  wayPointsList: CreateOrEditRoutPointDto[];
  Point: RoutPointDto;
  active = false;
  usedIn: 'view' | 'createOrEdit';
  usedInSubscription$: any;
  ngOnDestroy() {
    this.usedInSubscription$.unsubscribe();
    console.log('Unsubscribed/Destroid from View Point Modal');
  }

  ngOnInit(): void {
    //Where the using of this Component is coming from
    this.usedInSubscription$ = this._PointService.currentUsedIn.subscribe((res) => (this.usedIn = res));
  }

  show(record: RoutPointDto) {
    this.active = true;
    if (record) {
      console.log('this is the record : ', record);
      this.Point = record;
    }
    //tell the service that i have this SinglePoint Active Right Now
    this.modal.show();
  }

  close() {
    this.active = false;
    this.modal.hide();
  }
}
