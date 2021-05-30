import { AfterViewInit, Component, ElementRef, Inject, Injector, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import KTWizard from '@metronic/common/js/components/wizard';
import { KTUtil } from '@metronic/common/js/components/util';

import {
  CarriersForDropDownDto,
  CreateOrEditShippingRequestDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodsDetailsServiceProxy,
  ISelectItemDto,
  RoutStepCityLookupTableDto,
  SelectItemDto,
  ShippingRequestsServiceProxy,
  CreateOrEditShippingRequestVasListDto,
  ShippingRequestVasListOutput,
  RoutStepsServiceProxy,
  ShippingRequestRouteType,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  templateUrl: './create-or-edit-shipping-request-wizard.component.html',
  styleUrls: ['./create-or-edit-shipping-request-wizard.component.scss'],
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class CreateOrEditShippingRequestWizardComponent extends AppComponentBase implements OnDestroy, AfterViewInit, OnInit {
  active = false;
  saving = false;
  shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  allCarrierTenants: CarriersForDropDownDto[];
  allRoutTypes: any;
  allCitys: RoutStepCityLookupTableDto[];
  allFacilities: FacilityForDropdownDto[];
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
  // selectedVasesProperties = [];
  today = new Date();
  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _router: Router,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private enumToArray: EnumToArrayPipe
  ) {
    super(injector);
  }
  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests'),
    // new BreadcrumbItem(this.l('Entity_Name_Plural_Here') + '' + this.l('Details')),
  ];

  @ViewChild('wizard', { static: true }) el: ElementRef;
  model: any = {
    address1: 'Address Line 1',
    address2: 'Address Line 2',
    postcode: '3000',
    city: 'Melbourne',
    state: 'VIC',
    country: 'AU',
    package: 'Complete Workstation (Monitor, Computer, Keyboard & Mouse)',
    weight: '25',
    width: '110',
    height: '90',
    length: '150',
    delivery: 'overnight',
    packaging: 'regular',
    preferreddelivery: 'morning',
    locaddress1: 'Address Line 1',
    locaddress2: 'Address Line 2',
    locpostcode: '3072',
    loccity: 'Preston',
    locstate: 'VIC',
    loccountry: 'AU',
  };
  submitted = false;
  wizard: any;

  ngOnInit() {
    this.loadAllDropDownLists();
    this.allRoutTypes = this.enumToArray.transform(ShippingRequestRouteType);
  }

  ngOnDestroy() {
    this.wizard = undefined;
  }

  ngAfterViewInit() {
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: 3,
    });

    // Validation before going to next page
    this.wizard.on('beforeNext', (wizardObj) => {
      console.log('beforeNext', wizardObj);
      // https://angular.io/guide/forms
      // https://angular.io/guide/form-validation
      // validate the form and use below function to stop the wizard's step
      //wizardObj.stop();
    });

    // Change event
    this.wizard.on('change', () => {
      setTimeout(() => {
        KTUtil.scrollTop();
      }, 500);
    });
  }

  onSubmit() {
    console.log('Submited');
    this.submitted = true;
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

    /*this._routesServiceProxy.getAllRoutTypeForTableDropdown().subscribe((result) => {
          this.allRoutTypes = result;
        });*/

    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
    });
    this.loadallVases();
  }
  loadallVases() {
    // this._shippingRequestsServiceProxy.getAllShippingRequestVasesForTableDropdown().subscribe((result) => {
    //   this.allVases = result;
    //   this.allVases.forEach((item) => {
    //     this.selectedVasesProperties[item.id] = {
    //       vasId: item.id,
    //       vasName: item.vasName,
    //       vasCountDisabled: item.hasCount ? false : true,
    //       vasAmountDisabled: item.hasAmount ? false : true,
    //     };
    //   });
    //   console.log(this.selectedVasesProperties);
    // });
  }

  /**
   * resets the bidding start and end dates
   */
  resetBiddingDates(): void {
    this.shippingRequest.bidStartDate = undefined;
    this.shippingRequest.bidEndDate = undefined;
  }
  /**
   * validates bidding start+end date
   */
  validateBiddingDates() {
    //if end date is more than start date reset end date
    if (this.shippingRequest.bidStartDate > this.shippingRequest.bidEndDate) {
      this.shippingRequest.bidEndDate = undefined;
    }
  }
  /**
   * validates trips start/end date
   */
  validateTripsDates() {
    //checks if the trips end date is less than trips start date
    if (this.shippingRequest.endTripDate < this.shippingRequest.startTripDate) {
      this.shippingRequest.endTripDate = undefined;
    }
  }
}
