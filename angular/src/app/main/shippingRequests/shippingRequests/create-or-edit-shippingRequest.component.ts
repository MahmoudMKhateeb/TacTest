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

@Component({
  templateUrl: './create-or-edit-shippingRequest.component.html',
  styleUrls: ['./create-or-edit-shippingRequest.component.css'],
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
  active = false;
  saving = false;

  shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();
  allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];
  allCarrierTenants: CarriersForDropDownDto[];
  allTrucksTypes: SelectItemDto[];
  allTrailerTypes: SelectItemDto[];
  allGoodsDetails: SelectItemDto[];
  allRoutTypes: RouteRoutTypeLookupTableDto[];
  routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();
  allCitys: RoutStepCityLookupTableDto[];
  allFacilities: FacilityForDropdownDto[];
  allPorts: SelectItemDto[];
  createOrEditRoutStepDtoList: CreateOrEditRoutStepDto[] = [];
  facility: CreateOrEditFacilityDto = new CreateOrEditFacilityDto();
  allVases: ShippingRequestVasListDto[] = [];
  selectedVases: ShippingRequestVasListDto[] = [];
  zoom = 5;
  private geoCoder;

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
      this.shippingRequest.id = shippingRequestId;
      this.active = true;
    } else {
      this._shippingRequestsServiceProxy.getShippingRequestForEdit(shippingRequestId).subscribe((result) => {
        this.shippingRequest = result.shippingRequest;
        //this.shippingRequest.createOrEditRouteDto = result.shippingRequest.createOrEditRouteDto;
        this.createOrEditRoutStepDtoList = result.shippingRequest.createOrEditRoutStepDtoList;
        this.active = true;
      });
    }
    this.loadAllDropDownLists();
    this.refreshFacilities();
  }

  addRouteStep(): void {
    const item = this.routStep;
    this.createOrEditRoutStepDtoList.push(this.routStep);
    this.routStep = new CreateOrEditRoutStepDto();
    this.staticModal.hide();
  }

  save(): void {
    //if cloned request we will create it
    console.log(this._activatedRoute.snapshot.queryParams['clone']);
    if (this._activatedRoute.snapshot.queryParams['clone']) {
      console.log('cloned request');
      this.shippingRequest.id = undefined;
      // this.shippingRequest.goodsDetailId = undefined;
      // this.shippingRequest.createOrEditGoodsDetailDto.id = undefined;
      this.shippingRequest.createOrEditRoutStepDtoList.forEach((x) => (x.id = undefined));
      this.shippingRequest.fatherShippingRequestId = this._activatedRoute.snapshot.queryParams['id'];
      this.shippingRequest.isTachyonDeal = false;
    }
    this.saveInternal().subscribe((x) => {
      this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
    });
  }

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
    this._shippingRequestsServiceProxy.getAllCarriersForDropDown().subscribe((result) => {
      this.allCarrierTenants = result;
    });
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
    this._routStepsServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
    this._routStepsServiceProxy.getAllTrailerTypeForTableDropdown().subscribe((result) => {
      this.allTrailerTypes = result;
    });
    this._routStepsServiceProxy.getAllGoodsDetailForTableDropdown().subscribe((result) => {
      this.allGoodsDetails = result;
    });
    this._routesServiceProxy.getAllRoutTypeForTableDropdown().subscribe((result) => {
      this.allRoutTypes = result;
    });
    this._shippingRequestsServiceProxy.getAllPortsForDropdown().subscribe((result) => {
      this.allPorts = result;
    });
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
}
