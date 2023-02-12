import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CarriersForDropDownDto,
  CreateOrEditTmsPricePackageDto,
  NormalPricePackageDto,
  NormalPricePackagesServiceProxy,
  PriceOfferCommissionType,
  PricePackageType,
  SelectItemDto,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  TmsPricePackageServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { TmsPricePackagePricingMethod } from '@app/main/pricePackages/tms-price-package/create-tms-price-package-modal/tms-price-package-pricing-method';
import { TmsPricePackageCommissionType } from '@app/main/pricePackages/tms-price-package/create-tms-price-package-modal/tms-price-package-commission-type';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'app-create-tms-price-package-modal',
  templateUrl: './create-tms-price-package-modal.component.html',
  styleUrls: ['./create-tms-price-package-modal.component.css'],
})
export class CreateTmsPricePackageModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('Modal') modal: ModalDirective;
  @ViewChild('grid', { static: true }) dataGrid: DxDataGridComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  tmsPricePackage: CreateOrEditTmsPricePackageDto;
  isFormActive: boolean;
  isFormSaving: boolean;
  isFormLoading: boolean;
  transportTypes: SelectItemDto[];
  truckTypes: SelectItemDto[];
  cities: SelectItemDto[];
  shippers: SelectItemDto[];
  pricePackageType = PricePackageType;
  pricePackageTypes = this._enumToArrayPipe.transform(this.pricePackageType);
  routeTypes = this._enumToArrayPipe.transform(ShippingRequestRouteType);
  dataSource: any = {};
  shippingTypes: SelectItemDto[];
  pricingMethod: TmsPricePackagePricingMethod;
  pricingMethodEnum = TmsPricePackagePricingMethod;
  pricingMethods;
  calculatedPriceTitle: string;
  commissionType: TmsPricePackageCommissionType;
  commissionTypeEnum = TmsPricePackageCommissionType;
  commissionAmount: number;
  commissionTypes;
  calculatedPrice: number;
  totalCommission: number;
  carriersLoading: boolean;
  carriers: CarriersForDropDownDto[];
  selectedCarriers: CarriersForDropDownDto[];
  selectedRows: NormalPricePackageDto[] = [];
  totalPrice: number;

  constructor(
    private injector: Injector,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _normalPricePackagesServiceProxy: NormalPricePackagesServiceProxy,
    private _tmsPricePackagesServiceProxy: TmsPricePackageServiceProxy,
    private _enumToArrayPipe: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.tmsPricePackage = new CreateOrEditTmsPricePackageDto();
    this.isFormActive = false;
    this.isFormSaving = false;
    this.loadAllDropDowns();
    this.getAllShippingType();
    this.loadAllCarriers();
    this.pricingMethods = this._enumToArrayPipe.transform(this.pricingMethodEnum);
    // The Pricing Method default value is `Average`
    this.pricingMethod = TmsPricePackagePricingMethod.Average;
    this.commissionTypes = this._enumToArrayPipe.transform(TmsPricePackageCommissionType);
    this.calculatedPriceTitle = '';
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
    this.pricingMethod = TmsPricePackagePricingMethod.Average;
    this.calculatedPriceTitle = this.l('AveragePrice');
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
    this._tmsPricePackagesServiceProxy.getCompanies().subscribe((res) => {
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
    this.truckTypes = undefined;
    this.pricingMethod = undefined;
    this.calculatedPriceTitle = undefined;
    this.commissionType = undefined;
    this.commissionAmount = undefined;
    this.calculatedPrice = undefined;
    this.totalPrice = undefined;
    this.totalCommission = undefined;
    this.selectedCarriers = undefined;
    this.selectedRows = undefined;
    this.dataSource = {};
    this.modal.hide();
  }

  /**
   * loads All Normal Price Packeges For View Table
   * @private
   */
  private getAllNormalPricePackages(filter: any[] | undefined): void {
    let self = this;
    this.dataSource = {};
    this.dataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        if (isNotNullOrUndefined(filter)) {
          if (isNotNullOrUndefined(loadOptions.filter)) {
            loadOptions.filter.push(filter);
          } else {
            loadOptions.filter = filter;
          }
        }
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

  public createOrEdit(): void {
    this.isFormSaving = true;
    if (this.pricingMethod == this.pricingMethodEnum.FinalPrice) {
      this.tmsPricePackage.totalPrice = this.calculatedPrice;
    } else {
      this.tmsPricePackage.totalPrice = this.totalPrice;
    }
    this._tmsPricePackagesServiceProxy.createOrEdit(this.tmsPricePackage).subscribe(() => {
      this.notify.success(this.l('Success'));
      this.isFormSaving = false;
      this.modalSave.emit('');
      this.close();
    });
  }

  getAllShippingType() {
    this._shippingRequestServiceProxy.getAllShippingTypesForDropdown().subscribe((result) => {
      this.shippingTypes = result;
    });
  }

  calculatePriceForSelection() {
    let prices = this.selectedRows.map((dto, index, array) => {
      return dto.tachyonMSRequestPrice;
    });

    if (this.pricingMethod == this.pricingMethodEnum.Average) {
      this.calculateAveragePrice(prices);
    }

    if (this.pricingMethod == this.pricingMethodEnum.Maximum) {
      this.calculateMaximumPrice(prices);
    }

    if (this.pricingMethod == this.pricingMethodEnum.Minimum) {
      this.calculateMinimumPrice(prices);
    }

    if (this.pricingMethod != this.pricingMethodEnum.FinalPrice) {
      this.calculateTotalCommissionAndTotalPrice();
    }
    // case TmsPricePackagePricingMethod.FinalPrice:
    // don't do anything ( in final price method ) we add the price manually so no need to calculation
  }

  updatePricingMethod() {
    if (this.pricingMethod == this.pricingMethodEnum.Average) {
      this.calculatedPriceTitle = this.l('AveragePrice');
    }

    if (this.pricingMethod == this.pricingMethodEnum.Maximum) {
      this.calculatedPriceTitle = this.l('MaximumPrice');
    }

    if (this.pricingMethod == this.pricingMethodEnum.Minimum) {
      this.calculatedPriceTitle = this.l('MinimumPrice');
    }

    if (this.pricingMethod == this.pricingMethodEnum.FinalPrice) {
      this.calculatedPriceTitle = this.l('FinalPrice');
      this.calculatedPrice = undefined;
    }

    this.calculatePriceForSelection();
  }

  calculateTotalCommissionAndTotalPrice() {
    if (!isNotNullOrUndefined(this.commissionType)) {
      return;
    }

    if (this.commissionType == this.commissionTypeEnum.Percentage) {
      this.totalCommission = (this.calculatedPrice * this.commissionAmount) / 100;
    }

    if (this.commissionType == this.commissionTypeEnum.AddValue) {
      this.totalCommission = this.commissionAmount;
    }

    this.totalPrice = this.totalCommission + this.calculatedPrice;
  }

  applyFilters() {
    let carriersFilter = this.buildCarriersFilter();
    let pricePackageFilter = this.buildPricePackageFilter();

    let filter = [];
    if (isNotNullOrUndefined(carriersFilter) && carriersFilter.length > 0) {
      filter.push(carriersFilter);
    }

    if (isNotNullOrUndefined(pricePackageFilter) && pricePackageFilter.length > 0) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(pricePackageFilter);
    }

    this.getAllNormalPricePackages(filter);
  }

  private loadAllCarriers(): void {
    this.carriersLoading = true;
    this._shippingRequestServiceProxy
      .getAllCarriersForDropDown()
      .pipe(finalize(() => (this.carriersLoading = false)))
      .subscribe((res) => {
        this.carriers = res;
      });
  }

  private calculateAveragePrice(prices: number[]) {
    if (prices.length === 0) {
      this.calculatedPrice = 0;
      return;
    }

    let total = prices.reduce((last, next) => last + next, 0);

    this.calculatedPrice = total / prices.length;
  }

  private calculateMaximumPrice(prices: number[]) {
    this.calculatedPrice = Math.max(...prices);
  }

  private calculateMinimumPrice(prices: number[]) {
    this.calculatedPrice = Math.min(...prices);
  }

  private buildCarriersFilter(): any[] {
    let filter = [];

    if (!isNotNullOrUndefined(this.selectedCarriers)) {
      return filter;
    }

    for (let i = 0; i < this.selectedCarriers.length; i++) {
      if (i > 0) {
        filter.push('or');
      }
      filter.push(['TenantId', '=', this.selectedCarriers[i].id]);
    }

    return filter;
  }

  private buildPricePackageFilter(): any[] {
    let filter = [];

    if (isNotNullOrUndefined(this.tmsPricePackage.transportTypeId)) {
      filter.push(['transportTypeId', '=', this.tmsPricePackage.transportTypeId]);
    }

    if (isNotNullOrUndefined(this.tmsPricePackage.trucksTypeId)) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(['trucksTypeId', '=', this.tmsPricePackage.trucksTypeId]);
    }

    if (isNotNullOrUndefined(this.tmsPricePackage.originCityId)) {
      if (filter.length > 0) {
        filter.push('and');
      }

      filter.push(['originCityId', '=', this.tmsPricePackage.originCityId]);
    }

    if (isNotNullOrUndefined(this.tmsPricePackage.destinationCityId)) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(['destinationCityId', '=', this.tmsPricePackage.destinationCityId]);
    }

    return filter;
  }
}
