import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Injector,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { finalize } from 'rxjs/operators';
import KTWizard from '@metronic/common/js/components/wizard';

import {
  CarriersForDropDownDto,
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
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { animate, style, transition, trigger } from '@angular/animations';
import { FormBuilder, Validators } from '@angular/forms';
import * as moment from '@node_modules/moment';

@Component({
  templateUrl: './create-or-edit-shipping-request-wizard.component.html',
  styleUrls: ['./create-or-edit-shipping-request-wizard.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  animations: [
    appModuleAnimation(),
    trigger('grow', [
      transition(':enter', [style({ height: '0', overflow: 'hidden' }), animate(300, style({ height: '*' }))]),
      transition(':leave', [animate(200, style({ height: 0, overflow: 'hidden' }))]),
    ]),
  ],
  providers: [EnumToArrayPipe],
})
export class CreateOrEditShippingRequestWizardComponent extends AppComponentBase implements OnDestroy, AfterViewInit, OnInit, AfterViewChecked {
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
  today = new Date();
  activeShippingRequestId: number = this._activatedRoute.snapshot.queryParams['id'];
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];

  step1Dto = new CreateOrEditShippingRequestStep1Dto();
  step2Dto = new EditShippingRequestStep2Dto();
  step3Dto = new EditShippingRequestStep3Dto();
  step4Dto = new EditShippingRequestStep4Dto();
  activeStep: number;
  loading = false;
  shippingRequestReview: GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();
  cleanedVases: CreateOrEditShippingRequestVasListDto[] = [];
  selectedVasesProperties = [];
  public allCarriers: CarriersForDropDownDto[];

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
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    super(injector);
  }
  breadcrumbs: BreadcrumbItem[] = [new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests')];
  @ViewChild('wizard', { static: true }) el: ElementRef;
  step1Form = this.fb.group({
    shippingRequestType: [{ value: '', disabled: false }, Validators.required],
    shippingType: [{ value: '', disabled: false }, Validators.required],
    carrier: [''],
    tripsStartDate: [''],
    tripsEndDate: [''],
    biddingStartDate: [''],
    biddingEndDate: [''],
  });
  step2Form = this.fb.group({
    origin: ['', Validators.required],
    destination: ['', Validators.required],
    routeType: ['', Validators.required],
    numberOfDrops: ['', [Validators.minLength(1), Validators.maxLength(3)]],
    NumberOfTrips: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(2), Validators.min(1), Validators.max(20)]],
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
  selectedvas: any;

  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  ngOnInit() {
    this.loadAllDropDownLists();
    this.allRoutTypes = this.enumToArray.transform(ShippingRequestRouteType);
    this.GetAllCarriersForDropDown();
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
    // Validation before going to next page
    this.wizard.on('beforeNext', (wizardObj) => {
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
          //if step 4 passed load the review&submit
          this.reviewAndSubmit();
          break;
        }
      }
    });
  }
  ngAfterViewChecked() {
    this.cdr.detectChanges();
  }

  //publish
  onSubmit() {
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
  }

  createOrEditStep1() {
    this.saving = true;
    this.step1Dto.id = this.activeShippingRequestId || undefined;
    this.shippingRequestType == 'bidding' ? (this.step1Dto.isBid = true) : (this.step1Dto.isBid = false);
    this.shippingRequestType == 'tachyondeal' ? (this.step1Dto.isTachyonDeal = true) : (this.step1Dto.isTachyonDeal = false);
    this.shippingRequestType == 'directrequest' ? (this.step1Dto.isDirectRequest = true) : (this.step1Dto.isDirectRequest = false);
    this.step1Dto.startTripDate == null ? (this.step1Dto.startTripDate = moment(this.today)) : null;
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
    console.log('Review And Submit Lanched');
    this.saving = true;
    this.loading = true;
    this.updateRoutingQueries(this.activeShippingRequestId, 5);
    this._shippingRequestsServiceProxy.getShippingRequestForView(this.activeShippingRequestId).subscribe((res) => {
      this.shippingRequestReview = res;
      this.loading = false;
      this.getCordinatesByCityName(res.originalCityName, 'source');
      this.getCordinatesByCityName(res.destinationCityName, 'destanation');
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
        res.isBid ? (this.shippingRequestType = 'bidding') : '';
        res.isTachyonDeal ? (this.shippingRequestType = 'tachyondeal') : '';
        res.isDirectRequest ? (this.shippingRequestType = 'directrequest') : '';
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
        this.selectedVases = res.shippingRequestVasList;
      });
  }

  //get all steps for Edit
  loadAllStepsForEdit(step: number) {
    //this.saving = true;
    // console.log('this is the step 11', step);
    if (step == 1) {
      this.loadStep1ForEdit();
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
  /* get all carriers for view in drop dawon */
  GetAllCarriersForDropDown() {
    this._shippingRequestsServiceProxy.getAllCarriersForDropDown().subscribe((result) => {
      this.allCarriers = result;
    });
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
   * loads the vases list and Cleans Them out
   */
  loadallVases() {
    this._shippingRequestsServiceProxy.getAllShippingRequestVasesForTableDropdown().subscribe((result) => {
      result.forEach((x) => {
        const cleanVas = new CreateOrEditShippingRequestVasListDto();
        cleanVas.id = undefined;
        cleanVas.vasId = x.id; //get the vas id from All Vases
        cleanVas.numberOfTrips = undefined;
        cleanVas.requestMaxAmount = x.maxAmount;
        cleanVas.requestMaxCount = x.maxCount;
        cleanVas.vasName = x.vasName;
        this.cleanedVases.push(cleanVas);
      });
      //array the contains each vases and its Properties like hascount and hasAmount -- helpful for the vases table
      result.forEach((item) => {
        this.selectedVasesProperties[item.id] = {
          vasId: item.id,
          vasName: item.vasName,
          vasCountDisabled: item.hasCount ? false : true,
          vasAmountDisabled: item.hasAmount ? false : true,
        };
      });
    });
  }

  /**
   * Updates Router Query Parameters
   * @param shippingRequestId
   * @param Step
   */
  updateRoutingQueries(shippingRequestId, Step?) {
    this._router.navigate([], {
      queryParams: {
        id: shippingRequestId,
        completedSteps: Step || 1,
      },
    });
    this.activeStep = Step;
  }

  /**
   * validates trips start/end date
   */
  validateTripsDates() {
    //checks if the trips end date is less than trips start date
    if (this.step1Dto.endTripDate < this.step1Dto.startTripDate) {
      this.step1Dto.endTripDate = undefined;
    }
  }

  /**
   * validates bidding start+end date
   */
  validateBiddingDates() {
    console.log('Validate Bidding Dates Is Working');
    //if end date is more than start date reset end date
    if (this.step1Dto.bidStartDate > this.step1Dto.bidEndDate) {
      this.step1Dto.bidEndDate = undefined;
    }
  }
  /**
   * Resets Shipping Request Wizard
   */
  resetWizard() {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this._shippingRequestsServiceProxy
          .delete(this.activeShippingRequestId)
          .pipe(
            finalize(() => {
              this.saving = false;
            })
          )
          .subscribe((res) => {
            this.notify.success(this.l('Success'));
            this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
          });
      }
    });
  }

  /**
   * Validates Shipping Request Origing&Dest According to Shipping Type
   */
  validateShippingRequestType() {
    //check if user choose local-inside city  but the origin&des same
    if (this.step1Dto.shippingTypeId == 1) {
      this.step2Dto.destinationCityId = this.step2Dto.originCityId;
    } else if (this.step1Dto.shippingTypeId == 2) {
      // check if user select same city in source and destination
      if (this.step2Dto.originCityId == this.step2Dto.destinationCityId) {
        this.step2Form.controls['destination'].setErrors({ invalid: true });
        // this.step2Form.controls['origin'].setErrors({ invalid: true });
      }
    }
  }

  /**
   * Get City Cordinates By Providing its name
   * this finction is to draw the shipping Request Main Route in View SR Details in marketPlace
   * @param cityName
   * @param cityType   source/dest
   */
  getCordinatesByCityName(cityName: string, cityType: string) {
    console.log('cityName : ', cityName);
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode(
      {
        address: cityName,
      },
      (results, status) => {
        console.log(results);
        if (status == google.maps.GeocoderStatus.OK) {
          const Lat = results[0].geometry.location.lat();
          const Lng = results[0].geometry.location.lng();
          if (cityType == 'source') {
            this.origin = { lat: Lat, lng: Lng };
          } else {
            this.destination = { lat: Lat, lng: Lng };
          }
        } else {
          console.log('Something got wrong ' + status);
        }
      }
    );
  }
}
