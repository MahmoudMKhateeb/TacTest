import { Component, ViewChild, Injector, OnInit, Output, EventEmitter, OnDestroy, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  TruckAttendancesServiceProxy,
  CreateOrEditTruckAttendanceDto,
  AttendaceStatus,
  DedicatedShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { DxSchedulerComponent } from '@node_modules/devextreme-angular/ui/scheduler';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import * as moment from '@node_modules/moment';
import { Subscription } from 'rxjs';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AttendanceSchedularModel } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/attendance-schedular-model';
import { DedicatedTruckModel } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/dedicated-truck-model';
import { AttendanceSchedularResources } from '@app/main/shippingRequests/dedicatedShippingRequest/dedicated-shipping-request-attendance-sheet-modal/attendance-schedular-resources';
import Swal from 'sweetalert2';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';

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
  selectedTruckId: number;
  filteredTrucks: DedicatedTruckModel[] = [];
  attendanceSchedularResources: AttendanceSchedularResources[] = [];
  dataSourceForTrucks: any = {};
  shippingRequestId: number;
  canEditTruck: boolean;
  isRequestStatusCompleted: boolean;

  constructor(
    injector: Injector,
    private _truckAttendancesService: TruckAttendancesServiceProxy,
    private enumToArray: EnumToArrayPipe,
    private dedicatedShippingRequestsService: DedicatedShippingRequestsServiceProxy,
    private dateService: DateFormatterService
  ) {
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

  show(
    isRequestStatusCompleted?: boolean,
    truckId?: number,
    shippingRequestId?: number,
    rentalRange?: { rentalStartDate: moment.Moment; rentalEndDate: moment.Moment }
  ): void {
    this.isRequestStatusCompleted = isRequestStatusCompleted;
    if (isNotNullOrUndefined(shippingRequestId)) {
      this.shippingRequestId = shippingRequestId;
    }
    if (isNotNullOrUndefined(rentalRange)) {
      this.rentalRange = rentalRange;
    }
    if (isNotNullOrUndefined(truckId)) {
      this.getAllAttendance(truckId);
      this.selectedTruckId = truckId;
      this.filteredTrucks = this.trucks;
      this.selectTruck(truckId);
    } else {
      this.filteredTrucks = [];
      this.trucks = [];
      this.getTrucks();
    }
    this.modal.show();
  }

  close(): void {
    this.dataSourceForTrucks = {};
    this.dataSource = [];
    this.filteredTrucks = [];
    this.shippingRequestId = null;
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
            self.loading = false;
          })
          .catch((error) => {
            self.loading = false;
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
    this.dataSourceDevExtreme.store.load();
  }

  onAppointmentFormOpening(e: any): void {
    if (this.isCarrier && !this.isCarrierSaas) {
      e.cancel = true;
      return;
    }
    if (!this.canEditTruck) {
      e.cancel = true;
      return;
    }
    if ((this.isCarrierSaas || this.isShipper) && this.isRequestStatusCompleted) {
      e.cancel = true;
      return;
    }
    const isValidAppointment = this.isValidAppointment(e.component, e.appointmentData);
    e.cancel = true;
    if (!isValidAppointment) {
      this.notify.error(this.l('youCanNotAddOutsideRentalRange'));
      return;
    }
    let appointment;
    if (this.scheduler.selectedCellData.length > 0) {
      appointment = this.dataSource.find((item) => {
        const selectedStartDate = new Date(this.scheduler.selectedCellData[0].startDate);
        const selectedStartDateMoment = moment({
          y: selectedStartDate.getFullYear(),
          M: selectedStartDate.getMonth(),
          d: selectedStartDate.getDate(),
        });
        const itemStartDate = moment(item.startDate);
        const isSame = moment(itemStartDate.format('yyyy-MM-DD')).isSame(moment(selectedStartDateMoment.format('yyyy-MM-DD')));
        return isSame;
      });
    }
    this.editAppointmentData = isNotNullOrUndefined(appointment) ? { ...appointment } : { ...e.appointmentData };
    this.isCustomPopupVisible = true;
  }

  onHiding(e: any): void {
    this.editAppointmentData = new CreateOrEditTruckAttendanceDto();
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
    const startDate = new Date(this.editAppointmentData.startDate);
    const endDate = new Date(this.editAppointmentData.endDate);
    this.editAppointmentData.startDate = moment.utc({ y: startDate.getFullYear(), M: startDate.getMonth(), d: startDate.getDate() });
    this.editAppointmentData.endDate = moment.utc({ y: endDate.getFullYear(), M: endDate.getMonth(), d: endDate.getDate() });
    const payload = new CreateOrEditTruckAttendanceDto(this.editAppointmentData);
    if (!!this.editAppointmentData?.id?.toString()) {
      payload.id = this.editAppointmentData.id;
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
    const truck = this.trucks.find((truck) => truck.id === this.selectedTruckId);
    this.canEditTruck = isNotNullOrUndefined(truck) && !truck.submitInvoiceId && !truck.invoiceId;
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
    const localeDateMoment = moment({ y: date.getFullYear(), M: date.getMonth(), d: date.getDate() });
    if (
      !isNotNullOrUndefined(this.rentalRange) ||
      !isNotNullOrUndefined(this.rentalRange.rentalStartDate) ||
      !isNotNullOrUndefined(this.rentalRange.rentalEndDate)
    ) {
      return false;
    }
    const startDate = new Date(this.rentalRange.rentalStartDate.toISOString());
    const startDateMoment = moment({ y: startDate.getFullYear(), M: startDate.getMonth(), d: startDate.getDate() });
    const endDate = new Date(this.rentalRange.rentalEndDate.toISOString());
    const endDateMoment = moment({ y: endDate.getFullYear(), M: endDate.getMonth(), d: endDate.getDate() });
    return localeDateMoment.clone().isBefore(startDateMoment) || localeDateMoment.clone().isAfter(endDateMoment);
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

  getTrucks() {
    let self = this;
    this.dataSourceForTrucks = {};
    this.dataSourceForTrucks.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self.dedicatedShippingRequestsService
          .getAllDedicatedTrucks(JSON.stringify(loadOptions), self.shippingRequestId)
          .toPromise()
          .then((response) => {
            self.trucks = response.data;
            self.filteredTrucks = self.trucks;
            if (response.data.length > 0) {
              self.selectTruck(self.trucks[0].id);
            }
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });
    this.dataSourceForTrucks.store.load();
  }

  getBGColor(statusId) {
    const status = this.attendanceSchedularResources.find((item) => item.id === statusId);
    return status.color;
  }

  deleteAttendance($event, id: number) {
    this.scheduler.instance.hideAppointmentTooltip();
    $event.stopPropagation();
    $event.stopImmediatePropagation();
    this._truckAttendancesService.delete(id).subscribe(
      (res) => {
        this.getAllAttendance(this.selectedTruckId);
      },
      (error) => {
        this.loading = false;
      }
    );
  }
}
