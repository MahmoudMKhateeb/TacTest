import { Component, Injector, Input, NgZone, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';

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
  ShippingRequestDestinationCitiesDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

import { NgForm } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { DateFormatterService } from '@app/shared/common/hijri-gregorian-datepicker/date-formatter.service';
import { isNotNullOrUndefined } from 'codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './create-or-edit-shippingRequest.component.html',
  styleUrls: ['./create-or-edit-shippingRequest.component.scss'],
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe, DateFormatterService],
})
export class CreateOrEditShippingRequestComponent extends AppComponentBase implements OnInit {
  @ViewChild('shippingRequestForm') shippingRequestForm: NgForm;
  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests'),
    // new BreadcrumbItem(this.l('Entity_Name_Plural_Here') + '' + this.l('Details')),
  ];

  totalOffers: number;
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
  originallySelectedVases: CreateOrEditShippingRequestVasListDto[] = [];
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
  selectedvas: any;
  today = new Date();
  startBiddate: any;
  endBiddate: any;
  destinationCities: ShippingRequestDestinationCitiesDto[] = [];
  //CleanedVases
  cleanedVases: CreateOrEditShippingRequestVasListDto[] = [];
  @Input() parentForm: NgForm;
  @ViewChild('userForm', { static: false }) userForm: NgForm;
  minGreg: NgbDateStruct = { day: 1, month: 1, year: 1900 };
  minHijri: NgbDateStruct = { day: 1, month: 1, year: 1342 };
  todayGregorian = this.dateFormatterService.GetTodayGregorian();
  todayHijri = this.dateFormatterService.ToHijri(this.todayGregorian);
  startTripdate: any;
  endTripdate: any;
  minEndDate: NgbDateStruct;
  minHijriTripdate: NgbDateStruct;
  minGrogTripdate: NgbDateStruct;

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

  ngOnInit(): void {
    this.show(this._activatedRoute.snapshot.queryParams['id']);
    this.allRoutTypes = this.enumToArray.transform(ShippingRequestRouteType);
  }

  show(shippingRequestId?: number): void {
    if (!shippingRequestId) {
      //this is a create
      this.shippingRequest.id = null;
      this.active = true;
      this.loadAllDropDownLists();
    } else {
      //this is an edit
      this._shippingRequestsServiceProxy
        .getShippingRequestForEdit(shippingRequestId)
        .pipe(finalize(() => {}))
        .subscribe((result) => {
          this.shippingRequest = result.shippingRequest;
          if (result.shippingRequest.bidStartDate != null && result.shippingRequest.bidStartDate != undefined) {
            this.startBiddate = this.dateFormatterService.MomentToNgbDateStruct(result.shippingRequest.bidStartDate);
          }
          if (result.shippingRequest.bidEndDate != null && result.shippingRequest.bidEndDate != undefined) {
            this.endBiddate = this.dateFormatterService.MomentToNgbDateStruct(result.shippingRequest.bidEndDate);
          }
          this.startTripdate = this.dateFormatterService.MomentToNgbDateStruct(result.shippingRequest.startTripDate);
          this.minGrogTripdate = this.startTripdate;
          if (result.shippingRequest.endTripDate != null && result.shippingRequest.endTripDate != undefined) {
            this.endTripdate = this.dateFormatterService.MomentToNgbDateStruct(result.shippingRequest.endTripDate);
          }
          this.shippingRequestType =
            result.shippingRequest.isBid === true ? 'bidding' : result.shippingRequest.isDirectRequest ? 'directrequest' : 'tachyondeal';
          this.selectedVases = result.shippingRequest.shippingRequestVasList;
          this.originallySelectedVases = [...this.selectedVases];
          this.selectedRouteType = result.shippingRequest.routeTypeId;
          this.active = true;
          this.totalOffers = result.totalOffers;
          this.shippingRequest.shippingRequestDestinationCities = result.shippingRequest.shippingRequestDestinationCities;
          this.loadAllDropDownLists();
        });
    }
  }

  save(): void {
    //to be Removed Later
    // this.shippingRequest.createOrEditRoutPointDtoList = [];
    //
    this.saving = true;
    this.shippingRequest.isBid = this.shippingRequestType === 'bidding' ? true : false;
    this.shippingRequest.isTachyonDeal = this.shippingRequestType === 'tachyondeal' ? true : false;
    this.shippingRequest.isDirectRequest = this.shippingRequestType === 'directrequest' ? true : false;
    this.shippingRequest.routeTypeId = this.selectedRouteType; //milkrun / oneway ....
    this.shippingRequest.shippingRequestVasList = this.selectedVases;

    if (this.startBiddate != null && this.startBiddate != undefined) {
      this.shippingRequest.bidStartDate = this.GetGregorianAndhijriFromDatepickerChange(this.startBiddate).GregorianDate;
    }

    if (this.endBiddate != undefined) {
      this.shippingRequest.bidEndDate = this.GetGregorianAndhijriFromDatepickerChange(this.endBiddate).GregorianDate;
    }

    if (this.startTripdate != null && this.startTripdate != undefined) {
      this.shippingRequest.startTripDate = this.GetGregorianAndhijriFromDatepickerChange(this.startTripdate).GregorianDate;
    }

    if (this.endTripdate != null && this.endTripdate != undefined) {
      this.shippingRequest.endTripDate = this.GetGregorianAndhijriFromDatepickerChange(this.endTripdate).GregorianDate;
    }

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

  private loadDestinationCities(res: RoutStepCityLookupTableDto[]) {
    if (isNotNullOrUndefined(res)) {
      res.forEach((element) => {
        var item = new ShippingRequestDestinationCitiesDto();
        item.cityId = Number(element.id);
        item.cityName = element.displayName;
        this.destinationCities.push(item);
      });
    }
  }

  loadAllDropDownLists(): void {
    console.log('DD Was loaded');
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown(undefined).subscribe((result) => {
      this.allGoodCategorys = result;
    });

    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
      this.loadDestinationCities(result);
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
        this.getCapacityByTruck(this.allTrucksTypes, this.shippingRequest.trucksTypeId);
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

    this.loadallVases();
  }

  cancel(): void {
    this._router.navigate(['app/main/shippingRequests/shippingRequests']);
  }

  getCapacityByTruck(allTrucksTypes, trucksTypeId) {
    this.capacityLoading = true;
    if (trucksTypeId) {
      if (this.IfOther(allTrucksTypes, trucksTypeId)) {
        this._shippingRequestsServiceProxy.getAllCapacitiesForDropdown().subscribe((result) => {
          this.allCapacities = result;
          this.capacityLoading = false;
        });
      } else {
        this._shippingRequestsServiceProxy.getAllTuckCapacitiesByTuckTypeIdForDropdown(trucksTypeId).subscribe((result) => {
          this.allCapacities = result;
          this.capacityLoading = false;
        });
      }
    } else {
      this.shippingRequest.trucksTypeId = null;
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
    this.getCapacityByTruck(this.allTrucksTypes, trucksTypeId);
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

    //checks if the trips end date is less than trips start date
    if (startDate != undefined && endDate != undefined) {
      if (endDate < startDate) {
        this.endTripdate = undefined;
      }
    }
  }

  /**
   * validates bidding start+end date
   */
  validateBiddingDates($event: NgbDateStruct, type) {
    if (type == 'biddingStartDate') {
      this.startBiddate = this.minEndDate = $event;
    }
    if (type == 'biddingEndDate') {
      this.endBiddate = $event;
    }

    var startDate = this.dateFormatterService.NgbDateStructToMoment(this.startBiddate);
    var endDate = this.dateFormatterService.NgbDateStructToMoment(this.endBiddate);

    //   //if end date is more than start date reset end date
    if (startDate != undefined && endDate != undefined) {
      if (startDate > endDate) {
        this.shippingRequest.bidEndDate = this.endBiddate = undefined;
      }
    }
  }

  // check if user select same city in source and destination
  validateDuplicatedCites(event: Event) {
    let index: number = event.target['selectedIndex'] - 1;

    // if (this.shippingRequest.originCityId == this.shippingRequest.destinationCityId) this.shippingRequest.shippingRequestDestinationCities = null;
  }

  /**
   * Validates Shipping Request Origing&Dest According to Shipping Type
   */
  validateShippingRequestType() {
    //check if user choose local-inside city  but the origin&des same
    // if (this.shippingRequest.shippingTypeId == 1) {
    //   this.shippingRequest.destinationCityId = this.shippingRequest.originCityId;
    // } else if (this.shippingRequest.shippingTypeId == 2) {
    //   // check if user select same city in source and destination
    //   if (this.shippingRequest.originCityId == this.shippingRequest.destinationCityId) {
    //     this.shippingRequestForm.controls['destination'].setErrors({ invalid: true });
    //     this.shippingRequestForm.controls['origin'].setErrors({ invalid: true });
    //     this.notify.error(this.l(' SourceAndDestinationCantBeTheSame'));
    //   } else {
    //     this.shippingRequestForm.controls['destination'].setErrors(null);
    //     this.shippingRequestForm.controls['origin'].setErrors(null);
    //   }
    // }

    //check if user choose local-inside city  but the origin&des same
    if (this.shippingRequest.originCityId != null && this.shippingRequest.shippingTypeId == 1) {
      this.shippingRequest.shippingRequestDestinationCities = [];
      //local inside city
      //this.destinationCountry = this.originCountry;
      var city = new ShippingRequestDestinationCitiesDto();
      city.cityId = this.shippingRequest.originCityId;

      this.shippingRequest.shippingRequestDestinationCities.push(city);
    } else if (this.shippingRequest.shippingTypeId == 2) {
      // if route type is local betwenn cities check if user select same city in source and destination
      // this.destinationCities = this.sourceCities;
      // this.destinationCountry = this.originCountry;

      //if destination city one item selected and equals to origin, while shipping type is between cities
      if (
        isNotNullOrUndefined(this.shippingRequest.shippingRequestDestinationCities) &&
        this.shippingRequest.shippingRequestDestinationCities.length == 1 &&
        this.shippingRequest.shippingRequestDestinationCities.filter((c) => c.cityId == this.shippingRequest.originCityId).length > 0
      ) {
        this.shippingRequestForm.controls['destinationCity'].setErrors({ invalid: true });
        // this.shippingRequestForm.controls['destinationCountry'].setErrors({ invalid: true });
      }
      // else if (this.originCountry !== this.destinationCountry) {
      //   this.shippingRequestForm.controls['originCountry'].setErrors({ invalid: true });
      //   this.shippingRequestForm.controls['destinationCountry'].setErrors({ invalid: true });
      // }
      else {
        // this.clearValidation('destinationCity');
        // this.clearValidation('destinationCountry');
      }
    }
    //  else if (this.shippingRequest.shippingTypeId == 4) {
    //   //if route type is cross border prevent the countries to be the same
    //   if (this.originCountry === this.destinationCountry) {
    //     this.shippingRequestForm.controls['originCountry'].setErrors({ invalid: true });
    //     this.shippingRequestForm.controls['destinationCountry'].setErrors({ invalid: true });
    //   }
    //    else {
    //     this.clearValidation('originCountry');
    //     this.clearValidation('destinationCountry');
    //   }
    // }
  }

  clearValidation(controlName: string) {
    this.shippingRequestForm.controls[controlName].setErrors(null);
    this.shippingRequestForm.controls[controlName].updateValueAndValidity();
  }

  changeVasListSelection(event) {
    const vasId = event.option.vasId;
    const index = this.selectedVases.findIndex((item) => vasId === item.vasId);
    if (index === -1) {
      return;
    }
    const foundIndex = this.originallySelectedVases.findIndex((found) => found.vasId === vasId);
    if (foundIndex === -1) {
      return;
    }
    const keys = Object.keys(this.originallySelectedVases[foundIndex]);
    const item = this.selectedVases[index].toJSON();
    this.selectedVases.splice(index, 1);
    for (let i = 0; i < keys.length; i++) {
      const key = keys[i];
      const val = this.originallySelectedVases[foundIndex][key];
      if (val === null || val === undefined) {
        const cleaned = this.cleanedVases.find((cleanedItem) => cleanedItem.vasId === vasId);
        item[key] = cleaned[key];
        continue;
      }
      item[key] = val;
    }
    this.selectedVases.push(CreateOrEditShippingRequestVasListDto.fromJS(item));
  }

  trackByFunc(index: number, item: CreateOrEditShippingRequestVasListDto) {
    return item.vasId;
  }
}
