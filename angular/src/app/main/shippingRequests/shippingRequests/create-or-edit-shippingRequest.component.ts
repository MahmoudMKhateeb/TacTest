import { Component, ElementRef, Injector, NgZone, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import {
  CreateOrEditGoodsDetailDto,
  CreateOrEditRoutStepDto,
  CreateOrEditShippingRequestDto,
  GoodsDetailGoodCategoryLookupTableDto,
  GoodsDetailsServiceProxy,
  RoutStepCityLookupTableDto,
  RoutStepsServiceProxy,
  ShippingRequestGoodsDetailLookupTableDto,
  ShippingRequestRouteLookupTableDto,
  ShippingRequestsServiceProxy,
  ShippingRequestTrailerTypeLookupTableDto,
  ShippingRequestTrucksTypeLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Observable } from '@node_modules/rxjs';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { BsModalRef, BsModalService, ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { MapsAPILoader } from '@node_modules/@agm/core';

@Component({
  templateUrl: './create-or-edit-shippingRequest.component.html',
  styles: [
    `
      agm-map {
        height: 410px;
      }
    `,
  ],

  animations: [appModuleAnimation()],
})
export class CreateOrEditShippingRequestComponent extends AppComponentBase implements OnInit {
  active = false;
  saving = false;

  shippingRequest: CreateOrEditShippingRequestDto = new CreateOrEditShippingRequestDto();

  trucksTypeDisplayName = '';
  trailerTypeDisplayName = '';
  goodsDetailName = '';
  routeDisplayName = '';

  allTrucksTypes: ShippingRequestTrucksTypeLookupTableDto[];
  allTrailerTypes: ShippingRequestTrailerTypeLookupTableDto[];
  allGoodsDetails: ShippingRequestGoodsDetailLookupTableDto[];
  allRoutes: ShippingRequestRouteLookupTableDto[];
  allGoodCategorys: GoodsDetailGoodCategoryLookupTableDto[];

  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('ShippingRequest'), '/app/main/shippingRequests/shippingRequests'),
    new BreadcrumbItem(this.l('Entity_Name_Plural_Here') + '' + this.l('Details')),
  ];
  routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();
  allCitys: RoutStepCityLookupTableDto[];
  createOrEditRoutStepDtoList: CreateOrEditRoutStepDto[] = [];
  zoom = 5;
  @ViewChild('search') public searchElementRef: ElementRef;
  @ViewChild('staticModal') public staticModal: ModalDirective;
  private geoCoder;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private _router: Router,
    private _goodsDetailsServiceProxy: GoodsDetailsServiceProxy,
    private _routStepsServiceProxy: RoutStepsServiceProxy,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone
  ) {
    super(injector);
  }

  openModal() {
    this.routStep.latitude = '24.67911662122269';
    this.routStep.longitude = '46.6355543345471';
    this.zoom = 5;
    this.staticModal.show();
  }

  ngOnInit(): void {
    this.show(this._activatedRoute.snapshot.queryParams['id']);
    //load Places Autocomplete
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
          this.routStep.latitude = place.geometry.location.lat().toString();
          this.routStep.longitude = place.geometry.location.lng().toString();
          this.zoom = 12;
        });
      });
    });
  }

  show(shippingRequestId?: number): void {
    if (!shippingRequestId) {
      this.shippingRequest = new CreateOrEditShippingRequestDto();
      this.shippingRequest.createOrEditGoodsDetailDto = new CreateOrEditGoodsDetailDto();
      this.shippingRequest.id = shippingRequestId;
      this.trucksTypeDisplayName = '';
      this.trailerTypeDisplayName = '';
      this.goodsDetailName = '';
      this.routeDisplayName = '';

      this.active = true;
    } else {
      this._shippingRequestsServiceProxy.getShippingRequestForEdit(shippingRequestId).subscribe((result) => {
        this.shippingRequest = result.shippingRequest;

        this.trucksTypeDisplayName = result.trucksTypeDisplayName;
        this.trailerTypeDisplayName = result.trailerTypeDisplayName;
        this.goodsDetailName = result.goodsDetailName;
        this.routeDisplayName = result.routeDisplayName;

        this.active = true;
      });
    }
    this._shippingRequestsServiceProxy.getAllTrucksTypeForTableDropdown().subscribe((result) => {
      this.allTrucksTypes = result;
    });
    this._shippingRequestsServiceProxy.getAllTrailerTypeForTableDropdown().subscribe((result) => {
      this.allTrailerTypes = result;
    });
    this._shippingRequestsServiceProxy.getAllGoodsDetailForTableDropdown().subscribe((result) => {
      this.allGoodsDetails = result;
    });
    this._shippingRequestsServiceProxy.getAllRouteForTableDropdown().subscribe((result) => {
      this.allRoutes = result;
    });
    this._goodsDetailsServiceProxy.getAllGoodCategoryForTableDropdown().subscribe((result) => {
      this.allGoodCategorys = result;
    });
    this._routStepsServiceProxy.getAllCityForTableDropdown().subscribe((result) => {
      this.allCitys = result;
    });
  }

  addRouteStep(): void {
    const item = this.routStep;
    this.createOrEditRoutStepDtoList.push(this.routStep);
    this.routStep = new CreateOrEditRoutStepDto();
    this.staticModal.hide();
  }

  save(): void {
    this.saveInternal().subscribe((x) => {
      this._router.navigate(['/app/main/shippingRequests/shippingRequests']);
    });
  }

  saveAndNew(): void {
    this.saveInternal().subscribe((x) => {
      this.shippingRequest = new CreateOrEditShippingRequestDto();
    });
  }

  private saveInternal(): Observable<void> {
    this.saving = true;
    this.shippingRequest.createOrEditRoutStepDtoList = this.createOrEditRoutStepDtoList;

    return this._shippingRequestsServiceProxy.createOrEdit(this.shippingRequest).pipe(
      finalize(() => {
        this.saving = false;
        this.notify.info(this.l('SavedSuccessfully'));
      })
    );
  }

  mapClicked($event: MouseEvent) {
    // @ts-ignore
    this.routStep.latitude = $event.coords.lat.toString();
    // @ts-ignore
    this.routStep.longitude = $event.coords.lng.toString();
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
}
