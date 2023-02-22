import { Component, Injector, Input, OnDestroy, OnInit } from '@angular/core';
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
import { retry } from '@node_modules/rxjs/internal/operators';

@Component({
  selector: 'PointGoodDetailsComponent',
  templateUrl: './good-details.component.html',
  styleUrls: ['./good-details.component.css'],
})
export class GoodDetailsComponent extends AppComponentBase implements OnInit, OnDestroy {
  //inCase Of View Point
  @Input() goodDetailsListForView: GoodsDetailDto[];
  @Input() isForDedicated = false;
  @Input() isHomeDelivery = false;
  usedIn: 'view' | 'createOrEdit';
  //For Create/Edit
  Point: CreateOrEditRoutPointDto;
  goodsDetailList: CreateOrEditGoodsDetailDto[];
  MainGoodsCategory: number;
  allSubGoodCategorys: any;
  allSubGoodCategorysLoading = true;
  //tripServiceSubs$: Subscription;
  pointServiceSubs$: Subscription;
  usedInSubs$: Subscription;
  canAddMoreGoods = true;
  isDirectTrip = false;
  constructor(
    injector: Injector,
    private _TripService: TripService,
    private _PointsService: PointsService,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.isDirectTrip = !this._TripService.GetShippingRequestForViewOutput?.shippingRequest;

    this.usedInSubs$ = this._PointsService.currentUsedIn.subscribe((res) => (this.usedIn = res));
    if (this.usedIn !== 'view') {
      //take the Good Category From the Shared Service and bind it

      this.MainGoodsCategory = this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.goodCategoryId;
      this.loadGoodSubCategory(this._TripService.GetShippingRequestForViewOutput?.shippingRequest?.goodCategoryId);

      //get the value of the single way point fron the Shared Service
      this.pointServiceSubs$ = this._PointsService.currentSingleWayPoint.subscribe((res) => {
        this.Point = res;
        this.goodsDetailList = res.goodsDetailListDto || [];
      });
      //  this.goodsDetailList = [];
    }
  }

  getGoodSubDisplayname(id) {
    return this.allSubGoodCategorys ? this.allSubGoodCategorys.find((x) => x.id == id)?.displayName : '';
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
    if (FatherID || this.isDirectTrip) {
      this.allSubGoodCategorysLoading = true;
      this._goodsDetailsServiceProxy
        .getAllGoodCategoryForTableDropdown(FatherID)
        .pipe(retry(3))
        .subscribe((result) => {
          this.allSubGoodCategorys = result;
          this.allSubGoodCategorysLoading = false;
        });
    }
  }

  ngOnDestroy() {
    // this.tripServiceSubs$?.unsubscribe();
    this.pointServiceSubs$?.unsubscribe();
    this.usedInSubs$.unsubscribe();
  }
}
