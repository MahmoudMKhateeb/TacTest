import { Component, ViewChild, Injector, OnInit, Output, EventEmitter, OnDestroy, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { TruckAttendancesServiceProxy, CreateOrEditTruckAttendanceDto, AttendaceStatus } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DxSchedulerComponent } from '@node_modules/devextreme-angular';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import * as moment from '@node_modules/moment';
import { Subscription } from 'rxjs';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AttendanceSchedularModel } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/attendance-schedular-model';
import { DedicatedTruckModel } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-truck-model';
import { AttendanceSchedularResources } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/attendance-schedular-resources';
import Swal from 'sweetalert2';

@Component({
  selector: 'dedicated-shipping-request-attendance-sheet-modal',
  templateUrl: './dedicated-shipping-request-attendance-sheet-modal.component.html',
  styleUrls: ['dedicated-shipping-request-attendance-sheet-modal.component.scss'],
})
export class DedicatedShippingRequestAttendanceSheetModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild(DxSchedulerComponent, { static: false }) scheduler: DxSchedulerComponent;
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Input('trucks') trucks: DedicatedTruckModel[] = [];
  @Input('rentalRange') rentalRange: { rentalStartDate: moment.Moment; rentalEndDate: moment.Moment } = {
    rentalStartDate: null,
    rentalEndDate: null,
  };
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  private truckAttendanceSubscription: Subscription;
  private dataSourceDevExtreme: any;
  searchTerm: string;
  saving = false;
  loading = true;
  dataSource: AttendanceSchedularModel[] = [];
  editAppointmentData: any;
  isCustomPopupVisible = false;
  currentDate: Date = new Date();
  allAttendaceStatus: any[] = [];
  // attendanceDateFrom: Date;
  // attendanceDateTo: Date;
  selectedTruckId: number;
  filteredTrucks: DedicatedTruckModel[] = [];
  attendanceSchedularResources: AttendanceSchedularResources[] = [];

  constructor(injector: Injector, private _truckAttendancesService: TruckAttendancesServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
    this.updateAppointment = this.updateAppointment.bind(this);
    this.onHiding = this.onHiding.bind(this);
  }

  ngOnInit() {
    this.allAttendaceStatus = (this.enumToArray.transform(AttendaceStatus) as any[]).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.attendanceSchedularResources = this.allAttendaceStatus.map((item, index) => {
      return {
        id: item.key,
        text: item.value,
        color: item.key === AttendaceStatus.Present ? '#28a745' : item.key === AttendaceStatus.Absent ? '#dc3545' : '#ffc107',
      };
    });
  }

  show(truckId?: number): void {
    // this.request = request;
    // this.rejectInput.id = request.id;
    this.getAllAttendance(truckId);
    this.selectedTruckId = truckId;
    this.filteredTrucks = this.trucks;
    this.modal.show();
  }

  close(): void {
    // this.rejectInput = new CancelShippingRequestInput();
    this.modal.hide();
  }

  send() {
    // this.saving = true;
    // this._currentServ
    //   .cancelShipment(this.rejectInput)
    //   .pipe(
    //     finalize(() => {
    //       this.saving = false;
    //     })
    //   )
    //   .subscribe(() => {
    //     this.request.status = ShippingRequestStatus.Cancled;
    //     this.request.statusTitle = 'Cancled';
    //     this.modalsave.emit(this.rejectInput.cancelReason);
    //     this.close();
    //     this.notify.success('SuccessfullyCancled');
    //   });
  }

  private getAllAttendance(truckId: number) {
    this.loading = true;
    let self = this;
    this.dataSourceDevExtreme = {};
    this.dataSourceDevExtreme.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._truckAttendancesService
          .getAll(truckId, JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            self.dataSource = (response.data as CreateOrEditTruckAttendanceDto[]).map((item) => {
              return new AttendanceSchedularModel({
                id: item.id,
                text: (item as any).attendanceStatusTitle,
                statusId: self.allAttendaceStatus.find((attendance) => attendance.value === (item as any).attendanceStatusTitle).key,
                startDate: item.attendanceDate,
                endDate: item.attendanceDate,
              });
            });
            console.log('response', response);
            console.log('self.dataSource', self.dataSource);
            self.loading = false;
            // return {
            //     data: response.data,
            //     totalCount: response.totalCount,
            //     summary: response.summary,
            //     groupCount: response.groupCount,
            // };
          })
          .catch((error) => {
            self.loading = false;
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
    this.dataSourceDevExtreme.store.load();
    // this.truckAttendanceSubscription = this._truckAttendancesService.getAll(truckId, '').subscribe((response) => {
    //     console.log('response', response);
    //     this.dataSource = response.data;
    // });
  }

  onAppointmentFormOpening(e: any): void {
    const isValidAppointment = this.isValidAppointment(e.component, e.appointmentData);
    e.cancel = true;
    if (!isValidAppointment) {
      this.notify.error(this.l('youCanNotAddOutsideRentalRange'));
      return;
    }
    console.log('e', e);
    this.editAppointmentData = { ...e.appointmentData };
    // this.attendanceDateFrom = isNotNullOrUndefined(this.editAppointmentData) && isNotNullOrUndefined(this.editAppointmentData.attendanceDate) ? this.editAppointmentData.attendanceDate.toDate() : null;
    // this.attendanceDateTo = isNotNullOrUndefined(this.editAppointmentData) && isNotNullOrUndefined(this.editAppointmentData.attendanceDate) ? this.editAppointmentData.attendanceDate.toDate() : null;
    // if (this.editAppointmentData.id) {
    // }
    this.isCustomPopupVisible = true;
    console.log('this.editAppointmentData', this.editAppointmentData);
  }

  onHiding(e: any): void {
    this.editAppointmentData = new CreateOrEditTruckAttendanceDto();
    // this.editAppointmentData.attendanceDate = moment(this.attendanceDateFrom);
    // this.editAppointmentData.attendanceDate = moment(this.attendanceDateFrom);
    // this.attendanceDateFrom = null;
    // this.attendanceDateTo = null;
    this.isCustomPopupVisible = false;
  }

  updateAppointment(): void {
    if (!!this.editAppointmentData?.statusId?.toString() && !!this.editAppointmentData.startDate && !!this.editAppointmentData.endDate) {
      this.editAppointmentData.text = this.allAttendaceStatus.find((item) => item.key === this.editAppointmentData.statusId).value;
      this.createOrEditAttendance();
    }
    this.onHiding(null);
  }

  createOrEditAttendance() {
    const payload = new CreateOrEditTruckAttendanceDto(this.editAppointmentData);
    if (!!this.editAppointmentData?.id?.toString()) {
      payload.id = this.editAppointmentData.id;
      payload.startDate = null;
      payload.endDate = null;
    } else {
      payload.startDate = moment(new Date(this.editAppointmentData.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000);
      payload.endDate = moment(this.editAppointmentData.endDate);
    }
    payload.attendaceStatus = this.editAppointmentData.statusId;
    payload.dedicatedShippingRequestTruckId = this.selectedTruckId;
    this.loading = true;
    this._truckAttendancesService.createOrEdit(payload).subscribe(
      (res) => {
        this.getAllAttendance(this.selectedTruckId);
      },
      (error) => {
        this.loading = false;
      }
    );
  }

  selectTruck(id: number) {
    this.selectedTruckId = id;
    this.getAllAttendance(id);
  }

  filterTrucks() {
    this.filteredTrucks = this.trucks.filter(
      (truck) =>
        truck.plateNumber.toLowerCase().search(this.searchTerm.toLowerCase()) > -1 ||
        truck.status.toLowerCase().search(this.searchTerm.toLowerCase()) > -1
    );
  }

  ngOnDestroy() {
    if (isNotNullOrUndefined(this.truckAttendanceSubscription) && !this.truckAttendanceSubscription.closed) {
      this.truckAttendanceSubscription.unsubscribe();
    }
    this.trucks = [];
    this.filteredTrucks = [];
  }

  isDisabledDateCell(date: Date) {
    const localeDateMoment = moment.utc(date);
    if (
      !isNotNullOrUndefined(this.rentalRange) ||
      !isNotNullOrUndefined(this.rentalRange.rentalStartDate) ||
      !isNotNullOrUndefined(this.rentalRange.rentalEndDate)
    ) {
      return false;
    }
    const startDate = this.rentalRange?.rentalStartDate.toISOString().split('T')[0];
    const endDate = this.rentalRange?.rentalEndDate.toISOString().split('T')[0];
    return localeDateMoment.clone().add('d', 1).isBefore(moment(startDate)) || localeDateMoment.clone().isAfter(moment(endDate));
  }

  isValidAppointment(component: any, appointmentData: any) {
    const startDate = new Date(appointmentData.startDate);
    return !this.isDisabledDateCell(startDate);
  }

  onAppointmentDeleting(e: any) {
    e.cancel = true;
    this.loading = true;
    this._truckAttendancesService.delete(e.appointmentData.id).subscribe(
      (res) => {
        this.getAllAttendance(this.selectedTruckId);
      },
      (error) => {
        this.loading = false;
      }
    );
  }
}
