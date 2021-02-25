import { Component, ElementRef, Injector, NgZone, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { finalize, map } from 'rxjs/operators';

import {
  CarriersForDropDownDto,
  CreateOrEditFacilityDto,
  CreateOrEditRouteDto,
  CreateOrEditRoutStepDto,
  CreateOrEditShippingRequestDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GoodsDetailGoodCategoryLookupTableDto,
  GoodsDetailsServiceProxy,
  ISelectItemDto,
  RouteRoutTypeLookupTableDto,
  RoutesServiceProxy,
  RoutStepCityLookupTableDto,
  RoutStepsServiceProxy,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  CreateOrEditShippingRequestVasListDto,
  ShippingRequestVasListOutput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { MapsAPILoader } from '@node_modules/@agm/core';
import * as moment from '@node_modules/moment';
import { CreateOrEditGoodsDetailModalComponent } from '@app/main/goodsDetails/goodsDetails/create-or-edit-goodsDetail-modal.component';
import { RouteStepsForCreateShippingRequstComponent } from '../shippingRequests/ShippingRequestRouteSteps/RouteStepsForCreateShippingRequst.component';

@Component({
  templateUrl: './create-or-edit-shippingRequest.component.html',
  styleUrls: ['./create-or-edit-shippingRequest.component.scss'],
  animations: [appModuleAnimation()],
})
export class CreateOrEditShippingRequestComponent extends AppComponentBase implements OnInit {
  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests'),
    new BreadcrumbItem(this.l('Entity_Name_Plural_Here') + '' + this.l('Details')),
  ];

  @ViewChild('search') public searchElementRef: ElementRef;
  @ViewChild('staticModal') public staticModal: ModalDirective;
  @ViewChild('createFacilityModal') public createFacilityModal: ModalDirective;
  @ViewChild('createOrEditGoodsDetailModal') public createOrEditGoodsDetailModal: CreateOrEditGoodsDetailModalComponent;
  @ViewChild('routeStepsForCreateShippingRequstComponent')
  public RouteStepsForCreateShippingRequstComponent: RouteStepsForCreateShippingRequstComponent;
  active = false;
  saving = false;
  shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();
  allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  allCarrierTenants: CarriersForDropDownDto[];
  allRoutTypes: RouteRoutTypeLookupTableDto[];
  allCitys: RoutStepCityLookupTableDto[];
  allFacilities: FacilityForDropdownDto[];
  allPorts: SelectItemDto[];
  createOrEditRoutStepDtoList: CreateOrEditRoutStepDto[] = [];
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  allVases: ShippingRequestVasListOutput[];
  selectedVases: CreateOrEditShippingRequestVasListDto[] = [];
  zoom = 5;

  isTachyonDeal = false;
  isBid = false;
  shippingRequestType: string;
  selectedRouteType: number;
  allTransportTypes: ISelectItemDto[];
  allTrucksTypes: SelectItemDto[];
  allCapacities: SelectItemDto[];
  allShippingTypes: SelectItemDto[];

  truckTypeLoading: boolean;
  capacityLoading: boolean;

  today = new Date();

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _router: Router,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private _routesServiceProxy: RoutesServiceProxy,
    private _facilitiesServiceProxy: FacilitiesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.shippingRequest.createOrEditRouteDto = new CreateOrEditRouteDto();
    // this.routStep.createOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();
    this.show(this._activatedRoute.snapshot.queryParams['id']);
  }

  show(shippingRequestId?: number): void {
    if (!shippingRequestId) {
      //this is a create
      console.log('this is create');
      this.shippingRequest.id = undefined;
      this.active = true;
      this.loadAllDropDownLists();
    } else {
      console.log('this is edit');
      //this is an edit
      this._shippingRequestsServiceProxy
        .getShippingRequestForEdit(shippingRequestId)
        .pipe(
          finalize(() => {
            this.loadAllDropDownLists();
            //console.log(this.shippingRequest);
          })
        )
        .subscribe((result) => {
          this.shippingRequest = result.shippingRequest;
          this.shippingRequestType = result.shippingRequest.isBid === true ? 'bidding' : 'tachyondeal';
          this.selectedVases = result.shippingRequest.shippingRequestVasList;
          this.selectedRouteType = result.shippingRequest.createOrEditRouteDto.routTypeId;
          this.shippingRequest.createOrEditRouteDto = result.shippingRequest.createOrEditRouteDto;
          // this.createOrEditRoutStepDtoList = result.shippingRequest.createOrEditRoutStepDtoList;
          this.active = true;
          //console.log(this.shippingRequest);
        });
    }
    //console.log(this.shippingRequest);
  }

  save(): void {
    //this function fires when Create BTN is Clicked
    this.saving = true;
    this.shippingRequest.id = null;
    this.shippingRequest.isBid = this.shippingRequestType === 'bidding' ? true : false;
    this.shippingRequest.isTachyonDeal = this.shippingRequestType === 'tachyondeal' ? true : false;
    this.shippingRequest.createOrEditRouteDto.routTypeId = this.selectedRouteType; //milkrun / oneway ....
    this.shippingRequest.shippingRequestVasList = this.selectedVases;
    this._shippingRequestsServiceProxy
      .createOrEdit(this.shippingRequest)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('CreatedSuccessfully'));
        this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
      });
  } //end of create

  refreshFacilities() {
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
    });
  }

  loadAllDropDownLists(): void {
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });

    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });

    this._shippingRequestsServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });
    //Get these DD in Edit Only
    if (this.shippingRequest.id) {
      console.log('Load DD for Edit');
      this.capacityLoading = true;
      this.truckTypeLoading = true;
      this._shippingRequestsServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(this.shippingRequest.transportTypeId).subscribe((result) => {
        this.allTrucksTypes = result;
        this.truckTypeLoading = false;
      });
      this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(this.shippingRequest.trucksTypeId).subscribe((result) => {
        this.allCapacities = result;
        this.capacityLoading = false;
      });
    }

    this._shippingRequestsServiceProxy.getAllShippingTypesForDropdown().subscribe((result) => {
      this.allShippingTypes = result;
    });

    this._routesServiceProxy.getAllRoutTypeForTableDropdown().subscribe((result) => {
      this.allRoutTypes = result;
    });

    this.refreshFacilities();
    this.getAllVasList();
  }

  createFacility() {
    this.saving = true;
    console.log('facility Created');
    console.log(this.facility);
    this._facilitiesServiceProxy
      .createOrEdit(this.facility)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.createFacilityModal.hide();
        this.refreshFacilities();
      });
  }

  getAllVasList() {
    this._shippingRequestsServiceProxy.getAllShippingRequestVasesForTableDropdown().subscribe((result) => {
      this.allVases = result;

      if (!this.shippingRequest.id) {
        this.allVases.forEach((element) => {
          element.maxAmount = 0;
          element.maxCount = 0;
        });
      }
    });
  }

  shippingRequestTypeChanged(): void {
    this.shippingRequest.bidStartDate = undefined;
    this.shippingRequest.bidEndDate = undefined;
  }

  // this function is for the first 3 Conditional DD Which is TransportType --> TruckType --> Capacitiy

  transportTypeSelectChange(transportTypeId?: number) {
    if (transportTypeId > 0) {
      this.truckTypeLoading = true;
      this._shippingRequestsServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(transportTypeId).subscribe((result) => {
        this.allTrucksTypes = result;
        this.shippingRequest.trucksTypeId = null;
        this.truckTypeLoading = false;
      });
    } else {
      this.shippingRequest.trucksTypeId = null;
      this.allTrucksTypes = null;
      this.allCapacities = null;
    }
  }

  trucksTypeSelectChange(trucksTypeId?: number) {
    if (trucksTypeId > 0) {
      this.capacityLoading = true;
      this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(trucksTypeId).subscribe((result) => {
        this.allCapacities = result;
        this.shippingRequest.capacityId = null;
        this.capacityLoading = false;
      });
    } else {
      this.shippingRequest.capacityId = null;
      this.allCapacities = null;
    }
  }

  change($event: any) {
    // if item exist do nothing  ;
    // if item exist in this and not exist in selected remove it
    this.selectedVases.forEach((item, index) => {
      const listItem = $event.value.find((x) => x.id == item.vasId);
      if (!listItem) {
        console.log('item existed in selectedVases ', listItem);
        this.selectedVases.splice(index, 1);
      }
    });

    // if item not exist add it
    $event.value.forEach((e) => {
      const selectedItem = this.selectedVases.find((x) => x.vasId == e.id);
      if (!selectedItem) {
        const singleVas = new CreateOrEditShippingRequestVasListDto();
        singleVas.vasId = e.id;
        singleVas.requestMaxAmount = e.hasAmount ? 1 : 0;
        singleVas.requestMaxCount = e.hasCount ? 1 : 0;
        this.selectedVases.push(singleVas);
      }
    });
  }
  vasStatusChecker(vasId: number) {
    return this.allVases.find((value) => value.id == vasId);
  }
}
