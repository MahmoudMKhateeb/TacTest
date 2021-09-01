import { Component, OnInit, Injector, ViewChild, OnDestroy } from '@angular/core';
import { CreateOrEditGoodsDetailDto, CreateOrEditRoutPointDto, GoodsDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import { Subscription } from 'rxjs';

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
  Point: CreateOrEditRoutPointDto;
  goodsDetailList: CreateOrEditGoodsDetailDto[];
  MainGoodsCategory: number;
  allSubGoodCategorys: any;
  allSubGoodCategorysLoading = true;

  tripServiceSubs$: Subscription;
  pointServiceSubs$: Subscription;
  ngOnDestroy() {
    this.tripServiceSubs$.unsubscribe();
    this.pointServiceSubs$.unsubscribe();
    console.log('Destroy From Good Details Component');
  }

  ngOnInit(): void {
    //take the Good Category From the Shared Service and bind it
    this.tripServiceSubs$ = this._TripService.currentShippingRequest.subscribe(
      (res) => (this.MainGoodsCategory = res.shippingRequest.goodCategoryId)
    );
    //get the value of the single way point fron the Shared Service
    this.pointServiceSubs$ = this._PointsService.currentSingleWayPoint.subscribe((res) => {
      this.Point = res;
      this.goodsDetailList = res.goodsDetailListDto || [];
    });
    //  this.goodsDetailList = [];

    this.loadGoodSubCategory(this.MainGoodsCategory);
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
