import { Component, ViewChild, Injector, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  GetAllBidShippingRequestsForCarrierOutput,
  FacilityForDropdownDto,
  RoutStepsServiceProxy,
  ShippingRequestsTripServiceProxy,
  CreateOrEditShippingRequestTripDto,
  CreateOrEditRoutPointDto,
  ShippingRequestDto,
  CreateOrEditShippingRequestVasListDto,
  CreateOrEditShippingRequestTripVasDto,
  ShippingRequestVasDto,
  GetShippingRequestVasForViewDto,
  ShippingRequestsTripForViewDto,
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';
import { WayPointsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/wayPoints/wayPoints.component';
import Swal from 'sweetalert2';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'AddNewTripModal',
  styleUrls: ['./createOrEditTrip.component.scss'],
  templateUrl: './createOrEditTrip.component.html',
})
export class CreateOrEditTripComponent extends AppComponentBase implements OnInit {
  @ViewChild('addNewTripsModal', { static: true }) modal: ModalDirective;
  @ViewChild('wayPointsComponent') wayPointsComponent: WayPointsComponent;
  @ViewChild('createOrEditFacilityModal') createOrEditFacilityModal: CreateOrEditFacilityModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() shippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];

  Vases: CreateOrEditShippingRequestTripVasDto[];
  selectedVases: CreateOrEditShippingRequestTripVasDto[];

  allFacilities: FacilityForDropdownDto[];
  trip = new CreateOrEditShippingRequestTripDto();
  facilityLoading = false;
  saving = false;
  loading = true;
  routePointsFromChild: CreateOrEditRoutPointDto[];

  constructor(
    injector: Injector,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _waybillsServiceProxy: WaybillsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.Vases = [];
    this.selectedVases = [];
    // this.VasListFromFather.forEach((x) => {
    //   const vas = new CreateOrEditShippingRequestTripVasDto();
    //   vas.shippingRequestVasId = x.shippingRequestVas.id;
    //   vas.name = x.vasName;
    //   this.Vases.push(vas);
    // });
    //load the Facilites
    this.refreshOrGetFacilities();
    this.sortVases();
  }

  show(record?: CreateOrEditShippingRequestTripDto): void {
    if (record) {
      this._shippingRequestTripsService.getShippingRequestTripForEdit(record.id).subscribe((res) => {
        this.trip = res;
        this.wayPointsComponent.wayPointsList = this.trip.routPoints;
        this.selectedVases = res.shippingRequestTripVases;
        this.loading = false;
        // this.Vases = this.trip.shippingRequestTripVases;
        console.log('Vas From Father ', this.VasListFromFather);
        console.log('vases ', this.Vases);
        console.log('tripVases ', this.trip.shippingRequestTripVases);
      });
    } else {
      //this is a create
      this.loading = false;
      this.trip.shippingRequestId = this.shippingRequest.id;
    }
    //show
    this.modal.show();
  }
  close(): void {
    this.trip = new CreateOrEditShippingRequestTripDto();
    this.wayPointsComponent.wayPointsList = [];
    this.loading = true;
    this.modal.hide();
  }

  sortVases() {
    //console.log('Vases List ', this.Vases);
    // console.log('selected vases list ', this.selectedVases);
    // console.log('Shipping Request Trip Vases before loop', this.trip.shippingRequestTripVases);
    // //if edit get the Vases From select and Assighn them into Selected Vases
    // this.trip.shippingRequestTripVases.forEach((singleVas) => {
    //   const selectd = new GetShippingRequestVasForViewDto();
    //   selectd.shippingRequestVas.vasId = singleVas.shippingRequestVasId;
    //   this.selectedVases.push(selectd);
    // });
    // this.selectedVases.forEach((y) => {
    //   const z = new CreateOrEditShippingRequestTripVasDto();
    //   console.log('selected Vas  single', y);
    //   z.shippingRequestVasId = y.shippingRequestVas.vasId;
    //   console.log('vasId ', y.shippingRequestVas.vasId);
    //   this.trip.shippingRequestTripVases.push(z);
    //   console.log('Shipping Request Trip Vases ', this.trip.shippingRequestTripVases);
    // });
  }
  createOrEditTrip() {
    this.saving = true;
    console.log(this.trip);
    //this.sortVases();
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
        this.notify.info(this.l('SuccessfullySaved'));
      });
  }
  // viewTrip(id: number) {
  //   this._shippingRequestTripsService
  //     .getShippingRequestTripForView(id)
  //     .pipe(
  //       finalize(() => {
  //         this.saving = false;
  //       })
  //     )
  //     .subscribe((res) => {
  //       this.trip = res;
  //     });
  // }

  deleteTrip(tripid: number) {
    Swal.fire({
      title: this.l('areYouSure'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this._shippingRequestTripsService.delete(tripid).subscribe(() => {
          this.close();
          this.modalSave.emit(null);
          this.notify.info(this.l('SuccessfullyDeleted'));
        });
      } //end of if
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

  DownloadSingleDropWaybillPdf(id: number): void {
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
}