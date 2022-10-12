import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import {
  CreateOrEditGoodsDetailDto,
  CreateOrEditRoutPointDto,
  DangerousGoodTypesServiceProxy,
  GetAllGoodsCategoriesForDropDownOutput,
  GetAllUnitOfMeasureForDropDownOutput,
  GoodsDetailsServiceProxy,
  PickingType,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { Subscription } from '@node_modules/rxjs';
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

  private activeEditId: number;

  tripServiceSubs$: Subscription;
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

  constructor(
    injector: Injector,
    private _PointsService: PointsService,
    private _TripService: TripService,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _dangerousGoodTypesAppService: DangerousGoodTypesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.myGoodsDetailList = this.GoodDetailsListInput || [];
    //take the current Active WayPoint From the Shared Service
    this.tripServiceSubs$ = this._TripService.currentShippingRequest.subscribe((res) => (this.GoodCategory = res.shippingRequest.goodCategoryId));
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

  show(id?, isForDedicated = false) {
    this.active = true;
    this.goodsDetail = new CreateOrEditGoodsDetailDto();
    //if there is an id this is an edit
    if (isNotNullOrUndefined(id)) {
      this.activeEditId = id;
      this.goodCategoryId = this.myGoodsDetailList[id].goodCategoryId;
      this.weight = this.myGoodsDetailList[id].weight;
      this.amount = this.myGoodsDetailList[id].amount;
      this.unitOfMeasureId = this.myGoodsDetailList[id].unitOfMeasureId;
      this.description = this.myGoodsDetailList[id].description;
      this.isDangerousGood = this.myGoodsDetailList[id].isDangerousGood;
      this.dangerousGoodsCode = this.myGoodsDetailList[id].dangerousGoodsCode;
      this.dangerousGoodsTypeId = this.myGoodsDetailList[id].dangerousGoodTypeId;
      this.otherUnitOfMeasureName = this.myGoodsDetailList[id].otherUnitOfMeasureName;
      this.dimentions = this.myGoodsDetailList[id].dimentions;
    }
    if (this.weightValidation() || isForDedicated) {
      this.createOrEditGoodDetail.show();
    } else {
      this.active = false;
      Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: this.l('WeightLimitReached'),
      });
    }
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
    if (FatherID) {
      this._goodsDetailsServiceProxy
        .getAllGoodCategoryForTableDropdown(FatherID)
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

  /**
   * validates Good Details Weight and Amount
   */
  weightValidation(): boolean {
    let shippingRequestWeight: number;
    let totalWeightGoodDetails: number = 0;
    let wayPointList: CreateOrEditRoutPointDto[];
    let allowedeight: number;

    //get the Total Allowed Weight From the Shipping Request
    this._TripService.currentShippingRequest.subscribe((res) => {
      return (shippingRequestWeight = res.shippingRequest.totalWeight);
    });
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
    this.AllowedWeight = allowedeight;
    this.canAddMoreGoods.emit(allowedeight !== 0); // let the other components know
    return allowedeight !== 0;
  }

  ngOnDestroy() {
    this.tripServiceSubs$.unsubscribe();
  }
}
