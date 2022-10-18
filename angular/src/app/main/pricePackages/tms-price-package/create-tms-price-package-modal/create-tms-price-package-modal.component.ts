import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditTmsPricePackageDto,
  NormalPricePackagesServiceProxy,
  PricePackagesPricingInfoDto,
  PricePackageType,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  TmsPricePackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';

@Component({
  selector: 'app-create-tms-price-package-modal',
  templateUrl: './create-tms-price-package-modal.component.html',
  styleUrls: ['./create-tms-price-package-modal.component.css'],
})
export class CreateTmsPricePackageModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('Modal') modal: ModalDirective;
  @ViewChild('dataGrid', { static: true }) dataGrid: DxDataGridComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  tmsPricePackage: CreateOrEditTmsPricePackageDto;
  isFormActive: boolean;
  isFormSaving: boolean;
  isFormLoading: boolean;
  transportTypes: SelectItemDto[];
  truckTypes: SelectItemDto[];
  cities: SelectItemDto[];
  shippers: ShippersForDropDownDto[];
  pricePackageType = PricePackageType;
  pricePackageTypes = this._enumToArrayPipe.transform(this.pricePackageType);
  routeTypes = this._enumToArrayPipe.transform(ShippingRequestRouteType);
  dataSource: any = {};
  pricePackagesInfo: PricePackagesPricingInfoDto[] = [];

  constructor(
    private injector: Injector,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _normalPricePackagesServiceProxy: NormalPricePackagesServiceProxy,
    private _tmsPricePackagesServiceProxy: TmsPricePackageServiceProxy,
    private _enumToArrayPipe: EnumToArrayPipe
  ) {
    super(injector);
    this.getPricePackagesPricingInfo();
  }

  ngOnInit(): void {
    this.tmsPricePackage = new CreateOrEditTmsPricePackageDto();
    this.isFormActive = false;
    this.isFormSaving = false;
    this.loadAllDropDowns();
    this.getAllNormalPricePackages();
  }

  show(id?: number): void {
    //this is edit
    if (id) {
      this.isFormLoading = true;
      this._tmsPricePackagesServiceProxy.getForEdit(id).subscribe((res) => {
        this.tmsPricePackage = res;
        this.loadAllTruckTypeByTransportType(this.tmsPricePackage.transportTypeId);
        this.isFormLoading = false;
      });
    }
    this.modal.show();
    this.isFormActive = true;
  }

  /**
   * laod All Needed Drop Down
   * @private
   */
  private loadAllDropDowns(): void {
    this.loadAllShippers();
    this.loadAllTransportTypes();
    this.loadAllCities();
  }

  /**
   * loads All Shippers
   * @private
   */
  private loadAllShippers(): void {
    console.log('Shippers Are loading');
    this._shippingRequestServiceProxy.getAllShippersForDropDown().subscribe((res) => {
      this.shippers = res;
    });
  }

  /**
   * loads All TransportTypes
   * @private
   */
  private loadAllTransportTypes(): void {
    this._normalPricePackagesServiceProxy.getAllTranspotTypesForTableDropdown().subscribe((res) => {
      this.transportTypes = res;
    });
  }

  /**
   * load all truck type by transport type
   * @param transportTypeId
   */
  public loadAllTruckTypeByTransportType(transportTypeId: number): void {
    this._normalPricePackagesServiceProxy.getAllTruckTypesForTableDropdown(transportTypeId).subscribe((res) => {
      this.truckTypes = res;
    });
  }

  /**
   * load all cities for DropDown
   * @private
   */
  private loadAllCities(): void {
    this._normalPricePackagesServiceProxy.getAllCitiesForTableDropdown().subscribe((res) => {
      this.cities = res;
    });
  }

  close() {
    this.tmsPricePackage = new CreateOrEditTmsPricePackageDto();
    this.isFormActive = false;
    this.isFormSaving = false;
    this.modal.hide();
  }

  calculateDrFinalPrice() {
    if (isNotNullOrUndefined(this.tmsPricePackage.directRequestPrice) && isNotNullOrUndefined(this.tmsPricePackage.directRequestCommission)) {
      this.tmsPricePackage.directRequestTotalPrice = this.tmsPricePackage.directRequestPrice + this.tmsPricePackage.directRequestCommission;
    }
  }
  calculateTmsFinalPrice() {
    if (isNotNullOrUndefined(this.tmsPricePackage.tachyonManagePrice) && isNotNullOrUndefined(this.tmsPricePackage.tachyonManageCommission)) {
      this.tmsPricePackage.tachyonManageTotalPrice = this.tmsPricePackage.tachyonManagePrice + this.tmsPricePackage.tachyonManageCommission;
    }
  }

  /**
   * loads All Normal Price Packeges For View Table
   * @private
   */
  private getAllNormalPricePackages(): void {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        return self._normalPricePackagesServiceProxy
          .getAll(JSON.stringify(loadOptions))
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
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  /**
   * gets Pricing info for the price Packeges
   * min / max / avg ... etc
   * @private
   */
  private getPricePackagesPricingInfo(): void {
    this._normalPricePackagesServiceProxy.getPricePackagesPricingInfo().subscribe((res) => {
      this.pricePackagesInfo = res;
    });
  }

  public createOrEdit(): void {
    this.isFormSaving = true;
    this._tmsPricePackagesServiceProxy.createOrEdit(this.tmsPricePackage).subscribe(() => {
      this.notify.success(this.l('Success'));
      this.isFormSaving = false;
      this.modalSave.emit('');
      this.close();
    });
  }
}
