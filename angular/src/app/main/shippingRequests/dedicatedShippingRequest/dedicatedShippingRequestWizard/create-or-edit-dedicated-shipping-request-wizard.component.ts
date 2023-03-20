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
  CarriersForDropDownDto,
  CountyDto,
  CreateOrEditDedicatedStep1Dto,
  CreateOrEditShippingRequestVasListDto,
  DedicatedShippingRequestsServiceProxy,
  EditDedicatedStep2Dto,
  EntityTemplateServiceProxy,
  FacilitiesServiceProxy,
  GetAllGoodsCategoriesForDropDownOutput,
  GetShippingRequestForViewOutput,
  GoodsDetailsServiceProxy,
  ISelectItemDto,
  RoutStepsServiceProxy,
  SavedEntityType,
  SelectItemDto,
  ShippersForDropDownDto,
  ShippingRequestDestinationCitiesDto,
  ShippingRequestRouteType,
  ShippingRequestsServiceProxy,
  ShippingTypeEnum,
  TenantCityLookupTableDto,
  TenantRegistrationServiceProxy,
  TimeUnit,
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
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import Swal from 'sweetalert2';
import { DxValidationGroupComponent } from '@node_modules/devextreme-angular';
import { Calendar } from '@node_modules/primeng/calendar';

let _self;

