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
  CreateOrEditShippingRequestStep1Dto,
  CreateOrEditShippingRequestVasListDto,
  DedicatedShippingRequestsServiceProxy,
  EditDedicatedStep2Dto,
  EditShippingRequestStep2Dto,
  EditShippingRequestStep3Dto,
  EditShippingRequestStep4Dto,
  EntityTemplateServiceProxy,
  FacilitiesServiceProxy,
  FacilityForDropdownDto,
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
  ShippingRequestVasListOutput,
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
import { DateType } from '@app/admin/required-document-files/hijri-gregorian-datepicker/consts';
import { NgbDateStruct } from '@node_modules/@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import Swal from 'sweetalert2';
import { DxValidationGroupComponent } from '@node_modules/devextreme-angular';
import DOMComponent from '@node_modules/devextreme/core/dom_component';

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
  // @ViewChild('step2FormGroup', { static: false }) step2FormGroup: DxValidationGroupComponent;
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
  minHijriTripdate: NgbDateStruct;
  minGrogTripdate: NgbDateStruct;
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
  sourceCities: TenantCityLookupTableDto[];
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
    rentalRangeDates: [null],
  });
  // step2Form = this.fb.group({
  //   originCountry: ['', Validators.required],
  //   destinationCountry: ['', Validators.required],
  //   originCity: ['', Validators.required],
  //   destinationCity: ['', Validators.required],
  //   routeType: ['', Validators.required],
  //   numberOfDrops: ['', [Validators.minLength(1), Validators.maxLength(3)]],
  //   NumberOfTrips: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(2), Validators.min(1), Validators.max(20)]],
  // });
  isDropDownBoxOpened = false;
  rentalDurationUnits: any[];
  rentalRangeDates: any;
  private selectingDateFor = 1;
  maxSelectableDate: Date;
  selectedDestCitiesForEdit: ShippingRequestDestinationCitiesDto[] = [];

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
  }
  ngOnInit() {
    this.isEdit = this._activatedRoute.snapshot.queryParams['isEdit'] === 'true';
    console.log('this.isEdit', this.isEdit);
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
    console.log('this.allRoutTypes', this.allRoutTypes);
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
    // this.step1Form.valueChanges.subscribe((val) => {
    //     this.cdr.markForCheck();
    // });
    // Initialize form wizard
    this.wizard = new KTWizard(this.el.nativeElement, {
      startStep: this.stepToCompleteFrom || 1,
    });
    this.activeStep = this.wizard.getStep();
    //if there is no shipping Request ID go to Step 1
    console.log('this.activeShippingRequestId', this.activeShippingRequestId);
    this.activeShippingRequestId ? this.loadAllStepsForEdit(Number(this.stepToCompleteFrom)) : this.wizard.goTo(1);
    // Validation before going to next page
    this.wizard.on('beforeNext', (wizardObj) => {
      switch (this.wizard.getStep()) {
        case 1: {
          console.log('this.step1Form', this.step1Form);
          document.getElementById('step1FormGroupButton').click();
          this.step1FormGroup.instance.validate();
          if (this.step1Form.invalid || !this.validateOthersInputs()) {
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
          console.log('step 2');
          //check validation for vases
          let isVaild = true;
          // let tripsCountNotValid = this.selectedVases.filter(
          //   (r) => r.numberOfTrips == null || r.numberOfTrips === 0 // || r.numberOfTrips > this.step1Dto.numberOfTrips
          // ).length;
          this.selectedVases.forEach((element) => {
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
            //statements;
            //if step 4 passed load the review&submit
            this.reviewAndSubmit();
            break;
          } else {
            wizardObj.stop();
            // this.step2Form.markAllAsTouched();
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
    this.step1Dto.rentalStartDate = !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)
      ? moment(this.today)
      : this.step1Form.get('rentalRangeDates').value[0];
    this.step1Dto.rentalEndDate = !isNotNullOrUndefined(this.step1Form.get('rentalRangeDates').value)
      ? moment(this.today)
      : this.step1Form.get('rentalRangeDates').value[1];
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
        // this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
      });
  }
  createOrEditStep2() {
    this.saving = true;
    this.step2Dto.id = this.activeShippingRequestId;
    this.step2Dto.shippingRequestVasList = this.selectedVases.map((item) => {
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
    console.log('Review And Submit Lanched');
    this.saving = true;
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
        console.log('res', { ...res });
        this.step1Dto = res;
        const shippingRequestType = res.isBid ? 'bidding' : res.isTachyonDeal ? 'tachyondeal' : res.isDirectRequest ? 'directrequest' : '';
        this.step1Form.controls['shippingRequestType'].setValue(shippingRequestType);
        // this.step1Form.controls['shippingTypeId'].setValue(res.shippingTypeId);
        // this.step1Form.controls['carrier'].setValue(res.carrierTenantIdForDirectRequest);
        // this.step1Form.controls['tripsStartDate'].setValue(res.rentalStartDate);
        // this.step1Form.controls['tripsEndDate'].setValue(res.rentalEndDate);
        // this.step1Form.controls['bidStartDate'].setValue(res.bidStartDate);
        // this.step1Form.controls['bidEndDate'].setValue(res.bidEndDate);
        // this.step1Form.controls['Shipper'].setValue(res.shipperId);
        // this.step1Form.controls['capacityId'].setValue(res.capacityId);
        // this.step1Form.controls['transportTypeId'].setValue(res.transportTypeId);
        // this.step1Form.controls['trucksTypeId'].setValue(res.trucksTypeId);
        this.originCountry = res.countryId;
        this.step1Form.controls['originCountry'].setValue(res.countryId);
        this.selectedDestCitiesForEdit = [...res.shippingRequestDestinationCities];
        this.step1Form.controls['destinationCity'].setValue(res.shippingRequestDestinationCities);
        this.step1Form.controls['rentalRangeDates'].setValue([moment(res.rentalStartDate).toDate(), moment(res.rentalEndDate).toDate()]);
        console.log('loadStep1ForEdit this.step1Dto', this.step1Dto);
        console.log('loadStep1ForEdit this.step1Form', this.step1Form);
      });
  }

  loadStep2ForEdit() {
    this.loading = true;
    console.log('22');
    return this._dedicatedShippingRequestsServiceProxy
      .getStep2ForEdit(this.activeShippingRequestId)
      .pipe(
        finalize(() => {
          this.loading = false;
          // this.step2Form.markAllAsTouched();
        })
      )
      .subscribe((res) => {
        this.step2Dto = res;
        this.selectedVases = res.shippingRequestVasList;
        // this.step2Form.controls['originCountry'].setValue('');
        // this.step2Form.controls['destinationCountry'].setValue('');
        // this.step2Form.controls['originCity'].setValue(res.originCityId);
        // this.step2Form.controls['destinationCity'].setValue(res.shippingRequestDestinationCities);
        // this.step2Form.controls['routeType'].setValue(res.routeTypeId);
        // this.step2Form.controls['numberOfDrops'].setValue(res.numberOfDrops);
        // this.step2Form.controls['NumberOfTrips'].setValue(res.numberOfTrips);
        // console.log('loadStep2ForEdit this.step1Form', this.step2Form);
        console.log('loadStep2ForEdit this.step2Dto', this.step2Dto);
      });
  }

  //get all steps for Edit
  loadAllStepsForEdit(step: number) {
    //this.saving = true;
    console.log('this is the step 11', step);
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
      this.AllShippers = result.map((item) => {
        item.id = Number(item.id);
        return item;
      });
    });
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result.map((item) => {
        item.id = Number(item.id);
        return item;
      });
    });

    this._countriesServiceProxy.getAllCountriesWithCode().subscribe((res) => {
      this.allCountries = res.map((item) => {
        item.id = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
      this.allTransportTypes = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy.getAllShippingTypesForDropdown().subscribe((result) => {
      this.allShippingTypes = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy.getAllPackingTypesForDropdown().subscribe((result) => {
      this.allpackingTypes = result.map((item) => {
        (item.id as any) = Number(item.id);
        return item;
      });
    });

    this._shippingRequestsServiceProxy.getAllCarriersForDropDown().subscribe((result) => {
      this.allCarriers = result.map((item) => {
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
        // type === 'source' ? (this.sourceCities = res) : (this.destinationCities = res);
        this.destinationCities = [];
        this.loadDestinationCities(res);
        console.log('this.selectedDestCitiesForEdit', this.selectedDestCitiesForEdit);
        console.log('this.selectedDestCitiesForEdit.length > 0', this.selectedDestCitiesForEdit.length > 0);
        if (this.selectedDestCitiesForEdit.length > 0) {
          this.step1Dto.shippingRequestDestinationCities = [...this.selectedDestCitiesForEdit];
          // this.selectedDestCitiesForEdit = [];
          console.log('this.step1Dto', { ...this.step1Dto });
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
        // this.step1Dto.capacityId = null;
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
        // this.step1Dto.trucksTypeId = null;
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
   * validates rental start/end date
   */
  validateRentalDates($event: NgbDateStruct, type) {
    if (type === 'rentalStartDate') {
      this.rentalStartDate = $event;
      if ($event != null && $event.year < 1900) {
        this.minHijriTripdate = $event;
      } else {
        this.minGrogTripdate = $event;
      }
    }
    if (type === 'rentalEndDate') {
      this.rentalEndDate = $event;
    }

    let startDate = this.dateFormatterService.NgbDateStructToMoment(this.rentalStartDate);
    let endDate = this.dateFormatterService.NgbDateStructToMoment(this.rentalEndDate);

    if (isNotNullOrUndefined(this.rentalStartDate)) {
      this.step1Dto.rentalStartDate = this.GetGregorianAndhijriFromDatepickerChange(this.rentalStartDate).GregorianDate;
    }

    this.step1Dto.rentalStartDate = this.step1Dto.rentalStartDate == null ? moment(new Date()) : null;

    if (isNotNullOrUndefined(this.rentalEndDate)) {
      this.step1Dto.rentalEndDate = this.GetGregorianAndhijriFromDatepickerChange(this.rentalEndDate).GregorianDate;
    }

    //checks if the trips end date is less than trips start date
    if (startDate !== undefined && endDate !== undefined) {
      if (endDate < startDate) {
        this.step1Dto.rentalEndDate = this.rentalEndDate = undefined;
      }
    }
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
   * resets step2 inputs if the Route Type Change
   */
  resetStep2Inputs() {
    this.step1Dto.shippingRequestDestinationCities = [];
    // this.step1Dto.originCityId = this.originCountry = this.destinationCountry = undefined;
    // this.clearValidation('originCity');
    this.clearValidation('destinationCity');
    this.clearValidation('originCountry');
    this.clearValidation('destinationCountry');
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

  isOthersGoodCategoryId(goodCategoryId: number): boolean {
    const t = this.allGoodCategorys?.find((x) => x.id === goodCategoryId);
    const r = t?.displayName.toLowerCase().includes('others');
    if (r) {
      this.step1Form?.controls?.otherGoodsCategoryName?.setValidators([Validators.required]);
    } else {
      this.step1Form?.controls?.otherGoodsCategoryName?.clearValidators();
    }
    this.step1Form?.controls?.otherGoodsCategoryName?.updateValueAndValidity();
    return r;
  }

  isOthersTrucksTypeId(trucksTypeId: number): boolean {
    const t = this.allTrucksTypes?.find((x) => x.id === trucksTypeId?.toString());
    const r = t?.displayName.toLowerCase().includes('others');
    if (r) {
      this.step1Form?.controls?.otherTrucksTypeName?.setValidators([Validators.required]);
    } else {
      this.step1Form?.controls?.otherTrucksTypeName?.clearValidators();
    }
    this.step1Form?.controls?.otherTrucksTypeName?.updateValueAndValidity();
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
    // this.step3Dto.init(pharsedJson);
    this.loadTruckandCapacityForEdit();
    // this.step4Dto.init(pharsedJson);
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
    console.log('validateShippingRequestType');
    //check if user choose local-inside city  but the origin&des same
    if (this.step1Dto.shippingTypeId === 1) {
      // this.step1Dto.shippingRequestDestinationCities = [];
      //local inside city
      this.destinationCountry = this.originCountry;
      // let city = new ShippingRequestDestinationCitiesDto();
      // city.cityId = this.step1Dto.originCityId;
      //
      // this.step1Dto.shippingRequestDestinationCities.push(city);
    } else if (this.step1Dto.shippingTypeId === 2) {
      // if route type is local between cities check if user select same city in source and destination
      // this.destinationCities = this.sourceCities;
      this.destinationCountry = this.originCountry;

      //if destination city one item selected and equals to origin, while shipping type is between cities
      if (
        isNotNullOrUndefined(this.step1Dto.shippingRequestDestinationCities) &&
        this.step1Dto.shippingRequestDestinationCities.length === 1
        // && this.step1Dto.shippingRequestDestinationCities.filter((c) => c.cityId === this.step2Dto.originCityId).length > 0
      ) {
        this.step1Form.controls['destinationCity'].setErrors({ invalid: true });
        this.step1Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else if (this.originCountry !== this.destinationCountry) {
        this.step1Form.controls['originCountry'].setErrors({ invalid: true });
        this.step1Form.controls['destinationCountry'].setErrors({ invalid: true });
      } else {
        this.clearValidation('destinationCity');
        this.clearValidation('destinationCountry');
      }
    } else if (this.step1Dto.shippingTypeId === 4) {
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
    console.log('this.selectingDateFor', this.selectingDateFor);
    if ((this.selectingDateFor = 3)) {
      this.selectingDateFor = 1;
    }
    if ((this.selectingDateFor = 1)) {
      this.step1Dto.rentalStartDate = $event;
    }
    if ((this.selectingDateFor = 2)) {
      this.step1Dto.rentalEndDate = $event;
    }
    this.selectingDateFor = this.selectingDateFor + 1;
    console.log('event', $event);
  }

  changeMaxSelectableDate() {
    switch (this.step1Dto.rentalDurationUnit) {
      case TimeUnit.Daily: {
        this.maxSelectableDate = moment(this.today).add(this.step1Dto.rentalDuration, 'd').toDate();
        break;
      }
      case TimeUnit.Weekly: {
        this.maxSelectableDate = moment(this.today).add(this.step1Dto.rentalDuration, 'w').toDate();
        break;
      }
      case TimeUnit.Monthly: {
        this.maxSelectableDate = moment(this.today).add(this.step1Dto.rentalDuration, 'M').toDate();
        break;
      }
    }
    console.log('this.maxSelectableDate', this.maxSelectableDate);
  }

  destCitiesSelectionChange($event: any) {
    console.log('destCitiesSelectionChange event', $event);
  }

  onMultiTagPreparing(args) {
    console.log('onMultiTagPreparing args', args);
    const selectedItemsLength = args?.selectedItems?.length;
    const totalCount = this.step1Dto.shippingTypeId === 1 ? 1 : 100;

    if (selectedItemsLength < totalCount) {
      args.cancel = true;
    } else {
      return false;
      // args.text = `All selected (${selectedItemsLength})`;
    }
  }
}
