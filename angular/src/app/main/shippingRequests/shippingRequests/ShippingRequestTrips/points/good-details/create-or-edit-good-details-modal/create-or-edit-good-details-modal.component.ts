import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import {
  CreateOrEditGoodsDetailDto,
  CreateOrEditRoutPointDto,
  DangerousGoodTypesServiceProxy,
  GetAllGoodsCategoriesForDropDownOutput,
  GetAllUnitOfMeasureForDropDownOutput,
  GetShippingRequestForViewOutput,
  GoodsDetailsServiceProxy,
  PickingType,
  RoundTripType,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  ShippingTypeEnum,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import Swal from 'sweetalert2';
import { retry } from '@node_modules/rxjs/internal/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'createOrEditGoodDetailsModal',
  templateUrl: './create-or-edit-good-details-modal.component.html',
  styleUrls: ['./create-or-edit-good-details-modal.component.css'],
})
export class CreateOrEditGoodDetailsModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('createOrEditGoodDetail', { static: false }) public createOrEditGoodDetail: ModalDirective;
  @Input() GoodDetailsListInput: CreateOrEditGoodsDetailDto[];
  @Input() isHomeDelivery: boolean;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Output() canAddMoreGoods: EventEmitter<boolean> = new EventEmitter<boolean>();

  active = false;
  // singleWayPoint: CreateOrEditRoutPointDto;
  goodsDetail: CreateOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();
  myGoodsDetailList: CreateOrEditGoodsDetailDto[] = [];
  allUnitOfMeasure: GetAllUnitOfMeasureForDropDownOutput[];
  GoodCategory: number;
  allSubGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  isDangerousGoodLoading: boolean;
  allDangerousGoodTypes: SelectItemDto[];

  activeEditId: number;

  //tripServiceSubs$: Subscription;
  goodCategoryId: number;
  weight: number;
  amount: number;
  unitOfMeasureId: number;
  description: string;
  otherUnitOfMeasureName: string;
  isDangerousGood: boolean;
  dangerousGoodsTypeId: number;
  dangerousGoodsCode: string;
  dimentions: string;
  AllowedWeight: number;
  isForDedicated: boolean;
  isForPortsMovement: boolean;
  currentShippingRequest: GetShippingRequestForViewOutput;
  goodCategoryDefaultId: number;
  isWaterStockAvailable: boolean = true; // Set default value

  get shouldDisableCategoryForPortsMovement() {
    let roundTripType = this._PointsService?.currentShippingRequest?.shippingRequest?.roundTripType;
    const isReturnTrip = roundTripType === RoundTripType.WithReturnTrip;
    const isNotOneWayWithoutPortShuttling = roundTripType !== RoundTripType.OneWayRoutWithoutPortShuttling;
    const isNotReturnTrip = roundTripType !== RoundTripType.WithReturnTrip;
    const isNotWithoutReturnTrip = roundTripType !== RoundTripType.WithoutReturnTrip;
    const isFirstPoint = this._PointsService?.currentPointIndex == 1;
    const isAfterFirstPoint = this._PointsService?.currentPointIndex > 1;
    const isWithStorage = this._TripService.CreateOrEditShippingRequestTripDto.roundTripType == RoundTripType.WithStorage;
    if (isWithStorage) {
      if (this._PointsService.currentPointIndex === 5) {
        return true;
      }
      return false;
    }
    const isEligibleForDisablingCategory =
      (isAfterFirstPoint && isReturnTrip) || (isFirstPoint && isNotOneWayWithoutPortShuttling && isNotReturnTrip && isNotWithoutReturnTrip);

    return this.isForPortsMovement && isEligibleForDisablingCategory;
  }

  get isWeightRequiredForPortMovement() {
    if (this.isForPortsMovement && isNotNullOrUndefined(this._PointsService?.currentPointIndex)) {
      if (
        this._PointsService?.currentShippingRequest?.shippingRequest?.roundTripType == RoundTripType.WithReturnTrip &&
        this._PointsService?.currentPointIndex == 1
      ) {
        return false;
      }
      if (
        this._PointsService?.currentShippingRequest?.shippingRequest?.roundTripType !== RoundTripType.WithoutReturnTrip &&
        this._PointsService?.currentPointIndex > 0
      ) {
        return true;
      }
    }
    return false;
  }
  get isQtyRequiredForPortMovement() {
    if (this.isForPortsMovement && isNotNullOrUndefined(this._PointsService?.currentPointIndex)) {
      if (
        this._PointsService?.currentShippingRequest?.shippingRequest?.roundTripType !== RoundTripType.WithReturnTrip &&
        this._PointsService?.currentShippingRequest?.shippingRequest?.roundTripType !== RoundTripType.WithoutReturnTrip &&
        this._PointsService?.currentPointIndex > 0
      ) {
        return true;
      }
    }
    return false;
  }

  constructor(
    injector: Injector,
    public _PointsService: PointsService,
    public _TripService: TripService,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _dangerousGoodTypesAppService: DangerousGoodTypesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.currentShippingRequest = this._PointsService.currentShippingRequest;
    this.myGoodsDetailList = this.GoodDetailsListInput || [];
    if (this._TripService.GetShippingRequestForViewOutput) {
      this.GoodCategory = this._TripService.GetShippingRequestForViewOutput?.goodCategoryId;
    } else {
      this.GoodCategory = this._TripService.CreateOrEditShippingRequestTripDto?.goodCategoryId;
    }
    //take the current Active WayPoint From the Shared Service
    //this.tripServiceSubs$ = this._TripService.currentShippingRequest.subscribe((res) => (this.GoodCategory = res.shippingRequest.goodCategoryId));
    //sync the singleWayPoint From the Service
    this.loadAllDropDowns();
  }

  /**
   * load DropDowns
   */
  loadAllDropDowns() {
    this._shippingRequestsServiceProxy.getAllUnitOfMeasuresForDropdown().subscribe((result) => {
      this.allUnitOfMeasure = result;
    });
    this.loadGoodSubCategory(this.GoodCategory);
    this.loadGoodDangerousTypes();
  }

  show(id?: number, isForDedicated = false, isForPortsMovement = false) {
    this.isForDedicated = isForDedicated;
    this.isForPortsMovement = isForPortsMovement;
    this.active = true;
    this.goodsDetail = new CreateOrEditGoodsDetailDto();

    // Check if editing an existing record
    if (isNotNullOrUndefined(id)) {
      this.populateGoodsDetailForEdit(id);
    }

    const isImportWithStorage = this._PointsService.currentShippingRequest?.roundTripType === RoundTripType.WithStorage;
    //isForPortsMovement = isImportWithStorage;
    // Handle specific logic for port movements
    if (isForPortsMovement || isImportWithStorage) {
      this.handlePortsMovement(id);
    }

    // Validate weight and show detail modal or show error
    if (this.weightValidation() || isForDedicated) {
      this.createOrEditGoodDetail.show();
    } else {
      this.active = false;
      this.showWeightLimitError();
    }
  }

  private populateGoodsDetailForEdit(id: number) {
    console.log('this.myGoodsDetailList', this.myGoodsDetailList);
    this.activeEditId = id;
    const detail = this.myGoodsDetailList[id];
    this.goodCategoryId = detail.goodCategoryId;
    this.weight = detail.weight;
    this.amount = detail.amount;
    this.unitOfMeasureId = detail.unitOfMeasureId;
    this.description = detail.description;
    this.isDangerousGood = detail.isDangerousGood;
    this.dangerousGoodsCode = detail.dangerousGoodsCode;
    this.dangerousGoodsTypeId = detail.dangerousGoodTypeId;
    this.otherUnitOfMeasureName = detail.otherUnitOfMeasureName;
    this.dimentions = detail.dimentions;
  }

  private handlePortsMovement(id?: number) {
    console.log('handlePortsMovement');
    if (!isNotNullOrUndefined(id)) {
      // if there is a good id (edit)
      console.log('handlePortsMovement', id);
      this.amount = this._PointsService.currentShippingRequest?.shippingRequest?.numberOfPacking;
      this.weight = this._PointsService.currentShippingRequest?.shippingRequest?.totalWeight;
      this.goodCategoryId = this.allSubGoodCategorys?.length > 0 ? this.allSubGoodCategorys[0].id : null;

      const currentShippingRequest = this._PointsService.currentShippingRequest;
      const createOrEditShippingRequestTripDto = this._TripService.CreateOrEditShippingRequestTripDto;
      let isRoundTypeWithReturnTrip =
        createOrEditShippingRequestTripDto?.roundTripType === RoundTripType.WithReturnTrip ||
        currentShippingRequest?.roundTripType === RoundTripType.WithReturnTrip;

      if (isRoundTypeWithReturnTrip && this._PointsService.currentPointIndex === 3) {
        this.weight = null;
        this.getContainerUOMDefaultId();
      }

      if (createOrEditShippingRequestTripDto.shippingTypeId == ShippingTypeEnum.ExportPortMovements && this._PointsService.currentPointIndex === 1) {
        this.weight = null;
        this.getContainerUOMDefaultId();
      }
    } else {
      this.loadGoodSubCategory(null);
    }
  }

  private showWeightLimitError() {
    Swal.fire({
      icon: 'error',
      title: 'Oops...',
      text: this.l('WeightLimitReached'),
    });
  }

  close() {
    this.active = false;
    this.activeEditId = undefined;
    this.goodCategoryId = undefined;
    this.weight = undefined;
    this.amount = undefined;
    this.unitOfMeasureId = undefined;
    this.otherUnitOfMeasureName = undefined;
    this.description = undefined;
    this.isDangerousGood = undefined;
    this.dangerousGoodsCode = undefined;
    this.dangerousGoodsTypeId = undefined;
    this.dimentions = undefined;
    this.createOrEditGoodDetail.hide();
  }

  validateOthersInputs() {
    if (this.IfOther(this.allUnitOfMeasure, this.unitOfMeasureId) && !this.otherUnitOfMeasureName.trim()) return false;
    else return true;
  }

  AddOrEditGoodDetail() {
    if (!this.validateOthersInputs()) {
      this.notify.error(this.l('PleaseCompleteMissingFields'));
      return false;
    }
    this.goodsDetail.goodCategoryId = this.goodCategoryId;
    this.goodsDetail.weight = this.weight;
    this.goodsDetail.amount = this.amount;
    this.goodsDetail.unitOfMeasureId = this.unitOfMeasureId;
    this.goodsDetail.otherUnitOfMeasureName = this.otherUnitOfMeasureName;
    this.goodsDetail.description = this.description;
    this.goodsDetail.isDangerousGood = this.isDangerousGood;
    this.goodsDetail.dimentions = this.dimentions;
    this.goodsDetail.dangerousGoodTypeId = this.dangerousGoodsTypeId;
    this.goodsDetail.dangerousGoodsCode = this.dangerousGoodsCode;
    //inCase of Edit Update the Record Dont Create A new one
    if (typeof this.activeEditId !== 'undefined') {
      this.myGoodsDetailList[this.activeEditId] = this.goodsDetail;
    } else {
      this.myGoodsDetailList.push(this.goodsDetail);
    }
    this.modalSave.emit(this.myGoodsDetailList);
    this.close();
  }

  /**
   * load Goods Sub-Category Bu Shipping Request Category
   * @param FatherID
   */
  loadGoodSubCategory(FatherID) {
    //Get All Sub-Good Category
    console.log('FatherID', FatherID);
    if (this.shouldDisableCategoryForPortsMovement) {
      console.log('this.shouldDisableCategoryForPortsMovement ................................ ');
      this._goodsDetailsServiceProxy
        .getEmptyGoodsCategoryForDropDown()
        .pipe(retry(3))
        .subscribe((result) => {
          this.allSubGoodCategorys = [result];
          this.goodCategoryId = this.allSubGoodCategorys[this._PointsService.currentPointIndex]?.id;
          this.description = this.l('EmptyContainer');
        });
      return;
    }
    let goodCat = this._TripService.CreateOrEditShippingRequestTripDto.goodCategoryId;
    if (FatherID || goodCat) {
      console.log('Normal Good Cat ...................');
      this._goodsDetailsServiceProxy
        .getAllGoodCategoryForTableDropdown(FatherID || goodCat)
        .pipe(retry(3))
        .subscribe((result) => {
          this.allSubGoodCategorys = result;
        });
    }
  }

  /**
   * load All Good Dangerous Types For DropDown
   */
  loadGoodDangerousTypes() {
    this.isDangerousGoodLoading = true;
    this._dangerousGoodTypesAppService.getAllForDropdownList().subscribe((res) => {
      this.isDangerousGoodLoading = false;
      this.allDangerousGoodTypes = res;
    });
  }

  isWaterSelected() {
    const waterId = this.allSubGoodCategorys.find(
      (category) => category.displayName.toLowerCase() === 'water' || category.displayName == 'مياه' || category.displayName == 'ماء'
    )?.id;
    return this.goodCategoryId === waterId;
  }
  /**
   * check stock with reachwere
   */
  checkWaterStock(): void {
    let isSab = this.feature.isEnabled('App.Sab');
    if (isSab && this.isWaterSelected()) {
      //logic here
      this._PointsService.checkAvailability(this.amount).subscribe((isAvailable) => {
        this.isWaterStockAvailable = isAvailable;
      });
    }
  }

  /**
   * for custome dvx validator
   * @param e
   */
  validateStock(e) {
    return this.isWaterStockAvailable;
  }

  /**
   * validates Good Details Weight and Amount
   */
  weightValidation(): boolean {
    let shippingRequestWeight: number;
    let totalWeightGoodDetails: number = 0;
    let wayPointList: CreateOrEditRoutPointDto[];
    let allowedeight: number;

    if (this.isForPortsMovement) {
      this.AllowedWeight = this._PointsService.currentShippingRequest?.shippingRequest?.totalWeight;
      return true;
    }

    //get the Total Allowed Weight From the Shipping Request

    shippingRequestWeight = this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.totalWeight;

    //get the Way Points from the shared service
    this._PointsService.currentWayPointsList.subscribe((res) => {
      return (wayPointList = res);
    });
    //get the sum of total Added Goods by looping through each drop point and extract the amount * weight
    wayPointList.forEach((point) => {
      if (point.pickingType === PickingType.Dropoff) {
        if (!point.goodsDetailListDto) {
          this.AllowedWeight = allowedeight = shippingRequestWeight;
          // this.AllowedWeight;
          return true;
        }
        point.goodsDetailListDto.forEach((dropPoint) => {
          totalWeightGoodDetails += dropPoint.amount * dropPoint.weight;
        });
      }
    });
    //allowed weight is how much weight is left for the user
    allowedeight = shippingRequestWeight - (totalWeightGoodDetails - (this.weight === undefined ? 0 : this.weight));
    this.AllowedWeight = !this.isForDedicated ? allowedeight : this.weight;
    this.canAddMoreGoods.emit(allowedeight !== 0); // let the other components know
    return allowedeight !== 0;
  }

  getContainerUOMDefaultId() {
    this._shippingRequestsServiceProxy.getContainerUOMId().subscribe((res) => {
      this.amount = 1;
      this.unitOfMeasureId = res;
    });
  }

  ngOnDestroy() {
    //this.tripServiceSubs$.unsubscribe();
  }
}
