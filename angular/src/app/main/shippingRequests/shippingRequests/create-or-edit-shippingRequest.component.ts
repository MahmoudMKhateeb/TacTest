import { Component, Injector, NgZone, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';

import {
  CarriersForDropDownDto,
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
  CreateOrEditRoutPointDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { MapsAPILoader } from '@node_modules/@agm/core';

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

  active = false;
  saving = false;
  shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();
  allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  allCarrierTenants: CarriersForDropDownDto[];
  allRoutTypes: RouteRoutTypeLookupTableDto[];
  allCitys: RoutStepCityLookupTableDto[];
  allFacilities: FacilityForDropdownDto[];
  createOrEditRoutStepDtoList: CreateOrEditRoutStepDto[] = [];
  allVases: ShippingRequestVasListOutput[];
  selectedVases: CreateOrEditShippingRequestVasListDto[] = [];
  isTachyonDeal = false;
  isBid = false;
  shippingRequestType: string;
  selectedRouteType: number;
  allTransportTypes: ISelectItemDto[];
  allTrucksTypes: SelectItemDto[];
  allCapacities: SelectItemDto[];
  allShippingTypes: SelectItemDto[];
  allpackingTypes: SelectItemDto[];
  truckTypeLoading: boolean;
  capacityLoading: boolean;
  selectedVasesProperties = [];
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
    this.show(this._activatedRoute.snapshot.queryParams['id']);
  }

  show(shippingRequestId?: number): void {
    if (!shippingRequestId) {
      //this is a create
      this.shippingRequest.id = null;
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
          })
        )
        .subscribe((result) => {
          this.shippingRequest = result.shippingRequest;
          this.shippingRequestType = result.shippingRequest.isBid === true ? 'bidding' : 'tachyondeal';
          this.selectedVases = result.shippingRequest.shippingRequestVasList;
          console.log(this.selectedVases);
          this.selectedRouteType = result.shippingRequest.createOrEditRouteDto.routTypeId;
          this.shippingRequest.createOrEditRouteDto = result.shippingRequest.createOrEditRouteDto;
          this.active = true;
        });
    }
  }

  save(): void {
    this.saving = true;
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

    this._shippingRequestsServiceProxy.getAllPackingTypesForDropdown().subscribe((result) => {
      this.allpackingTypes = result;
    });

    this._routesServiceProxy.getAllRoutTypeForTableDropdown().subscribe((result) => {
      this.allRoutTypes = result;
    });

    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
    });
    this.loadallVases();
  }

  loadallVases() {
    this._shippingRequestsServiceProxy.getAllShippingRequestVasesForTableDropdown().subscribe((result) => {
      this.allVases = result;
      this.allVases.forEach((item) => {
        this.selectedVasesProperties[item.id] = {
          vasId: item.id,
          vasName: item.vasName,
          vasCountDisabled: item.hasAmount ? false : true,
          vasAmountDisabled: item.hasCount ? false : true,
        };
      });
      console.log(this.selectedVasesProperties);
    });
  }

  resetBiddingDates(): void {
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

  //select a vas and move it to Selected Vases
  selectVases($event: any) {
    //if deSelectAll emptyTheSelectedItemsArray
    if ($event.value.length === 0) {
      return (this.selectedVases = []);
    }
    // if item exist do nothing  ;
    // if item exist in this and not exist in selected remove it
    this.selectedVases.forEach((item, index) => {
      const listItem = $event.value.find((x) => x.id == item.vasId);
      if (!listItem) {
        this.selectedVases.splice(index, 1);
      }
    });
    // if item not exist add it
    $event.value.forEach((e) => {
      const selectedItem = this.selectedVases.find((x) => x.vasId == e.id);
      if (!selectedItem) {
        const singleVas = new CreateOrEditShippingRequestVasListDto();
        singleVas.vasId = e.id;
        this.selectedVases.push(singleVas);
      }
    });
  }
}
