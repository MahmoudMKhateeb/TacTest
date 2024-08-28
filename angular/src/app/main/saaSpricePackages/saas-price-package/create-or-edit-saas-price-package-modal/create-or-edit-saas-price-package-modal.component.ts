/* tslint:disable:triple-equals */
import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditSaasPricePackageDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailsServiceProxy,
  RoundTripType,
  SaasPricePackageServiceProxy,
  SelectItemDto,
  ShippingTypeEnum,
  TripLoadingTypeEnum,
} from '@shared/service-proxies/service-proxies';
import { DxDataGridComponent } from '@node_modules/devextreme-angular';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-create-or-edit-saas-price-package-modal',
  templateUrl: './create-or-edit-saas-price-package-modal.component.html',
  styleUrls: ['./create-or-edit-saas-price-package-modal.component.css'],
})
export class CreateOrEditSaasPricePackageModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('Modal') modal: ModalDirective;
  @ViewChild('grid', { static: true }) dataGrid: DxDataGridComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  pricePackageForSaasDto: CreateOrEditSaasPricePackageDto;
  isFormActive: boolean;
  isFormSaving: boolean;
  isFormLoading: boolean;
  transportTypes: SelectItemDto[];
  truckTypes: SelectItemDto[];

  dataSource: any = {};

  AllActorsShipper: SelectItemDto[];
  pricePackageOriginLocations: SelectItemDto[];
  pricePackageDestinationLocations: SelectItemDto[];
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  allShippingTypes: SelectItemDto[];
  ShippingTypeEnum = ShippingTypeEnum;
  LoadingTypes = this._enumToArrayPipe.transform(TripLoadingTypeEnum);
  RoundTripType: any;

  constructor(
    private injector: Injector,
    private _pricePackageForSaas: SaasPricePackageServiceProxy,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _enumToArrayPipe: EnumToArrayPipe
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.pricePackageForSaasDto = new CreateOrEditSaasPricePackageDto();
    this.isFormActive = false;
    this.isFormSaving = false;
    this.loadAllDropDowns();
  }

  show(id?: number): void {
    if (id) {
      this.isFormLoading = true;
      this.pricePackageForSaasDto.id = id;
      this._pricePackageForSaas.getForEdit(id).subscribe((res) => {
        this.pricePackageForSaasDto = res;
        (this.pricePackageForSaasDto.originCityId as any) = this.pricePackageForSaasDto.originCityId.toString();
        (this.pricePackageForSaasDto.destinationCityId as any) = this.pricePackageForSaasDto.destinationCityId.toString();
        (this.pricePackageForSaasDto.actorShipperId as any) = this.pricePackageForSaasDto.actorShipperId.toString();
        (this.pricePackageForSaasDto.tripLoadingType as any) = this.pricePackageForSaasDto.tripLoadingType.toString();
        (this.pricePackageForSaasDto.roundTripType as any) = this.pricePackageForSaasDto.roundTripType?.toString();

        this.isFormLoading = false;
        this.modal.show();
        this.isFormActive = true;
        this.filteredRoundTripBasedOnShippingType();
      });
    } else {
      this.isFormActive = true;
      this.modal.show();
    }
  }

  /**
   * load All Needed Drop Down
   * @private
   */
  private loadAllDropDowns(): void {
    this.loadCities();
    this.loadAllTransportTypes();
    this.loadAllActors();
    this.getAllGoodCat();
    this.allShippingTypes = this._enumToArrayPipe.transform(ShippingTypeEnum).map((item) => {
      const selectItem = new SelectItemDto();
      (selectItem.id as any) = Number(item.key);
      selectItem.displayName = item.value;
      return selectItem;
    });
  }

  /**
   * loads All TransportTypes
   * @private
   */
  private loadAllTransportTypes(): void {
    this._pricePackageForSaas.getAllTransportTypeForDropdown().subscribe((res) => {
      this.transportTypes = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  /**
   * load all truck type by transport type
   * @param transportTypeId
   */
  public loadAllTruckTypeByTransportType(transportTypeId: number): void {
    this._pricePackageForSaas.getAllTruckTypeForDropdown(transportTypeId).subscribe((res) => {
      this.truckTypes = res.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });
  }

  /**
   * load all cities for DropDown
   * @private
   */
  private loadCities(originCountry?: number | undefined): void {
    this._pricePackageForSaas.getAllCitiesForDropdown(undefined).subscribe((res) => {
      this.pricePackageOriginLocations = res;
      this.pricePackageDestinationLocations = res;
    });
  }

  close() {
    this.pricePackageForSaasDto = new CreateOrEditSaasPricePackageDto();
    this.isFormActive = false;
    this.isFormSaving = false;
    this.truckTypes = undefined;

    this.dataSource = {};

    this.modal.hide();
  }

  public createOrEdit(): void {
    if (this.pricePackageForSaasDto.shippingTypeId == this.ShippingTypeEnum.LocalInsideCity) {
      this.pricePackageForSaasDto.destinationCityId = this.pricePackageForSaasDto.originCityId;
    }
    this.isFormSaving = true;
    this._pricePackageForSaas.createOrEdit(this.pricePackageForSaasDto).subscribe(() => {
      this.notify.success(this.l('Success'));
      this.isFormSaving = false;
      this.modalSave.emit('');
      this.close();
    });
  }

  transportTypeChanged() {
    this.loadAllTruckTypeByTransportType(this.pricePackageForSaasDto.transportTypeId);
  }

  loadAllActors() {
    this._pricePackageForSaas.getAllActorShippersForDropdown().subscribe((res) => {
      this.AllActorsShipper = res;
    });
  }

  getAllGoodCat() {
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });
  }

  filteredRoundTripBasedOnShippingType() {
    //if (!isNotNullOrUndefined(this.RoundTripType)) return;
    let shippingType = this.pricePackageForSaasDto.shippingTypeId;
    if (shippingType == ShippingTypeEnum.ImportPortMovements) {
      this.RoundTripType = this._enumToArrayPipe
        .transform(RoundTripType)
        .filter(
          (item: { key: string; value: string }) =>
            Number(item.key) != RoundTripType.OneWayRoutWithoutPortShuttling &&
            Number(item.key) != RoundTripType.TwoWayRoutsWithoutPortShuttling &&
            Number(item.key) != RoundTripType.TwoWayRoutsWithPortShuttling
        );
    } else if (shippingType == ShippingTypeEnum.ExportPortMovements) {
      this.RoundTripType = this._enumToArrayPipe
        .transform(RoundTripType)
        .filter(
          (item: { key: string; value: string }) =>
            Number(item.key) != RoundTripType.WithReturnTrip &&
            Number(item.key) != RoundTripType.WithoutReturnTrip &&
            Number(item.key) != RoundTripType.WithStorage
        );
    }
  }

  canSelectRoundType() {
    let shippingType = this.pricePackageForSaasDto.shippingTypeId;
    return shippingType == ShippingTypeEnum.ImportPortMovements || shippingType == ShippingTypeEnum.ExportPortMovements;
  }
}
