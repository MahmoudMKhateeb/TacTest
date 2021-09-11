import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import {
  CreateOrEditGoodsDetailDto,
  DangerousGoodTypesServiceProxy,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailDto,
  GoodsDetailsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { Subscription } from '@node_modules/rxjs';

@Component({
  selector: 'createOrEditGoodDetailsModal',
  templateUrl: './create-or-edit-good-details-modal.component.html',
  styleUrls: ['./create-or-edit-good-details-modal.component.css'],
})
export class CreateOrEditGoodDetailsModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('createOrEditGoodDetail', { static: false }) public createOrEditGoodDetail: ModalDirective;

  active = false;
  // singleWayPoint: CreateOrEditRoutPointDto;
  goodsDetail: CreateOrEditGoodsDetailDto = new GoodsDetailDto();
  myGoodsDetailList: CreateOrEditGoodsDetailDto[] = [];
  allUnitOfMeasure: SelectItemDto[];
  GoodCategory: number;
  allSubGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  isDangerousGoodLoading: boolean;
  allDangerousGoodTypes: SelectItemDto[];

  private activeEditId: number;

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
  @Input() GoodDetailsListInput: CreateOrEditGoodsDetailDto[];
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  tripServiceSubs$: Subscription;
  // pointServiceSubs$: Subscription;
  goodCategoryId: number;
  weight: number;
  amount: number;
  unitOfMeasureId: number;
  description: string;
  isDangerousGood: boolean;
  dangerousGoodsTypeId: number;
  dangerousGoodsCode: string;
  dimentions: string;
  ngOnDestroy() {
    this.tripServiceSubs$.unsubscribe();
    // this.pointServiceSubs$.unsubscribe();
  }
  ngOnInit(): void {
    this.myGoodsDetailList = this.GoodDetailsListInput || [];
    //take the current Active WayPoint From the Shared Service
    this.tripServiceSubs$ = this._TripService.currentShippingRequest.subscribe((res) => (this.GoodCategory = res.shippingRequest.goodCategoryId));
    //sync the singleWayPoint From the Service
    // this.pointServiceSubs$ = this._PointsService.currentSingleWayPoint.subscribe((res) => (this.singleWayPoint = res));
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

  show(id?) {
    this.active = true;
    // console.log('this is a Good Details Edit OutSide Of Edit....', id, this.myGoodsDetailList);
    this.goodsDetail = new GoodsDetailDto();
    //if there is an id this is an edit
    if (typeof id !== 'undefined') {
      // console.log('this is a Good Details Edit ....', id);
      // console.log(this.myGoodsDetailList);
      this.activeEditId = id;
      this.goodCategoryId = this.myGoodsDetailList[id].goodCategoryId;
      this.weight = this.myGoodsDetailList[id].weight;
      this.amount = this.myGoodsDetailList[id].amount;
      this.unitOfMeasureId = this.myGoodsDetailList[id].unitOfMeasureId;
      this.description = this.myGoodsDetailList[id].description;
      this.isDangerousGood = this.myGoodsDetailList[id].isDangerousGood;
      this.dangerousGoodsCode = this.myGoodsDetailList[id].dangerousGoodsCode;
      this.dangerousGoodsTypeId = this.myGoodsDetailList[id].dangerousGoodTypeId;

      this.dimentions = this.myGoodsDetailList[id].dimentions;
    }
    this.createOrEditGoodDetail.show();
  }
  close() {
    this.active = false;
    this.activeEditId = undefined;
    this.goodCategoryId = undefined;
    this.weight = undefined;
    this.amount = undefined;
    this.unitOfMeasureId = undefined;
    this.description = undefined;
    this.isDangerousGood = undefined;
    this.dangerousGoodsCode = undefined;
    this.dangerousGoodsTypeId = undefined;
    this.dimentions = undefined;
    this.createOrEditGoodDetail.hide();
  }

  AddOrEditGoodDetail() {
    this.goodsDetail.goodCategoryId = this.goodCategoryId;
    this.goodsDetail.weight = this.weight;
    this.goodsDetail.amount = this.amount;
    this.goodsDetail.unitOfMeasureId = this.unitOfMeasureId;
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
      this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(FatherID).subscribe((result) => {
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
}
