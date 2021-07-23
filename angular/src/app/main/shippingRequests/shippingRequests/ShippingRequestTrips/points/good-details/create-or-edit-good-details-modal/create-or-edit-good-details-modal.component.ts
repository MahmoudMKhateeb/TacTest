import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { PointsService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/points.service';
import {
  CreateOrEditGoodsDetailDto,
  CreateOrEditRoutPointDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailDto,
  GoodsDetailsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { GoodDetailsComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/points/good-details/good-details.component';
import { Subscription } from '@node_modules/rxjs';

@Component({
  selector: 'createOrEditGoodDetailsModal',
  templateUrl: './create-or-edit-good-details-modal.component.html',
  styleUrls: ['./create-or-edit-good-details-modal.component.css'],
})
export class CreateOrEditGoodDetailsModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @ViewChild('createOrEditGoodDetail', { static: false }) public createOrEditGoodDetail: ModalDirective;

  active = false;
  singleWayPoint: CreateOrEditRoutPointDto;
  goodsDetail: GoodsDetailDto = new GoodsDetailDto();
  goodsDetailList: CreateOrEditGoodsDetailDto[] = [];

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
  @Input() GoodDetailsListInput: CreateOrEditGoodsDetailDto[];
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  tripServiceSubs$: Subscription;
  pointServiceSubs$: Subscription;
  ngOnDestroy() {
    this.tripServiceSubs$.unsubscribe();
    this.pointServiceSubs$.unsubscribe();
    console.log('Destroy From Create/Edit Good Details Component');
  }
  ngOnInit(): void {
    this.goodsDetailList = this.GoodDetailsListInput || [];
    //take the current Active WayPoint From the Shared Service
    this.tripServiceSubs$ = this._TripService.currentShippingRequest.subscribe((res) => (this.GoodCategory = res.shippingRequest.goodCategoryId));
    //sync the singleWayPoint From the Service
    this.pointServiceSubs$ = this._PointsService.currentSingleWayPoint.subscribe((res) => (this.singleWayPoint = res));
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
    this.goodsDetailList.push(this.goodsDetail);
    this.modalSave.emit(this.goodsDetailList);
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
