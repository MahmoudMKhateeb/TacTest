import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import {
  CreateOrEditShippingRequestTripDto,
  ShippingRequestDto,
  ShippingRequestsTripServiceProxy,
  TrackingServiceProxy,
  TrackingShippingRequestTripDto,
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { CreateOrEditTripComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trips/createOrEditTripModal/createOrEditTrip.component';
import { AppConsts } from '@shared/AppConsts';
import { FileUpload } from 'primeng/fileupload';
import { HttpClient } from '@angular/common/http';
import { finalize } from 'rxjs/operators';
import { ModalDirective } from 'ngx-bootstrap/modal';
import Swal from 'sweetalert2';
import { ViewImportedTripsFromExcelModalComponent } from '../shippingRequests/shippingRequests/ShippingRequestTrips/trips/ImportedTrips/view-imported-trips-from-excel-modal/view-imported-trips-from-excel-modal.component';
import { DxDataGridComponent } from '@node_modules/devextreme-angular/ui/data-grid';
import { Workbook } from 'exceljs';
import { exportDataGrid } from 'devextreme/excel_exporter';
import { saveAs } from 'file-saver';

@Component({
  selector: 'app-direct-trips',
  templateUrl: './direct-trips.component.html',
  styleUrls: ['./direct-trips.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class DirectTripsComponent extends AppComponentBase implements OnInit {
  @ViewChild('grid', { static: true }) dataGrid: DxDataGridComponent;
  @ViewChild('AddNewTripModal', { static: true }) addNewTripModal: CreateOrEditTripComponent;
  @ViewChild('ExcelFileUpload', { static: false }) excelFileUpload: FileUpload;
  @ViewChild('ViewImportedTripsModal', { static: false }) modal: ViewImportedTripsFromExcelModalComponent;
  @ViewChild('PointExcelFileUpload', { static: false }) PointExcelFileUpload: FileUpload;
  @ViewChild('GoodDetailsExcelFileUpload', { static: false }) GoodDetailsExcelFileUpload: FileUpload;
  @ViewChild('ViewImportedPointsModal', { static: false }) pointModal: ModalDirective;
  @ViewChild('ViewImportedGoodDetailsModal', { static: false }) goodDetailsModal: ModalDirective;

  popupPosition: any = { of: window, at: 'top', my: 'top', offset: { y: 10 } };
  dataSource: any = {};
  loading = false;
  uploadUrl: string;
  uploadPointUrl: string;
  uploadGoodDetailsUrl: string;
  list: any[] = [];
  saving = false;
  pointsList: any;
  goodDetailsList: any;

  shippingRequest: ShippingRequestDto = ShippingRequestDto.fromJS({
    splitInvoiceFlag: '',
    bidEndDate: undefined,
    bidStartDate: undefined,
    bidStatus: undefined,
    bidStatusTitle: '',
    canAddTrip: false,
    carrierPrice: 0,
    carrierTenantId: 0,
    endTripDate: undefined,
    goodCategoryId: 0,
    hasAccident: false,
    id: 0,
    isBid: false,
    isDirectRequest: false,
    isPriceAccepted: false,
    isRejected: false,
    isSaas: false,
    isTachyonDeal: false,
    numberOfDrops: 0,
    numberOfPacking: 0,
    numberOfTrips: 0,
    otherGoodsCategoryName: '',
    otherTransportTypeName: '',
    otherTrucksTypeName: '',
    packingTypeId: 0,
    price: 0,
    requestType: undefined,
    routeTypeId: undefined,
    shipperInvoiceNo: '',
    shipperReference: '',
    shippingTypeId: 0,
    startTripDate: moment(),
    status: undefined,
    statusTitle: '',
    totalWeight: 0,
    totalsTripsAddByShippier: 0,
  });
  tripSubDetails: TrackingShippingRequestTripDto;

  constructor(
    injector: Injector,
    private shippingRequestsTripServiceProxy: ShippingRequestsTripServiceProxy,
    private _httpClient: HttpClient,
    private _trackingServiceProxy: TrackingServiceProxy
  ) {
    super(injector);
    this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportShipmentsFromExcel';
    this.uploadPointUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportPointsFromExcel';
    this.uploadGoodDetailsUrl = AppConsts.remoteServiceBaseUrl + '/Helper/ImportGoodsDetailsFromExcel';
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    let self = this;

    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self.shippingRequestsTripServiceProxy
          .getAllDx(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  reloadPage() {
    this.getData();
  }

  uploadExcel(data: { files: File }): void {
    const formData: FormData = new FormData();
    const file = data.files[0];
    this.loading = true;
    formData.append('file', file, file.name);
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

  onUploadExcelError(): void {
    // this.notify.error(this.l('ImportUploadFailed'));
    Swal.fire({
      icon: 'error',
      title: this.l('ImportUploadFailed'),
      showConfirmButton: true,
    });
  }
  showEdit(id) {
    let record = new CreateOrEditShippingRequestTripDto();
    record.id = id;
    this.addNewTripModal.show(record);
  }

  getForView(selectedRow: any) {
    console.log('getForView selectedRow.key.id', selectedRow.key.id);
    this._trackingServiceProxy.getForView(selectedRow.key.id).subscribe((result) => {
      selectedRow.key.tripSubDetails = result;
    });
    setTimeout(() => {
      this.dataGrid.instance.getScrollable().update();
    }, 500);
  }

  collapsedRow(selectedRow: any) {
    console.log('collapsedRow selectedRow.key.id', selectedRow.key.id);
    selectedRow.key.tripSubDetails = undefined;
  }

  onExporting(e) {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Trips');

    exportDataGrid({
      component: e.component,
      worksheet,
      autoFilterEnabled: true,
      customizeCell: ({ gridCell, excelCell }) => {
        if (gridCell.rowType === 'data') {
          if (gridCell.column.dataField === 'waybillNumber') {
            excelCell.value = (<number>gridCell.value).toString();
          }
        }
      },
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'DataGrid.xlsx');
      });
    });
    e.cancel = true;
  }
}