@Component({
  templateUrl: './create-or-edit-dedicated-shipping-request-wizard.component.html',
  styleUrls: ['./create-or-edit-dedicated-shipping-request-wizard.component.scss'],
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
export class CreateOrEditDedicatedShippingRequestWizardComponent
  extends AppComponentBase
  implements OnDestroy, AfterViewInit, OnInit, AfterViewChecked
{
  @ViewChild('userForm', { static: false }) userForm: NgForm;
  @ViewChild('wizard', { static: true }) el: ElementRef;
  @ViewChild('step1FormGroup', { static: false }) step1FormGroup: DxValidationGroupComponent;
  @ViewChild('primeCalendar', { static: false }) primeCalendar: Calendar;
  @Input() parentForm: NgForm;
  active = false;
  saving = false;
  allGoodCategorys: GetAllGoodsCategoriesForDropDownOutput[];
  allRoutTypes: any;
  selectedVases: CreateOrEditShippingRequestVasListDto[] = [];
  isTachyonDeal = false;
  isBid = false;
  shippingRequestType: string;
  allTransportTypes: ISelectItemDto[];
  allTrucksTypes: SelectItemDto[];
  allCapacities: SelectItemDto[];
  allShippingTypes: SelectItemDto[];
  allShippingRequestsTypes: SelectItemDto[] = [];
  allpackingTypes: SelectItemDto[];
  truckTypeLoading: boolean;
  capacityLoading: boolean;
  today = new Date();
  activeShippingRequestId: number = this._activatedRoute.snapshot.queryParams['id'];
  isEdit: boolean;
  templateId: number = this._activatedRoute.snapshot.queryParams['templateId'];
  stepToCompleteFrom: number = this._activatedRoute.snapshot.queryParams['completedSteps'];
  minGreg: NgbDateStruct = { day: 1, month: 1, year: 1900 };
  minHijri: NgbDateStruct = { day: 1, month: 1, year: 1342 };
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  minHijriEndDate: NgbDateStruct;
  minGrogEndDate: NgbDateStruct;
  activeStep: number;
  loading = false;
  rentalStartDate: any;
  rentalEndDate: any;
  startBiddate: any;
  endBiddate: any;
  shippingRequestReview: GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();
  cleanedVases: CreateOrEditShippingRequestVasListDto[] = [];
  selectedVasesProperties = [];
  requestType: any;
  AllShippers: ShippersForDropDownDto[];
  allCarriers: CarriersForDropDownDto[];
  isCarrierSass = false;
  destinationCities: ShippingRequestDestinationCitiesDto[] = [];
  citiesLoading = false;
  allCountries: CountyDto[];
  originCountry: number;
  destinationCountry: number;
  entityType = SavedEntityType;
  breadcrumbs: BreadcrumbItem[] = [new BreadcrumbItem(this.l('DedicatedShippingRequest'), '/app/main/shippingRequests/shippingRequests')];
  submitted = false;
  wizard: any;
  origin = { lat: null, lng: null };
  destination = { lat: null, lng: null };
  step1Dto = new CreateOrEditDedicatedStep1Dto();
  step2Dto = new EditDedicatedStep2Dto();
  step1Form = this.fb.group({
    Shipper: [null],
    shippingRequestType: [{ value: null, disabled: false }, this.isCarrierSass ? Validators.required : false],
    carrier: [{ value: null, disabled: false }],
    expectedMileage: [null],
    originCountry: [null],
    destinationCountry: [null],
    destinationCity: [null],
    shippingTypeId: [null],
    rentalStartDate: [null],
    rentalEndDate: [null],
    rentalDurationUnit: [null],
    numberOfTrucks: [null],
    serviceAreaNotes: [null],
    rentalDuration: [null],
    goodCategoryId: [null],
    packingTypeId: [null],
    transportTypeId: [null],
    trucksTypeId: [null],
    capacityId: [null],
    otherGoodsCategoryName: [null],
    otherTransportTypeName: [null],
    otherTrucksTypeName: [null],
    otherPackingTypeName: [null],
    shippingRequestDestinationCities: [null],
    bidStartDate: [null],
    bidEndDate: [null],
    isDrafted: [null],
    draftStep: [null],
    shipperActorId: [null],
    carrierActorId: [null],
    isInternalBrokerRequest: [null],
    rentalRangeDates: [null, Validators.required],
    ActorShipper: [''],
    ActorCarrier: [''],
    IsInternalBrokerRequest: [''],
  });
  rentalDurationUnits: any[];
  rentalRangeDates: any;
  private selectingDateFor = 1;
  maxSelectableDate: Date;
  selectedDestCitiesForEdit: ShippingRequestDestinationCitiesDto[] = [];
  AllActorsShippers: SelectItemDto[];
  AllActorsCarriers: SelectItemDto[];
  ShippingTypeEnum = ShippingTypeEnum;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy,
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
    _self = this;
    this.step1Dto.isInternalBrokerRequest = false;
  }

  ngOnInit() {
    this.isEdit = this._activatedRoute.snapshot.queryParams['isEdit'] === 'true';
    this.validateCarrierForDirectRequest();
    this.loadAllDropDownLists();
    this.allRoutTypes = (this.enumToArray.transform(ShippingRequestRouteType) as any[]).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.rentalDurationUnits = (this.enumToArray.transform(TimeUnit) as any[]).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.isCarrierSass = this.feature.isEnabled('App.CarrierAsASaas');
    this.useShippingRequestTemplate();
    if (this.feature.isEnabled('App.MarketPlace')) {
      this.allShippingRequestsTypes.push(
        new SelectItemDto(
          SelectItemDto.fromJS({
            id: 'bidding',
            displayName: this.l('Marketplace'),
            isOther: null,
          })
        )
      );
    }
    this.allShippingRequestsTypes.push(
      new SelectItemDto(
        SelectItemDto.fromJS({
          id: 'tachyondeal',
          displayName: this.l('TachyonManageService'),
          isOther: null,
        })
      )
    );
    if (this.feature.isEnabled('App.SendDirectRequest')) {
      this.allShippingRequestsTypes.push(
        new SelectItemDto(
          SelectItemDto.fromJS({
            id: 'directrequest',
            displayName: this.l('DirectRequest'),
            isOther: null,
          })
        )
      );
    }
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
    this.activeShippingRequestId ? this.loadAllStepsForEdit(Number(this.stepToCompleteFrom)) : this.wizard.goTo(1);
    // Validation before going to next page
    this.wizard.on('beforeNext', (wizardObj) => {
      switch (this.wizard.getStep()) {
        case 1: {
          console.log('this.step1Form', this.step1Form);
          document.getElementById('step1FormGroupButton').click();
          this.step1FormGroup.instance.validate();
          if (this.step1Form.invalid || !this.validateOthersInputs() || !this.validateRentalDuration()) {
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
          //check validation for vases
          let isVaild = true;
          this.selectedVases?.forEach((element) => {
            let isDisabledAmount = this.selectedVasesProperties[element.vasId].vasAmountDisabled;
            let isDisabledCount = this.selectedVasesProperties[element.vasId].vasCountDisabled;
            if ((element.requestMaxCount <= 0 && !isDisabledCount) || (element.requestMaxAmount <= 0 && !isDisabledAmount)) {
              isVaild = false;
              return;
            }
          });
          if (isVaild) {
            this.createOrEditStep2();
            wizardObj.goNext();
            this.reviewAndSubmit();
            break;
          } else {
            wizardObj.stop();
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
    if (this.IfOther(this.allGoodCategorys, this.step1Dto.goodCategoryId) && !this.step1Dto.otherGoodsCategoryName.trim()) {
      return false;
    }
    if (this.IfOther(this.allTransportTypes, this.step1Dto.transportTypeId) && !this.step1Dto.otherTransportTypeName.trim()) {
      return false;
    }
    if (this.IfOther(this.allTrucksTypes, this.step1Dto.trucksTypeId) && !this.step1Dto.otherTrucksTypeName.trim()) {
      return false;
    }
    if (this.IfOther(this.allpackingTypes, this.step1Dto.packingTypeId) && !this.step1Dto.otherPackingTypeName.trim()) {
      return false;
    }
    return true;
  }

  validateRentalDuration() {
    if (!isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)) {
      return false;
    }
    if (
      isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value) &&
      (!isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value[0]) ||
        !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value[1]))
    ) {
      return false;
    }
    return true;
  }

  //publish
  onSubmit() {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        this.saving = true;
        this._dedicatedShippingRequestsServiceProxy
          .publishDedicatedShippingRequest(this.activeShippingRequestId)
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
    this.step1Form.get('shippingRequestType').value === 'bidding' ? (this.step1Dto.isBid = true) : (this.step1Dto.isBid = false);
    this.step1Form.get('shippingRequestType').value === 'tachyondeal' ? (this.step1Dto.isTachyonDeal = true) : (this.step1Dto.isTachyonDeal = false);
    this.step1Form.get('shippingRequestType').value === 'directrequest' || this.isCarrierSass
      ? (this.step1Dto.isDirectRequest = true)
      : (this.step1Dto.isDirectRequest = false);
    const startDate = !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)
      ? moment.utc(this.today).toDate()
      : new Date(this.step1Form.get('rentalRangeDates').value[0]);
    const endDate = !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)
      ? moment.utc(this.today).toDate()
      : new Date(this.step1Form.get('rentalRangeDates').value[1]);
    this.step1Dto.rentalStartDate = !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)
      ? moment.utc(this.today)
      : moment.utc({ y: startDate.getFullYear(), M: startDate.getMonth(), d: startDate.getDate() });
    this.step1Dto.rentalEndDate = !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)
      ? moment.utc(this.today)
      : moment.utc({ y: endDate.getFullYear(), M: endDate.getMonth(), d: endDate.getDate() });
    if (this.isCarrierSass) {
      this.step1Dto.carrierTenantIdForDirectRequest = this.appSession.tenantId;
    }
    this._dedicatedShippingRequestsServiceProxy
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
      });
  }

  createOrEditStep2() {
    this.saving = true;
    this.step2Dto.id = this.activeShippingRequestId;
    this.step2Dto.shippingRequestVasList = this.selectedVases?.map((item) => {
      item.numberOfTrips = 0;
      return item;
    });
    this._dedicatedShippingRequestsServiceProxy
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
  }

  //get the summary and displays it for user
  reviewAndSubmit() {
    this.loading = true;
    this.updateRoutingQueries(this.activeShippingRequestId, 3);
    this._shippingRequestsServiceProxy.getShippingRequestForView(this.activeShippingRequestId).subscribe((res) => {
      this.shippingRequestReview = res;
      this.getRequestType(res.shippingRequest.isBid, res.shippingRequest.isDirectRequest);
      this.loading = false;
      this.getCordinatesByCityName(res.originalCityName, 'source');
      this.getCordinatesByCityName(res.destinationCityName, 'destanation');
    });
  }

  loadStep1ForEdit() {
    this.loading = true;
    return this._dedicatedShippingRequestsServiceProxy
      .getStep1ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.step1Form.markAllAsTouched();
        })
      )
      .subscribe((res) => {
        this.step1Dto = res;
        const shippingRequestType = res.isBid ? 'bidding' : res.isTachyonDeal ? 'tachyondeal' : res.isDirectRequest ? 'directrequest' : '';
        this.step1Form.controls['shippingRequestType'].setValue(shippingRequestType);
        this.originCountry = res.countryId;
        this.step1Form.controls['originCountry'].setValue(res.countryId);
        this.selectedDestCitiesForEdit = [...res.shippingRequestDestinationCities];
        this.step1Form.controls['destinationCity'].setValue(res.shippingRequestDestinationCities);
        this.step1Form.controls['rentalRangeDates'].setValue([moment(res.rentalStartDate).toDate(), moment(res.rentalEndDate).toDate()]);
      });
  }

  loadStep2ForEdit() {
    this.loading = true;
    return this._dedicatedShippingRequestsServiceProxy
      .getStep2ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe((res) => {
        this.step2Dto = res;
        this.selectedVases = res.shippingRequestVasList;
      });
  }

  //get all steps for Edit
  loadAllStepsForEdit(step: number) {
    if (step === 1) {
      this.loadStep1ForEdit();
    } else if (step === 2) {
      this.loadStep1ForEdit();
      this.loadStep2ForEdit();
    } else if (step === 3) {
      this.loadStep1ForEdit();
      this.loadStep2ForEdit();
      this.reviewAndSubmit();
    }
  }

  loadAllDropDownLists(): void {
    this._shippingRequestsServiceProxy.getAllShippersForDropDown().subscribe((result) => {
      this.AllShippers = result?.map((item) => {
        item.id = Number(item.id);
        return item;
      });
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
      this.allGoodCategorys = result?.map((item) => {
        item.id = Number(item.id);
        return item;
      });
    });

    this._countriesServiceProxy.getAllCountriesWithCode().subscribe((res) => {
      this.allCountries = res?.map((item) => {
        item.id = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
      this.allTransportTypes = result?.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy
      .getAllShippingTypesForDropdown()
      .pipe(
        finalize(() => {
          if (!this.isEnabled('App.PortMovement')) {
            this.allShippingTypes = this.allShippingTypes.filter((x) => x.id.toString() != '3');
          }
        })
      )
      .subscribe((result) => {
        this.allShippingTypes = result?.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
      });

    // this.allShippingTypes = this.enumToArray.transform(ShippingTypeEnum).map((item) => {
    //   const selectItem = new SelectItemDto();
    //   (selectItem.id as any) = Number(item.key);
    //   selectItem.displayName = item.value;
    //   return selectItem;
    // });

    this._shippingRequestsServiceProxy.getAllPackingTypesForDropdown().subscribe((result) => {
      this.allpackingTypes = result?.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy.getAllCarriersForDropDown().subscribe((result) => {
      this.allCarriers = result?.map((item) => {
        item.id = Number(item.id);
        return item;
      });
    });

    this.loadallVases();
  }

  loadCitiesByCountryId(countryId: number, type: 'source' | 'destination') {
    this.citiesLoading = true;
    this._countriesServiceProxy
      .getAllCitiesForTableDropdown(countryId)
      .pipe(
        finalize(() => {
          this.citiesLoading = false;
        })
      )
      .subscribe((res) => {
        this.destinationCities = [];
        this.loadDestinationCities(res);
        if (this.selectedDestCitiesForEdit.length > 0) {
          this.step1Dto.shippingRequestDestinationCities = [...this.selectedDestCitiesForEdit];
        } else {
          this.step1Dto.shippingRequestDestinationCities = [];
        }
      });
  }

  private loadDestinationCities(res: TenantCityLookupTableDto[]) {
    if (isNotNullOrUndefined(res)) {
      res.forEach((element) => {
        let item = new ShippingRequestDestinationCitiesDto();
        item.cityId = Number(element.id);
        item.cityName = element.displayName;
        this.destinationCities.push(item);
      });
    }
  }

  loadTruckandCapacityForEdit() {
    //Get these DD in Edit Only
    if (this.step1Dto.transportTypeId) {
      this.capacityLoading = true;
      this.truckTypeLoading = true;
      this._shippingRequestsServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(this.step1Dto.transportTypeId).subscribe((result) => {
        this.allTrucksTypes = result.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
        this.truckTypeLoading = false;
        this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(this.step1Dto.trucksTypeId).subscribe((result) => {
          this.allCapacities = result.map((item) => {
            (item.id as any) = Number(item.id);
            return item;
          });
          this.capacityLoading = false;
        });
      });
    }
  }

  trucksTypeSelectChange(trucksTypeId?: number) {
    if (trucksTypeId > 0) {
      this.capacityLoading = true;
      if (this.IfOther(this.allTrucksTypes, trucksTypeId)) {
        this._shippingRequestsServiceProxy.getAllCapacitiesForDropdown().subscribe((result) => {
          this.allCapacities = result.map((item) => {
            (item.id as any) = Number(item.id);
            return item;
          });
          // this.step1Dto.capacityId = null;
          this.capacityLoading = false;
        });
      }
      this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(trucksTypeId).subscribe((result) => {
        this.allCapacities = result.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
        this.capacityLoading = false;
      });
    } else {
      this.step1Dto.capacityId = null;
      this.allCapacities = null;
    }
  }

  transportTypeSelectChange(transportTypeId?: number) {
    if (transportTypeId > 0) {
      this.truckTypeLoading = true;
      this._shippingRequestsServiceProxy.getAllTruckTypesByTransportTypeIdForDropdown(transportTypeId).subscribe((result) => {
        this.allTrucksTypes = result.map((item) => {
          (item.id as any) = Number(item.id);
          return item;
        });
        this.truckTypeLoading = false;
      });
    } else {
      this.step1Dto.trucksTypeId = null;
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
   * validates bidding start+end date
   */
  validateBiddingDates($event: NgbDateStruct, type) {
    if (type === 'biddingStartDate') {
      this.startBiddate = $event;
      if ($event != null && $event.year < 1900) {
        this.minHijriEndDate = $event;
      } else {
        this.minGrogEndDate = $event;
      }
    }
    if (type === 'biddingEndDate') {
      this.endBiddate = $event;
    }

    let startDate = this.dateFormatterService.NgbDateStructToMoment(this.startBiddate);
    let endDate = this.dateFormatterService.NgbDateStructToMoment(this.endBiddate);

    this.step1Dto.bidStartDate = this.GetGregorianAndhijriFromDatepickerChange(this.startBiddate).GregorianDate;

    if (this.endBiddate !== undefined) {
      this.step1Dto.bidEndDate = this.GetGregorianAndhijriFromDatepickerChange(this.endBiddate).GregorianDate;
    }

    //   //if end date is more than start date reset end date
    if (startDate !== undefined && endDate !== undefined) {
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
            this._router.navigate(['/app/main/shippingRequests/dedicatedShippingRequestWizard']);
            this.activeShippingRequestId = undefined;
            this.wizard.goTo(1);
            this.step1Dto = new CreateOrEditDedicatedStep1Dto();
            this.step2Dto = new EditDedicatedStep2Dto();
          });
      }
    });
  }

  /**
   * clears an input previous validation
   * @param controlName
   */
  clearValidation(controlName: string) {
    this.step1Form.controls[controlName].setErrors(null);
    this.step1Form.controls[controlName].updateValueAndValidity();
  }

  /**
   * Get City Cordinates By Providing its name
   * this finction is to draw the shipping Request Main Route in View SR Details in marketPlace
   * @param cityName
   * @param cityType   source/dest
   */
  getCordinatesByCityName(cityName: string, cityType: string) {
    const geocoder = new google.maps.Geocoder();
    geocoder.geocode(
      {
        address: cityName,
      },
      (results, status) => {
        if (status === google.maps.GeocoderStatus.OK) {
          const Lat = results[0].geometry.location.lat();
          const Lng = results[0].geometry.location.lng();
          if (cityType === 'source') {
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

  /**
   * Control Carrier Field Validation if shipping Request type is Direct request make the carrier required
   */
  validateCarrierForDirectRequest() {
    if (this.step1Form.get('shippingRequestType').value === 'directrequest') {
      this.step1Form.controls.carrier.setValidators([Validators.required]);
      this.step1Form.controls['carrier'].setValidators([Validators.required]);
    } else {
      this.step1Form.controls.carrier.clearValidators();
      this.step1Form.controls['carrier'].clearValidators();
    }
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
    let pharsedJson = JSON.parse(savedEntityJsonString);
    this.step1Dto.init(pharsedJson);
    this.step1Form.get('shippingRequestType').setValue(this.step1Dto.isBid ? 'bidding' : '');
    this.step1Form.get('shippingRequestType').setValue(this.step1Dto.isTachyonDeal ? 'tachyondeal' : '');
    this.step1Form.get('shippingRequestType').setValue(this.step1Dto.isDirectRequest ? 'directrequest' : '');
    this.step1Dto.rentalEndDate = this.step1Dto.rentalStartDate = this.step1Dto.bidStartDate = this.step1Dto.bidEndDate = null; //empty Shipping Request Dates
    this.originCountry = pharsedJson.originCountryId;
    this.destinationCountry = pharsedJson.destinationCountryId;
    this.loadCitiesByCountryId(this.originCountry, 'source');
    this.loadCitiesByCountryId(this.destinationCountry, 'destination');
    this.step2Dto.init(pharsedJson);
    this.loadTruckandCapacityForEdit();
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

  /**
   * Validates Shipping Request Origing&Dest According to Shipping Type
   */
  validateShippingRequestType() {
    //check if user choose local-inside city  but the origin&des same
    if (this.step1Dto.shippingTypeId === ShippingTypeEnum.LocalInsideCity) {
      //local inside city
      this.destinationCountry = this.originCountry;
      this.step1Form.get('destinationCountry').clearValidators();
      this.step1Form.get('destinationCountry').updateValueAndValidity();
    } else if (this.step1Dto.shippingTypeId === ShippingTypeEnum.LocalBetweenCities) {
      // if route type is local between cities check if user select same city in source and destination
      this.destinationCountry = this.originCountry;
      this.step1Form.get('destinationCountry').clearValidators();
      this.step1Form.get('destinationCountry').updateValueAndValidity();

      //if destination city one item selected and equals to origin, while shipping type is between cities
      if (isNotNullOrUndefined(this.step1Dto.shippingRequestDestinationCities) && this.step1Dto.shippingRequestDestinationCities.length === 1) {
        this.step1Form.controls['destinationCity'].setErrors({ invalid: true });
        this.step1Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else if (this.originCountry !== this.destinationCountry) {
        this.step1Form.controls['originCountry'].setErrors({ invalid: true });
        this.step1Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        this.clearValidation('destinationCity');
        this.clearValidation('destinationCountry');
      }
    } else if (this.step1Dto.shippingTypeId === ShippingTypeEnum.CrossBorderMovements) {
      //if route type is cross border prevent the countries to be the same
      if (this.originCountry === this.destinationCountry) {
        this.step1Form.controls['originCountry'].setErrors({ invalid: true });
        this.step1Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        this.clearValidation('originCountry');
        this.clearValidation('destinationCountry');
      }
    }
  }

  validateGroup(params) {
    params.validationGroup.validate();
  }

  asyncValidationCannotContainSpace(params) {
    return new Promise((resolve) => {
      resolve(!_self.cannotContainSpace(params.value));
    });
  }

  rentalDatesSelected($event: any) {
    if (this.selectingDateFor === 3) {
      this.selectingDateFor = 1;
    }
    if (this.selectingDateFor === 1) {
      this.step1Dto.rentalStartDate = $event;
      this.changeMaxSelectableDate($event, true);
    }
    if (this.selectingDateFor === 2) {
      this.step1Dto.rentalEndDate = $event;
    }
    this.selectingDateFor = this.selectingDateFor + 1;
  }

  changeMaxSelectableDate(date?: any, shouldOpenCalendar = false) {
    if (!isNotNullOrUndefined(date)) {
      date = this.today;
    }
    switch (this.step1Dto.rentalDurationUnit) {
      case TimeUnit.Days: {
        this.maxSelectableDate = moment(date).add(this.step1Dto.rentalDuration, 'd').toDate();
        break;
      }
      case TimeUnit.Weeks: {
        this.maxSelectableDate = moment(date).add(this.step1Dto.rentalDuration, 'w').toDate();
        break;
      }
      case TimeUnit.Months: {
        this.maxSelectableDate = moment(date).add(this.step1Dto.rentalDuration, 'M').toDate();
        break;
      }
    }
    if (shouldOpenCalendar && !this.primeCalendar.overlayVisible) {
      this.primeCalendar.showOverlay();
    }
  }

  shippingTypeChanged() {
    this.step1Dto.shippingRequestDestinationCities = [];
    this.validateShippingRequestType();
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
}
