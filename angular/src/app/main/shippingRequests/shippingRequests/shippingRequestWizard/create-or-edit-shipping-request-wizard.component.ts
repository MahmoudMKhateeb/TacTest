import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Inject,
  Injector,
  NgZone,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
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
  CreateOrEditShippingRequestStep1Dto,
  EditShippingRequestStep2Dto,
  EditShippingRequestStep3Dto,
  EditShippingRequestStep4Dto,
  GetShippingRequestForViewOutput,
  ShippingRequestDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { FormBuilder, Validators } from '@angular/forms';
import * as moment from '@node_modules/moment';

@Component({
  templateUrl: './create-or-edit-shipping-request-wizard.component.html',
  styleUrls: ['./create-or-edit-shipping-request-wizard.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  animations: [
    appModuleAnimation(),
    trigger('grow', [
      // Note the trigger name
      transition(':enter', [
        // :enter is alias to 'void => *'
        style({ height: '0', overflow: 'hidden' }),
        animate(300, style({ height: '*' })),
      ]),
      transition(':leave', [
        // :leave is alias to '* => void'
        animate(200, style({ height: 0, overflow: 'hidden' })),
      ]),
    ]),
  ],
  providers: [EnumToArrayPipe],
})
export class CreateOrEditShippingRequestWizardComponent extends AppComponentBase implements OnDestroy, AfterViewInit, OnInit, OnChanges {
  active = false;
  saving = false;
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
  allTransportTypes: ISelectItemDto[];
  allTrucksTypes: SelectItemDto[];
  allCapacities: SelectItemDto[];
  allShippingTypes: SelectItemDto[];
  allpackingTypes: SelectItemDto[];
  truckTypeLoading: boolean;
  capacityLoading: boolean;
  // selectedVasesProperties = [];
  today = new Date();
  activeShippingRequestId: number = this._activatedRoute.snapshot.queryParams['id'];
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['step'];

  step1Dto = new CreateOrEditShippingRequestStep1Dto();
  step2Dto = new EditShippingRequestStep2Dto();
  step3Dto = new EditShippingRequestStep3Dto();
  step4Dto = new EditShippingRequestStep4Dto();
  activeStep: number;
  loading = false;

