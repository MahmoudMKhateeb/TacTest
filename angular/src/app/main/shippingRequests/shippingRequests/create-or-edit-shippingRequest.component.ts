import { Component, ElementRef, Injector, NgZone, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';

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
  ShippingRequestVasListDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Observable } from '@node_modules/rxjs';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { BsModalRef, BsModalService, ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { MapsAPILoader } from '@node_modules/@agm/core';
import { CreateOrEditFacilityModalComponent } from '@app/main/addressBook/facilities/create-or-edit-facility-modal.component';
import { VasForCreateShippingRequstModalComponent } from '@app/main/shippingRequests/shippingRequests/ShippingRequestVas/VasForCreateShippingRequstModal.component';
import * as moment from '@node_modules/moment';

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
  @ViewChild('VasForCreateShippingRequstModalComponent', { static: true })
  VasForCreateShippingRequstModalComponent: VasForCreateShippingRequstModalComponent;

  active = false;
  saving = false;

  shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();
  allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  allCarrierTenants: CarriersForDropDownDto[];
  allTrailerTypes: SelectItemDto[];
  allGoodsDetails: SelectItemDto[];
  allRoutTypes: RouteRoutTypeLookupTableDto[];
  routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();
  allCitys: RoutStepCityLookupTableDto[];
  allFacilities: FacilityForDropdownDto[];
  allPorts: SelectItemDto[];
  createOrEditRoutStepDtoList: CreateOrEditRoutStepDto[] = [];
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  allVases: ShippingRequestVasListDto[];
  selectedVases: ShippingRequestVasListDto[] = [];
  zoom = 5;
  private geoCoder;
  isTachyonDeal = false;
  isBid = false;
  shippingRequestType: string;
  shippingrequestBidStratDate: moment.Moment;
  shippingrequestBidEndDate: moment.Moment;
  SelectedGoodCategory: number;
  selectedRouteType: number;
  allTransportTypes: ISelectItemDto[];
  allTrucksTypes: SelectItemDto[];
  allCapacities: SelectItemDto[];
  fakestartDateinput: any;
  fakeendtDateinput: any;
  truckTypeLoading: boolean;
  capacityLoading: boolean;
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

  openModal() {
    //load Places Autocomplete
    // this.loadMapApi();
    // this.facility.latitude = 24.67911662122269;
    // this.facility.longitude = 46.6355543345471;
    // this.zoom = 5;
    this.staticModal.show();
  }

  openCreateFacilityModal() {
    //load Places Autocomplete
    this.loadMapApi();
    this.facility.latitude = 24.67911662122269;
    this.facility.longitude = 46.6355543345471;
    this.zoom = 5;
    this.createFacilityModal.show();
  }

  ngOnInit(): void {
    this.shippingRequest.createOrEditRouteDto = new CreateOrEditRouteDto();
    //this.routStep.createOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();
    this.show(this._activatedRoute.snapshot.queryParams['id']);
  }

  show(shippingRequestId?: number): void {
    if (!shippingRequestId) {
      this.shippingRequest.id = undefined;
      this.active = true;
    } else {
      this._shippingRequestsServiceProxy.getShippingRequestForEdit(shippingRequestId).subscribe((result) => {
        this.shippingRequest = result.shippingRequest;
        this.shippingRequestType = result.shippingRequest.isBid === true ? 'bidding' : 'tachyondeal';
        this.selectedVases = result.shippingRequest.shippingRequestVasList;
        this.selectedRouteType = result.shippingRequest.createOrEditRouteDto.routTypeId;
        this.shippingRequest.createOrEditRouteDto = result.shippingRequest.createOrEditRouteDto;
        this.createOrEditRoutStepDtoList = result.shippingRequest.createOrEditRoutStepDtoList;
        this.active = true;
      });
    }
    this.loadAllDropDownLists();
    //this.refreshFacilities();
  }

  addRouteStep(): void {
    const item = this.routStep;
    this.createOrEditRoutStepDtoList.push(this.routStep);
    this.routStep = new CreateOrEditRoutStepDto();
    this.staticModal.hide();
  }

  save(): void {
    //this function fires when Create BTN is Clicked
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

  private saveInternal(): Observable<void> {
    this.saving = true;
    this.shippingRequest.createOrEditRoutStepDtoList = this.createOrEditRoutStepDtoList;
    this.shippingRequest.shippingRequestVasList = this.selectedVases;
    return this._shippingRequestsServiceProxy.createOrEdit(this.shippingRequest).pipe(
      finalize(() => {
        this.saving = false;
        this.notify.info(this.l('SavedSuccessfully'));
      })
    );
  }

  loadMapApi() {
    this.mapsAPILoader.load().then(() => {
      this.geoCoder = new google.maps.Geocoder();

      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement);
      autocomplete.addListener('place_changed', () => {
        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocomplete.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude and zoom
          this.facility.latitude = place.geometry.location.lat();
          this.facility.longitude = place.geometry.location.lng();
          this.zoom = 12;
        });
      });
    });
  }

  mapClicked($event: MouseEvent) {
    // @ts-ignore
    this.facility.latitude = $event.coords.lat;
    // @ts-ignore
    this.facility.longitude = $event.coords.lng;
    // @ts-ignore
    this.getAddress($event.coords.lat, $event.coords.lng);
  }

  getAddress(latitude, longitude) {
    this.geoCoder.geocode({ location: { lat: latitude, lng: longitude } }, (results, status) => {
      console.log(results);
      console.log(status);
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 12;
          //this.address = results[0].formatted_address;
        } else {
          window.alert('No results found');
        }
      } else {
        window.alert('Geocoder failed due to: ' + status);
      }
    });
  }

  refreshFacilities() {
    this._routStepsServiceProxy.getAllFacilitiesForDropdown().subscribe((result) => {
      this.allFacilities = result;
    });
  }

  loadAllDropDownLists(): void {
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown().subscribe((result) => {
      this.allGoodCategorys = result;
    });
    // this._shippingRequestsServiceProxy.getAllCarriersForDropDown().subscribe((result) => {
    //   this.allCarrierTenants = result;
    // });
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });

    this._shippingRequestsServiceProxy.getAllTransportTypesForDropdown().subscribe((result) => {
      this.allTransportTypes = result;
    });

    // this._routStepsServiceProxy.getAllGoodsDetailForTableDropdown().subscribe((result) => {
    //   this.allGoodsDetails = result;
    // });
    this._routesServiceProxy.getAllRoutTypeForTableDropdown().subscribe((result) => {
      this.allRoutTypes = result;
    });
    // this._shippingRequestsServiceProxy.getAllPortsForDropdown().subscribe((result) => {
    //   this.allPorts = result;
    // });
    //Nwely added

    this.getAllVasList();
  }

  createFacility() {
    this.saving = true;

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

  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
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
    this.shippingrequestBidStratDate = undefined;
    this.shippingrequestBidEndDate = undefined;
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
}
