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
  WaybillsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import { finalize } from '@node_modules/rxjs/operators';
import { RouteStepsForCreateShippingRequstComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestRouteSteps/RouteStepsForCreateShippingRequst.component';
import Swal from 'sweetalert2';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { FileDownloadService } from '@shared/utils/file-download.service';

@Component({
  selector: 'AddNewTripModal',
  styleUrls: ['./AddNewTrip.scss'],
  templateUrl: './AddNewTrip.modal.html',
})
export class AddNewTripComponent extends AppComponentBase implements OnInit {
  @ViewChild('addNewTripsModal', { static: true }) modal: ModalDirective;
  @ViewChild('RouteSteps') RouteSteps: RouteStepsForCreateShippingRequstComponent;
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
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _fileDownloadService: FileDownloadService
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
        this.RouteSteps.wayPointsList = this.trip.routPoints;
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
    this.RouteSteps.wayPointsList = [];
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

  /**
   * downloads Masteer WayBill For EachTrip
   * @param id
   */
  printMasterWaybill(tripId) {
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(tripId).subscribe((res) => {
      this._fileDownloadService.downloadTempFile(res);
    });
    console.log('test');
  }
}
