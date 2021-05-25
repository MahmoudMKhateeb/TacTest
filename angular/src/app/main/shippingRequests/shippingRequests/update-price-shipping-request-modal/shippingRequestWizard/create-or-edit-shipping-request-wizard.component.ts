import { AfterViewInit, Component, ElementRef, Injector, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import KTWizard from './wizard';
import { KTUtil } from '../../../../../../../src/assets/metronic/common/js/components/util';

import {
  CarriersForDropDownDto,
  CreateOrEditShippingRequestDto,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GoodsDetailGoodCategoryLookupTableDto,
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
export class CreateOrEditShippingRequestWizardComponent extends AppComponentBase implements OnInit, OnDestroy, AfterViewInit {
  active = false;
  saving = false;
  // shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();
  // allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  // allCarrierTenants: CarriersForDropDownDto[];
  // allRoutTypes: any;
  // allCitys: RoutStepCityLookupTableDto[];
  // allFacilities: FacilityForDropdownDto[];
  // allVases: ShippingRequestVasListOutput[];
  // selectedVases: CreateOrEditShippingRequestVasListDto[] = [];
  // isTachyonDeal = false;
  // isBid = false;
  // shippingRequestType: string;
  // selectedRouteType: number;
  // allTransportTypes: ISelectItemDto[];
  // allTrucksTypes: SelectItemDto[];
  // allCapacities: SelectItemDto[];
  // allShippingTypes: SelectItemDto[];
  // allpackingTypes: SelectItemDto[];
  // truckTypeLoading: boolean;
  // capacityLoading: boolean;
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

  ngOnInit(): void {}
  ngOnDestroy() {
    this.wizard = undefined;
  }
  ngAfterViewInit() {
    // document.getElementsByClassName('brand-logo').click();
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: 1,
    });

    // Validation before going to next page
    this.wizard.on('beforeNext', (wizardObj) => {
      // https://angular.io/guide/forms
      // https://angular.io/guide/form-validation
      // validate the form and use below function to stop the wizard's step
      // wizardObj.stop();
    });

    // Change event
    this.wizard.on('change', () => {
      setTimeout(() => {
        KTUtil.scrollTop();
      }, 500);
    });
  }

  onSubmit() {
    this.submitted = true;
  }
}
