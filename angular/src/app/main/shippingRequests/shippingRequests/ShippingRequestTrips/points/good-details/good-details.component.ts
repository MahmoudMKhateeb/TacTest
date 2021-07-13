import { Component, OnInit, Injector, ViewChild, OnDestroy } from '@angular/core';
import { CreateOrEditRoutPointDto, GoodsDetailsServiceProxy } from '@shared/service-proxies/service-proxies';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';

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
  singleWayPoint: CreateOrEditRoutPointDto;
  MainGoodsCategory: number;
  allSubGoodCategorys: any;

  currentShippingRequestSubscription: any;
  currentSingleWayPointSubscription: any;
  ngOnInit(): void {
    //take the Good Category From the Shared Service and bind it
    this.currentShippingRequestSubscription = this._TripService.currentShippingRequest.subscribe(
      (res) => (this.MainGoodsCategory = res.shippingRequest.goodCategoryId)
    );
    //get the value of the single way point fron the Shared Service
    this.currentSingleWayPointSubscription = this._PointsService.currentSingleWayPoint.subscribe((res) => (this.singleWayPoint = res));
    this.loadGoodSubCategory(this.MainGoodsCategory);
  }

  getGoodSubDisplayname(id) {
    return this.allSubGoodCategorys ? this.allSubGoodCategorys.find((x) => x.id == id)?.displayName : 0;
  }

  DeleteGoodDetail(id) {
    this.singleWayPoint.goodsDetailListDto.splice(id, 1);
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

  ngOnDestroy() {
    this.currentShippingRequestSubscription.unsubscribe();
    this.currentSingleWayPointSubscription.unsubscribe();
  }
}
