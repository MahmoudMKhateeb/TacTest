import { Component, ViewChild, Injector, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  AssignDriverAndTruckToShippmentByCarrierInput,
  CreateOrEditTachyonPriceOfferDto,
  SelectItemDto,
  ShippingRequestsTripServiceProxy,
  TachyonPriceOffersServiceProxy,
  TrucksServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'assignDriverTruckModal',
  templateUrl: './assignDriverTruckModal.component.html',
})
export class AssignDriverTruckModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('assignDriverTruckModal', { static: false }) modal: ModalDirective;
  @Output() modalsave: EventEmitter<any> = new EventEmitter<any>();
  assignDriverAndTruck: AssignDriverAndTruckToShippmentByCarrierInput = new AssignDriverAndTruckToShippmentByCarrierInput();
  allDrivers: SelectItemDto[] = [];
  allTrucks: SelectItemDto[] = [];
  saving = false;
  loading = true;

  constructor(
    injector: Injector,
    private _trucksServiceProxy: TrucksServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.getAllDrivers();
    this.getAllTrucks();
  }

  show(tripId): void {
    this.assignDriverAndTruck.id = tripId;
    this.modal.show();
  }
  close(): void {
    this.assignDriverAndTruck = new AssignDriverAndTruckToShippmentByCarrierInput();

    this.modal.hide();
  }

  save() {
    this.saving = true;

    // this._tachyonPriceOffersServiceProxy
    //   .createOrEditTachyonPriceOffer(this.priceOffer)
    //   .pipe(
    //     finalize(() => {
    //       this.saving = false;
    //     })
    //   )
    //   .subscribe(() => {
    //     this.modalsave.emit('');
    //     this.close();
    //     this.notify.success('priceOfferSentSuccessfully');
    //   });
  }

  /**
   * this method is for Getting All Carriers Drivers For DD
   */
  getAllDrivers() {
    if (this.feature.isEnabled('App.Carrier')) {
      this._trucksServiceProxy.getAllDriversForDropDown().subscribe((res) => {
        this.allDrivers = res;
      });
    }
  }

  /**
   * this method is for Getting All Carriers Trucks For DD
   */
  getAllTrucks() {
    if (this.feature.isEnabled('App.Carrier')) {
      this._trucksServiceProxy.getAllCarrierTrucksForDropDown().subscribe((res) => {
        this.allTrucks = res;
      });
    }
  }

  /**
   * this function is to assign Driver And Truck To shipping Request Trip
   */
  assignDriverandTruck() {
    this.saving = true;

    this._shippingRequestTripsService
      .assignDriverAndTruckToShippmentByCarrier(this.assignDriverAndTruck)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.modalsave.emit('');
        this.close();
        this.notify.success('driverandTrucksAssignedSuccessfully');
      });
  }
}
