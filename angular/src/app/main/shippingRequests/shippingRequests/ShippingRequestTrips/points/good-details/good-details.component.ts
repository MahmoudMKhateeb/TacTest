import { Component, OnInit, Injector, ViewChild, OnDestroy, Input } from '@angular/core';
import {
  CreateOrEditGoodsDetailDto,
  CreateOrEditRoutPointDto,
  GoodsDetailDto,
  GoodsDetailsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { Subscription } from 'rxjs';
import { first } from '@node_modules/rxjs/internal/operators';

@Component({
  selector: 'PointGoodDetailsComponent',
  templateUrl: './good-details.component.html',
  styleUrls: ['./good-details.component.css'],
})
export class GoodDetailsComponent extends AppComponentBase implements OnInit, OnDestroy {
  constructor(
    injector: Injector,
    private _TripService: TripService,
    private _PointsService: PointsService,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
  ) {
    super(injector);
  }
  //inCase Of View Point
  @Input() goodDetailsListForView: GoodsDetailDto[];
  usedIn: 'view' | 'createOrEdit';
  //For Create/Edit
  Point: CreateOrEditRoutPointDto;
  goodsDetailList: CreateOrEditGoodsDetailDto[];
  MainGoodsCategory: number;
  allSubGoodCategorys: any;
  allSubGoodCategorysLoading = true;
  tripServiceSubs$: Subscription;
  pointServiceSubs$: Subscription;
  usedInSubs$: Subscription;
  canAddMoreGoods = true;

  ngOnDestroy() {
    this.tripServiceSubs$?.unsubscribe();
    this.pointServiceSubs$?.unsubscribe();
    this.usedInSubs$.unsubscribe();
    console.log('Destroy From Good Details Component');
  }

  ngOnInit(): void {
    this.usedInSubs$ = this._PointsService.currentUsedIn.subscribe((res) => (this.usedIn = res));
    console.log(' this.usedIn:   ', this.usedIn);
    if (this.usedIn !== 'view') {
      //take the Good Category From the Shared Service and bind it
      this.tripServiceSubs$ = this._TripService.currentShippingRequest.pipe(first()).subscribe((res) => {
        this.MainGoodsCategory = res.shippingRequest.goodCategoryId;
        this.loadGoodSubCategory(res.shippingRequest.goodCategoryId);
        //get the value of the single way point fron the Shared Service
      });
      //get the value of the single way point fron the Shared Service
      this.pointServiceSubs$ = this._PointsService.currentSingleWayPoint.subscribe((res) => {
        this.Point = res;
        this.goodsDetailList = res.goodsDetailListDto || [];
      });
      //  this.goodsDetailList = [];
    }
  }

  getGoodSubDisplayname(id) {
    return this.allSubGoodCategorys ? this.allSubGoodCategorys.find((x) => x.id == id)?.displayName : 0;
  }

  /**
   * Detele Good Detail by id
   * @param id
   */
  DeleteGoodDetail(id) {
    this.goodsDetailList.splice(id, 1);
    console.log('goods Detail List : ', this.goodsDetailList);
  }

  /**
   * load Goods Sub-Category Bu Shipping Request Category
   * @param FatherID
   */
  loadGoodSubCategory(FatherID) {
    //Get All Sub-Good Category
    if (FatherID) {
      this.allSubGoodCategorysLoading = true;
      this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(FatherID).subscribe((result) => {
        this.allSubGoodCategorys = result;
        this.allSubGoodCategorysLoading = false;
      });
    }
  }
}
