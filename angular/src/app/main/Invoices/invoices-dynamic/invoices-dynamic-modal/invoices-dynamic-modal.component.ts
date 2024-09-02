import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/common/app-component-base';

import {
  InvoiceServiceProxy,
  CommonLookupServiceProxy,
  ISelectItemDto,
  SelectItemDto,
  DynamicInvoiceItemDto,
  DynamicInvoiceServiceProxy,
  DynamicInvoiceForViewDto,
  TenantServiceProxy,
  TenantRegistrationServiceProxy,
  ShippingRequestsServiceProxy,
  TenantCityLookupTableDto,
  CreateOrEditDynamicInvoiceDto,
  PricePackageServiceProxy,
  CreateOrEditDynamicInvoiceCustomItemDto,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import * as moment from 'moment';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import Swal from 'sweetalert2';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'invoices-dynamic-modal',
  styleUrls: ['./invoices-dynamic-modal.component.css'],
  templateUrl: './invoices-dynamic-modal.component.html',
})
export class InvoiceDynamicModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Tenant: ISelectItemDto;
  TenantDto: ISelectItemDto;
  Tenants: ISelectItemDto[] = [];
  Waybills: number[] = [];
  SelectedWaybills: SelectItemDto[] = [];
  edition: string;
  isBrokerPayableInvoice: boolean;
  dataSource: DynamicInvoiceItemDto[] = [];
  dataSourceForEdit: DynamicInvoiceItemDto;
  notes: any;
  root: DynamicInvoiceForViewDto = new DynamicInvoiceForViewDto();
  trucks: SelectItemDto[] = [];
  cities: TenantCityLookupTableDto[] = [];
  trucksFiltered: SelectItemDto[] = [];
  originCitiesFiltered: TenantCityLookupTableDto[] = [];
  destCitiesFiltered: TenantCityLookupTableDto[] = [];
  activeIndex: number = null;
  isView: boolean;

  // Custom Items
  customItemsDataSource: CreateOrEditDynamicInvoiceCustomItemDto[] = [];
  customItemForEdit: CreateOrEditDynamicInvoiceCustomItemDto;
  activeCustomItemIndex: number = null;

  constructor(
    injector: Injector,
    private _currentSrv: InvoiceServiceProxy,
    private _TenantServiceproxy: TenantServiceProxy,
    private _pricePackageServiceProxy: PricePackageServiceProxy,
    private _ShippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _TenantRegistration: TenantRegistrationServiceProxy,
    private _DynamicInvoiceServiceProxy: DynamicInvoiceServiceProxy,
    private _CommonServ: CommonLookupServiceProxy,
    private _DateFormatterService: DateFormatterService
  ) {
    super(injector);
  }

  ngOnInit() {
    if (!isNotNullOrUndefined(this.root.items)) {
      this.root.items = [];
    }
    if (!isNotNullOrUndefined(this.root.customItems)) {
      this.root.customItems = [];
    }
    this.getAllTrucksTypeForTableDropdown();
    this.GetAllCitiesForTableDropdown();
    this.isBrokerPayableInvoice = true;
  }

  show(forWho: string, id?: number, isView = false): void {
    this.isView = isView;
    this.getAllTrucksTypeForTableDropdown();
    this.GetAllCitiesForTableDropdown();
    if (isNotNullOrUndefined(id)) {
      this.getForView(id);
    }
    this.edition = forWho;
    this.Tenant = undefined;
    this.Waybills = undefined;
    this.SelectedWaybills = undefined;
    this.active = true;
    this.modal.show();
  }

  getAllTrucksTypeForTableDropdown() {
    this._DynamicInvoiceServiceProxy.getAllTrucks().subscribe(
      (response) => (this.trucks = response.items),
      (error) => {}
    );
  }

  GetAllCitiesForTableDropdown() {
    this._TenantRegistration.getAllCitiesForTableDropdown(null).subscribe(
      (cities) => {
        this.cities = cities;
      },
      (error) => {}
    );
  }

  save(): void {
    this.saving = true;

    if (!this.Tenant?.id) {
      Swal.fire({
        title: this.l('Tenant is required'),
        icon: 'warning',
        showCloseButton: true,
      });
      this.saving = false;
      return;
    }

    const items = [...this.root.items];

    // Process items to reset certain fields if waybillNumber is present
    items.forEach((item) => {
      if (item.waybillNumber) {
        item.truckId = null;
        item.workDate = null;
        item.quantity = null;
        item.destinationCityId = null;
        item.originCityId = null;
        item.containerNumber = null;
      }
    });

    const body = new CreateOrEditDynamicInvoiceDto();
    body.id = this.root.id ? this.root.id : null;
    body.items = items;
    body.notes = this.notes;
    body.customItems = this.root.customItems || []; // Default to empty array if customItems is not set

    if (this.edition === 'shipper' || (this.edition === 'broker' && this.isBrokerPayableInvoice)) {
      body.creditTenantId = Number(this.Tenant.id);
    } else if (this.edition === 'carrier' || (this.edition === 'broker' && !this.isBrokerPayableInvoice)) {
      body.debitTenantId = Number(this.Tenant.id);
    }

    this._DynamicInvoiceServiceProxy.createOrEdit(body).subscribe(
      () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      },
      () => {
        this.saving = false;
      },
      () => {
        this.saving = false;
      }
    );
  }

  close(): void {
    this.root = new DynamicInvoiceForViewDto();
    this.isBrokerPayableInvoice = false;
    this.edition = undefined;
    this.active = false;
    this.modal.hide();
    this.Tenants = [];
    this.Waybills = [];
    this.SelectedWaybills = [];
    this.dataSource = [];
    this.dataSourceForEdit = null;
    this.customItemsDataSource = [];
    this.customItemForEdit = null;
    this.notes = null;
    this.trucks = [];
    this.cities = [];
    this.activeIndex = null;
    this.activeCustomItemIndex = null;
  }

  search(tenantName: string, initValue = false) {
    this._CommonServ.getAutoCompleteTenants(tenantName, this.edition).subscribe((result) => {
      this.Tenants = result;
      if (initValue) {
        this.Tenant = this.Tenants[0];
        this.searchForWaybills('');
      }
    });
  }

  LoadWaybills(event): void {
    this.getAllTrucksTypeForTableDropdown();
    this.GetAllCitiesForTableDropdown();
  }

  searchForWaybills(event): void {
    this._DynamicInvoiceServiceProxy.searchByWaybillNumber(event.query).subscribe((res) => {
      this.Waybills = res;
    });
  }

  private getForView(id: number) {
    this._DynamicInvoiceServiceProxy.getForView(id).subscribe(
      (data) => {
        this.root = data;
        this.notes = this.root.notes;
        let tenantName = isNotNullOrUndefined(data.creditCompany) ? data.creditCompany : data.debitCompany;
        this.search(tenantName, true);
        this.dataSource = this.root.items;
        this.customItemsDataSource = this.root.customItems;
        if (this.root.items.length > 0) {
          this.root.items.map((item) => {
            item.workDate = !!item.waybillNumber ? null : moment(item.workDate);
            if (isNotNullOrUndefined(item.waybillNumber)) {
              this.fillDynamicInvoicItem(Number(item.waybillNumber), item);
            }
          });
        }
      },
      (error) => {}
    );
  }

  // Waybills Logic
  addNew() {
    this.dataSourceForEdit = new DynamicInvoiceItemDto();
  }

  saveToArray() {
    if (
      !isNotNullOrUndefined(this.dataSourceForEdit) ||
      !isNotNullOrUndefined(this.dataSourceForEdit.price) ||
      !isNotNullOrUndefined(this.dataSourceForEdit.description) ||
      ('' + this.dataSourceForEdit.description).length === 0
    ) {
      Swal.fire({
        title: this.l('FormIsNotValidMessage'),
        icon: 'warning',
        showCloseButton: true,
      });
      return;
    }
    if (!isNotNullOrUndefined(this.root.items)) {
      this.root.items = [];
    }
    if (isNotNullOrUndefined(this.dataSourceForEdit.workDate)) {
      this.dataSourceForEdit.workDate = moment(this.dataSourceForEdit.workDate);
    }
    if (isNotNullOrUndefined(this.dataSourceForEdit.originCityId) && (this.dataSourceForEdit.originCityId as any) instanceof Object) {
      this.dataSourceForEdit.originCityId = (this.dataSourceForEdit.originCityId as any).id;
    }
    if (isNotNullOrUndefined(this.dataSourceForEdit.destinationCityId) && (this.dataSourceForEdit.destinationCityId as any) instanceof Object) {
      this.dataSourceForEdit.destinationCityId = (this.dataSourceForEdit.destinationCityId as any).id;
    }
    if (isNotNullOrUndefined(this.dataSourceForEdit.truckId) && (this.dataSourceForEdit.truckId as any) instanceof Object) {
      this.dataSourceForEdit.truckId = (this.dataSourceForEdit.truckId as any).id;
    }

    if (
      !isNotNullOrUndefined(this.dataSourceForEdit.id) &&
      isNotNullOrUndefined(this.dataSourceForEdit.price) &&
      isNotNullOrUndefined(this.dataSourceForEdit.description) &&
      !isNotNullOrUndefined(this.activeIndex)
    ) {
      this.root.items.push(this.dataSourceForEdit);
      this.dataSourceForEdit = null;
      return;
    }
    this.root.items[this.activeIndex] = this.dataSourceForEdit;
    this.activeIndex = null;
    this.dataSourceForEdit = null;
  }

  editRow(i: number, row: DynamicInvoiceItemDto) {
    this.dataSourceForEdit = DynamicInvoiceItemDto.fromJS(row.toJSON());
    this.activeIndex = i;
    (this.dataSourceForEdit.workDate as any) = !!row.workDate ? moment(moment(row.workDate).format('yyyy-MM-DD')).toDate() : null;
    (this.dataSourceForEdit.truckId as any) = !((row.truckId as any) instanceof Object)
      ? this.trucks.find((truck) => Number(truck.id) === Number(row.truckId))
      : row.truckId;
    (this.dataSourceForEdit.originCityId as any) = !((row.originCityId as any) instanceof Object)
      ? this.cities.find((city) => Number(city.id) === Number(row.originCityId))
      : row.originCityId;
    (this.dataSourceForEdit.destinationCityId as any) = !((row.destinationCityId as any) instanceof Object)
      ? this.cities.find((city) => Number(city.id) === Number(row.destinationCityId))
      : row.destinationCityId;
  }

  deleteRow(i: number) {
    this.root.items.splice(i, 1);
  }

  cancelAddToArray() {
    this.dataSourceForEdit = null;
    this.activeIndex = null;
  }

  // Custom Items Logic
  addNewCustomItem() {
    this.customItemForEdit = new CreateOrEditDynamicInvoiceCustomItemDto();
  }

  saveToCustomItemsArray() {
    if (
      !isNotNullOrUndefined(this.customItemForEdit) ||
      !isNotNullOrUndefined(this.customItemForEdit.price) ||
      !isNotNullOrUndefined(this.customItemForEdit.description) ||
      ('' + this.customItemForEdit.description).length === 0
    ) {
      Swal.fire({
        title: this.l('FormIsNotValidMessage'),
        icon: 'warning',
        showCloseButton: true,
      });
      return;
    }

    // Calculate Vat Amount and Total Amount
    this.customItemForEdit.vatAmount = (this.customItemForEdit.price * this.customItemForEdit.vatTax) / 100;
    this.customItemForEdit.totalAmount = this.customItemForEdit.price + this.customItemForEdit.vatAmount;

    if (!isNotNullOrUndefined(this.root.customItems)) {
      this.root.customItems = [];
    }

    if (
      !isNotNullOrUndefined(this.customItemForEdit.id) &&
      isNotNullOrUndefined(this.customItemForEdit.price) &&
      isNotNullOrUndefined(this.customItemForEdit.description) &&
      !isNotNullOrUndefined(this.activeCustomItemIndex)
    ) {
      this.root.customItems.push(this.customItemForEdit);
      this.customItemForEdit = null;
      return;
    }
    this.root.customItems[this.activeCustomItemIndex] = this.customItemForEdit;
    this.activeCustomItemIndex = null;
    this.customItemForEdit = null;
  }

  editCustomItem(i: number, item: CreateOrEditDynamicInvoiceCustomItemDto) {
    this.customItemForEdit = CreateOrEditDynamicInvoiceCustomItemDto.fromJS(item.toJSON());
    this.activeCustomItemIndex = i;
  }

  deleteCustomItem(i: number) {
    this.root.customItems.splice(i, 1);
  }

  cancelAddToCustomItemsArray() {
    this.customItemForEdit = null;
    this.activeCustomItemIndex = null;
  }

  filterCity(event: any, src: number) {
    const filteredCities = this.cities.filter((city) => city.displayName.toLowerCase().search(event.query.toLowerCase()) > -1);
    src === 1 ? (this.originCitiesFiltered = [...filteredCities]) : (this.destCitiesFiltered = [...filteredCities]);
  }

  filterTrucks(event: any) {
    const filteredTrucks = this.trucks.filter((truck) => truck.displayName.toLowerCase().search(event.query.toLowerCase()) > -1);
    this.trucksFiltered = [...filteredTrucks];
  }

  getCityToDisplay(cityId) {
    const id = cityId instanceof Object ? cityId.id : cityId;
    const foundCity = !!cityId && this.cities.length > 0 ? this.cities.find((city) => Number(city.id) === Number('' + id)).displayName : '';
    return foundCity;
  }

  getTruckToDisplay(truckId) {
    const id = truckId instanceof Object ? truckId.id : truckId;
    return !!truckId && this.trucks.length > 0 ? this.trucks.find((truck) => Number(truck.id) === Number('' + id)).displayName : '';
  }

  getWorkDate(workDate): string {
    return this._DateFormatterService.ToString(this._DateFormatterService.MomentToNgbDateStruct(workDate));
  }

  clearOnSelect() {
    this.clearWaybillRelatedFields();
    this.fillDynamicInvoicItem(Number(this.dataSourceForEdit.waybillNumber), this.dataSourceForEdit);
  }

  private fillDynamicInvoicItem(waybillNumber: number, destItem: DynamicInvoiceItemDto) {
    this._DynamicInvoiceServiceProxy.getDynamicInvoiceItemInfo(waybillNumber).subscribe((res) => {
      (destItem.destinationCityId as any) = this.cities.find((city) => city.displayName.toLowerCase() === res.destinationCityName.toLowerCase());
      (destItem.originCityId as any) = this.cities.find((city) => city.displayName.toLowerCase() === res.originCityName.toLowerCase());
      (destItem.truckId as any) = this.trucks.find((truck) => truck.displayName === res.plateNumber);
      (destItem.workDate as any) = !!res.workDate ? moment(moment(res.workDate).format('yyyy-MM-DD')).toDate() : null;
      destItem.containerNumber = res.containerNumber;
      destItem.quantity = res.quantity;
    });
  }

  isFormInvalid(Form: NgForm): boolean {
    return Form.invalid;
  }

  clearWaybillRelatedFields() {
    this.dataSourceForEdit.destinationCityId = null;
    this.dataSourceForEdit.workDate = null;
    this.dataSourceForEdit.containerNumber = null;
    this.dataSourceForEdit.quantity = null;
    this.dataSourceForEdit.originCityId = null;
    this.dataSourceForEdit.truckId = null;
  }
  calculateCustomItemTotals() {
    if (this.customItemForEdit && this.customItemForEdit.price && this.customItemForEdit.vatTax !== null) {
      // Calculate Vat Amount and Total Amount
      this.customItemForEdit.vatAmount = (this.customItemForEdit.price * this.customItemForEdit.vatTax) / 100;
      this.customItemForEdit.totalAmount = this.customItemForEdit.price + this.customItemForEdit.vatAmount;
    } else {
      this.customItemForEdit.vatAmount = 0;
      this.customItemForEdit.totalAmount = this.customItemForEdit.price || 0;
    }
  }
}
