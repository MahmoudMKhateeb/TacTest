import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import {
  CreateOrEditRoutPointDto,
  DedicatedShippingRequestsServiceProxy,
  GetAllTrucksWithDriversListDto,
  SelectItemDto,
  ShippingRequestsTripServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import Swal from 'sweetalert2';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'Storage-Details-Modal',
  templateUrl: './storage-details.component.html',
})
export class StorageDetailsModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Input('wayPointsList') wayPointsList: CreateOrEditRoutPointDto[] = [];
  @Input('usedIn') usedIn: 'view' | 'createOrEdit';
  isEdit = false;
  active = false;
  i = 0;
  allDrivers: SelectItemDto[];
  allTrucks: GetAllTrucksWithDriversListDto[];
  constructor(
    injector: Injector,
    private _dedicatedShippingRequestService: DedicatedShippingRequestsServiceProxy,
    public _tripService: TripService,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.loadDropDowns();
    abp.event.on('trip.crud.actorCarrierChanged', () => {
      this.loadDropDowns();
    });
  }

  show(pointIndex) {
    this.active = true;
    this.i = pointIndex;
    if (this.wayPointsList[pointIndex]?.id) {
      this.isEdit = true;
    }
    this.modal.show();
  }

  save() {
    // Save logic
    this.changeDriverAndTruckForPortMovment();
    //this.close();
  }

  close() {
    // Close modal logic
    this.active = false;
    this.isEdit = false;
    this.modal.hide();
  }

  loadDropDowns() {
    //Drivers
    this._dedicatedShippingRequestService
      .getAllDriversForDropDown(undefined, this._tripService.CreateOrEditShippingRequestTripDto?.carrierActorId)
      .subscribe((res) => {
        this.allDrivers = res.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
      });
    //trucks
    this._dedicatedShippingRequestService
      .getAllTrucksWithDriversList(undefined, undefined, this._tripService.CreateOrEditShippingRequestTripDto?.carrierActorId)
      .subscribe((res) => {
        this.allTrucks = res;
      });
  }

  changeDriverAndTruckForPortMovment() {
    const driverId = this.wayPointsList[this.i].driverUserId;
    const truckId = this.wayPointsList[this.i].truckId;

    const changeDriver = isNotNullOrUndefined(driverId);
    const changeTruck = isNotNullOrUndefined(truckId);
    const tripId = this._tripService.activeTripId;

    if (!changeDriver && !changeTruck) {
      this.close();
      return;
    }

    let title;
    if (changeDriver && changeTruck) {
      title = this.l('areYouSureYouWantToChangeDriverAndTruck');
    } else if (changeDriver) {
      title = this.l('areYouSureYouWantToChangeDriver');
    } else if (changeTruck) {
      title = this.l('areYouSureYouWantToChangeTheTruck');
    }

    Swal.fire({
      title: title,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        const observables = [];
        if (changeDriver) {
          observables.push(this._shippingRequestTripsService.replaceDriver(driverId, tripId));
        }
        if (changeTruck) {
          observables.push(this._shippingRequestTripsService.replaceTruck(truckId, tripId));
        }

        // Execute all service calls
        forkJoin(observables).subscribe(() => {
          this.close();
          this.notify.info(this.l('Saved'));
          this.usedIn === 'createOrEdit'
            ? abp.event.trigger('storageTripStorageDataChangedFromCreate')
            : abp.event.trigger('storageTripStorageDataChangedFromView');
        });
      }
    });
  }
}
