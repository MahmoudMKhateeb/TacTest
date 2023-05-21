import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Injector,
  Input,
  NgZone,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { finalize } from 'rxjs/operators';
import KTWizard from '@metronic/common/js/components/wizard';

import {
  ActorSelectItemDto,
  CarriersForDropDownDto,
  CountyDto,
  CreateOrEditShippingRequestStep1Dto,
  CreateOrEditShippingRequestVasListDto,
  EditShippingRequestStep2Dto,
  EditShippingRequestStep3Dto,
  EditShippingRequestStep4Dto,
  EntityTemplateServiceProxy,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GetShippingRequestForViewOutput,
  GoodCategoriesServiceProxy,
  GoodsDetailsServiceProxy,
  ISelectItemDto,
  RoundTripType,
  RoutStepsServiceProxy,
  SavedEntityType,
  SelectFacilityItemDto,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestDestinationCitiesDto,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  ShippingRequestVasDto,
  ShippingRequestVasListOutput,
  ShippingTypeEnum,
  TenantCityLookupTableDto,
  TenantRegistrationServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { animate, style, transition, trigger } from '@angular/animations';
import { FormBuilder, NgForm, Validators } from '@angular/forms';
import * as moment from '@node_modules/moment';
import { DateType } from '@app/admin/required-document-files/hijri-gregorian-datepicker/consts';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import Swal from 'sweetalert2';

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
  providers: [EnumToArrayPipe, DateFormatterService],
})
export class CreateOrEditShippingRequestWizardComponent extends AppComponentBase implements OnDestroy, AfterViewInit, OnInit, AfterViewChecked {
  active = false;
  saving = false;
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  allCarrierTenants: CarriersForDropDownDto[];
  allRoutTypes: any;
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
  templateId: number = this._activatedRoute.snapshot.queryParams['templateId'];
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];
  selectedDateType: DateType = DateType.Hijri; // or DateType.Gregorian
  @Input() parentForm: NgForm;
  @ViewChild('userForm', { static: false }) userForm: NgForm;
  minGreg: NgbDateStruct = { day: 1, month: 1, year: 1900 };
  minHijri: NgbDateStruct = { day: 1, month: 1, year: 1342 };
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  minHijriEndDate: NgbDateStruct;
  minGrogEndDate: NgbDateStruct;
  minHijriTripdate: NgbDateStruct;
  minGrogTripdate: NgbDateStruct;
  step1Dto = new CreateOrEditShippingRequestStep1Dto();
  step2Dto = new EditShippingRequestStep2Dto();
  step3Dto = new EditShippingRequestStep3Dto();
  step4Dto = new EditShippingRequestStep4Dto();
  activeStep: number;
  loading = false;
  startTripdate: any;
  endTripdate: any;
  startBiddate: any;
  endBiddate: any;
  shippingRequestReview: GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();
  cleanedVases: CreateOrEditShippingRequestVasListDto[] = [];
  selectedVasesProperties = [];
  requestType: any;
  AllShippers: ShippersForDropDownDto[];
  AllActorsShippers: ActorSelectItemDto[];
  AllActorsCarriers: ActorSelectItemDto[];
  public allCarriers: CarriersForDropDownDto[];
  isCarrierSass = false;
  sourceCities: TenantCityLookupTableDto[];
  destinationCities: ShippingRequestDestinationCitiesDto[] = [];
  citiesLoading = false;
  allCountries: CountyDto[];
  originCountry: number;
  destinationCountry: number;
  entityType = SavedEntityType;
  ShippingTypeEnum = ShippingTypeEnum;
  allRoundTripTypes: SelectItemDto[];
  allOriginPorts: SelectFacilityItemDto[] = [];
  generalGoodsCategoryId: number;
  ShippingRequestRouteType = ShippingRequestRouteType;
  wayPoints = [];

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _countriesServiceProxy: TenantRegistrationServiceProxy,
    private _router: Router,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private _facilitiesServiceProxy: FacilitiesServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private enumToArray: EnumToArrayPipe,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private _templateService: EntityTemplateServiceProxy
  ) {
    super(injector);
    this.step1Dto.isInternalBrokerRequest = false;
  }

  breadcrumbs: BreadcrumbItem[] = [new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests')];
  @ViewChild('wizard', { static: true }) el: ElementRef;
  step1Form = this.fb.group({
    shippingRequestType: [
      {
        value: '',
        disabled: false,
      },
      this.isCarrierSass || this.step1Dto.isInternalBrokerRequest ? Validators.required : false,
    ],
    shippingType: [{ value: '', disabled: false }, Validators.required],
    carrier: [{ value: '', disabled: false }],
    tripsStartDate: [''],
    tripsEndDate: [''],
    biddingStartDate: [''],
    biddingEndDate: [''],
    Shipper: [''],
    ActorShipper: [''],
    ActorCarrier: [''],
    ShipperReference: [''],
    ShipperInvoiceNumber: [''],
    IsInternalBrokerRequest: [''],
    roundTripType: [''],
  });
  step2Form = this.fb.group({
    originCountry: ['', Validators.required],
    destinationCountry: ['', Validators.required],
    originCity: ['', Validators.required],
    originFacility: [''],
    destinationCity: ['', Validators.required],
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
    otherTransportTypeName: [null],
    otherTrucksTypeName: [null],
    otherGoodsCategoryName: [null],
    otherPackingTypeName: [null],
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
    this.isCarrierSass = this.feature.isEnabled('App.CarrierAsASaas');
    this.useShippingRequestTemplate();
  }

  ngOnDestroy() {
    this.wizard = undefined;
  }

  getRequestType(isBid, isDirectRequest) {
    if (isBid) {
      this.requestType = this.l('Marketplace');
    } else if (isDirectRequest) {
      this.requestType = this.l('DirectRequest');
    } else {
      this.requestType = this.l('TachyonManageService');
    }
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
            if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements) {
              this.removeValidationForImportPortRequestOnStep2Form();
            }
            if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ExportPortMovements) {
              this.removeValidationForExportPortRequestOnStep2Form();
            }
            wizardObj.goNext();
          }
          break;
        }
        case 2: {
          console.log('step 2 fired', this.step2Form);
          if (this.step2Form.invalid) {
            wizardObj.stop();
            this.step2Form.markAllAsTouched();
            this.notify.error(this.l('PleaseCompleteMissingFields'));
          } else {
            this.createOrEditStep2();
            this.removeRequiredGoodDetailsValidationForPortsMovement();
            wizardObj.goNext();
          }
          break;
        }
        case 3: {
          console.log('step 3');
          if (this.step3Form.invalid || !this.validateOthersInputs()) {
            //console.log(this.step3Form);
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
          console.log('step 4', this.step3Form);
          //check validation for vases
          let isVaild = true;
          var tripsCountNotValid = this.selectedVases.filter(
            (r) =>
              this.selectedVasesProperties[r.vasId] &&
              (r.numberOfTrips == null || r.numberOfTrips == 0 || r.numberOfTrips > this.step2Dto.numberOfTrips)
          ).length;
          this.selectedVases.forEach((element) => {
            var isDisabledAmount = this.selectedVasesProperties[element.vasId] ? this.selectedVasesProperties[element.vasId].vasAmountDisabled : true;
            var isDisabledCount = this.selectedVasesProperties[element.vasId] ? this.selectedVasesProperties[element.vasId].vasCountDisabled : true;
            if ((element.requestMaxCount <= 0 && !isDisabledCount) || (element.requestMaxAmount <= 0 && !isDisabledAmount)) {
              isVaild = false;
              return;
            }
          });
          if (isVaild && tripsCountNotValid == 0) {
            this.createOrEditStep4();
            wizardObj.goNext();
            //statements;
            //if step 4 passed load the review&submit
            this.reviewAndSubmit();
            break;
          } else {
            wizardObj.stop();
            this.step3Form.markAllAsTouched();
            this.notify.error(this.l('PleaseConfirmVasesFields'));
          }
        }
      }
    });
  }

  ngAfterViewChecked() {
    this.cdr.detectChanges();
  }

  validateOthersInputs() {
    if (this.IfOther(this.allGoodCategorys, this.step3Dto.goodCategoryId) && !this.step3Dto.otherGoodsCategoryName.trim()) {
      return false;
    }
    if (this.IfOther(this.allTransportTypes, this.step3Dto.transportTypeId) && !this.step3Dto.otherTransportTypeName.trim()) {
      return false;
    }
    if (this.IfOther(this.allTrucksTypes, this.step3Dto.trucksTypeId) && !this.step3Dto.otherTrucksTypeName.trim()) {
      return false;
    }
    if (this.IfOther(this.allpackingTypes, this.step3Dto.packingTypeId) && !this.step3Dto.otherPackingTypeName.trim()) {
      return false;
    }
    return true;
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
            if (this.feature.isEnabled('App.TachyonDealer')) {
              this._router.navigate(['/app/main/tms/shippingRequests']);
            } else {
              this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
            }
          });
      }
    });
    this.submitted = true;
  }

  createOrEditStep1() {
    this.saving = true;
    this.step1Dto.id = this.activeShippingRequestId || undefined;
    // debugger;
    if (this.shippingRequestType == 'directrequest' || this.isCarrierSass || this.step1Dto.isInternalBrokerRequest) {
      this.step1Dto.isDirectRequest = true;
      this.step1Dto.isBid = false;
      this.step1Dto.isTachyonDeal = false;
      this.shippingRequestType == 'directrequest';
    } else {
      this.step1Dto.isDirectRequest = false;
    }
    this.shippingRequestType == 'bidding' ? (this.step1Dto.isBid = true) : (this.step1Dto.isBid = false);
    this.shippingRequestType == 'tachyondeal' ? (this.step1Dto.isTachyonDeal = true) : (this.step1Dto.isTachyonDeal = false);

    this.step1Dto.startTripDate == null ? (this.step1Dto.startTripDate = moment(this.today)) : null;
    if (this.isCarrierSass || this.step1Dto.isInternalBrokerRequest) {
      this.step1Dto.carrierTenantIdForDirectRequest = this.appSession.tenantId;
    }
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
      this.getRequestType(res.shippingRequest.isBid, res.shippingRequest.isDirectRequest);
      this.loading = false;
      this.getCordinatesByCityName(res.originalCityName, 'source');
      this.wayPoints = [];
      for (let i = 0; i < res.destinationCitiesDtos.length; i++) {
        const destCity = res.destinationCitiesDtos[i];
        this.getCordinatesByCityName(destCity.cityName, 'destanation');
      }
      // this.getCordinatesByCityName(res.destinationCityName, 'destanation');
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
        this.fillAllRoundTrips(true);
        if (
          this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements ||
          this.step1Dto.shippingTypeId == ShippingTypeEnum.ExportPortMovements
        ) {
          this.prepareStep2Inputs();
        }
      });
  }

  loadStep2ForEdit() {
    this.loading = true;
    console.log('22');
    return this._shippingRequestsServiceProxy
      .getStep2ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.step2Form.markAllAsTouched();
          if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements) {
            this.removeValidationForImportPortRequestOnStep2Form();
          }
          if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ExportPortMovements) {
            this.removeValidationForExportPortRequestOnStep2Form();
          }
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
    this._shippingRequestsServiceProxy.getAllShippersForDropDown().subscribe((result) => {
      this.AllShippers = result;
    });
    this._shippingRequestsServiceProxy.getAllCarriersActorsForDropDown().subscribe((result) => {
      this.AllActorsCarriers = result;
      // this.AllActorsCarriers.unshift( SelectItemDto.fromJS({id: null, displayName: this.l('Myself'), isOther: false}));
    });

    this._shippingRequestsServiceProxy.getAllShippersActorsForDropDown().subscribe((result) => {
      this.AllActorsShippers = result;
      // this.AllActorsShippers.unshift( SelectItemDto.fromJS({id: null, displayName: this.l('Myself'), isOther: false}));
    });

    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });

    this._countriesServiceProxy.getAllCountriesWithCode().subscribe((res) => {
      this.allCountries = res;
    });

    this._shippingRequestsServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });

    // this._shippingRequestsServiceProxy.getAllShippingTypesForDropdown().subscribe((result) => {
    //   this.allShippingTypes = result;
    // });
    this.allShippingTypes = this.enumToArray.transform(ShippingTypeEnum).map((item) => {
      const selectItem = new SelectItemDto();
      (selectItem.id as any) = Number(item.key);
      selectItem.displayName = item.value;
      return selectItem;
    });

    // if port movement feature not set, remove it from list
    if (!this.isEnabled('App.PortMovement')) {
      this.allShippingTypes = this.allShippingTypes.filter((x) => x.id.toString() != '3' && x.id.toString() != '5');
    }

    this._shippingRequestsServiceProxy.getAllPackingTypesForDropdown().subscribe((result) => {
      this.allpackingTypes = result;
    });

    this._facilitiesServiceProxy.getAllPortsForTableDropdown().subscribe((result) => {
      this.allOriginPorts = result;
    });

    this._shippingRequestsServiceProxy.getAllCarriersForDropDown().subscribe((result) => {
      this.allCarriers = result;
    });

    this.loadallVases();
  }

  private getGeneralGoodsId() {
    this._goodsDetailsServiceProxy.getGeneralGoodsCategoryId().subscribe((result) => {
      this.generalGoodsCategoryId = result;
      this.step3Dto.goodCategoryId ??= this.generalGoodsCategoryId;
      console.log('getGeneralGoodsId this.step3Dto', this.step3Dto);
    });
  }

  fillAllRoundTrips(isInit = false) {
    if (!isInit) {
      this.step1Dto.roundTripType = null;
    }
    // this.step1Form.get('roundTripType').clearValidators();
    // this.step1Form.get('roundTripType').updateValueAndValidity();
    console.log(
      'this.step1Dto.shippingTypeId != ShippingTypeEnum.ImportPortMovements',
      this.step1Dto.shippingTypeId != ShippingTypeEnum.ImportPortMovements
    );
    console.log(
      'this.step1Dto.shippingTypeId != ShippingTypeEnum.ExportPortMovements',
      this.step1Dto.shippingTypeId != ShippingTypeEnum.ExportPortMovements
    );
    if (
      this.step1Dto.shippingTypeId != ShippingTypeEnum.ImportPortMovements &&
      this.step1Dto.shippingTypeId != ShippingTypeEnum.ExportPortMovements
    ) {
      return;
    }
    this.step1Form.get('roundTripType').setValidators([Validators.required]);
    this.step1Form.get('roundTripType').updateValueAndValidity();
    this.allRoundTripTypes = this.enumToArray
      .transform(RoundTripType)
      .filter((item) => {
        if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements) {
          return item.key == RoundTripType.WithoutReturnTrip || item.key == RoundTripType.WithReturnTrip;
        }
        if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ExportPortMovements) {
          return item.key != RoundTripType.WithoutReturnTrip && item.key != RoundTripType.WithReturnTrip;
        }
      })
      .map((item) => {
        const selectItem = new SelectItemDto();
        (selectItem.id as any) = Number(item.key);
        selectItem.displayName = item.value;
        return selectItem;
      });
    // this.step1Dto.roundTripType = Number(this.allRoundTripTypes[0].id);
    // this.step1Form.get('roundTripType').setValue(this.step1Dto.roundTripType);
    // this.step1Form.get('roundTripType').markAsTouched();
    // this.step1Form.get('roundTripType').updateValueAndValidity();
  }

  loadCitiesByCountryId(countryId: number, type: 'source' | 'destination', citiesToFill?: number[]) {
    this.step2Dto.shippingRequestDestinationCities = [];
    this.destinationCities = [];
    this.citiesLoading = true;
    this._countriesServiceProxy
      .getAllCitiesForTableDropdown(countryId)
      .pipe(
        finalize(() => {
          this.citiesLoading = false;
        })
      )
      .subscribe((res) => {
        type === 'source' ? (this.sourceCities = res) : this.loadDestinationCities(res, citiesToFill);
        if (type === 'destination') {
          citiesToFill = undefined;
        }
        if (
          this.step1Dto.shippingTypeId == ShippingTypeEnum.LocalBetweenCities ||
          this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements ||
          this.step1Dto.shippingTypeId == ShippingTypeEnum.ExportPortMovements
        ) {
          this.sourceCities = res;
          // this.step2Dto.originCityId = this.step2Dto.destinationCityId = null;
          this.loadDestinationCities(res);
        }
      });
  }

  private loadDestinationCities(res: TenantCityLookupTableDto[], citiesToFill?: number[]) {
    if (isNotNullOrUndefined(res) && (!isNotNullOrUndefined(this.destinationCities) || this.destinationCities.length === 0)) {
      res.forEach((element) => {
        var item = new ShippingRequestDestinationCitiesDto();
        item.cityId = Number(element.id);
        item.cityName = element.displayName;
        this.destinationCities.push(item);
      });
      if (isNotNullOrUndefined(citiesToFill)) {
        this.step2Dto.shippingRequestDestinationCities = this.destinationCities.filter(
          (city) =>
            citiesToFill.includes(city.cityId) && !this.step2Dto.shippingRequestDestinationCities?.map((item) => item.cityId)?.includes(city.cityId)
        );
      }
    }
  }

  loadTruckandCapacityForEdit() {
    //Get these DD in Edit Only
    if (this.step3Dto.transportTypeId) {
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
      if (this.IfOther(this.allTrucksTypes, trucksTypeId)) {
        this._shippingRequestsServiceProxy.getAllCapacitiesForDropdown().subscribe((result) => {
          this.allCapacities = result;
          this.step3Dto.capacityId = null;
          this.capacityLoading = false;
        });
      }
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
        const selectedTransportType = this.allTransportTypes.find((item) => Number(item.id) === Number(transportTypeId));
        if (
          result.length > 0 &&
          (selectedTransportType.displayName.toLowerCase() === 'other' || selectedTransportType.displayName.toLowerCase() === 'others')
        ) {
          this.step3Dto.trucksTypeId = Number(result[0].id);
          this.trucksTypeSelectChange(this.step3Dto.trucksTypeId);
        } else {
          this.step3Dto.trucksTypeId = null;
        }
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
      queryParamsHandling: 'merge',
    });
    this.activeStep = Step;
  }

  /**
   * validates trips start/end date
   */
  validateTripsDates($event: NgbDateStruct, type) {
    if (type == 'tripsStartDate') {
      this.startTripdate = $event;
      if ($event != null && $event.year < 1900) {
        this.minHijriTripdate = $event;
      } else {
        this.minGrogTripdate = $event;
      }
    }
    if (type == 'tripsEndDate') {
      this.endTripdate = $event;
    }

    var startDate = this.dateFormatterService.NgbDateStructToMoment(this.startTripdate);
    var endDate = this.dateFormatterService.NgbDateStructToMoment(this.endTripdate);

    if (this.startTripdate != null && this.startTripdate != undefined) {
      this.step1Dto.startTripDate = this.GetGregorianAndhijriFromDatepickerChange(this.startTripdate).GregorianDate;
    }

    this.step1Dto.startTripDate == null ? (this.step1Dto.startTripDate = moment(new Date())) : null;

    if (this.endTripdate != null && this.endTripdate != undefined) {
      this.step1Dto.endTripDate = this.GetGregorianAndhijriFromDatepickerChange(this.endTripdate).GregorianDate;
    }

    //checks if the trips end date is less than trips start date
    if (startDate != undefined && endDate != undefined) {
      if (endDate < startDate) {
        this.step1Dto.endTripDate = this.endTripdate = undefined;
      }
    }
  }

  /**
   * validates bidding start+end date
   */
  validateBiddingDates($event: NgbDateStruct, type) {
    if (type == 'biddingStartDate') {
      this.startBiddate = $event;
      if ($event != null && $event.year < 1900) {
        this.minHijriEndDate = $event;
      } else {
        this.minGrogEndDate = $event;
      }
    }
    if (type == 'biddingEndDate') {
      this.endBiddate = $event;
    }

    var startDate = this.dateFormatterService.NgbDateStructToMoment(this.startBiddate);
    var endDate = this.dateFormatterService.NgbDateStructToMoment(this.endBiddate);

    this.step1Dto.bidStartDate = this.GetGregorianAndhijriFromDatepickerChange(this.startBiddate).GregorianDate;

    if (this.endBiddate != undefined) {
      this.step1Dto.bidEndDate = this.GetGregorianAndhijriFromDatepickerChange(this.endBiddate).GregorianDate;
    }

    //   //if end date is more than start date reset end date
    if (startDate != undefined && endDate != undefined) {
      if (startDate > endDate) {
        this.step1Dto.bidEndDate = this.endBiddate = undefined;
      }
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
            this._router.navigate(['/app/main/shippingRequests/shippingRequestWizard']);
            this.activeShippingRequestId = undefined;
            this.wizard.goTo(1);
            this.step1Dto = new CreateOrEditShippingRequestStep1Dto();
            this.step2Dto = new EditShippingRequestStep2Dto();
            this.step3Dto = new EditShippingRequestStep3Dto();
            this.step4Dto = new EditShippingRequestStep4Dto();
          });
      }
    });
  }

  /**
   * Validates Shipping Request Origing&Dest According to Shipping Type
   */
  validateShippingRequestType() {
    if (this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements) {
      this.step2Form.get('originFacility').setValidators([Validators.required]);
      this.step2Form.get('originFacility').updateValueAndValidity();
    }
    //check if user choose local-inside city  but the origin&des same
    if (this.step2Dto.originCityId != null && this.step1Dto.shippingTypeId == ShippingTypeEnum.LocalInsideCity) {
      this.step2Dto.shippingRequestDestinationCities = [];
      //local inside city
      this.destinationCountry = this.originCountry;
      var city = new ShippingRequestDestinationCitiesDto();
      city.cityId = this.step2Dto.originCityId;

      this.step2Dto.shippingRequestDestinationCities.push(city);
    } else if (this.step1Dto.shippingTypeId == ShippingTypeEnum.LocalBetweenCities) {
      // if route type is local betwenn cities check if user select same city in source and destination
      // this.destinationCities = this.sourceCities;
      this.destinationCountry = this.originCountry;

      //if destination city one item selected and equals to origin, while shipping type is between cities
      if (
        isNotNullOrUndefined(this.step2Dto.shippingRequestDestinationCities) &&
        this.step2Dto.shippingRequestDestinationCities.length == 1 &&
        this.step2Dto.shippingRequestDestinationCities.filter((c) => c.cityId == this.step2Dto.originCityId).length > 0
      ) {
        this.step2Form.controls['destinationCity'].setErrors({ invalid: true });
        this.step2Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else if (this.originCountry !== this.destinationCountry) {
        this.step2Form.controls['originCountry'].setErrors({ invalid: true });
        this.step2Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        this.clearValidation('destinationCity');
        this.clearValidation('destinationCountry');
      }
    } else if (this.step1Dto.shippingTypeId == ShippingTypeEnum.CrossBorderMovements) {
      //if route type is cross border prevent the countries to be the same
      if (this.originCountry === this.destinationCountry) {
        this.step2Form.controls['originCountry'].setErrors({ invalid: true });
        this.step2Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        this.clearValidation('originCountry');
        this.clearValidation('destinationCountry');
        this.clearValidation('originFacility');
      }
    }
  }

  /**
   * clears an input previous validation
   * @param controlName
   */
  clearValidation(controlName: string) {
    this.step2Form.controls[controlName].setErrors(null);
    this.step2Form.controls[controlName].updateValueAndValidity();
  }

  /**
   * resets step2 inputs if the Route Type Change
   */
  resetStep2Inputs() {
    this.step2Dto.shippingRequestDestinationCities = [];
    this.step2Dto.originFacilityId = this.step2Dto.originCityId = this.originCountry = this.destinationCountry = undefined;
    this.clearValidation('originCity');
    this.clearValidation('destinationCity');
    this.clearValidation('originCountry');
    this.clearValidation('destinationCountry');
  }

  /**
   * prepare step2 inputs if the round Type Change for ports movement
   */
  prepareStep2Inputs() {
    console.log('prepareStep2Inputs');
    switch (Number(this.step1Dto.roundTripType)) {
      case RoundTripType.TwoWayRoutsWithPortShuttling:
        this.step2Dto.routeTypeId = ShippingRequestRouteType.MultipleDrops;
        this.step2Dto.numberOfDrops = 3;
        break;
      case RoundTripType.TwoWayRoutsWithoutPortShuttling:
      case RoundTripType.WithReturnTrip: {
        this.step2Dto.routeTypeId = ShippingRequestRouteType.MultipleDrops;
        this.step2Dto.numberOfDrops = 2;
        break;
      }
      case RoundTripType.WithoutReturnTrip:
      case RoundTripType.OneWayRoutWithoutPortShuttling:
      default: {
        this.step2Dto.routeTypeId = ShippingRequestRouteType.SingleDrop;
        this.step2Dto.numberOfDrops = 1;
        break;
      }
    }
    // this.step2Dto.shippingRequestDestinationCities = [];
    // this.step2Dto.originCityId = this.originCountry = this.destinationCountry = undefined;
    // this.clearValidation('originCity');
    // this.clearValidation('destinationCity');
    // this.clearValidation('originCountry');
    // this.clearValidation('destinationCountry');
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
            this.wayPoints.push({ location: { lat: Lat, lng: Lng } });
            this.destination = { lat: Lat, lng: Lng };
          }
        } else {
          console.log('Something got wrong ' + status);
        }
      }
    );
  }

  /**
   * Control Carrier Field Validation if shipping Request type is Direct request make the carrier required
   */
  validateCarrierForDirectRequest() {
    if (this.shippingRequestType === 'directrequest') {
      this.step1Form.controls.carrier.setValidators([Validators.required]);
    } else {
      this.step1Form.controls.carrier.clearValidators();
    }
  }

  isOthersGoodCategoryId(goodCategoryId: number): boolean {
    const t = this.allGoodCategorys?.find((x) => x.id == goodCategoryId);
    const r = t?.displayName.toLowerCase().includes('others');
    if (r) {
      this.step3Form?.controls?.otherGoodsCategoryName?.setValidators([Validators.required]);
    } else {
      this.step3Form?.controls?.otherGoodsCategoryName?.clearValidators();
    }
    this.step3Form?.controls?.otherGoodsCategoryName?.updateValueAndValidity();
    return r;
  }

  isOthersTrucksTypeId(trucksTypeId: number): boolean {
    const t = this.allTrucksTypes?.find((x) => x.id == trucksTypeId?.toString());
    const r = t?.displayName.toLowerCase().includes('others');
    if (r) {
      this.step3Form?.controls?.otherTrucksTypeName?.setValidators([Validators.required]);
    } else {
      this.step3Form?.controls?.otherTrucksTypeName?.clearValidators();
    }
    this.step3Form?.controls?.otherTrucksTypeName?.updateValueAndValidity();
    return r;
  }

  /**
   * get Shipping Request Template by TemplateId
   * then pass the res to parseJsonToDtoData to fill the data
   * @private
   */
  private useShippingRequestTemplate() {
    if (isNotNullOrUndefined(this.templateId)) {
      this.loading = true;
      this._templateService
        .getForView(this.templateId)
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe((res) => {
          // this.templateName = res.templateName;
          this.parseJsonToDtoData(res.savedEntity);
        });
    }
  }

  /**
   * convert Fill the Dto From The Json Data
   * @param savedEntityJsonString
   * @private
   */
  private parseJsonToDtoData(savedEntityJsonString: string): void {
    let parsedJson = JSON.parse(savedEntityJsonString);
    this.step1Dto.init(parsedJson);
    this.step1Dto.isBid ? (this.shippingRequestType = 'bidding') : '';
    this.step1Dto.isTachyonDeal ? (this.shippingRequestType = 'tachyondeal') : '';
    this.step1Dto.isDirectRequest ? (this.shippingRequestType = 'directrequest') : '';
    this.step1Dto.endTripDate = this.step1Dto.startTripDate = this.step1Dto.bidStartDate = this.step1Dto.bidEndDate = null; //empty Shipping Request Dates
    this.fillAllRoundTrips();
    this.step1Dto.roundTripType = parsedJson.roundTripType;
    this.step2Dto.init(parsedJson);
    this.originCountry = parsedJson.originCountryId;
    this.destinationCountry = parsedJson.destinationCountryId;
    this.step2Dto.shippingRequestDestinationCities = undefined;
    this.loadCitiesByCountryId(this.originCountry, 'source');
    let citiesToFill = parsedJson.shippingRequestDestinationCities.map((item) => item.cityId);
    this.loadCitiesByCountryId(this.destinationCountry, 'destination', citiesToFill);
    this.step2Dto.originCityId = parsedJson.originCityId;
    this.step2Dto.originFacilityId = parsedJson.originFacilityId;
    this.step3Dto.init(parsedJson);
    this.step3Dto.goodCategoryId = parsedJson.goodCategoryId;
    this.loadTruckandCapacityForEdit();
    this.step4Dto.init(parsedJson);
    if (isNotNullOrUndefined(parsedJson.shippingRequestVasList)) {
      this.selectedVases = parsedJson.shippingRequestVasList.map((item) => {
        let dto = new CreateOrEditShippingRequestVasListDto();
        // I'm using this way to obtain an object that have .toJson() method
        dto.shippingRequestId = item.shippingRequestId;
        dto.id = item.id;
        dto.vasId = item.vasId;
        dto.vasName = item.vasName;
        dto.numberOfTrips = item.numberOfTrips;
        dto.requestMaxAmount = item.requestMaxAmount;
        dto.requestMaxCount = item.requestMaxCount;
        return dto;
      });
    }

    Swal.fire({
      icon: 'success',
      title: this.l('TemplateImportedSuccessfully'),
      showConfirmButton: false,
      timer: 2000,
    });

    this._router.navigate([], {
      queryParams: {
        templateName: null,
        templateId: null,
      },
      queryParamsHandling: 'merge',
    });
  }

  routeTypeChanged() {
    if (this.step2Dto.routeTypeId == 1) {
      this.step2Dto.numberOfDrops = 1;
    } else {
      this.step2Dto.numberOfDrops = undefined;
    }

    if (this.step1Dto.shippingTypeId != 1) {
      this.step2Dto.shippingRequestDestinationCities = [];
    }
  }

  removeCarrierInputFromForm1() {
    if (!this.step1Dto.isInternalBrokerRequest) {
      this.step1Form.get('ActorCarrier').clearValidators();
      this.step1Form.get('ActorCarrier').updateValueAndValidity();
      this.step1Dto.carrierActorId = null;
      if (this.shippingRequestType === 'directrequest') {
        this.step1Form.get('carrier').setValidators([Validators.required]);
        this.step1Form.get('carrier').updateValueAndValidity();
        this.step1Dto.carrierTenantIdForDirectRequest = null;
      }
    } else {
      this.step1Form.get('ActorCarrier').setValidators([Validators.required]);
      this.step1Form.get('ActorCarrier').updateValueAndValidity();
      if (this.shippingRequestType === 'directrequest') {
        this.step1Form.get('carrier').setValue(null);
        this.step1Form.get('carrier').clearValidators();
        this.step1Form.get('carrier').updateValueAndValidity();
        this.step1Dto.carrierTenantIdForDirectRequest = null;
      }
    }
  }

  goToDedicatedWizard() {
    this._router.navigate(['/app/main/shippingRequests/dedicatedShippingRequestWizard']);
  }

  getCityIdFromSelectedPort($event: any) {
    this.step2Dto.originCityId = this.allOriginPorts?.find((port) => port.id == $event)?.cityId;
    console.log('this.step2Dto', this.step2Dto);
  }

  removeValidationForImportPortRequestOnStep2Form() {
    this.step2Form.get('destinationCountry').clearValidators();
    this.step2Form.get('destinationCountry').updateValueAndValidity();
    this.step2Form.get('originCity').clearValidators();
    this.step2Form.get('originCity').updateValueAndValidity();
    this.step2Form.get('routeType').clearValidators();
    this.step2Form.get('routeType').updateValueAndValidity();
  }

  removeValidationForExportPortRequestOnStep2Form() {
    this.step2Form.get('destinationCountry').clearValidators();
    this.step2Form.get('destinationCountry').updateValueAndValidity();
    this.step2Form.get('routeType').clearValidators();
    this.step2Form.get('routeType').updateValueAndValidity();
    this.step2Form.get('originFacility').clearValidators();
    this.step2Form.get('originFacility').updateValueAndValidity();
  }

  private removeRequiredGoodDetailsValidationForPortsMovement() {
    if (
      this.step1Dto.shippingTypeId == ShippingTypeEnum.ExportPortMovements ||
      this.step1Dto.shippingTypeId == ShippingTypeEnum.ImportPortMovements
    ) {
      this.getGeneralGoodsId();
      this.step3Form.get('goodCategory').clearValidators();
      this.step3Form.get('goodCategory').updateValueAndValidity();
    } else {
      this.step3Form.get('goodCategory').setValidators([Validators.required]);
      this.step3Form.get('goodCategory').updateValueAndValidity();
    }
  }
}
