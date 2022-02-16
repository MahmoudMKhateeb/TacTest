import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Injector, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';

import {
  GetShippingRequestVasForViewDto,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  ShippingRequestsTripServiceProxy,
  ImportTripDto,
  ImportRoutePointDto,
} from '@shared/service-proxies/service-proxies';
import { CreateOrEditTripComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/createOrEditTripModal/createOrEditTrip.component';
import { ViewTripModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/viewTripModal/viewTripModal.component';
import { TripService } from '../trip.service';
import Swal from 'sweetalert2';
import { finalize } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { FileUpload } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'TripsForViewShippingRequest',
  templateUrl: './tripsForViewShippingRequest.component.html',
  styleUrls: ['./tripsForViewShippingRequest.component.scss'],
})
export class TripsForViewShippingRequestComponent extends AppComponentBase implements AfterViewInit {
  @ViewChild('dataTablechild', { static: false }) dataTable: Table;
  @ViewChild('paginatorchild', { static: false }) paginator: Paginator;
  @ViewChild('AddNewTripModal', { static: false }) AddNewTripModal: CreateOrEditTripComponent;
  @ViewChild('ViewTripModal', { static: false }) ViewTripModal: ViewTripModalComponent;
  @ViewChild('ExcelFileUpload', { static: false }) excelFileUpload: FileUpload;
  @ViewChild('PointExcelFileUpload', { static: false }) PointExcelFileUpload: FileUpload;
  @ViewChild('ViewImportedTripsModal', { static: false }) modal: ModalDirective;
  @ViewChild('ViewImportedPointsModal', { static: false }) pointModal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter();
  @Input() ShippingRequest: ShippingRequestDto;
  @Input() VasListFromFather: GetShippingRequestVasForViewDto[];
  tripsByTmsEnabled: boolean;
  saving = false;
  uploadUrl: string;
  uploadPointUrl: string;
  isArabic = false;
  active = false;
  list: ImportTripDto;
  pointsList: ImportRoutePointDto;
  loading: boolean = false;

  constructor(
    injector: Injector,
    private _TripService: TripService,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private changeDetectorRef: ChangeDetectorRef,
    private _shippingRequestTripsService: ShippingRequestsTripServiceProxy,
    private _httpClient: HttpClient
  ) {
    super(injector);
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportShipmentsFromExcel';
    this.uploadPointUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportPointsFromExcel';
  }

  ngOnInit(): void {
    this.isArabic = abp.localization.currentLanguage.name.startsWith('ar');
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
      });
  }

  updateAddTripsByTmsFeature() {
    Swal.fire({
      title: this.l('areYouSure'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.l('Yes'),
      cancelButtonText: this.l('No'),
    }).then((result) => {
      if (result.value) {
        this.saving = true;
        this._shippingRequestTripsService.changeAddTripsByTmsFeature().subscribe(() => {
          this.tripsByTmsEnabled = !this.tripsByTmsEnabled;
          this.saving = false;
        });
      } //end of if
    });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  ngAfterViewInit(): void {
    // update Trip Service and send vases list to trip component
    this._shippingRequestsServiceProxy.getShippingRequestForView(this.ShippingRequest.id).subscribe((result) => {
      this.VasListFromFather = result.shippingRequestVasDtoList;
      this._TripService.updateShippingRequest(result);
    });

    this.primengTableHelper.adjustScroll(this.dataTable);
    this.tripsByTmsEnabled = this.ShippingRequest.addTripsByTmsEnabled;
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
          this.modal.show();
        } else if (response.error != null) {
          this.loading = false;
          //this.notify.error(this.l('ImportFailed'));
          this.notify.error(response.error.message);
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
          this.notify.error(response.error.message);
        }
      });
  }

  onUploadExcelError(): void {
    this.notify.error(this.l('ImportTrucksUploadFailed'));
  }
}
