import { Component, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import {
  CreateOrEditRoutPointDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailDto,
  GoodsDetailsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';

@Component({
  selector: 'createOrEditGoodDetailsModal',
  templateUrl: './create-or-edit-good-details-modal.component.html',
  styleUrls: ['./create-or-edit-good-details-modal.component.css'],
})
export class CreateOrEditGoodDetailsModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditGoodDetail', { static: false }) public createOrEditGoodDetail: ModalDirective;

  active = false;
  singleWayPoint: CreateOrEditRoutPointDto;
  goodsDetail: GoodsDetailDto = new GoodsDetailDto();
  allUnitOfMeasure: SelectItemDto[];
  GoodCategory: number;
  allSubGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];

  constructor(
    injector: Injector,
    private _PointsService: PointsService,
    private _TripService: TripService,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy
  ) {
    super(injector);
  }
  currentSingleWayPointSubscription: any;
  currentShippingRequestSubscription: any;
  ngOnInit(): void {
    //take the current Active WayPoint From the Shared Service
    this.currentSingleWayPointSubscription = this._PointsService.currentSingleWayPoint.subscribe((res) => (this.singleWayPoint = res));
    this.currentShippingRequestSubscription = this._TripService.currentShippingRequest.subscribe(
      (res) => (this.GoodCategory = res.shippingRequest.goodCategoryId)
    );
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
  }

  show() {
    this.active = true;
    this.createOrEditGoodDetail.show();
  }
  close() {
    this.active = false;
    this.goodsDetail = new GoodsDetailDto();
    this.createOrEditGoodDetail.hide();
  }

  AddGoodDetail() {
    if (!this.singleWayPoint.goodsDetailListDto) {
      this.singleWayPoint.goodsDetailListDto = [];
    }
    this.singleWayPoint.goodsDetailListDto.push(this.goodsDetail);
    //this._PointsService.updateSinglePoint(this.singleWayPoint);
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
}
