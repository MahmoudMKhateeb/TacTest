import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Injector,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';

import {
  GetShippingRequestVasForViewDto,
  ShippingRequestDto,
  ShippingRequestFlag,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  ShippingRequestStatus,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripCancelStatus,
  ShippingRequestTripStatus,
  ShippingTypeEnum,
} from '@shared/service-proxies/service-proxies';
import { ViewTripModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/viewTripModal/viewTripModal.component';
import { TripService } from '../trip.service';
import { AddNewRemarksTripModalComponent } from './add-new-remarks-trip-modal/add-new-remarks-trip-modal.component';
import { finalize } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { FileUpload } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';
import Swal from 'sweetalert2';
import { ViewImportedTripsFromExcelModalComponent } from './ImportedTrips/view-imported-trips-from-excel-modal/view-imported-trips-from-excel-modal.component';

@Component({
  selector: 'TripsForViewShippingRequest',
  templateUrl: './tripsForViewShippingRequest.component.html',
  styleUrls: ['./tripsForViewShippingRequest.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class TripsForViewShippingRequestComponent extends AppComponentBase implements OnInit, AfterViewInit, OnChanges {
  @ViewChild('dataTablechild', { static: false }) dataTable: Table;
  @ViewChild('paginatorchild', { static: false }) paginator: Paginator;
  @ViewChild('ViewTripModal', { static: false }) ViewTripModal: ViewTripModalComponent;
  @ViewChild('AddRemarksModal', { static: false }) AddRemarksModal: AddNewRemarksTripModalComponent;
  @ViewChild('saveAsTemplateModal', { static: false }) saveAsTemplateModal: ViewTripModalComponent;

  @ViewChild('ExcelFileUpload', { static: false }) excelFileUpload: FileUpload;
  @ViewChild('PointExcelFileUpload', { static: false }) PointExcelFileUpload: FileUpload;
  @ViewChild('GoodDetailsExcelFileUpload', { static: false }) GoodDetailsExcelFileUpload: FileUpload;
  @ViewChild('ViewImportedTripsModal', { static: false }) modal: ViewImportedTripsFromExcelModalComponent;
  @ViewChild('ViewImportedPointsModal', { static: false }) pointModal: ModalDirective;
  @ViewChild('ViewImportedGoodDetailsModal', { static: false }) goodDetailsModal: ModalDirective;
  @ViewChild('VasesExcelFileUpload', { static: false }) VasesExcelFileUpload: FileUpload;
  @ViewChild('ViewImportedVasesModal', { static: false }) VasesModal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter();
  @Input() ShippingRequest: ShippingRequestDto;
  //@Input() shippingRequestForView: GetShippingRequestForViewOutput;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];
  tripsByTmsEnabled = false;
  ShippingRequestTripStatusEnum = ShippingRequestTripStatus;
  ShippingRequestStatusEnum = ShippingRequestStatus;
  ShippingRequestTripCancelStatusEnum = ShippingRequestTripCancelStatus;
  @Output() incidentResolved: EventEmitter<void>;
  saving = false;
  uploadUrl: string;
  uploadPointUrl: string;
  tripVases: string;
  isArabic = false;
  active = false;
  list: any;
  pointsList: any;
  goodDetailsList: any;
  vasesList: any;
  loading = false;
  uploadGoodDetailsUrl: string;
  ShippingRequestRouteTypeEnum = ShippingRequestRouteType;
  CanAssignDriverAndTruck: boolean;
  shippingRequestFlagEnum = ShippingRequestFlag;

  type = 'Trip';
  ShippingRequestTripStatus = ShippingRequestTripStatus;
  showBtnAddTrips: boolean;
  ShippingRequestFlagEnum = ShippingRequestFlag;
  ShippingTypeEnum = ShippingTypeEnum;

  constructor(
    injector: Injector,
    public _TripService: TripService,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private changeDetectorRef: ChangeDetectorRef,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _httpClient: HttpClient
  ) {
    super(injector);
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportShipmentsFromExcel';
    this.uploadPointUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportPointsFromExcel';
    this.uploadGoodDetailsUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportGoodsDetailsFromExcel';
    this.tripVases = AppConsts.remoteServiceBaseUrl + '/Helper/ImportTripVasesFromExcel';
    this.incidentResolved = new EventEmitter<void>();
  }

  ngOnInit() {
    // update Trip Service and send vases list to trip component
    this._shippingRequestsServiceProxy.getShippingRequestForView(this.ShippingRequest.id).subscribe((result) => {
      //this.shippingRequestForView = result;
      this.ShippingRequest = result.shippingRequest;
      this.VasListFromFather = result.shippingRequestVasDtoList;
      this.modifyShowBtnAddTrips(result.shippingRequest.canAddTrip);
      this.tripsByTmsEnabled = true;
      this._TripService.GetShippingRequestForViewOutput = result;
    });
  }

  /**
   * checks if the current user has the ability to edit a trip
   * @constructor
   */
  get CanEditTrip(): boolean {
    //check if shipper or  Carrier has a Saas feature and Can edit this trip
    return (
      this.feature.isEnabled('App.Shipper') ||
      this.isTachyonDealer ||
      (this.feature.isEnabled('App.CarrierAsASaas') &&
        this.ShippingRequest.carrierTenantId === this._TripService.GetShippingRequestForViewOutput.tenantId)
    );
  }

  getShippingRequestsTrips(event?: LazyLoadEvent) {
    this.changeDetectorRef.detectChanges();
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();
    this._shippingRequestTripsService
      .getAll(
        this.ShippingRequest.id,
        undefined,
        this.primengTableHelper.getSorting(this.dataTable) || 'startTripDate ASC',
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
        this.CanAssignDriverAndTruck = result.items[0]?.canAssignTrucksAndDrivers;
      });
  }

  // updateAddTripsByTmsFeature() {
  //   Swal.fire({
  //     title: this.l('areYouSure'),
  //     icon: 'warning',
  //     showCancelButton: true,
  //     confirmButtonText: this.l('Yes'),
  //     cancelButtonText: this.l('No'),
  //   }).then((result) => {
  //     if (result.value) {
  //       this.saving = true;
  //       this._shippingRequestTripsService.changeAddTripsByTmsFeature().subscribe(() => {
  //         this.saving = false;
  //       });
  //     } //end of if
  //   });
  // }
  refreshWhenIncidentResolved() {
    this.incidentResolved.emit();
  }

  reloadPage(): void {
    if (!!this.paginator) {
      this.paginator.changePage(this.paginator.getPage());
    }
  }
  ngAfterViewInit(): void {
    this.primengTableHelper.adjustScroll(this.dataTable);
    abp.event.on('ShippingRequestTripCreatedEvent', (args) => {
      this._shippingRequestsServiceProxy.canAddTripForShippingRequest(this.ShippingRequest.id).subscribe((result) => {
        this.ShippingRequest.canAddTrip = result;
        this.modifyShowBtnAddTrips(result);
      });
    });
  }

  private modifyShowBtnAddTrips(canAddTrip: boolean) {
    if (this._TripService.GetShippingRequestForViewOutput.shippingRequestFlag === ShippingRequestFlag.Normal) {
      this.showBtnAddTrips = canAddTrip;
    } else {
      const isBroker = this.hasCarrierClients && this.hasShipperClients;
      this.showBtnAddTrips =
        (this.isTachyonDealer &&
          (this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.Completed ||
            this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PrePrice ||
            this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PostPrice ||
            this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.NeedsAction)) ||
        ((this.isShipper || isBroker || this.isCarrierSaas) &&
          canAddTrip &&
          (this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PrePrice ||
            this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PostPrice ||
            this._TripService.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.NeedsAction));
    }
  }

  uploadExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    this.loading = true;
    formData.append('file', file, file.name);
    formData.append('ShippingRequestId', this.ShippingRequest.id.toString());
    this._httpClient
      .post<any>(this.uploadUrl, formData)
      .pipe(
        finalize(() => {
          this.excelFileUpload.clear();
        })
      )
      .subscribe((response) => {
        if (response.success) {
          this.list = response.result.importShipmentListDto;
          this.loading = false;
          this.notify.success(this.l('ImportProcessStart'));
          this.saving = true;
          this.modal.show(this.list);
        } else if (response.error != null) {
          this.loading = false;
          //this.notify.error(this.l('ImportFailed'));
          // this.notify.error(response.error.message);
          Swal.fire({
            icon: 'error',
            title: response.error.message,
            showConfirmButton: true,
          });
        }
      });
  }

  uploadPointsExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    this.loading = true;
    formData.append('file', file, file.name);
    formData.append('ShippingRequestId', this.ShippingRequest.id.toString());
    this._httpClient
      .post<any>(this.uploadPointUrl, formData)
      .pipe(
        finalize(() => {
          this.PointExcelFileUpload.clear();
        })
      )
      .subscribe((response) => {
        if (response.success) {
          this.pointsList = response.result.importPointListDto;
          this.loading = false;
          this.notify.success(this.l('ImportProcessStart'));
          this.saving = true;
          this.pointModal.show();
        } else if (response.error != null) {
          this.loading = false;
          // this.notify.error(this.l('ImportFailed'));
          // this.notify.error(response.error.message);
          Swal.fire({
            icon: 'error',
            title: response.error.message,
            showConfirmButton: true,
          });
        }
      });
  }

  uploadGoodDetailsExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    this.loading = true;
    formData.append('file', file, file.name);
    formData.append('ShippingRequestId', this.ShippingRequest.id.toString());
    this._httpClient
      .post<any>(this.uploadGoodDetailsUrl, formData)
      .pipe(
        finalize(() => {
          this.GoodDetailsExcelFileUpload.clear();
        })
      )
      .subscribe((response) => {
        if (response.success) {
          this.goodDetailsList = response.result.importGoodsDetaiListDto;
          this.loading = false;
          this.notify.success(this.l('ImportProcessStart'));
          this.saving = true;
          this.goodDetailsModal.show();
        } else if (response.error != null) {
          this.loading = false;
          // this.notify.error(this.l('ImportFailed'));
          // this.notify.error(response.error.message);
          Swal.fire({
            icon: 'error',
            title: response.error.message,
            showConfirmButton: true,
          });
        }
      });
  }

  uploadVasesExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    this.loading = true;
    formData.append('file', file, file.name);
    formData.append('ShippingRequestId', this.ShippingRequest.id.toString());
    this._httpClient
      .post<any>(this.tripVases, formData)
      .pipe(
        finalize(() => {
          this.VasesExcelFileUpload.clear();
        })
      )
      .subscribe((response) => {
        if (response.success) {
          this.vasesList = response.result.importTripVasesListDto;
          this.loading = false;
          this.notify.success(this.l('ImportProcessStart'));
          this.saving = true;
          this.VasesModal.show();
        } else if (response.error != null) {
          this.loading = false;
          // this.notify.error(this.l('ImportFailed'));
          // this.notify.error(response.error.message);
          Swal.fire({
            icon: 'error',
            title: response.error.message,
            showConfirmButton: true,
          });
        }
      });
  }

  onUploadExcelError(): void {
    // this.notify.error(this.l('ImportUploadFailed'));
    Swal.fire({
      icon: 'error',
      title: this.l('ImportUploadFailed'),
      showConfirmButton: true,
    });
  }

  /**
   * check if user can reset a trip
   * if user is Carrier/TMS and the Trip Status is not new/Delivered
   * @param record
   */
  canResetTrip(record): boolean {
    return (
      (this.isCarrier || this.isTachyonDealer) &&
      record.status !== this.ShippingRequestTripStatusEnum.New &&
      record.status !== this.ShippingRequestTripStatusEnum.Delivered
    );
  }
  ngOnChanges(changes: SimpleChanges): void {
    this.reloadPage();
  }
  getCancelStatus(statusId) {
    if (statusId === this.ShippingRequestTripCancelStatusEnum.Canceled) {
      return this.l('CanceledTrip');
    } else if (statusId === this.ShippingRequestTripCancelStatusEnum.Rejected) {
      return this.l('RejectedTripCancelation');
    } else if (statusId === this.ShippingRequestTripCancelStatusEnum.WaitingForTMSApproval) {
      return this.l('WaitingCancelApproveFromTMS');
    } else {
      return this.l('None');
    }
  }

  createBayanTrip(id: any) {
    this._shippingRequestTripsService.createBayanIntegrationTrip(id).subscribe(() => {});
  }

  printBayanTrip(id: any) {
    // this._shippingRequestTripsService.printBayanIntegrationTrip(id).subscribe((result) => {
    //   console.log(result);
    //   let file = new Blob([result], { type: 'application/pdf' });
    //   var fileURL = URL.createObjectURL(file);
    //   window.open(fileURL);
    // });

    const url = AppConsts.remoteServiceBaseUrl + '/File/PrintBayanIntegrationTrip?tripId=' + id;
    location.href = url;
    return url;
  }
}
