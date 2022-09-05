import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
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
  TrucksTypeSelectItemDto,
  TenantCityLookupTableDto,
  NormalPricePackagesServiceProxy,
  CreateOrEditDynamicInvoiceDto,
  CreateOrEditDynamicInvoiceItemDto,
  ListResultDtoOfSelectItemDto,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import * as moment from 'moment';
import { DateTimeService } from '@app/shared/common/timing/date-time.service';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import Swal from 'sweetalert2';

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
  private isForShipper: boolean;
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

  constructor(
    injector: Injector,
    private _currentSrv: InvoiceServiceProxy,
    private _TenantServiceproxy: TenantServiceProxy,
    private _NormalPricePackagesServiceProxy: NormalPricePackagesServiceProxy,
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
    this.getAllTrucksTypeForTableDropdown();
    this.GetAllCitiesForTableDropdown();
  }

  show(forWho: number, id?: number): void {
    this.getAllTrucksTypeForTableDropdown();
    this.GetAllCitiesForTableDropdown();
    if (isNotNullOrUndefined(id)) {
      this.getForView(id);
    }
    this.isForShipper = forWho === 1;
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
    this._NormalPricePackagesServiceProxy.getAllCitiesForTableDropdown().subscribe(
      (cities) => {
        this.cities = cities;
      },
      (error) => {}
    );
  }

  save(): void {
    this.saving = true;

    if (this.Tenant?.id) {
    } else {
      this.Tenant = undefined;
    }
    if (!this.Tenant?.id) {
      return;
    }
    const body = new CreateOrEditDynamicInvoiceDto({
      id: isNotNullOrUndefined(this.root.id) ? this.root.id : null,
      creditTenantId: this.isForShipper ? Number(this.Tenant.id) : null,
      debitTenantId: !this.isForShipper ? Number(this.Tenant.id) : null,
      notes: this.notes,
      items: this.root.items,
    });
    this._DynamicInvoiceServiceProxy.createOrEdit(body).subscribe(
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
    // this._currentSrv
    //     .onDemand(parseInt(this.Tenant.id), this.SelectedWaybills)
    //     .pipe(
    //         finalize(() => {
    //             this.saving = false;
    //         })
    //     )
    //     .subscribe(() => {
    //     });
  }

  close(): void {
    this.root = new DynamicInvoiceForViewDto();
    this.isForShipper = false;
    this.active = false;
    this.modal.hide();
    this.Tenants = [];
    this.Waybills = [];
    this.SelectedWaybills = [];
    this.dataSource = [];
    this.dataSourceForEdit = null;
    this.notes = null;
    this.trucks = [];
    this.cities = [];
    this.activeIndex = null;
  }

  search(event, initValue = false) {
    this._CommonServ.getAutoCompleteTenants(event.query, this.isForShipper ? 'shipper' : 'carrier').subscribe((result) => {
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
    // this._currentSrv.getUnInvoicedWaybillsByTenant(parseInt(this.Tenant.id)).subscribe((res) => {
    //     // this.Waybills = res;
    // });
  }

  searchForWaybills(event): void {
    this._DynamicInvoiceServiceProxy.searchByWaybillNumber(event.query).subscribe((res) => {
      this.Waybills = res;
    });
  }

  logEvent(eventName) {
    // this.events.unshift(eventName);
  }

  private getForView(id: number) {
    this._DynamicInvoiceServiceProxy.getForView(id).subscribe(
      (data) => {
        this.root = data;
        this.notes = this.root.notes;
        this.search({ query: this.isForShipper ? data.creditCompany : data.debitCompany }, true);
        this.dataSource = this.root.items;
        if (this.root.items.length > 0) {
          this.root.items.map((item) => {
            item.workDate = !!item.waybillNumber ? null : moment(item.workDate);
          });
        }
      },
      (error) => {}
    );
  }

  onRowEditInit(item: DynamicInvoiceItemDto) {
    // this.root.items[item.id] = {...item};
  }

  onRowEditSave(item: DynamicInvoiceItemDto) {
    // if (item.price > 0) {
    //     delete this.clonedProducts[item.id];
    // }
    // else {
    //     this.messageService.add({severity:'error', summary: 'Error', detail:'Invalid Price'});
    // }
  }

  onRowEditCancel(item: DynamicInvoiceItemDto, index: number) {
    // this.products2[index] = this.clonedProducts[product.id];
    // delete this.clonedProducts[product.id];
  }

  addNew() {
    this.dataSourceForEdit = new DynamicInvoiceItemDto();
    // this.dataSource.push(new DynamicInvoiceItemDto());
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
    // if (isNotNullOrUndefined(this.dataSourceForEdit.truckTypeId) && (this.dataSourceForEdit.truckTypeId as any) instanceof Object) {
    //     this.dataSourceForEdit.truckTypeId = (this.dataSourceForEdit.truckTypeId as any).id;
    // }
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
    // const index = this.root.items.findIndex((item) => item.id === this.dataSourceForEdit.id);
    // if (index > -1 && isNotNullOrUndefined(this.dataSourceForEdit.price) && isNotNullOrUndefined(this.dataSourceForEdit.description)) {
    // }
    this.activeIndex = null;
    this.dataSourceForEdit = null;
  }

  deleteRow(i: number) {
    this.root.items.splice(i, 1);
  }

  filterCity(event: any, src: number) {
    const filteredCities = this.cities.filter((city) => city.displayName.toLowerCase().search(event.query.toLowerCase()) > -1);
    src === 1 ? (this.originCitiesFiltered = [...filteredCities]) : (this.destCitiesFiltered = [...filteredCities]);
  }

  filterTrucks(event: any) {
    const filteredTrucks = this.trucks.filter((truck) => truck.displayName.toLowerCase().search(event.query.toLowerCase()) > -1);
    this.trucksFiltered = [...filteredTrucks];
  }

  getCityToDisplay(originCityId) {
    if (isNaN(originCityId)) {
      return;
    }
    return !!originCityId && this.cities.length > 0 ? this.cities.find((city) => Number(city.id) === Number('' + originCityId)).displayName : '';
  }

  // getTruckToDisplay(truckTypeId) {
  //   return !!truckTypeId ? this.cities.find((city) => Number(city.id) === Number('' + truckTypeId)).displayName : '';
  // }

  getWorkDate(workDate): string {
    return this._DateFormatterService.ToString(this._DateFormatterService.MomentToNgbDateStruct(workDate));
  }

  clearOnSelect() {
    this.dataSourceForEdit.destinationCityId = null;
    this.dataSourceForEdit.workDate = null;
    this.dataSourceForEdit.containerNumber = null;
    this.dataSourceForEdit.quantity = null;
    this.dataSourceForEdit.originCityId = null;
    this.dataSourceForEdit.destinationCityId = null;
    this.dataSourceForEdit.truckId = null;
  }

  editRow(i: number, row: DynamicInvoiceItemDto) {
    this.dataSourceForEdit = DynamicInvoiceItemDto.fromJS(row.toJSON());
    this.activeIndex = i;
    (this.dataSourceForEdit.workDate as any) = !!row.workDate ? moment(moment(row.workDate).format('yyyy-MM-DD')).toDate() : null;
    (this.dataSourceForEdit.truckId as any) = this.trucks.find((truck) => Number(truck.id) === this.dataSourceForEdit.truckId);
    (this.dataSourceForEdit.originCityId as any) = this.cities.find((city) => Number(city.id) === this.dataSourceForEdit.originCityId);
    (this.dataSourceForEdit.destinationCityId as any) = this.cities.find((city) => Number(city.id) === this.dataSourceForEdit.destinationCityId);
  }

  cancelAddToArray() {
    this.dataSourceForEdit = null;
    this.activeIndex = null;
  }

  getTruckToDisplay(truckId: number) {
    return !!truckId && this.trucks.length > 0 ? this.trucks.find((truck) => Number(truck.id) === Number('' + truckId)).displayName : '';
  }
}
