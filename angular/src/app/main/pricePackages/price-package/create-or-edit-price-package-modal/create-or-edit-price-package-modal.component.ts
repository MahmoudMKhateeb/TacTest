/* tslint:disable:triple-equals */
import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CarriersForDropDownDto,
  CompanyType,
  CreateOrEditPricePackageDto,
  CreateOrEditServiceAreaDto,
  PricePackageServiceProxy,
  PricePackageType,
  PricePackageUsageType,
  SelectItemDto,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  TenantRegistrationServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { TmsPricePackagePricingMethod } from '@app/main/pricePackages/price-package/create-or-edit-price-package-modal/tms-price-package-pricing-method';
import { TmsPricePackageCommissionType } from '@app/main/pricePackages/price-package/create-or-edit-price-package-modal/tms-price-package-commission-type';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { finalize } from '@node_modules/rxjs/operators';
import { DestinationCompanyType } from '@app/main/pricePackages/price-package-appendix/create-or-edit-price-package-appendix/destination-company-type';

@Component({
  selector: 'app-create-or-edit-price-package-modal',
  templateUrl: './create-or-edit-price-package-modal.component.html',
  styleUrls: ['./create-or-edit-price-package-modal.component.css'],
})
export class CreateOrEditPricePackageModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('Modal') modal: ModalDirective;
  @ViewChild('grid', { static: true }) dataGrid: DxDataGridComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  pricePackageDto: CreateOrEditPricePackageDto;
  isFormActive: boolean;
  isFormSaving: boolean;
  isFormLoading: boolean;
  transportTypes: SelectItemDto[];
  truckTypes: SelectItemDto[];
  cities: SelectItemDto[];
  countries: SelectItemDto[];
  companies: SelectItemDto[];
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
  selectedRows: any[] = [];
  totalPrice: number;
  companyType: DestinationCompanyType;
  companyTypeEnum = DestinationCompanyType;
  companyTypes = this._enumToArrayPipe.transform(DestinationCompanyType);
  routeTypeEnum = ShippingRequestRouteType;
  truckTypeLoading: boolean;
  companiesLoading: boolean;
  countriesLoading: boolean;
  serviceAreasSelectionLimit: number;

  constructor(
    private injector: Injector,
    private _shippingRequestServiceProxy: ShippingRequestsServiceProxy,
    private _pricePackagesServiceProxy: PricePackageServiceProxy,
    private _enumToArrayPipe: EnumToArrayPipe,
    private _tenantRegistrationServiceProxy: TenantRegistrationServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.pricePackageDto = new CreateOrEditPricePackageDto();
    this.isFormActive = false;
    this.isFormSaving = false;
    this.truckTypeLoading = false;
    this.companiesLoading = false;
    this.countriesLoading = false;
    this.loadAllDropDowns();
    this.getAllShippingType();
    this.loadAllCarriers();
    this.pricingMethods = this._enumToArrayPipe.transform(this.pricingMethodEnum);
    // The Pricing Method default value is `Average`
    this.pricingMethod = TmsPricePackagePricingMethod.Average;
    this.commissionTypes = this._enumToArrayPipe.transform(TmsPricePackageCommissionType);
    this.calculatedPriceTitle = '';
    this.serviceAreasSelectionLimit = 0;
  }

  show(id?: number): void {
    if (id) {
      this.isFormLoading = true;
      this._pricePackagesServiceProxy.getForEdit(id).subscribe((res) => {
        this.pricePackageDto = res;
        this.companyType =
          this.pricePackageDto.usageType == PricePackageUsageType.AsCarrier ? this.companyTypeEnum.Carrier : this.companyTypeEnum.Shipper;
        this.loadAllTruckTypeByTransportType(this.pricePackageDto.transportTypeId);
        if (this.companyType == this.companyTypeEnum.Shipper) {
          this.pricingMethod = this.pricingMethodEnum.FinalPrice;
          this.calculatedPriceTitle = this.l('FinalPrice');
          this.calculatedPrice = Number(this.pricePackageDto.totalPrice.toFixed(2));
        }
        if (this.pricePackageDto.type == this.pricePackageType.Dedicated) {
          this.loadAllCities(this.pricePackageDto.originCountryId);
          this.loadCountries();
        }
        this.isFormLoading = false;
        this.modal.show();
        this.isFormActive = true;
      });
    } else {
      if (this.isTachyonDealerOrHost) {
        this.companyType = this.companyTypeEnum.Shipper;
        this._pricePackagesServiceProxy.getCompanies(CompanyType.Shipper).subscribe((res) => {
          this.companies = res;
          this.isFormActive = true;
          this.modal.show();
        });
      } else {
        this.companyType = this.companyTypeEnum.Carrier;
        this.isFormActive = true;
        this.modal.show();
      }

      this.pricingMethod = TmsPricePackagePricingMethod.Average;
      this.calculatedPriceTitle = this.l('AveragePrice');
    }
  }

  /**
   * load All Needed Drop Down
   * @private
   */
  private loadAllDropDowns(): void {
    if (this.isTachyonDealerOrHost && !this.pricePackageDto.id) {
      this.loadAllCompanies();
    }
    this.loadAllTransportTypes();
    if (this.pricePackageDto?.type == this.pricePackageType.Dedicated) {
      this.loadCountries();
    } else {
      this.loadAllCities();
    }
  }

  /**
   * loads All Shippers
   * @private
   */
  private loadAllCompanies(): void {
    // there is a difference between company type in front end  & backend
    let companyType = this.companyType == this.companyTypeEnum.Carrier ? CompanyType.Carrier : CompanyType.Shipper;
    this.companiesLoading = true;
    this._pricePackagesServiceProxy
      .getCompanies(companyType)
      .pipe(finalize(() => (this.companiesLoading = false)))
      .subscribe((res) => {
        this.companies = res;
      });
  }

  /**
   * loads All TransportTypes
   * @private
   */
  private loadAllTransportTypes(): void {
    this._pricePackagesServiceProxy.getAllTransportTypeForDropdown().subscribe((res) => {
      this.transportTypes = res;
    });
  }

  /**
   * load all truck type by transport type
   * @param transportTypeId
   */
  public loadAllTruckTypeByTransportType(transportTypeId: number): void {
    this.truckTypeLoading = true;
    this._pricePackagesServiceProxy
      .getAllTruckTypeForDropdown(transportTypeId)
      .pipe(finalize(() => (this.truckTypeLoading = false)))
      .subscribe((res) => {
        this.truckTypes = res;
      });
  }

  /**
   * load all cities for DropDown
   * @private
   */
  private loadAllCities(originCountry?: number | undefined): void {
    this._tenantRegistrationServiceProxy.getAllCitiesForTableDropdown(originCountry).subscribe((res) => {
      this.cities = res;
      if (isNotNullOrUndefined(this.pricePackageDto?.id) && isNotNullOrUndefined(this.pricePackageDto?.serviceAreas)) {
        (this.pricePackageDto.serviceAreas as any[]) = this.cities.filter((x) => this.pricePackageDto?.serviceAreas?.some((s) => s.cityId == +x.id));
      }
    });
  }

  close() {
    this.pricePackageDto = new CreateOrEditPricePackageDto();
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
    this.companyType = undefined;
    this.companiesLoading = false;
    this.truckTypeLoading = false;
    this.dataSource = {};
    this.modal.hide();
  }

  /**
   * loads All Normal Price Packages For View Table
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
        return self._pricePackagesServiceProxy
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
          .catch(() => {
            throw new Error('Data Loading Error');
          });
      },
    });
  }

  public createOrEdit(): void {
    this.isFormSaving = true;

    if (this.companyType == this.companyTypeEnum.Shipper) {
      if (this.pricingMethod == this.pricingMethodEnum.FinalPrice) {
        this.pricePackageDto.totalPrice = this.calculatedPrice;
      } else {
        this.pricePackageDto.totalPrice = this.totalPrice;
      }
    }

    if (this.isTachyonDealerOrHost) {
      this.pricePackageDto.usageType =
        this.companyType == this.companyTypeEnum.Carrier ? PricePackageUsageType.AsCarrier : PricePackageUsageType.AsTachyonManageService;
    } else {
      this.pricePackageDto.usageType = undefined;
    }
    if (isNotNullOrUndefined(this.pricePackageDto?.serviceAreas)) {
      this.pricePackageDto.serviceAreas = this.pricePackageDto.serviceAreas.map((item) => {
        let dto = new CreateOrEditServiceAreaDto();
        dto.cityId = +(item as any).id;
        return dto;
      });
    }

    this._pricePackagesServiceProxy.createOrEdit(this.pricePackageDto).subscribe(() => {
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
    let prices = this.selectedRows.map((dto) => {
      return dto.totalPrice;
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
    this.updateCalculatedPriceInputTitle();

    this.calculatePriceForSelection();
  }

  updateCalculatedPriceInputTitle() {
    if (this.pricingMethod == this.pricingMethodEnum.Average) {
      this.calculatedPriceTitle =
        this.pricePackageDto.type == this.pricePackageType.Dedicated ? this.l('AveragePricePerTruck') : this.l('AveragePrice');
    }

    if (this.pricingMethod == this.pricingMethodEnum.Maximum) {
      this.calculatedPriceTitle =
        this.pricePackageDto.type == this.pricePackageType.Dedicated ? this.l('MaximumPricePerTruck') : this.l('MaximumPrice');
    }

    if (this.pricingMethod == this.pricingMethodEnum.Minimum) {
      this.calculatedPriceTitle =
        this.pricePackageDto.type == this.pricePackageType.Dedicated ? this.l('MinimumPricePerTruck') : this.l('MinimumPrice');
    }

    if (this.pricingMethod == this.pricingMethodEnum.FinalPrice) {
      this.calculatedPriceTitle = this.pricePackageDto.type == this.pricePackageType.Dedicated ? this.l('FinalPricePerTruck') : this.l('FinalPrice');
      this.calculatedPrice = undefined;
    }
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
    // let serviceAreasFilter = this.buildServiceAreasFilter();

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
    /*if (isNotNullOrUndefined(serviceAreasFilter) && serviceAreasFilter.length > 0) {
          if (filter.length > 0) {
            filter.push('and');
          }
          filter.push(serviceAreasFilter);
        }*/

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
    if (prices.length == 0) {
      this.calculatedPrice = 0;
      return;
    }

    let total = prices.reduce((last, next) => last + next, 0);

    this.calculatedPrice = Number((total / prices.length).toFixed(2));
  }

  private calculateMaximumPrice(prices: number[]) {
    this.calculatedPrice = Number(Math.max(...prices).toFixed(2));
  }

  private calculateMinimumPrice(prices: number[]) {
    this.calculatedPrice = Number(Math.min(...prices).toFixed(2));
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

  private buildServiceAreasFilter(): any[] {
    let filter = [];

    if (!isNotNullOrUndefined(this.pricePackageDto?.serviceAreas)) {
      return filter;
    }

    for (let i = 0; i < this.pricePackageDto.serviceAreas.length; i++) {
      if (i > 0) {
        filter.push('or');
      }
      filter.push(['ServiceAreas', 'anyof', +this.pricePackageDto.serviceAreas[i].id]);
    }

    return filter;
  }

  private buildPricePackageFilter(): any[] {
    let filter = [];

    if (isNotNullOrUndefined(this.pricePackageDto.type)) {
      filter.push(['type', '=', this.pricePackageDto.type]);
    }
    if (isNotNullOrUndefined(this.pricePackageDto.transportTypeId)) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(['transportTypeId', '=', this.pricePackageDto.transportTypeId]);
    }
    if (isNotNullOrUndefined(this.pricePackageDto.originCountryId)) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(['originCountryId', '=', this.pricePackageDto.originCountryId]);
    }

    if (isNotNullOrUndefined(this.pricePackageDto.truckTypeId)) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(['truckTypeId', '=', this.pricePackageDto.truckTypeId]);
    }

    if (isNotNullOrUndefined(this.pricePackageDto.originCityId)) {
      if (filter.length > 0) {
        filter.push('and');
      }

      filter.push(['originCityId', '=', this.pricePackageDto.originCityId]);
    }

    if (isNotNullOrUndefined(this.pricePackageDto.destinationCityId)) {
      if (filter.length > 0) {
        filter.push('and');
      }
      filter.push(['destinationCityId', '=', this.pricePackageDto.destinationCityId]);
    }

    return filter;
  }

  validateOriginAndDestination() {
    if (this.pricePackageDto.shippingTypeId == 1) {
      this.pricePackageDto.destinationCityId = this.pricePackageDto.originCityId;
    } else {
      this.pricePackageDto.destinationCityId = undefined;
      this.pricePackageDto.originCityId = undefined;
    }
  }

  checkShippingTypeWhenOriginCityChanged() {
    if (this.pricePackageDto.shippingTypeId == 1) {
      this.pricePackageDto.destinationCityId = this.pricePackageDto.originCityId;
    }
  }

  transportTypeChanged() {
    this.loadAllTruckTypeByTransportType(this.pricePackageDto.transportTypeId);
    if (this.companyType == this.companyTypeEnum.Shipper) {
      this.applyFilters();
    }
  }

  shippingTypeChanged() {
    this.pricePackageDto.serviceAreas = undefined;
    this.serviceAreasSelectionLimit = this.pricePackageDto?.shippingTypeId == 1 ? 1 : undefined;

    this.validateOriginAndDestination();

    if (this.companyType == this.companyTypeEnum.Shipper) {
      this.applyFilters();
    }
  }

  originCityChanged() {
    this.checkShippingTypeWhenOriginCityChanged();

    if (this.companyType == this.companyTypeEnum.Shipper) {
      this.applyFilters();
    }
  }

  companyTypeChanged() {
    this.loadAllCompanies();
    this.pricePackageDto.destinationTenantId = undefined;
  }

  originCountryChanged() {
    if (isNotNullOrUndefined(this.pricePackageDto)) {
      this.applyFilters();
      this.pricePackageDto.originCityId = undefined;
      this.pricePackageDto.destinationCityId = undefined;
      this.loadAllCities(this.pricePackageDto.originCountryId);
    }
  }

  loadCountries() {
    this.countriesLoading = true;

    this._tenantRegistrationServiceProxy
      .getAllCountryForTableDropdown()
      .pipe(
        finalize(() => {
          this.countriesLoading = false;
        })
      )
      .subscribe((result) => {
        this.countries = result;
      });
  }

  pricePackageTypeChanged() {
    this.updateCalculatedPriceInputTitle();
    this.applyFilters();
    if (this.pricePackageDto?.type != this.pricePackageType.Dedicated) {
      return;
    }

    this.pricePackageDto.originCityId = undefined;
    this.pricePackageDto.destinationCityId = undefined;
    this.loadCountries();
  }
}
