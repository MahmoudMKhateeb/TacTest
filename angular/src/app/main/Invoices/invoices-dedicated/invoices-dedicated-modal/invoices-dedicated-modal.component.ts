import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';

import {
  CommonLookupServiceProxy,
  CreateOrEditDedicatedInvoiceDto,
  CreateOrEditDedicatedInvoiceItemDto,
  DedicatedDynamiceInvoicesServiceProxy,
  DedicatedShippingRequestsServiceProxy,
  DynamicInvoiceItemDto,
  DynamicInvoiceServiceProxy,
  InvoiceAccountType,
  InvoiceServiceProxy,
  ISelectItemDto,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  TenantCityLookupTableDto,
  TenantRegistrationServiceProxy,
  TruckAttendancesServiceProxy,
  WorkingDayType,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import Swal from 'sweetalert2';
import { NgForm } from '@angular/forms';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-invoices-dedicated-modal',
  styleUrls: ['./invoices-dedicated-modal.component.css'],
  templateUrl: './invoices-dedicated-modal.component.html',
})
export class InvoiceDedicatedModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[] = [];
  dataSourceForEdit: CreateOrEditDedicatedInvoiceItemDto;
  root: CreateOrEditDedicatedInvoiceDto = new CreateOrEditDedicatedInvoiceDto();
  activeIndex: number = null;
  isView: boolean;
  dedicatedShippingRequests: SelectItemDto[];
  selectedDedicateTruckId: number;
  dedicateTrucks: SelectItemDto[];
  filteredDedicateTrucks: SelectItemDto[];
  pricePerDay: number;
  allNumberOfDays: number;
  allNumberOfDaysUpdate = new Subject<number>();
  taxVat: number;
  workingDayType = WorkingDayType;
  allWorkingDayTypes: any[] = [];
  private selectedShippingRequestId: number;
  truckValidationMap: Map<number, Array<number>> = new Map();
  filteredWorkingDayTypes: any[] = [];

  constructor(
    injector: Injector,
    private _currentSrv: InvoiceServiceProxy,
    private _ShippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _TenantRegistration: TenantRegistrationServiceProxy,
    private _DynamicInvoiceServiceProxy: DynamicInvoiceServiceProxy,
    private _DedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy,
    private _DedicatedDynamiceInvoicesServiceProxy: DedicatedDynamiceInvoicesServiceProxy,
    private _AttendanceSheetServiceProxy: TruckAttendancesServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _DateFormatterService: DateFormatterService,
    private _EnumToArrayPipeService: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit() {
    if (!isNotNullOrUndefined(this.root.dedicatedInvoiceItems)) {
      this.root.dedicatedInvoiceItems = [];
    }
    this.subscribeToAllNumberOfDaysChanges();
  }

  private getTaxVat() {
    this._DedicatedDynamiceInvoicesServiceProxy.getTaxVat().subscribe((res) => {
      this.taxVat = res;
    });
  }

  show(invoiceAccountType: number, id?: number, tenantName?: string, isView = false): void {
    this.isView = isView;
    this.filteredWorkingDayTypes = this.allWorkingDayTypes = this._EnumToArrayPipeService.transform(WorkingDayType).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.getTaxVat();
    if (isNotNullOrUndefined(id)) {
      this.getForEdit(id, tenantName);
    }
    this.root.invoiceAccountType = invoiceAccountType;
    this.Tenant = undefined;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
    if (!this.root?.tenantId || !this.root?.shippingRequestId || !this.selectedDedicateTruckId) {
      return;
    }
    const payload = CreateOrEditDedicatedInvoiceDto.fromJS(this.root.toJSON());
    payload.tenantId = (payload.tenantId as any) instanceof Object ? (payload.tenantId as any).id : payload.tenantId;
    this._DedicatedDynamiceInvoicesServiceProxy.createOrEdit(payload).subscribe(
      (result) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
        this.saving = false;
      },
      (error) => {
        this.saving = false;
      },
      () => {
        this.saving = false;
      }
    );
    this.activeIndex = null;
  }

  close(): void {
    this.root = new CreateOrEditDedicatedInvoiceDto();
    this.dataSourceForEdit = null;
    this.active = false;
    this.Tenants = [];
    this.activeIndex = null;
    this.truckValidationMap = new Map();
    this.modal.hide();
  }

  search(event, initValue = false) {
    this._CommonServ.getAutoCompleteTenantsByAccountType(event.query, this.root.invoiceAccountType).subscribe((result) => {
      this.Tenants = result;
      if (initValue) {
        const tenant = this.Tenants.find((item) => Number(item.id) === this.root.tenantId);
        (this.root.tenantId as any) = tenant;
        this.getDedicatedRequestsByTenant(this.root.tenantId);
      }
    });
  }

  private getForEdit(id: number, tenantName: string) {
    this._DedicatedDynamiceInvoicesServiceProxy.getDedicatedInvoiceForEdit(id).subscribe((data) => {
      this.root = data;
      this.search({ query: tenantName }, true);
    });
  }

  addNew() {
    this.dataSourceForEdit = new CreateOrEditDedicatedInvoiceItemDto();
    this.selectedDedicateTruckId = null;
  }

  saveToArray() {
    if (
      !isNotNullOrUndefined(this.dataSourceForEdit) ||
      !isNotNullOrUndefined(this.dataSourceForEdit.workingDayType) ||
      !isNotNullOrUndefined(this.dataSourceForEdit.numberOfDays) ||
      !isNotNullOrUndefined(this.allNumberOfDays)
    ) {
      Swal.fire({
        title: this.l('FormIsNotValidMessage'),
        icon: 'warning',
        showCloseButton: true,
      });
      return;
    }
    if (!isNotNullOrUndefined(this.root.dedicatedInvoiceItems)) {
      this.root.dedicatedInvoiceItems = [];
    }
    if (this.dataSourceForEdit.workingDayType == this.workingDayType.OverTime) {
      this.dataSourceForEdit.pricePerDay = this.pricePerDay;
    }
    this.dataSourceForEdit.dedicatedShippingRequestTruckId = Number(this.selectedDedicateTruckId);
    this.dataSourceForEdit.allNumberDays = this.allNumberOfDays;
    if (
      !isNotNullOrUndefined(this.dataSourceForEdit.id) &&
      isNotNullOrUndefined(this.dataSourceForEdit.numberOfDays) &&
      isNotNullOrUndefined(this.dataSourceForEdit.workingDayType) &&
      !isNotNullOrUndefined(this.activeIndex)
    ) {
      this.root.dedicatedInvoiceItems.push(this.dataSourceForEdit);
      this.validateTrucks(this.dataSourceForEdit);
      this.dataSourceForEdit = null;
      return;
    }
    this.validateTrucks(this.dataSourceForEdit);
    this.root.dedicatedInvoiceItems[this.activeIndex] = this.dataSourceForEdit;
    this.activeIndex = null;
    this.dataSourceForEdit = null;
  }

  deleteRow(i: number) {
    this.root.dedicatedInvoiceItems.splice(i, 1);
  }

  editRow(i: number, row: CreateOrEditDedicatedInvoiceItemDto) {
    const truckIndex = this.filteredDedicateTrucks.findIndex((truck) => Number(truck.id) == row.dedicatedShippingRequestTruckId);
    if (truckIndex === -1) {
      this.filteredDedicateTrucks.push(this.dedicateTrucks.find((truck) => Number(truck.id) == row.dedicatedShippingRequestTruckId));
    }
    const workingDayTypeIndex = this.filteredWorkingDayTypes.findIndex((item) => Number(item.key) == Number(row.workingDayType));
    if (workingDayTypeIndex === -1) {
      this.filteredWorkingDayTypes.push(this.allWorkingDayTypes.find((item) => Number(item.key) == Number(row.workingDayType)));
    }
    this.dataSourceForEdit = CreateOrEditDedicatedInvoiceItemDto.fromJS(row.toJSON());
    this.allNumberOfDays = row.allNumberDays;
    this.selectedDedicateTruckId = row.dedicatedShippingRequestTruckId;
    this.activeIndex = i;
  }

  cancelAddToArray() {
    if (this.truckValidationMap.has(this.selectedDedicateTruckId)) {
      if (new Set(this.truckValidationMap.get(this.selectedDedicateTruckId)).size == 2) {
        const truckIndex = this.filteredDedicateTrucks.findIndex((truck) => Number(truck.id) == this.selectedDedicateTruckId);
        this.filteredDedicateTrucks.splice(truckIndex, 1);
      }
    }
    this.dataSourceForEdit = null;
    this.activeIndex = null;
  }

  isFormInvalid(Form: NgForm): boolean {
    return (
      Form.invalid || !this.root.dedicatedInvoiceItems || this.root.dedicatedInvoiceItems.length === 0 || isNotNullOrUndefined(this.dataSourceForEdit)
    );
  }

  getDedicatedRequestsByTenant(event) {
    this.dedicateTrucks = [];
    this.filteredDedicateTrucks = [];
    this.truckValidationMap = new Map();
    this.filteredWorkingDayTypes = this.allWorkingDayTypes;
    this.dedicatedShippingRequests = [];
    this._DedicatedShippingRequestsServiceProxy.getDedicatedRequestsByTenant(Number(event.id)).subscribe((res) => {
      this.dedicatedShippingRequests = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  getDedicateTrucksByRequest(event) {
    this.dedicateTrucks = [];
    this.filteredDedicateTrucks = [];
    this.truckValidationMap = new Map();
    this.filteredWorkingDayTypes = this.allWorkingDayTypes;
    this._DedicatedShippingRequestsServiceProxy.getDedicateTrucksByRequest(Number(event.value)).subscribe((res) => {
      this.filteredDedicateTrucks = this.dedicateTrucks = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
    // const tenantId = (this.root.tenantId as any) instanceof Object ? (this.root.tenantId as any).id : this.root.tenantId;
    this.selectedShippingRequestId = Number(event.value);
    this.getDefaultNumberOfDays();
  }

  private getDedicatedPricePerDay() {
    this._DedicatedDynamiceInvoicesServiceProxy
      .getDedicatePricePerDay(this.selectedShippingRequestId, this.root.invoiceAccountType, Number(this.allNumberOfDays))
      .subscribe((res) => {
        this.pricePerDay = res;
        this.calculateValues(null);
      });
  }

  calculateValues($event: any) {
    if (!isNotNullOrUndefined(this.dataSourceForEdit)) {
      return;
    }
    this.dataSourceForEdit.itemSubTotalAmount = this.dataSourceForEdit?.numberOfDays * this.pricePerDay;
    this.dataSourceForEdit.vatAmount = (this.taxVat * this.dataSourceForEdit?.itemSubTotalAmount) / 100;
    this.dataSourceForEdit.itemTotalAmount = this.dataSourceForEdit?.itemSubTotalAmount + this.dataSourceForEdit?.vatAmount;
  }

  getWorkingDayTitle(workingDayType: WorkingDayType) {
    return this.allWorkingDayTypes.length > 0 ? this.allWorkingDayTypes.find((item) => Number(item.key) === workingDayType).value : '';
  }

  // getTotalOfSubtotalAmounts() {
  //     let totalOfSubtotalAmounts = 0;
  //     this.root?.dedicatedInvoiceItems.map((item) => totalOfSubtotalAmounts += item.itemSubTotalAmount);
  //     return totalOfSubtotalAmounts;
  // }
  // getTotalOfVatAmount() {
  //     let totalOfVatAmount = 0;
  //     this.root?.dedicatedInvoiceItems.map((item) => totalOfVatAmount += item.vatAmount);
  //     return totalOfVatAmount;
  // }
  // getTotalOfTotalAmount() {
  //     let totalOfTotalAmount = 0;
  //     this.root?.dedicatedInvoiceItems.map((item) => totalOfTotalAmount += item.itemTotalAmount);
  //     return totalOfTotalAmount;
  // }

  getTotalByAttributeName(field: string) {
    let total = 0;
    this.root?.dedicatedInvoiceItems.map((item) => (total += item[field]));
    return total;
  }

  LoadNumberOfDays($event: any) {
    if ($event?.value == WorkingDayType.Normal) {
      this.getDefaultNumberOfDays();
    }
    if ($event?.value == WorkingDayType.OverTime) {
      this.allNumberOfDays = 0;
    }
    if (this.dataSourceForEdit.workingDayType && this.selectedDedicateTruckId) {
      this._AttendanceSheetServiceProxy
        .getDaysNumberByWorkingDayType(this.dataSourceForEdit.workingDayType, this.selectedDedicateTruckId)
        .subscribe((res) => {
          this.dataSourceForEdit.numberOfDays = res;
          this.calculateValues(null);
        });
    }
  }

  private getDefaultNumberOfDays() {
    this._DedicatedDynamiceInvoicesServiceProxy.getDefaultNumberOfDays(this.selectedShippingRequestId).subscribe((res) => {
      this.allNumberOfDays = res;
      this.getDedicatedPricePerDay();
    });
  }

  private subscribeToAllNumberOfDaysChanges() {
    this.allNumberOfDaysUpdate.pipe(debounceTime(300), distinctUntilChanged()).subscribe((value) => {
      this.getDedicatedPricePerDay();
    });
  }

  private validateTrucks(dataSourceForEdit: CreateOrEditDedicatedInvoiceItemDto) {
    let array = [];
    if (this.truckValidationMap.has(dataSourceForEdit.dedicatedShippingRequestTruckId)) {
      array = this.truckValidationMap.get(dataSourceForEdit.dedicatedShippingRequestTruckId);
    }
    array.push(dataSourceForEdit.workingDayType);
    if (new Set(array).size == 2) {
      this.filteredDedicateTrucks = this.dedicateTrucks.filter((item) => Number(item.id) != this.selectedDedicateTruckId);
    }
    this.truckValidationMap.set(dataSourceForEdit.dedicatedShippingRequestTruckId, array);
  }

  filterWorkingDayTypes() {
    const allWorkingDayTypes = [...this.allWorkingDayTypes];
    if (this.truckValidationMap.size > 0) {
      if (this.truckValidationMap.has(this.selectedDedicateTruckId)) {
        this.filteredWorkingDayTypes = allWorkingDayTypes.filter(
          (item) => !this.truckValidationMap.get(this.selectedDedicateTruckId).includes(item.key)
        );
        if (isNotNullOrUndefined(this.activeIndex) && this.filteredWorkingDayTypes.length === 0) {
          this.filteredWorkingDayTypes = allWorkingDayTypes.filter((item) =>
            this.truckValidationMap.get(this.selectedDedicateTruckId).includes(item.key)
          );
        }
        return;
      }
    }
    this.filteredWorkingDayTypes = allWorkingDayTypes;
  }
}
