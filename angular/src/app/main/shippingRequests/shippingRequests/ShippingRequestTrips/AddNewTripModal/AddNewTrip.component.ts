import { Component, ViewChild, Injector, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetAllBidShippingRequestsForCarrierOutput,
  FacilityForDropdownDto,
  RoutStepsServiceProxy,
  ShippingRequestsTripServiceProxy,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditRoutPointDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';
import { RouteStepsForCreateShippingRequstComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestRouteSteps/RouteStepsForCreateShippingRequst.component';

@Component({
  selector: 'AddNewTripModal',
  templateUrl: './AddNewTrip.modal.html',
})
export class AddNewTripComponent extends AppComponentBase implements OnInit {
  @ViewChild('addNewTripsModal', { static: true }) modal: ModalDirective;
  @ViewChild('RouteSteps') RouteSteps: RouteStepsForCreateShippingRequstComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() shippingRequestId: number;
  @Input() MainGoodsCategory: number;

  allFacilities: FacilityForDropdownDto[];
  facilityLoading = false;
  trip = new CreateOrEditShippingRequestTripDto();
  saving = false;
  routePointsFromChild: CreateOrEditRoutPointDto[];
  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    //load the Facilites
    this.refreshOrGetFacilities();
  }

  show(record?: CreateOrEditShippingRequestTripDto): void {
    if (record) {
      console.log('edit');
      this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this.trip = res;
        this.RouteSteps.wayPointsList = this.trip.routPoints;
      });
    } else {
      this.trip.shippingRequestId = this.shippingRequestId;
    }
    //show
    this.modal.show();
  }
  close(): void {
    this.trip = new CreateOrEditShippingRequestTripDto();
    this.RouteSteps.wayPointsList = [];
    this.modal.hide();
  }

  createOrEditTrip() {
    this.saving = true;
    console.log(this.trip);
    this._shippingRequestTripsService
      .createOrEdit(this.trip)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.close();
        this.modalSave.emit(null);
      });
  }

  refreshOrGetFacilities() {
    console.log('facilities should be loaded ');
    this.facilityLoading = true;
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
      this.facilityLoading = false;
    });
  }
}