  shippingRequestReview: GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();
  shippingRequestRev: ShippingRequestDto = this.shippingRequestReview.shippingRequest;
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
    private enumToArray: EnumToArrayPipe,
    private fb: FormBuilder
  ) {
    super(injector);
  }
  breadcrumbs: BreadcrumbItem[] = [new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests')];
  @ViewChild('wizard', { static: true }) el: ElementRef;
  step1Form = this.fb.group({
    shippingRequestType: [{ value: '', disabled: false }, Validators.required],
    shippingType: [{ value: '', disabled: false }, Validators.required],
    tripsDateRange: ['', Validators.required],
    biddingDateRangeTest: [{ value: '', disabled: false }],
  });
  step2Form = this.fb.group({
    origin: ['', Validators.required],
    destination: ['', Validators.required],
    routeType: ['', Validators.required],
    numberOfDrops: ['', [Validators.minLength(1), Validators.maxLength(3), Validators.min(1), Validators.max(100)]],
    NumberOfTrips: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(3), Validators.min(1), Validators.max(100)]],
  });
  step3Form = this.fb.group({
    goodCategory: ['', Validators.required],
    weight: [null, [Validators.required, Validators.min(1)]],
    packingType: [null, Validators.required],
    numberOfPacking: [null, [Validators.required, Validators.minLength(1), Validators.min(1), Validators.max(10000)]],
    transportType: [null, Validators.required],
    truckType: [null, Validators.required],
    capacity: [null, Validators.required],
  });
  step4Form = this.fb.group({});

  submitted = false;
  wizard: any;
  biddingDateRange: Date[];
  tripsDateRange: Date[];
  selectedVasesProperties = [];
  selectedvas: any;
  ngOnChanges(changes: SimpleChanges) {
    console.log(`Changes ------------------------------- `);
    console.log(changes);
  }

  ngOnInit() {
    this.loadAllDropDownLists();
    this.allRoutTypes = this.enumToArray.transform(ShippingRequestRouteType);
    //to be removed later
    // this._shippingRequestsServiceProxy.publishShippingRequest(this.activeShippingRequestId).subscribe((res) => {
    //   console.log(`get step 2 for Edit `, res);
    // });
    //this.activeShippingRequestId && this.activeStep > 3 ?  : 0;
    //this.reviewAndSubmit();
  }

  ngOnDestroy() {
    this.wizard = undefined;
  }

  ngAfterViewInit() {
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: this.stepToCompleteFrom || 1,
    });
    this.activeStep = this.wizard.getStep();
    //if there is no shipping Request ID go to Step 1
    this.activeShippingRequestId ? this.loadAllStepsForEdit(this.stepToCompleteFrom) : this.wizard.goTo(1);

    //if the step is last one get the all submited Details for Review
    this.activeStep == 5 ? this.reviewAndSubmit() : 0;
    console.log(`current Active Step , `, this.activeStep);
    // Validation before going to next page
    this.wizard.on('beforeNext', (wizardObj) => {
      //console.log('beforeNext', wizardObj);
      switch (this.wizard.getStep()) {
        case 1: {
          if (this.step1Form.invalid) {
            wizardObj.stop();
            this.step1Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep1();
            wizardObj.goNext();
          }
          break;
        }
        case 2: {
          console.log('step 2 fired');
          if (this.step2Form.invalid) {
            wizardObj.stop();
            this.step2Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep2();
            wizardObj.goNext();
          }
          break;
        }
        case 3: {
          console.log('step 3');
          if (this.step3Form.invalid) {
            wizardObj.stop();
            this.step3Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep3();
            wizardObj.goNext();
          }
          //statements;
          break;
        }
        case 4: {
          console.log('step 4');
          this.createOrEditStep4();
          wizardObj.goNext();
          //statements;
          console.log('review And Publish Should Be loaded');
          this.reviewAndSubmit();
          break;
        }
      }
    });
  }
  //publish
  onSubmit() {
    // console.log('Submited');
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this._shippingRequestsServiceProxy
          .publishShippingRequest(this.activeShippingRequestId)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          )
          .subscribe((res) => {
            this.notify.success(this.l('ShippingRequestPublishedSuccessfully'));
            this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
          });
      }
    });

    this.submitted = true;
    // console.log(`current Step`, this.wizard.getStep);
  }

  createOrEditStep1() {
    this.saving = true;
    this.step1Dto.id = this.activeShippingRequestId || undefined;
    this.shippingRequestType == 'bidding' ? (this.step1Dto.isBid = true) : (this.step1Dto.isBid = false);
    this.shippingRequestType == 'tachyondeal' ? (this.step1Dto.isTachyonDeal = true) : (this.step1Dto.isTachyonDeal = false);
    this.step1Dto.bidStartDate = this.biddingDateRange ? moment(this.biddingDateRange[0]) : undefined;
    this.step1Dto.bidEndDate = this.biddingDateRange ? moment(this.biddingDateRange[1]) : undefined;
    this.step1Dto.startTripDate = moment(this.tripsDateRange[0]);
    this.step1Dto.endTripDate = moment(this.tripsDateRange[1]);
    this._shippingRequestsServiceProxy
      .createOrEditStep1(this.step1Dto)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe((res) => {
        //get the shipping Request id from the response of the step one in order to complete other steps
        this.activeShippingRequestId = res;
        this.notify.info(this.l('SavedSuccessfully'));
        this.updateRoutingQueries(res, 1);

        // this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
      });
  }
  createOrEditStep2() {
    this.saving = true;
    this.step2Dto.id = this.activeShippingRequestId;
    this._shippingRequestsServiceProxy
      .editStep2(this.step2Dto)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.updateRoutingQueries(this.activeShippingRequestId, 2);
        })
      )
      .subscribe((res) => {
        this.notify.info(this.l('SavedSuccessfully'));
      });
    console.log('step 2');
  }
  createOrEditStep3() {
    this.saving = true;
    this.step3Dto.id = this.activeShippingRequestId;
    this._shippingRequestsServiceProxy
      .editStep3(this.step3Dto)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.updateRoutingQueries(this.activeShippingRequestId, 3);
        })
      )
      .subscribe((res) => {
        this.notify.info(this.l('SavedSuccessfully'));
      });
    //console.log('step 3');
  }
  createOrEditStep4() {
    this.saving = true;
    this.step4Dto.id = this.activeShippingRequestId;
    this.step4Dto.shippingRequestVasList = this.selectedVases;
    this._shippingRequestsServiceProxy
      .editStep4(this.step4Dto)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.updateRoutingQueries(this.activeShippingRequestId, 4);
        })
      )
      .subscribe((res) => {
        this.notify.info(this.l('SavedSuccessfully'));
      });
  }
  //get the summary and displays it for user
  reviewAndSubmit() {
    this.saving = true;
    this.loading = true;
    console.log(`review And Submit Function Fired`);
    this.updateRoutingQueries(this.activeShippingRequestId, 5);
    this._shippingRequestsServiceProxy.getShippingRequestForView(this.activeShippingRequestId).subscribe((res) => {
      this.shippingRequestReview = res;
      this.shippingRequestRev = res.shippingRequest;
      console.log(this.shippingRequestReview);
      this.loading = false;
    });
  }
  loadStep1ForEdit() {
    this.loading = true;
    return this._shippingRequestsServiceProxy
      .getStep1ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.step1Form.markAllAsTouched();
        })
      )
      .subscribe((res) => {
        this.biddingDateRange = [moment(this.step1Dto.bidStartDate).toDate(), moment(this.step1Dto.bidEndDate).toDate()];
        this.tripsDateRange = [moment(this.step1Dto.startTripDate).toDate(), moment(this.step1Dto.endTripDate).toDate()];
        this.shippingRequestType = res.isBid === true ? 'bidding' : 'tachyondeal';
        this.step1Dto = res;
      });
  }
  loadStep2ForEdit() {
    this.loading = true;
    return this._shippingRequestsServiceProxy
      .getStep2ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.step2Form.markAllAsTouched();
        })
      )
      .subscribe((res) => {
        this.step2Dto = res;
      });
  }
  loadStep3ForEdit() {
    this.loading = true;

    this._shippingRequestsServiceProxy
      .getStep3ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.step3Form.markAllAsTouched();
        })
      )
      .subscribe((res) => {
        this.step3Dto = res;
        this.loadTruckandCapacityForEdit();
      });
  }
  loadStep4ForEdit() {
    this.loading = true;
    this._shippingRequestsServiceProxy
      .getStep4ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.step4Form.markAllAsTouched();
        })
      )
      .subscribe((res) => {
        this.step4Dto = res;
      });
  }
  //get all steps for Edit
  loadAllStepsForEdit(step: number) {
    //this.saving = true;
    // console.log('this is the step 11', step);
    if (step == 1) {
      this.loadStep1ForEdit();
      console.log(`step 1 For Edit`);
    } else if (step == 2) {
      this.loadStep1ForEdit();
      this.loadStep2ForEdit();
    } else if (step == 3) {
      this.loadStep1ForEdit();
      this.loadStep2ForEdit();
      this.loadStep3ForEdit();
    } else if (step == 4) {
      this.loadStep1ForEdit();
      this.loadStep2ForEdit();
      this.loadStep3ForEdit();
      this.loadStep4ForEdit();
    } else if (step == 5) {
      this.loadStep1ForEdit();
      this.loadStep2ForEdit();
      this.loadStep3ForEdit();
      this.loadStep4ForEdit();
      this.reviewAndSubmit();
    }
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

  loadTruckandCapacityForEdit() {
    //Get these DD in Edit Only
    if (this.activeShippingRequestId && this.step3Dto.transportTypeId) {
      this.capacityLoading = true;
      this.truckTypeLoading = true;
      this._shippingRequestsServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(this.step3Dto.transportTypeId).subscribe((result) => {
        this.allTrucksTypes = result;
        this.truckTypeLoading = false;
      });
      this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(this.step3Dto.trucksTypeId).subscribe((result) => {
        this.allCapacities = result;
        this.capacityLoading = false;
      });
    }
  }
  trucksTypeSelectChange(trucksTypeId?: number) {
    if (trucksTypeId > 0) {
      this.capacityLoading = true;
      this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(trucksTypeId).subscribe((result) => {
        this.allCapacities = result;
        this.step3Dto.capacityId = null;
        this.capacityLoading = false;
      });
    } else {
      this.step3Dto.capacityId = null;
      this.allCapacities = null;
    }
  }
  transportTypeSelectChange(transportTypeId?: number) {
    if (transportTypeId > 0) {
      this.truckTypeLoading = true;
      this._shippingRequestsServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(transportTypeId).subscribe((result) => {
        this.allTrucksTypes = result;
        this.step3Dto.trucksTypeId = null;
        this.truckTypeLoading = false;
      });
    } else {
      this.step3Dto.trucksTypeId = null;
      this.allTrucksTypes = null;
      this.allCapacities = null;
    }
  }
  /**
   * resets the bidding start and end dates
   */
  resetBiddingDates(): void {
    this.biddingDateRange = undefined;
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
  loadallVases() {
    this._shippingRequestsServiceProxy.getAllShippingRequestVasesForTableDropdown().subscribe((result) => {
      this.allVases = result;
      this.allVases.forEach((item) => {
        this.selectedVasesProperties[item.id] = {
          vasId: item.id,
          vasName: item.vasName,
          vasCountDisabled: item.hasCount ? false : true,
          vasAmountDisabled: item.hasAmount ? false : true,
        };
      });
      //console.log(this.selectedVasesProperties);
    });
  }

  updateRoutingQueries(shippingRequestId, Step?) {
    this._router.navigate([], {
      queryParams: {
        id: shippingRequestId,
        step: Step || 1,
      },
    });
    this.activeStep = Step;
  }
}
